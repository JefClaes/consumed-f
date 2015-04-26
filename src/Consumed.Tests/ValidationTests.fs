module ValidationTests

open Consumed.Contracts
open Consumed.Railway
open Consumed.CommandHandling
open NUnit.Framework
open FsUnit

let assertReturnsEmptyArgument x result =
    match result with
    | Failure(ArgumentEmpty x) -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating consume command without id returns argument empty``() = 
    validateCommand ( Consume ( "", "book", "Game of Thrones", "http://gameofthrones.com" ) )
    |> assertReturnsEmptyArgument "id"
      
[<Test>]
let ``Validating consume command without description returns argument empty``() = 
    validateCommand ( Consume ( "1", "book", "", "http://gameofthrones.com" ) )
    |> assertReturnsEmptyArgument "description"

[<Test>]
let ``Validating consume command with category that does not exist returns out of range``() = 
    let actual = validateCommand ( Consume ( "1", "i_do_not_exist", "", "http://gameofthrones.com" ) )
    match actual with
    | Failure(ArgumentOutOfRange "category") -> Assert.Pass()
    | _ -> Assert.Fail()
   
[<Test>]
let ``Validating consume command without url returns argument empty``() = 
    validateCommand ( Consume ( "1", "book", "Game of Thrones", "" ) )
    |> assertReturnsEmptyArgument "url"
    
[<Test>]
let ``Validating consume command with url in wrong structure returns argument structure``() = 
    let actual = validateCommand ( Consume ( "1", "book", "Game of Thrones", "gameofthrones.com" ) )
    match actual with
    | Failure(ArgumentStructure "url") -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating remove command with empty id returns argument empty``() = 
    validateCommand ( Remove ( "" ) ) |> assertReturnsEmptyArgument "id"
    