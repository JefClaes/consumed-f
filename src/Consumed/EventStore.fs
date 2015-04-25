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

        writeToDisk ( serialize eventOnDisk  )

        Success eventOnDisk

    let read path stream =       
        let deserialize x = JsonConvert.DeserializeObject<StoredEvent>(x)

        let readFromDisk = File.ReadAllLines path

        match stream with
        | "$all" -> readFromDisk |> Seq.map deserialize
        | _ -> raise (NotSupportedException("Only $all stream supported"))
        

            

      