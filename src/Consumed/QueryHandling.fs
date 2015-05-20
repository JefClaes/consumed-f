namespace Consumed

open System
open Contracts
open EventStore

module QueryHandling =

    type List = { Categories : seq<Category> }
        and Category = { Name : string; Items : seq<Item>; }
        and Item = { Id : string; Timestamp : DateTime; Category : string; Description: string; Url: string }

    let handle read query =
        match query with
        | Query.List ->
            (
                let folder state e = 
                    match e with
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
                | EventStream.Exists ( _ , events ) -> 
                    (
                        let items = Seq.fold folder [] events
                        let categories = 
                            items 
                            |> Seq.groupBy (fun x -> x.Category)
                            |> Seq.map (fun ( x , y ) -> { Name = x; Items = y |> Seq.sortBy (fun x -> x.Timestamp) })
                        { Categories = categories }
                    )
            )