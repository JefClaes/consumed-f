// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
namespace Consumed

open Contracts
open CLIParsing
open Railway
open CommandHandling
open QueryHandling
open EventStore

module program =

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv
      
        let path = "d:\\store.txt"

        let exec() = 
            match parse argv with
            | Success(Command(cmd)) -> 
                (
                    let handleCommand cmd = 
                        cmd 
                        |> validateCommand
                        >>= handleCommand thetime 
                        >>= switch ( handleCommandSideEffects (store path) )

                    match handleCommand cmd with
                    | Success e -> printfn "Yay! Something happened = %A" e
                    | Failure(ArgumentEmpty x) -> printfn "Argument empty = %A" x   
                    | Failure(ArgumentStructure x) -> printfn "Argument structure invalid = %A" x
                    | Failure(ArgumentOutOfRange x) -> printfn "Argument out of range = %A" x
                )  
            | Success(Query(query)) ->
                (
                    let list = handleQuery (read path) query

                    for c in list.Categories do
                        printfn "%s" c.Name
                        for i in c.Items do
                            let ts = i.Timestamp.ToString("dd/MM/yyyy")
                            printfn "%s - %s | %s (%s)" ts i.Id i.Description i.Url
                )
            | Failure ArgumentsMissing -> printfn "Arguments missing. Expecting at least two arguments."
            | Failure NotFound -> printfn "Could not find command or query. Check arguments."
            | Failure(KeyLooksLikeValue k) -> printfn "Key %s looks like a value" k
            | Failure(KeyMissing k) -> printfn "Key %s missing" k

        exec()

        System.Console.ReadLine() |> ignore

        0 // return an integer exit code
