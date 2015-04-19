module ValidationTests

open Consumed.Contracts
open Consumed.Railway
open Consumed.Handling
open NUnit.Framework
open FsUnit

let assertReturnsEmptyArgument x result =
    match result with
    | Failure(ArgumentEmpty x) -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating consume command without id returns argument empty``() = 
    validate( Consume ( "", "Game of Thrones", "http://gameofthrones.com" ) )
    |> assertReturnsEmptyArgument "id"
      
[<Test>]
let ``Validating consume command without description returns argument empty``() = 
    validate( Consume ( "1", "", "http://gameofthrones.com" ) )
    |> assertReturnsEmptyArgument "description"
   
[<Test>]
let ``Validating consume command without url returns argument empty``() = 
    validate( Consume ( "1", "Game of Thrones", "" ) )
    |> assertReturnsEmptyArgument "url"
    
[<Test>]
let ``Validating consume command with url in wrong structure returns argument structure``() = 
    let actual = validate( Consume ( "1", "Game of Thrones", "gameofthrones.com" ) )
    match actual with
    | Failure(ArgumentStructure "url") -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating remove command with empty id returns argument empty``() = 
    validate( Remove ( "" ) ) |> assertReturnsEmptyArgument "id"
    