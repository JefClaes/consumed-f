module ValidationTests

open Consumed.Contracts
open Consumed.Railway
open Consumed.Validation
open NUnit.Framework
open FsUnit

let assertReturnsEmptyArgument x result =
    match result with
    | Failure(ValidationFailure.ArgumentEmpty x) -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating consume command without id returns argument empty``() = 
    validateCommand( Consume ( "", "Game of Thrones", "http://gameofthrones.com" ) )
    |> assertReturnsEmptyArgument "id"
      
[<Test>]
let ``Validating consume command without description returns argument empty``() = 
    validateCommand( Consume ( "1", "", "http://gameofthrones.com" ) )
    |> assertReturnsEmptyArgument "description"
   
[<Test>]
let ``Validating consume command without url returns argument empty``() = 
    validateCommand( Consume ( "1", "Game of Thrones", "" ) )
    |> assertReturnsEmptyArgument "url"
    
[<Test>]
let ``Validating consume command with url in wrong structure returns argument structure``() = 
    let actual = validateCommand( Consume ( "1", "Game of Thrones", "gameofthrones.com" ) )
    match actual with
    | Failure(ValidationFailure.ArgumentStructure "url") -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating remove command with empty id returns argument empty``() = 
    validateCommand( Remove ( "" ) ) |> assertReturnsEmptyArgument "id"
    