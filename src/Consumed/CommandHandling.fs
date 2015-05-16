namespace Consumed

open System
open Contracts
open Railway

module CommandHandling = 
             
    type ValidationFailure =
        | ArgumentEmpty of string
        | ArgumentStructure of string  
        | ArgumentOutOfRange of string

    type CommandFailure =
        | ItemAlreadyConsumed
        | ItemDoesNotExist

    type HandlingFailure =
        | Validation of ValidationFailure
        | Command of CommandFailure

    type CmdResult = | Event of stream : string * event : Event 
       
    let validationFails f = Failure(HandlingFailure.Validation(f))
    let commandFails f = Failure(HandlingFailure.Command(f))

    let validate cmd =
        match cmd with
        | Consume data -> 
            (
                if data.Id = "" then validationFails(ArgumentEmpty("id"))
                else if data.Category = "" then validationFails(ArgumentEmpty("category"))
                else if data.Description = "" then validationFails(ArgumentEmpty("description"))
                else if data.Url = "" then validationFails(ArgumentEmpty("url"))
                else if not ( [| "book"; "movie" |] |> Seq.exists (fun x -> x.Equals(data.Category, StringComparison.OrdinalIgnoreCase) ) ) then validationFails(ArgumentOutOfRange("category"))         
                else if not ( data.Url.Contains("http://") || data.Url.Contains("https://") ) then validationFails(ArgumentStructure("url"))
                else Success cmd
            )
        | Remove data -> 
            (
                if data.Id = "" then validationFails(ArgumentEmpty("id"))
                else Success cmd
            )
    
    let thetime() = DateTime.UtcNow
   
    let handle read thetime cmd =
        match cmd with
        | Command.Consume data ->
            let name = sprintf "consumeditem/%s" data.Id

            match read name with
            | EventStream.Exists _ ->
                commandFails ItemAlreadyConsumed
            | EventStream.NotExists name -> 
                Success ( Event ( name, Consumed { Timestamp = thetime(); Id = data.Id; Category = data.Category; Description = data.Description; Url = data.Url } ))
        | Command.Remove data ->
            let name = sprintf "consumeditem/%s" data.Id

            match read name with
            | EventStream.NotExists ( _ ) ->
                commandFails ItemDoesNotExist
            | EventStream.Exists ( name , _ ) ->
                Success ( Event ( name, Removed { Timestamp = thetime(); Id = data.Id } ) )
  
    let handleCommandSideEffects store input =
        match input with
        | Event ( stream, event ) -> ( store stream event )