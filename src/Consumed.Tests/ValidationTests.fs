module ValidationTests

open Consumed.Contracts
open Consumed.Railway
open Consumed.CommandHandling
open NUnit.Framework
open FsUnit

let assertReturnsEmptyArgument x result =
    match result with
    | Failure(HandlingFailure.Validation(ArgumentEmpty x)) -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating consume command without id returns argument empty``() = 
    validate ( Consume { Id = ""; Category = "book"; Description = "Game of Thrones"; Url = "http://gameofthrones.com" } )
    |> assertReturnsEmptyArgument "id"
      
[<Test>]
let ``Validating consume command without description returns argument empty``() = 
    validate ( Consume { Id =  "1"; Category = "book"; Description = ""; Url = "http://gameofthrones.com" } )
    |> assertReturnsEmptyArgument "description"

[<Test>]
let ``Validating consume command with category that does not exist returns out of range``() = 
    let actual = validate ( Consume { Id = "1"; Category = "i_do_not_exist"; Description = "Game of Thrones"; Url = "http://gameofthrones.com" } )
    match actual with
    | Failure(HandlingFailure.Validation(ArgumentOutOfRange "category")) -> Assert.Pass()
    | _ -> Assert.Fail()
   
[<Test>]
let ``Validating consume command without url returns argument empty``() = 
    validate ( Consume { Id = "1"; Category = "book"; Description = "Game of Thrones"; Url = "" } )
    |> assertReturnsEmptyArgument "url"
    
[<Test>]
let ``Validating consume command with url in wrong structure returns argument structure``() = 
    let actual = validate ( Consume { Id = "1"; Category = "book"; Description = "Game of Thrones"; Url = "gameofthrones.com" } )
    match actual with
    | Failure(HandlingFailure.Validation(ArgumentStructure "url")) -> Assert.Pass()
    | _ -> Assert.Fail()

[<Test>]
let ``Validating remove command with empty id returns argument empty``() = 
    validate ( Remove { Id = "" } ) |> assertReturnsEmptyArgument "id"
    