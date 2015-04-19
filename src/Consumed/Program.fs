// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
namespace Consumed

open Contracts
open CLIParsing
open Railway
open Handling

module program =

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv
        
        let exec() = 
            match parse argv with
            | Success(Command(cmd)) -> 
            (
                match cmd |> validate >>= handle with
                | Success e -> printfn "Yay! Something happened = %A" e
                | Failure(ArgumentEmpty x) -> printfn "Argument empty = %A" x   
                | Failure(ArgumentStructure x) -> printfn "Argument structure invalid = %A" x
            )  
            | Success(Query(qry)) -> printfn "Query not implemented."     
            | Failure ArgumentsMissing -> printfn "Arguments missing. Expecting at least two arguments."
            | Failure NotFound -> printfn "Could not find command or query. Check arguments."
            | Failure(KeyLooksLikeValue k) -> printfn "Key %s looks like a value" k
            | Failure(KeyMissing k) -> printfn "Key %s missing" k

        exec()

        0 // return an integer exit code
