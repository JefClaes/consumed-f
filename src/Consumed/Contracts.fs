namespace Consumed 

open System

module Contracts =

    type Command =
        | Consume of id : string * category : string * description : string * url : string
        | Remove of id : string

    type Query =
        | List

    type Event =
        | Consumed of timestamp : DateTime * id : string * category : string * description : string * url : string
        | Removed of timestamp : DateTime * id : string
