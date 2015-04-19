namespace Consumed

open Contracts
open Railway

module Handling =

    type HandlingFailure =
     | ArgumentEmpty of string
     | ArgumentStructure of string    

    let validate cmd =
        match cmd with
        | Consume ( id, description, url ) -> 
            (
                if id = "" then Failure(ArgumentEmpty("id"))
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

    let handle cmd =       
        match cmd with
        | Command.Consume ( id, description, url ) ->
            Success ( Consumed(id, description, url) )
        | Command.Remove ( id ) ->
            Success ( Removed(id) )

    let handleInPipeline cmd = cmd |> validate >>= switch handle

         
