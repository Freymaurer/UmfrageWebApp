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
open ModelMsgs


module Server =

    open Shared
    open Fable.Remoting.Client

    /// A proxy you can use to talk to server directly
    let apiSurvey : SurveyAPI =
      Remoting.createApi()
      |> Remoting.withRouteBuilder Route.builder
      |> Remoting.buildProxy<SurveyAPI>


// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = {
        Pin = ""
        Pageindex = 1
        Modal = PinIn.emptyModal
        RatingCollector = Some {Question1 = 0; Question2 = 0; Question3 = 0; Question4 = 0}
        Task = None
        AdditionalText = ""
        Debug = ""
        Result = None
        }
    initialModel, Cmd.none

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel, msg with

    | _, GetServertimeRequest (rating,addTxt,task,pin) ->
        let requestCmd =
            Cmd.OfAsync.either
                Server.apiSurvey.GetServertime
                (rating,addTxt,task,pin)
                (Ok >> GetServertimeResponse)
                (Error >> GetServertimeResponse)
        currentModel,requestCmd
    | _, GetServertimeResponse (Ok value) ->
        let nextModel = {
            currentModel with
                Debug = value
        }
        nextModel,Cmd.none
    | _,GetServertimeResponse (Error e) ->
        let nextModel = {
            currentModel with
                Debug = e.Message
        }
        nextModel,Cmd.none

    | _, WriteSurveyResultsRequest (rating,addTxt,task,pin) ->
        let requestCmd =
            Cmd.OfAsync.either
                Server.apiSurvey.WriteSurveyResult
                (rating,addTxt,task,pin)
                (Ok >> WriteSurveyResultsResponse)
                (Error >> WriteSurveyResultsResponse)
        currentModel,requestCmd
    | _, WriteSurveyResultsResponse (Ok value) ->
        let nextModel = {
            currentModel with
                Debug = value
                Result = Some true
                Pageindex = 4
        }
        nextModel,Cmd.none
    | _,WriteSurveyResultsResponse (Error e) ->
        let nextModel = {
            currentModel with
                Debug = e.Message
                Result = Some false
                Pageindex = 4
        }
        nextModel,Cmd.none

    | _, UpdatePin (input) ->
        let nextModel = {
            currentModel with
                Pin = input
        }
        nextModel,Cmd.none
    | _, CheckPin (dispatch) ->
        let nextModel =
            if Array.contains currentModel.Pin Pins.pinList
            then {currentModel with
                    Pageindex = 2}
            else {currentModel with
                    Modal = PinIn.failModal CloseModal dispatch  }
        nextModel,Cmd.none
    | _, CloseModal ->
        let nextModel = {
            currentModel with
                Modal = PinIn.emptyModal
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
        [ Hero.head [ Props [ Style [ Background "rgba(51, 153, 255, 0.15)"] ] ]
            [ Navbar.navbar [ ]
                [ Container.container [ ]
                    [ PinIn.navBrand
                    ]
                ]
              br []
              br []
              Heading.h1 [ Heading.Props [ Style [Color "#d9d9d9"] ]
                           Heading.Modifiers [ Modifier.TextAlignment (Screen.All,TextAlignment.Centered)
                                               Modifier.TextSize (Screen.All,TextSize.Is1) ] ] [ strong [] [ str "Umfrage" ] ]
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
                        | 1 -> yield PinIn.mainPinInModule dispatch
                        | 2 -> yield Rating.mainRatingModule model dispatch
                        | 3 -> yield BoxAndSend.mainBoxAndSendModule model dispatch
                        | 4 -> yield Result.mainResultsModule model
                        | _ -> yield str "Alright then, keep your secrets"
                    ]
            ]
          Hero.foot
            [ Props [ Style [ Background "rgba(51, 153, 255, 0.15)"] ] ]
            [ Text.p
                [ Modifiers [ Modifier.TextColor IsGreyLighter;
                              Modifier.TextAlignment (Screen.All,TextAlignment.Centered) ] ]
                [ PinIn.safeComponents ]
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
