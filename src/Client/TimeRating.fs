module TimeRating

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

open System
open System.IO



let timeRating strText (model:Model) dispatch  =
    let relatedTask =
        model.TaskArray
        |> Array.find (fun x -> x.Name = strText)
        |> fun x -> if x.TimeNeeded.IsSome then string x.TimeNeeded.Value else ""
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
                    [ str strText ]
                  Level.title [ Modifiers [ Modifier.TextColor IsWhiteTer ] ]
                    [ Input.number
                        [ Input.OnChange (fun e -> let x = !!e.target?value
                                                   dispatch (UpdateTimeRating (strText,x)) )
                          Input.Value relatedTask ]
                    ]
                ]
            ]
        ]

let timeRatings (model:Model) dispatch=
    let taskElementArr = model.TaskArray |> Array.map (fun x -> timeRating x.Name model dispatch)
    form
        [ ]
        taskElementArr

let checkForwardTimeRating (model:Model) =
    model.TaskArray
    |> Array.tryFind (fun x -> x.TimeNeeded = None)
    |> fun x -> x.IsNone

let mainTimeRatingModule (model:Model) (dispatch : Msg -> unit) =
    let currentTask =
        if model.Task.IsSome
        then match model.Task.Value with | Excercise1 -> "Übung 1" | Excercise2 -> "Übung 2" | Excercise3 -> "Übung 3" | Excercise4 -> "Übung 4" | Excercise5 -> "Übung 5" | Excercise6 -> "Übung 6" | Excercise7 -> "Übung 7"
                                         | Excercise8 -> "Übung 8" | Excercise9 -> "Übung 9" | Excercise10 -> "Übung 10" | _ -> "Missing"
        else ""
    let dropdownItemTask taskNr task=
        Dropdown.Item.a [ Dropdown.Item.Props [ OnClick (fun _ -> dispatch (UpdateTaskRequest task)) ] ]
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
                                           [ str (if model.Task.IsNone then "Übung" else currentTask) ]
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
                      [ strong [ Style [ Color "white" ] ]
                               [str "Um für die Zukunft den Arbeitsaufwand besser einschätzen zu können bitten wir euch, die von euch benötigte Zeit pro Task anzugeben (in Minuten)."]]
              ]
          br []
          Columns.columns
              [ Columns.IsCentered ]
              [ if model.Task = Some Loader
                then yield strong [ Style [Color "white"]] [str ".. loading"]
                elif model.Task = Some FailedLoad
                then yield Box.box' [] [strong [ ] [str "Zu der gewünschten Aufgabe gibt es noch keine Task Liste."]]
                else yield Column.column
                            [ Column.Width (Screen.All, Column.IsThreeFifths)]
                            [ timeRatings model dispatch]
              ]
          br []
          Columns.columns
              [ ]
              [ Column.column
                    [ Column.Offset (Screen.All,Column.IsOneQuarter)
                      Column.Width (Screen.All,Column.IsOneQuarter) ]
                    [ Button.a
                        [ Button.OnClick (fun _ -> dispatch (UpdatePageIndex (model.Pageindex - 1))) ]
                        [ str "Zurück"]
                    ]
                Column.column
                  [ Column.Width (Screen.All,Column.IsOneQuarter) ]
                  [ Button.a
                      [ Button.OnClick (fun _ -> dispatch (UpdatePageIndex (model.Pageindex + 1)))
                        (if checkForwardTimeRating model then Button.IsStatic false else Button.IsStatic true)]
                      [ str "Weiter"]
                  ]
              ]
        ]
