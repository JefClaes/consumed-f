module EventStoreTests

open Consumed.Contracts
open Consumed.EventStore
open NUnit.Framework
open FsUnit
open System
open System.IO

[<Test>]
let ``specific and $all stream can be stored and read``() =
    let path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "eventstore.txt")
    if (File.Exists path) then
        File.Delete path

    store path "test/1" ( Event.Removed { Timestamp = DateTime.Now; Id = "1" } ) |> ignore
    store path "test/2" ( Event.Removed { Timestamp = DateTime.Now; Id = "2" } ) |> ignore

    let allStream = read path "$all"

    match allStream with
    | EventStream.NotExists name -> Assert.Fail ( sprintf "%s stream does not exist" name )
    | EventStream.Exists ( _ , events) -> events |> Seq.length |> should equal 2

    let testStream1 = read path "test/1"

    match testStream1 with
    | EventStream.NotExists name -> Assert.Fail ( sprintf "%s stream does not exist" name )
    | EventStream.Exists ( _ , events ) -> events |> Seq.length |> should equal 1