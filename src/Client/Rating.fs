module Rating

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

open Browser


let createRatings (prevRating:Ratings) questionNumber rating =
    match questionNumber with
    |1 -> { prevRating with Question1 = rating }
    |2 -> { prevRating with Question2 = rating }
    |3 -> { prevRating with Question3 = rating }
    |4 -> { prevRating with Question4 = rating }
    | _ -> failwith "Question requested is not known."

let checkCurrentRating (prevRating:Ratings) questionNumber =
    match questionNumber with
    |1 -> prevRating.Question1
    |2 -> prevRating.Question2
    |3 -> prevRating.Question3
    |4 -> prevRating.Question4
    | _ -> failwith "Question requested is not known."

let question questionTxt qoi (model:Model) dispatch =
    let currentRating = checkCurrentRating model.RatingCollector.Value qoi
    let emptyOrFilledStar starNo =
        if currentRating >= starNo
        then Icon.icon [ Icon.Modifiers [ Modifier.TextColor IsDanger] ]
                [ i [ ClassName "fas fa-star"] [] ]
        else Icon.icon [ ]
                [ i [ ClassName "far fa-star"] [] ]
    let starButton starNo =
        Button.span
            [ Button.Color IsInfo
              Button.IsInverted
              Button.OnClick (fun _ -> let newRatings = createRatings model.RatingCollector.Value qoi starNo
                                       dispatch (UpdateRatingCollector newRatings)
                             )
            ]
            [ emptyOrFilledStar starNo ]
    Level.level [ ]
        [ Level.item [ Level.Item.HasTextCentered
                       Level.Item.Modifiers [ Modifier.TextColor IsWhiteTer ]
                       Level.Item.Props [ Style [ BorderBottom "1px solid lightblue"
                                                  PaddingBottom "15px"
                                                  Width "100%"] ] ]
            [ div [ ]
                [ Level.heading [ Modifiers [ Modifier.TextSize (Screen.All,TextSize.Is5) ]
                                  Props [ Style [ TextTransform "none"
                                                  PaddingBottom "5px" ]
                                        ]
                                ]
                    [ str questionTxt ]
                  Level.title [ Modifiers [ Modifier.TextColor IsWhiteTer ] ]
                    [ Button.list
                        [ Button.List.HasAddons; Button.List.IsCentered ]
                        [ starButton 1
                          starButton 2
                          starButton 3
                          starButton 4
                          starButton 5
                        ]
                    ]
                ]
            ]
        ]

let ratings model dispatch=
    form
        [ ]
        [ question "Fandet Ihr die Aufgaben für den gewählten Task zu schwer?" 1 model dispatch
          question "Hättet Ihr euch eine verstärkte Betreung gewünscht?" 2 model dispatch
          question "Wurden Euch genügen Ressourcen genannt um die Aufgaben lösen zu können?" 3 model dispatch
          question "Waren die Themen und Anwendungsmöglichkeiten gut erklärt?" 4 model dispatch ]

let checkForward (model:Model) =
    let ranged1To5 num = if num < 1 || num > 5 then false else true
    if model.Task.IsSome && model.RatingCollector.IsSome
        then if ranged1To5 model.RatingCollector.Value.Question1 &&
                ranged1To5 model.RatingCollector.Value.Question2 &&
                ranged1To5 model.RatingCollector.Value.Question3 &&
                ranged1To5 model.RatingCollector.Value.Question4
             then true
             else false
        else false

let mainRatingModule (model:Model) (dispatch : Msg -> unit) =
    let currentTask =
        if model.Task.IsSome
        then match model.Task.Value with | Excercise1 -> "Aufgabe 1" | Excercise2 -> "Aufgabe 2" | Excercise3 -> "Aufgabe 3" | Excercise4 -> "Aufgabe 4" | Excercise5 -> "Aufgabe 5" | Excercise6 -> "Aufgabe 6" | Excercise7 -> "Aufgabe 7"
                                         | Excercise8 -> "Aufgabe 8" | Excercise9 -> "Aufgabe 9" | Excercise10 -> "Aufgabe 10" | _ -> "Wählt die zu bewertende Aufgabe"
        else ""
    let dropdownItemTask taskNr task=
        Dropdown.Item.a [ Dropdown.Item.Props [ OnClick (fun _ -> dispatch (UpdateTask task)) ] ]
            [ str (sprintf "Aufgabe %i" taskNr) ]
    div []
        [ Columns.columns
            [ Columns.IsCentered ]
            [ Column.column
                [ ]
                [ Dropdown.dropdown [ Dropdown.IsHoverable;
                                      Dropdown.Props [ Id "DropdownMenu"
                                                       OnMouseOver (fun _ -> let dropdownElement = document.getElementById "DropdownMenu"
                                                                             dropdownElement?style?border <- "1.5px solid #ff66b3"
                                                                             dropdownElement?style?borderRadius <- "3px"
                                                                   )
                                                       OnMouseLeave (fun _ -> let dropdownElement = document.getElementById "DropdownMenu"
                                                                              dropdownElement?style?border <- "1px solid grey"
                                                                              dropdownElement?style?borderRadius <- "3px") ]
                                      Dropdown.IsRight ]
                      [ div [ Style [ Width "100%" ] ]
                          [ Button.button [  ] 
                              [ span
                                    [  ]
                                    [ str (if model.Task.IsNone then "Aufgabe" else currentTask) ]
                                Icon.icon [ Icon.Size IsSmall ]
                                          [ Fa.i [ Fa.Solid.AngleDown ]
                                                 [ ]
                                          ]
                                       
                              ]
                              
                          ]
                        Dropdown.menu [ ]
                          [ Dropdown.content [ ]
                              [ dropdownItemTask 1 Excercise1
                                dropdownItemTask 2 Excercise2
                                dropdownItemTask 3 Excercise3
                                dropdownItemTask 4 Excercise4
                                dropdownItemTask 5 Excercise5
                                Dropdown.divider []
                                dropdownItemTask 6 Excercise6
                                dropdownItemTask 7 Excercise7
                                dropdownItemTask 8 Excercise8
                                dropdownItemTask 9 Excercise9
                                dropdownItemTask 10 Excercise10
                              ]
                          ]
                      ]
                ]
            ]
          br []
          Columns.columns
              [ Columns.IsCentered ]
              [   Column.column
                      [ Column.Width (Screen.All, Column.IsThreeFifths)]
                      [ ratings model dispatch]
              ]
          br []
          Columns.columns
              [ Columns.IsCentered ]
              [ Column.column
                  []
                  [ Button.a
                      [ Button.OnClick (fun _ -> dispatch (UpdatePageIndex 3))
                        (if checkForward model then Button.IsStatic false else Button.IsStatic true)]
                      [ str "Weiter"]
                  ]
              ]
        ]
