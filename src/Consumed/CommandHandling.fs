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
       
    let validateCommand cmd =
        match cmd with
        | Consume ( id, category, description, url ) -> 
            (
                if id = "" then Failure(ArgumentEmpty("id"))
                else if category = "" then Failure(ArgumentEmpty("category"))
                else if not ( [| "book"; "movie" |] |> Seq.exists (fun x -> x.Equals(category, StringComparison.OrdinalIgnoreCase) ) ) then Failure(ArgumentOutOfRange("category")) 
                else if description = "" then Failure(ArgumentEmpty("description"))
                else if url = "" then Failure(ArgumentEmpty("url"))
                else if not ( url.Contains("http://") || url.Contains("https://") ) then Failure(ArgumentStructure("url"))
                else Success cmd
            )
        | Remove id -> 
            (
                if id = "" then Failure(ArgumentEmpty("id"))
                else Success cmd
            )
    
    let thetime() = DateTime.UtcNow

    let handleCommand thetime cmd =       
        match cmd with
        | Command.Consume ( id, category, description, url ) ->
            Success ( Event 
                ( 
                    sprintf "consumeditem/%s" id, 
                    Consumed( thetime(), id, category, description, url) 
                ))
        | Command.Remove ( id ) ->
            Success ( Event
                (
                    sprintf "consumeditem/%s" id,
                    Removed( thetime(), id ) 
                ))                
  
    let handleCommandSideEffects store input =
        match input with
        | Event ( stream, event ) -> ( store stream event )