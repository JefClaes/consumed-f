namespace Consumed

open System
open Railway
open Contracts

module Validation =

    type ValidationFailure =
        | ArgumentEmpty of string
        | ArgumentStructure of string

    let validateCommand cmd =
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
