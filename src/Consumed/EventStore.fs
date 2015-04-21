namespace Consumed

open Contracts

module EventStore =
    let store stream e =

        match e with
        | Consumed ( timestamp, id, category, description, url ) -> ()
        | Removed ( timestamp, id ) -> ()