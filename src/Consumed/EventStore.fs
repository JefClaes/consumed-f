namespace Consumed

open System
open System.IO
open Newtonsoft.Json
open Railway

module EventStore =
      
    type EventOnDisk =  { Stream : string; Event : Object  }

    let store path stream e =
        let serialize e = JsonConvert.SerializeObject e

        let writeToDisk ( line : string ) = 
            use wr = new StreamWriter(path, true)
            wr.WriteLine(line)       
        
        let eventOnDisk = { Stream = stream; Event = e } 

        writeToDisk ( serialize eventOnDisk  )

        Success eventOnDisk

      