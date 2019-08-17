namespace Shared

type Counter = { Value : int }

module Route =
    /// Defines how routes are generated on server and mapped from client
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type ICounterApi =
    { initialCounter : unit -> Async<Counter> }


type Ratings = {
    Question1 : int
    Question2 : int
    Question3 : int
    Question4 : int
}

type Tasks =
| Excercise1
| Excercise2
| Excercise3
| Excercise4
| Excercise5
| Excercise6
| Excercise7
| Excercise8
| Excercise9
| Excercise10

