namespace Consumed

open System
open Contracts
open System.IO
open Newtonsoft.Json
open Railway

module EventStore =
      
    type StoredEvent =  { Stream : string; Body : Event  }

    let store path stream e =
        let serialize e = JsonConvert.SerializeObject e

        let writeToDisk ( line : string ) = 
            use wr = new StreamWriter(path, true)
            wr.WriteLine(line)       
        
        let eventOnDisk = { Stream = stream; Body = e } 

        serialize eventOnDisk |> writeToDisk |> Success

    let read path stream =       
        let deserialize x = JsonConvert.DeserializeObject<StoredEvent>(x)

        let readFromDisk = File.ReadAllLines path |> Seq.map deserialize

        let eventsFromDisk = 
            match stream with
            | "$all" -> readFromDisk 
            | _ -> readFromDisk |> Seq.filter (fun e -> e.Stream = stream)

        match eventsFromDisk |> Seq.isEmpty with
        | true -> EventStream.NotExists stream
        | false -> EventStream.Exists (stream, eventsFromDisk |> Seq.map (fun e -> e.Body))