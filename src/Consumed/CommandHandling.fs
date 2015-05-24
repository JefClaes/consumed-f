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
       
    let succeeds s = Success s
    let validationFails f = Failure(Validation(f))
    let commandFails f = Failure(Command(f))

    let validate cmd =
        match cmd with
        | Consume data -> 
            (                
                if data.Category = "" then validationFails(ArgumentEmpty("category"))
                else if data.Description = "" then validationFails(ArgumentEmpty("description"))
                else if data.Url = "" then validationFails(ArgumentEmpty("url"))
                else if not ( [| "book"; "movie" |] |> Seq.exists (fun x -> x.Equals(data.Category, StringComparison.OrdinalIgnoreCase) ) ) then validationFails(ArgumentOutOfRange("category"))         
                else if not ( data.Url.Contains("http://") || data.Url.Contains("https://") ) then validationFails(ArgumentStructure("url"))
                else succeeds cmd
            )
        | Remove data -> 
            (
                if data.Id = "" then validationFails(ArgumentEmpty("id"))
                else succeeds cmd
            )
    
    let thetime() = DateTime.UtcNow
   
    let handle read ( thetime : unit -> DateTime ) cmd =
        match cmd with
        | Command.Consume data ->
            let time = thetime()
            let id = time.ToString("yyyyMMddHHmmss")
            let stream = sprintf "consumeditem/%s" id

            match read stream with
            | EventStream.Exists _ ->
                commandFails ItemAlreadyConsumed
            | EventStream.NotExists stream -> 
                succeeds ( Event ( stream, Consumed { Timestamp = time; Id = id; Category = data.Category; Description = data.Description; Url = data.Url } ) )
        | Command.Remove data ->
            let stream = sprintf "consumeditem/%s" data.Id

            match read stream with
            | EventStream.NotExists ( _ ) ->
                commandFails ItemDoesNotExist
            | EventStream.Exists ( stream , _ ) ->
                succeeds ( Event ( stream, Removed { Timestamp = thetime(); Id = data.Id } ) )
  
    let sideEffects store input =
        match input with
        | Event ( stream, event ) -> store stream event