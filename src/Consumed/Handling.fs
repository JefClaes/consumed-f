namespace Consumed

open System
open Contracts
open Railway
open EventStore

module Handling = 
    
    type ValidationFailure =
        | ArgumentEmpty of string
        | ArgumentStructure of string    
        | ArgumentOutOfRange of string
        
    type CmdResult =
        | Event of stream : string * event : Event 
    
    type ConsumedItemReadModel = 
        { 
            Id : string; 
            Timestamp : DateTime;
            Category : string; 
            Description: string; 
            Url: string 
        }
     
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
                
    let handleQuery read query =
        match query with
        | Query.List ->
            (                  
                let eventsFromStore = read "$all" 
                let events = eventsFromStore |> Seq.map (fun e -> e.Body) 

                let folder state x = 
                    match x with
                    | Event.Consumed ( timestamp, id, category, description, url ) -> 
                       { Id = id; Timestamp = timestamp; Category = category; Description = description; Url = url } :: state
                    | Event.Removed ( timestamp, id ) -> 
                        List.filter (fun x -> x.Id <> id) state

                let result = Seq.fold folder List.empty events

                result
            )     
    
    let commandSideEffects store input =
        match input with
        | Event ( stream, event ) -> ( store stream event )