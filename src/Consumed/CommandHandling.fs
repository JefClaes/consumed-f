namespace Consumed

open System
open Contracts
open Railway

module CommandHandling = 
    
    type ValidationFailure =
        | ArgumentEmpty of string
        | ArgumentStructure of string    
        | ArgumentOutOfRange of string
        
    type CmdResult =
        | Event of stream : string * event : Event 
       
    type ConsumedItem = { Id : string; Category : string; Description : string ; Url : string }

    let validateCommand cmd =
        match cmd with
        | Consume data -> 
            (
                if data.Id = "" then Failure(ArgumentEmpty("id"))
                else if data.Category = "" then Failure(ArgumentEmpty("category"))
                else if not ( [| "book"; "movie" |] |> Seq.exists (fun x -> x.Equals(data.Category, StringComparison.OrdinalIgnoreCase) ) ) then Failure(ArgumentOutOfRange("category")) 
                else if data.Description = "" then Failure(ArgumentEmpty("description"))
                else if data.Url = "" then Failure(ArgumentEmpty("url"))
                else if not ( data.Url.Contains("http://") || data.Url.Contains("https://") ) then Failure(ArgumentStructure("url"))
                else Success cmd
            )
        | Remove data -> 
            (
                if data.Id = "" then Failure(ArgumentEmpty("id"))
                else Success cmd
            )
    
    let thetime() = DateTime.UtcNow
   
    let handleCommand thetime cmd =       
        match cmd with
        | Command.Consume data ->
            Success ( Event 
                ( 
                    sprintf "consumeditem/%s" data.Id, 
                    Consumed 
                        { 
                            Timestamp = thetime()
                            Id = data.Id;
                            Category = data.Category;
                            Description = data.Description;
                            Url = data.Url
                        } 
                ))
        | Command.Remove data ->
            Success ( Event
                (
                    sprintf "consumeditem/%s" data.Id,
                    Removed { Timestamp = thetime(); Id = data.Id } 
                ))                
  
    let handleCommandSideEffects store input =
        match input with
        | Event ( stream, event ) -> ( store stream event )