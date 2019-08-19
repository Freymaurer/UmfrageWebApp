module PinIn

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

let [<Literal>] ENTER_KEY = 13.
let onEnter msg (dispatch : Msg -> unit) =
    OnKeyDown (fun ev ->
        if ev.keyCode = ENTER_KEY then
            dispatch msg)

let emptyModal =
    Modal.modal [Modal.IsActive false] []

let failModal (msg:Msg) dispatch =
    Modal.modal [ Modal.IsActive true
                  Modal.Props [ onEnter msg dispatch ] //does not work, dont know why
                ]
        [ Modal.background [ Props [ OnClick (fun _ -> dispatch msg) ] ] [ ]   
          Modal.Card.card [ ]
            [ Modal.Card.head [ ]
                [ Modal.Card.title [ ]
                    [ str "Upsi!" ] ]
              Modal.Card.body [ ]
                              [ Text.p [Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Justified)] ]
                                       [str "Das scheint kein korrekter Pin zu sein! Bitte überprüfe deine Eingabe und sollte das Problem bestehen beschwere dich bei dem Praktikumsleiter ;)" ]
                              ]
              Modal.Card.foot [ ]
                [ Button.button [ Button.OnClick (fun _ -> dispatch msg) ]
                    [ str "Zurück" ] ]
            ]
        ]


let safeComponents =
    let components =
        span [ ]
           [ a [ Href "https://github.com/SAFE-Stack/SAFE-template" ]
               [ strong [ Style [Color "#d9d9d9"] ] [ str "SAFE  " ]
                 strong [ Style [Color "#d9d9d9"] ] [ str Version.template ] ]
             str ", "
             a [ Href "http://suave.io"; Style [Color "#ff80b3"] ] [ str "Suave" ]
             str ", "
             a [ Href "http://fable.io"; Style [Color "#ff80b3"] ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io"; Style [Color "#ff80b3"] ] [ str "Elmish" ]
             str ", "
             a [ Href "https://fulma.github.io/Fulma"; Style [Color "#ff80b3"] ] [ str "Fulma" ]
             str ", "
             a [ Href "https://bulmatemplates.github.io/bulma-templates/"; Style [Color "#ff80b3"] ] [ str "Bulma\u00A0Templates" ]
             str ", "
             a [ Href "https://zaid-ajaj.github.io/Fable.Remoting/"; Style [Color "#ff80b3"] ] [ str "Fable.Remoting" ]

           ]

    span
        [ ]
        [ str "Version "
          strong [ Style [Color "#d9d9d9"] ] [ str Version.app ]
          str " powered by: "
          components ]

let containerBox (dispatch : Msg -> unit) =
    Box.box' [ Props [ onEnter (CheckPin dispatch) dispatch ] ]
        [ Field.div [ Field.IsGrouped ]
            [ Control.p [ Control.IsExpanded ]
                [ Input.password
                    [ Input.Placeholder "Pin"
                      Input.OnChange (fun e -> let x = !!e.target?value
                                               dispatch (UpdatePin x)
                                     )
                    ]
                ] 
              Control.p [ ]
                [ Button.a
                    [ Button.Color IsInfo
                      Button.OnClick (fun _ -> dispatch (CheckPin dispatch)) ]
                    [ Icon.icon [ ]
                        [ i [ClassName "fas fa-sign-in-alt"] [] ]
                    ]
                ]
            ]
        ]

let mainPinInModule dispatch =
    Column.column
        [ Column.Width (Screen.All, Column.Is6)
          Column.Offset (Screen.All, Column.Is3) ]
        [ Container.container
            [ ]
            [ strong [ Style [ Color "#d9d9d9" ] ] [str "Der zugewiesene Pin stellt sicher, dass pro Pin nur einmal abgestimmt wird. Es wird für jede Aufgabe und jeden Pin immer die letzte Abstimmung ausgewertet. Die Umfrage verläuft absolut anonym!" ] ]
          br []
          containerBox dispatch ]
