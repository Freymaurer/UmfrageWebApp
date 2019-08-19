namespace Shared

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
| Loader

module Pins =

    let pinList = [|
        "h2R4AVMZ"
        "CqK5sA1P"
        "0M6YRQJN"
        "SPY53MDM"
        "KLrY9IZ6"
        "9nKE5TL2"
        "Fhu3YMAC"
        "LmnSKr8F"
        "xb9jFV4B"
        "Uk3BBVcU"
        "Te7N4e3V"
        "VFBELp8w"
        "rcUaZS4R"
        "D6NUkBz6"
        "8WAp3yMe"
        "7fENe4rB"
        "JtAMZ6Ys"
        "VAKJn62D"
        "dxuYmA6U"
        "ad7WKjUB"
        "8AhHUwpV"
        "73WvvCWv"
        "UAg5qPrG"
        "L27qQJ6D"
        "erpe4hF5"
        "9r8Q4Jp8"
        "n9jZ3M8M"
        "Bt8J5f5a"
        "Z3Q7eCFK"
        "cwG2Y7Rz"
        "KAA9f3rf"
        "EAg5QmHg"
        "5xyauSVy"
        "V6yz355w"
        "Uk2Wwe4g"
        "QDm7VBNM"
        "xB5XKH3r"
        "dMBXU9RZ"
        "Uxse5Ebx"
        "3ZpG5CS9"
        "vs5jTNfz"
        "ZA2k53jr"
        "vuvrC6Ha"
        "w35Euyfe"
        "zsBtCe7p"
        "MT8PQdGp"
        "tCyU4x2L"
        "vwkWY88m"
        "56Md6dxB"
        "hallo"
      |]

type TaskInfo = {
    Name : string
    TimeNeeded : float option
}

module Route =
    /// Defines how routes are generated on server and mapped from client
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type SurveyAPI =
    { WriteSurveyResult : (Ratings*string*Tasks*string*TaskInfo []) -> Async<string>
      GetServertime : (Ratings*string*Tasks*string) -> Async<string>
      GetTaskScheme : (Tasks) -> Async<string []*Tasks>}