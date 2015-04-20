﻿namespace Consumed

open System
open Contracts
open Railway

module Handling =

    type HandlingFailure =
     | ArgumentEmpty of string
     | ArgumentStructure of string    
     | ArgumentOutOfRange of string

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

    let handle cmd =       
        match cmd with
        | Command.Consume ( id, category, description, url ) ->
            Success ( Consumed(id, category, description, url) )
        | Command.Remove ( id ) ->
            Success ( Removed(id) )

    let handleInPipeline cmd = cmd |> validate >>= switch handle

         
