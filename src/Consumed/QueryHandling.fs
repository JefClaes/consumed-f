namespace Consumed

open System
open Contracts
open EventStore

module QueryHandling =

    type ConsumedItemList =
        {
            Categories : seq<ConsumedCategory>
        }
    and ConsumedCategory = 
        {
            Name : string;
            Items : seq<ConsumedItem>;
        }
    and ConsumedItem = 
        { 
            Id : string; 
            Timestamp : DateTime;
            Category : string; 
            Description: string; 
            Url: string 
        }

    let handleQuery read query =
        match query with
        | Query.List ->
            (             
                let folder state x = 
                    match x with
                    | Event.Consumed data -> 
                        { 
                            Id = data.Id; 
                            Timestamp = data.Timestamp; 
                            Category = data.Category; 
                            Description = data.Description; 
                            Url = data.Url 
                        } :: state
                    | Event.Removed data -> 
                        List.filter (fun x -> x.Id <> data.Id) state

                match read "$all"  with
                | EventStream.NotExists _ -> { Categories = Seq.empty }
                | EventStream.Exists ( _, events ) -> 
                    (
                        let items = Seq.fold folder List.empty events
                        let categories = 
                            items 
                            |> Seq.groupBy (fun x -> x.Category)
                            |> Seq.map (fun ( x, y ) -> { Name = x; Items = y })
                        { Categories = categories }                    
                    )               
            )
