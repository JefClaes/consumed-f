namespace Consumed

open Contracts
open Railway

module CLIParsing =

    type ParserFailure =
        | ArgumentsMissing 
        | KeyMissing of string
        | KeyLooksLikeValue of string
        | NotFound

    type ParseResult =
        | Command of Command
        | Query of Query
       
    let parse input =
        let ensureEnoughElements input =
            match ( input |> Seq.length > 1 ) with
            | true -> Success input 
            | false -> Failure ArgumentsMissing 
               
        let pair input =    
            input 
            |> Seq.pairwise   
            |> Seq.mapi (fun i x -> if i % 2 = 0 then Some(x) else None)
            |> Seq.choose id               
             
        let ensureKeysDontLookLikeValue ( arguments : seq<string * string> ) =
            let looksLikeValue = 
                arguments 
                |> Seq.tryFind ( fun ( k, v ) -> not (k.StartsWith("-") || k.StartsWith("--")) )
            match looksLikeValue with
            | Some ( key, value ) -> Failure(KeyLooksLikeValue key)
            | None -> Success arguments            

        let stripKeys ( arguments : seq<string * string> ) =        
            arguments |> Seq.map (fun ( k, v ) -> ( k.Replace("-", "").ToLower(), v ))
    
        let ensureKeyExists key arguments =      
            match arguments |> Seq.exists (fun ( k, v ) -> k = key ) with
            | true -> Success arguments
            | false -> Failure(KeyMissing key)

        let toCommandOrQuery arguments =
            match arguments |> Seq.toList with
            | [ ( "n", "consume" ); ("id", id ); ( "d", description ); ( "u", url ) ] ->
                Success(Command(Consume(id, description, url)))
            | [ ( "n", "remove" ); ( "id" , id ) ] ->
                Success(Command(Remove(id)))
            | [ ( "n", "list" )] ->
                Success(Query(List))
            | _ -> Failure NotFound 
              
        input 
            |> ensureEnoughElements
            >>= switch pair
            >>= ensureKeysDontLookLikeValue
            >>= switch stripKeys
            >>= ensureKeyExists "n"
            >>= toCommandOrQuery

