namespace Consumed 

module Contracts =

    type Command =
        | Consume of id : string * category : string * description : string * url : string
        | Remove of id : string

    type Query =
        | List

    type Event =
        | Consumed of id : string * category : string * description : string * url : string
        | Removed of id : string
