module CommandHandlingTests

open System
open Consumed.Contracts
open Consumed.Railway
open Consumed.CommandHandling
open NUnit.Framework
open FsUnit

[<Test>]
let ``Handling consume command, consumes item``() = 
    let thetime() = new DateTime(2015, 1, 1)
    let timestamp = thetime()
    let actual = handleCommand thetime ( Consume( { Id = "1"; Category = "book"; Description = "SQL Performance Explained"; Url = "http://sqlperfexplained.com" }) )
    match actual with
    | Success ( Event ( "consumeditem/1", Event.Consumed
        {
            Timestamp = timestamp;
            Id = "1";
            Category = "book"; 
            Description = "SQL Performance Explained";
            Url = "http://sqlperfexplained.com"
        } ) ) -> Assert.Pass()
    | _ -> Assert.Fail(actual.ToString())

[<Test>]
let ``Handling remove command, removes item``() = 
    let thetime() = new DateTime(2015, 1, 1)
    let timestamp = thetime()
    let actual = handleCommand thetime ( Remove { Id = "1" } )
    match actual with
    | Success ( Event ( "consumeditem/1", Removed { Timestamp = timestamp; Id = "1" } ) ) -> Assert.Pass()
    | _ -> Assert.Fail(actual.ToString())