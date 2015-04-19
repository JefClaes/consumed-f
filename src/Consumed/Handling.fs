namespace Consumed

open Contracts
open Railway

module Handling =

    let handleCommand cmd = 
        match cmd with
        | Command.Consume ( id, description, url ) ->
            Consumed(id, description, url)
        | Command.Remove ( id ) ->
            Removed(id)
         
