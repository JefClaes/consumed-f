// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
namespace Consumed

open System
open System.IO
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
                  
        if ((argv |> Seq.length) = 1 && argv.[0] = "-help") then
            printfn "Following commands are available:"
            printfn "-n consume -id id -c category -d description -u url"
            printfn "-n remove -id id"
            printfn "-n list"
            Environment.Exit(0)

        let path = "C:\\store.txt"

        if not (File.Exists path) then File.Create path |> ignore

        let exec() = 
            match parse argv with
            | Success(ParseResult.Command(cmd)) -> 
                (
                    let read stream = read path stream
                    let handleCommand cmd = 
                        cmd 
                        |> validate
                        >>= CommandHandling.handle read thetime 
                        >>= switch ( sideEffects (store path) )

                    match handleCommand cmd with
                    | Success e 
                        -> printfn "Yay! Something happened = %A" e
                    | Failure(Command(ItemAlreadyConsumed)) 
                        -> printfn "Item was already consumed"
                    | Failure(Command(ItemDoesNotExist)) 
                        -> printfn "Item does not exist"
                    | Failure(Validation(ArgumentEmpty x)) 
                        -> printfn "Argument empty = %A" x
                    | Failure(Validation(ArgumentStructure x)) 
                        -> printfn "Argument structure invalid = %A" x
                    | Failure(Validation(ArgumentOutOfRange x)) 
                        -> printfn "Argument out of range = %A" x
                )
            | Success(Query(query)) ->
                (
                    let list = QueryHandling.handle (read path) query

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

        Console.ReadLine() |> ignore

        0 // return an integer exit code