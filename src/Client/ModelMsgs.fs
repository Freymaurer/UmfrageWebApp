module ModelMsgs

open Elmish
open Elmish.React
open Fable.FontAwesome
open Fable.FontAwesome.Free
open Fable.React
open Fable.React.Props
open Fulma
open Thoth.Json
open Fable.Core.JsInterop

open Shared

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = {
    Pin : string
    Pageindex : int
    Modal : ReactElement
    RatingCollector : Ratings option
    Task : Tasks option
    AdditionalText : string
    }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
    | Increment
    | Decrement
    | InitialCountLoaded of Counter
    | UpdatePin of string
    | CheckPin of (Msg -> unit)
    | CloseModal
    | UpdateRatingCollector of Ratings
    | UpdatePageIndex of int
    | UpdateAdditionalText of string
    | UpdateTask of Tasks
    | WriteSurveyResultsRequest of (Ratings*string*Tasks*string)
    | WriteSurveyResultsResponse of Result<unit,exn>

