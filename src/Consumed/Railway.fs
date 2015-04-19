namespace Consumed

module Railway =

    // convert a single value into a two-track result
    // the two-track type
    type Result<'TSuccess,'TFailure> = 
        | Success of 'TSuccess
        | Failure of 'TFailure

    // convert a single value into a two-track result
    let succeed x = 
        Success x

    // convert a single value into a two-track result
    let fail x = 
        Failure x

    // apply either a success function or failure function
    let either successFunc failureFunc twoTrackInput =
        match twoTrackInput with
        | Success s -> successFunc s
        | Failure f -> failureFunc f

    // convert a switch function into a two-track function
    let bind f = either f fail

    // pipe a two-track value into a switch function 
    let (>>=) x f = bind f x

    // compose two switches into another switch
    let (>=>) s1 s2 = s1 >> bind s2

    // convert a one-track function into a switch
    let switch f = f >> succeed

    // convert a one-track function into a two-track function
    let map f = either (f >> succeed) fail

    // convert a dead-end function into a one-track function
    let tee f x = f x; x 