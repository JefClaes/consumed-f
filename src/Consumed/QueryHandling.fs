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
                let eventsFromStore = read "$all" 
                let events = eventsFromStore |> Seq.map (fun e -> e.Body) 

                let folder state x = 
                    match x with
                    | Event.Consumed ( timestamp, id, category, description, url ) -> 
                        { Id = id; Timestamp = timestamp; Category = category; Description = description; Url = url } :: state
                    | Event.Removed ( timestamp, id ) -> 
                        List.filter (fun x -> x.Id <> id) state

                let items = Seq.fold folder List.empty events
                let categories = 
                    items 
                    |> Seq.groupBy (fun x -> x.Category)
                    |> Seq.map (fun ( x, y ) -> { Name = x; Items = y })
                { Categories = categories }
            )
