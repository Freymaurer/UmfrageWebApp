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
        [ question "War die Vorlesung verständlich?" 1 model dispatch
          question "Waren die Aufgabenstellungen der Tasks verständlich?" 2 model dispatch
          question "Konntet ihr die Tasks mit Hilfe der Vorlesung lösen?" 3 model dispatch
          question "Waren die Tasks zu schwer?" 4 model dispatch ]

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
    div []
        [ br []
          Columns.columns
              [ Columns.IsCentered ]
              [   Column.column
                      [ Column.Width (Screen.All, Column.IsThreeFifths)]
                      [ ratings model dispatch]
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
                        (if checkForward model then Button.IsStatic false else Button.IsStatic true)]
                      [ str "Weiter"]
                  ]
              ]
        ]
