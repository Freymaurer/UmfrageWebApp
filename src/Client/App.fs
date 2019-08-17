module App

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
open PinIn
open Rating
open BoxAndSend
open ModelMsgs


let pinList = [|
    "h2R4AVMZ"
    "CqK5sA1P"
    "0M6YRQJN"
    "SPY53MDM"
    "KLrY9IZ6"
    "hallo"
  |]

module Server =

    open Shared
    open Fable.Remoting.Client

    /// A proxy you can use to talk to server directly
    let api : ICounterApi =
      Remoting.createApi()
      |> Remoting.withRouteBuilder Route.builder
      |> Remoting.buildProxy<ICounterApi>
let initialCounter = Server.api.initialCounter

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = {
        Pin = ""
        Pageindex = 1
        Modal = emptyModal
        RatingCollector = Some {Question1 = 0; Question2 = 0; Question3 = 0; Question4 = 0}
        Task = None
        AdditionalText = ""
        }
    initialModel, Cmd.none

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel, msg with
    | _, UpdatePin (input) ->
        let nextModel = {
            currentModel with
                Pin = input
        }
        nextModel,Cmd.none
    | _, CheckPin (dispatch) ->
        let nextModel =
            if Array.contains currentModel.Pin pinList
            then {currentModel with
                    Pageindex = 2}
            else {currentModel with
                    Modal = failModal CloseModal dispatch  }
        nextModel,Cmd.none
    | _, CloseModal ->
        let nextModel = {
            currentModel with
                Modal = emptyModal
        }
        nextModel,Cmd.none
    | _, UpdateRatingCollector (inputRatings) ->
        let nextModel = {
            currentModel with
                RatingCollector = Some inputRatings
        }
        nextModel,Cmd.none
    | _, UpdatePageIndex (pageInd) ->
        let nextModel = {
            currentModel with
                Pageindex = pageInd
        }
        nextModel,Cmd.none
    | _, UpdateAdditionalText (inputStr) ->
        let nextModel = {
            currentModel with
                AdditionalText = inputStr
        }
        nextModel,Cmd.none
    | _, UpdateTask (inputTask) ->
        let nextModel = {
            currentModel with
                Task = Some inputTask
        }
        nextModel,Cmd.none
    | _ -> currentModel, Cmd.none

let view (model : Model) (dispatch : Msg -> unit) =
    Hero.hero [ Hero.IsFullHeight;
                Hero.Props [ Style [ BackgroundImage "linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url(\"https://i.imgsafe.org/ba/baa924a5e3.png\")"
                                     Height "50%"
                                     BackgroundPosition "center"
                                     BackgroundRepeat "no-repeat"
                                     BackgroundSize "cover"
                                     Position PositionOptions.Relative ]
                           ]
                ]
        [ Hero.head [ ]
            [ Navbar.navbar [ ]
                [ Container.container [ ]
                    [ navBrand
                    ]
                ]
              br []
              br []
              Heading.h1 [ Heading.Props [ Style [Color "#d9d9d9"] ]
                           Heading.Modifiers [ Modifier.TextAlignment (Screen.All,TextAlignment.Centered) ] ] [ str "Umfrage" ]
              Heading.h4 [ Heading.Props [ Style [Color "#d9d9d9"] ]
                           Heading.IsSubtitle
                           Heading.Modifiers [ Modifier.TextAlignment (Screen.All,TextAlignment.Centered) ]
                         ]
                         [ str "Programmieren für Biologen" ]
            ]

          Hero.body [ Props [ Style [ Props.Display DisplayOptions.Block ] ] ]
            [   Container.container [ Container.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [
                        match model.Pageindex with
                        | 1 -> yield mainPinInModule dispatch
                        | 2 -> yield mainRatingModule model dispatch
                        | 3 -> yield mainBoxAndSendModule model dispatch
                        | _ -> yield str "Alright then, keep your secrets"
                    ]
            ]
          Hero.foot
            [ ]
            [ Text.p
                [ Modifiers [ Modifier.TextColor IsGreyLighter;
                              Modifier.TextAlignment (Screen.All,TextAlignment.Centered) ] ]
                [ safeComponents ]
            ]
          //static page elements
          model.Modal
        ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
