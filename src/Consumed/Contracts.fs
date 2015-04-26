namespace Consumed 

open System

module Contracts =

    type Command =
        | Consume of id : string * category : string * description : string * url : string
        | Remove of id : string

    type Query =
        | List
           
    type Event =
        | Consumed of data : ConsumedData
        | Removed of data : RemovedData
    and ConsumedData = { Timestamp : DateTime; Id : string; Category : string; Description : string; Url : string }
    and RemovedData = { Timestamp : DateTime; Id : string }

    type StoredEvent =  { Stream : string; Body : Event  }
