module Result

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


let positiveResultElements =
    Box.box'
        [ ]
        [ strong
            [ ]
            [ str "Deine Eingaben wurden übermittelt und gespeichert! Du kannst die Website nun verlassen oder weiter Übungen bewerten." ]
        ]

let negativeResultElements =
    Box.box'
        [ ]
        [ strong
            [ ]
            [ str "Upsi. Da scheint es ein Problem bei der Übertragung deiner Daten gegeben zu haben. Probier es später erneut oder sag den Betreuern bescheid!" ]
        ]

let cheaterResultElements =
    Box.box'
        [ ]
        [ strong
            [ ]
            [ str "Hmmm. Du solltest noch nicht hier sein.. entweder du hast geschummelt oder das Programm ist kaputt. All right, then. Keep your secrets." ]
        ]

let baseModel = {
    Pin = ""
    Pageindex = 2
    Modal = PinIn.emptyModal
    RatingCollector = Some {Question1 = 0; Question2 = 0; Question3 = 0; Question4 = 0}
    Task = None
    AdditionalText = ""
    Debug = ""
    Result = None
    TaskArray = [||]
    }

let mainResultsModule (model:Model) (dispatch : Msg -> unit) =
    div [ ]
        [ Column.column
            [ Column.Width (Screen.All,Column.IsHalf)
              Column.Offset (Screen.All,Column.IsOneQuarter)]
            [ match model.Result with
              | Some true -> yield positiveResultElements
              | Some false -> yield negativeResultElements
              | None -> yield cheaterResultElements
            ]
          Column.column
            [ Column.Width (Screen.All,Column.IsHalf)
              Column.Offset (Screen.All,Column.IsOneQuarter) ]
            [ Button.button
                [ Button.OnClick (fun _ -> let newModel = { baseModel with Pin = model.Pin }
                                           dispatch (UpdateModel newModel)
                                 ) ]
                [ str "Weitere Übungen bewerten" ]
            ]
        ]
