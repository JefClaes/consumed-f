namespace Consumed

open System
open System.IO
open Newtonsoft.Json

module EventStore =
      
    let store path stream e =
        let serialize e = JsonConvert.SerializeObject e

        let writeToDisk ( line : string ) = 
            use wr = new StreamWriter(path, true)
            wr.WriteLine(line)

        writeToDisk ( serialize e )

        e

      