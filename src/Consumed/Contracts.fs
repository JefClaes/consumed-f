namespace Consumed 

open System

module Contracts =

    type Command =
        | Consume of data : ConsumeData
        | Remove of data : RemoveData
    and ConsumeData =  { Category : string; Description : string; Url : string }
    and RemoveData = { Id : string; }

    type Query = | List
           
    type Event =
        | Consumed of data : ConsumedData
        | Removed of data : RemovedData
    and ConsumedData = { Timestamp : DateTime; Id : string; Category : string; Description : string; Url : string }
    and RemovedData = { Timestamp : DateTime; Id : string }
      
    type EventStream =
        | NotExists of name : string
        | Exists of name : string * events : seq<Event>