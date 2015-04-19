// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
namespace Consumed

open CLIParsing
open Railway

module program =

    [<EntryPoint>]
    let main argv = 
        printfn "%A" argv

        let result = parse argv

        match result with
        | Success cmd -> printfn "Found command"
        | Failure ArgumentsMissing -> printfn "Arguments missing, expecting at least two"
        | Failure CommandNotFound -> printfn "Could not match command"
        | Failure(KeyLooksLikeValue k) -> printfn "Key %A looks like a value" k
        | Failure(KeyMissing k) -> printfn "Key %A missing" k

        0 // return an integer exit code
