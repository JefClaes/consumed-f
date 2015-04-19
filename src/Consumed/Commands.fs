namespace Consumed 

module Commands =

    type Command =
        | Consume of id : string * description : string * url : string
        | Remove of id : string
        | List 

