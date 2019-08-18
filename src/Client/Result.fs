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
            [ str "Nice! Deine Eingaben wurden übermittelt und gespeichert!" ]
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


let mainResultsModule (model:Model) (*(dispatch : Msg -> unit)*) =
    div [ ]
        [ Column.column
            [ Column.Width (Screen.All,Column.IsHalf)
              Column.Offset (Screen.All,Column.IsOneQuarter)]
            [ match model.Result with
              | Some true -> yield positiveResultElements
              | Some false -> yield negativeResultElements
              | None -> yield cheaterResultElements
            ]
        ]
