module Tests

open Consumed.Railway
open Consumed.CLIParsing
open Consumed.Commands
open NUnit.Framework
open FsUnit
    
[<Test>]
let ``Parsing without arguments fails w ArgumentsMissing``() =  
    match parse [| |] with
    | Failure ArgumentsMissing -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Parsing with an extra key ignores the key``() = 
    let expected = Consume("1", "The Dark Tower", "http://thedarktower.com")
    let actual = parse [| "--n"; "consume"; "--id"; "1"; "--d"; "The Dark Tower"; "--u"; "http://thedarktower.com"; "--z" |]
    match actual with
    | Success x -> x |> should equal expected
    | Failure f -> Assert.Fail(f.ToString())  
      
[<Test>]
let ``Parsing input returns typed command``() =  
    let expected = Consume("2", "The Dark Tower", "http://thedarktower.com")
    let actual = parse [| "--n"; "consume"; "--id"; "2"; "--d"; "The Dark Tower"; "--u"; "http://thedarktower.com"; |]
    match actual with
    | Success x -> x |> should equal expected
    | Failure f -> Assert.Fail(f.ToString()) 

[<Test>]
let ``Parsing remove command``() =  
    let id = "1"
    let expected = Remove(id)
    let actual = parse [| "--n"; "remove"; "--id"; id |]
    match actual with
    | Success s -> s |> should equal expected
    | Failure f -> Assert.Fail(f.ToString()) 
    
[<Test>]
let ``Parsing when command not found fails w CommandNotFound``() =  
    match parse [| "--n"; "i_do_not_exist"; |] with
    | Failure(CommandNotFound) -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Parsing when command looks like value fails w KeyLooksLikeValue``() =  
    match parse [| "n"; "consume"; |] with
    | Failure(KeyLooksLikeValue "n") -> Assert.Pass()
    | _ -> Assert.Fail()