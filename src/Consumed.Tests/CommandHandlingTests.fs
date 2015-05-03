module CommandHandlingTests

open System
open Consumed.Contracts
open Consumed.Railway
open Consumed.CommandHandling
open NUnit.Framework
open FsUnit

[<Test>]
let ``Handling consume command, consumes item``() = 
    let read name = EventStream.NotExists name
    let thetime() = new DateTime(2015, 1, 1)
    let timestamp = thetime()
    let actual = handleCommand read thetime ( Consume( { Id = "1"; Category = "book"; Description = "SQL Performance Explained"; Url = "http://sqlperfexplained.com" }) )
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
    let read name = 
        EventStream.Exists 
            ( 
                name, 
                [ 
                    Event.Consumed 
                        { 
                            Timestamp = new DateTime(2014, 1, 1); 
                            Id = "1";
                            Category = "book";
                            Description = "SQL Performance Explained";
                            Url = "http://sqlperfexplained.com"
                        } 
                ]
            )
    let thetime() = new DateTime(2015, 1, 1)
    let timestamp = thetime()
    let actual = handleCommand read thetime ( Remove { Id = "1" } )
    match actual with
    | Success ( Event ( "consumeditem/1", Removed { Timestamp = timestamp; Id = "1" } ) ) -> Assert.Pass()
    | _ -> Assert.Fail(actual.ToString())

[<Test>]
let ``Handling remove command when item hasn't been consumed yet, fails because item does not yet exist``() =
    let read name = EventStream.NotExists name
    let thetime() = new DateTime(2015, 1, 1)
    let actual = handleCommand read thetime ( Remove { Id = "1" } )
    match actual with
    | Failure ItemDoesNotExist -> Assert.Pass()
    | _ -> Assert.Fail()