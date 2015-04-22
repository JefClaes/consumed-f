module HandlingTests

open System
open Consumed.Contracts
open Consumed.Railway
open Consumed.Handling
open NUnit.Framework
open FsUnit

[<Test>]
let ``Handling consume command, consumes item``() = 
    let thetime() = new DateTime(2015, 1, 1)
    let timestamp = thetime()
    let actual = handle thetime ( Consume("1", "book", "SQL Performance Explained", "http://sqlperfexplained.com"))
    match actual with
    | Success ( HandleResult.Event ( "consumeditem/1", Event.Consumed(timestamp, "1", "book", "SQL Performance Explained", "http://sqlperfexplained.com" ) ) ) -> Assert.Pass()
    | _ -> Assert.Fail(actual.ToString())

[<Test>]
let ``Handling remove command, removes item``() = 
    let thetime() = new DateTime(2015, 1, 1)
    let timestamp = thetime()
    let actual = handle thetime ( Remove("1") )
    match actual with
    | Success ( HandleResult.Event ( "consumeditem/1", Removed(timestamp, "1" ) ) ) -> Assert.Pass()
    | _ -> Assert.Fail(actual.ToString())