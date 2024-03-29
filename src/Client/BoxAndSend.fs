module BoxAndSend

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


let mainBoxAndSendModule (model:Model) (dispatch : Msg -> unit) =
    div [ ]
        [ Level.item [ Level.Item.HasTextCentered
                       Level.Item.Modifiers [ Modifier.TextColor IsWhiteTer ]
                     ]
            [ div []
                [Level.heading [ Modifiers [ Modifier.TextSize (Screen.All,TextSize.Is5) ]
                                 Props [ Style [ TextTransform "none"
                                                 PaddingBottom "5px" ]
                                       ]
                               ]
                   [ str "Ihr wollt uns Vorschläge machen oder einfach nur Frust rauslassen, schreibt es uns!" ]
                 Level.title [ ]
                   [ Media.media [ ]
                       [ Media.left [ ]
                           [ Image.image [ Image.Is64x64 ]
                               [ img [ Src "/csblogo.png" ] ] ]
                         Media.content [ ]
                           [ Field.div [ ]
                               [ Control.div [ ]
                                   [ textarea [ ClassName "textarea"
                                                Placeholder "Add a message ..."
                                                OnChange (fun e -> let x = !!e.target?value
                                                                   dispatch (UpdateAdditionalText x)
                                                         )
                                                Value model.AdditionalText
                                              ]
                                              [ ]
                                   ] 
                               ]
                           ]
                       ]
                   ]
                ]
            ]
          br []
          br []
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
                    [ Button.OnClick (fun _ -> dispatch (WriteSurveyResultsRequest (model.RatingCollector.Value,model.AdditionalText,model.Task.Value,model.Pin,model.TaskArray))) //WriteSurveyResultsRequest GetServertimeRequest
                      (if model.Task.IsNone then Button.IsStatic true else Button.IsStatic false)]
                    [ str "Senden"]
                ]
            ]
        ]
