module ParsingTests

open Consumed.Railway
open Consumed.CLIParsing
open Consumed.Contracts
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
    | Success(Command(x)) -> x |> should equal expected
    | _ -> Assert.Fail()
      
[<Test>]
let ``Parsing consume command``() =  
    let expected = Consume("2", "The Dark Tower", "http://thedarktower.com")
    let actual = parse [| "--n"; "consume"; "--id"; "2"; "--d"; "The Dark Tower"; "--u"; "http://thedarktower.com"; |]
    match actual with
    | Success(Command(x)) -> x |> should equal expected
    | _ -> Assert.Fail() 

[<Test>]
let ``Parsing remove command``() =  
    let id = "1"
    let expected = Remove(id)
    let actual = parse [| "--n"; "remove"; "--id"; id |]
    match actual with
    | Success(Command(s)) -> s |> should equal expected
    | _ -> Assert.Fail() 

[<Test>]
let ``Parsing list query``() =  
    let expected = Query.List
    let actual = parse [| "--n"; "list"; |]
    match actual with
    | Success(Query(s)) -> s |> should equal expected
    | _ -> Assert.Fail() 

    
[<Test>]
let ``Parsing when command not found fails w CommandNotFound``() =  
    match parse [| "--n"; "i_do_not_exist"; |] with
    | Failure(ParserFailure.NotFound) -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Parsing when command looks like value fails w KeyLooksLikeValue``() =  
    match parse [| "n"; "consume"; |] with
    | Failure(KeyLooksLikeValue "n") -> Assert.Pass()
    | _ -> Assert.Fail()