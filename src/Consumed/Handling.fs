namespace Consumed

open System
open Contracts
open Railway
open EventStore

module Handling = 
    
    type HandlingFailure =
        | ArgumentEmpty of string
        | ArgumentStructure of string    
        | ArgumentOutOfRange of string

    type HandleResult =
        | Event of stream : string * event : Event 

    let validate cmd =
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

    let handle thetime cmd =       
        match cmd with
        | Command.Consume ( id, category, description, url ) ->
            Success ( HandleResult.Event 
                ( 
                    sprintf "consumeditem/%s" id, 
                    Consumed( thetime(), id, category, description, url) 
                ))
        | Command.Remove ( id ) ->
            Success ( HandleResult.Event
                (
                    sprintf "consumeditem/%s" id,
                    Removed( thetime(), id ) 
                ))        
    
    let sideEffects result =
        match result with
        | HandleResult.Event ( stream, event ) -> ( store "D:\store.txt" stream event )

    let handleInPipeline cmd = cmd |> validate >>= ( handle thetime ) >>= switch sideEffects 

         
