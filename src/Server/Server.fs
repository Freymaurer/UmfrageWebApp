open System.IO
open System
open System.Reflection
open System.Net

open Shared

open Suave
open Suave.Files
open Suave.Successful
open Suave.Filters
open Suave.Operators
open Suave.Logging

open Fable.Remoting.Server
open Fable.Remoting.Suave


let writeSurvey (rating:Ratings) (additionalTxt:string) (task:Tasks) (pin:string) =
    if Array.contains pin Pins.pinList |> not then failwith "Pin not found in pinlist. Error 01."
    let datetime = (System.DateTime.Now.ToString()) |> fun x -> x.Replace (" ","_") |> fun x -> x.Replace (":","-")
    let infotxt =
        sprintf
            "%s\t%i\t%i\t%i\t%i\t|||%s|||\t"
            (string task) rating.Question1 rating.Question2 rating.Question3 rating.Question4 additionalTxt
    let txtName =
        sprintf "SurveyResults/%s_%s_%s.txt" pin (string task) datetime
    let baseDirectory = __SOURCE_DIRECTORY__
    let fullPath = Path.Combine(baseDirectory, txtName)
    File.WriteAllText(fullPath,infotxt)

let giveServerPath (rating:Ratings) (additionalTxt:string) (task:Tasks) (pin:string) =
    //let datetime = (System.DateTime.Now.ToString()) |> fun x -> x.Replace (" ","_") |> fun x -> x.Replace (":","-")
    let txtName =
       sprintf "SurveyResults\%s_%s.txt" pin (string task) (*datetime*) //_%s
    let baseDirectory = __SOURCE_DIRECTORY__
    let fullPath = Path.Combine(baseDirectory, txtName)
    let infotxt =
        sprintf
            "%s\t%i\t%i\t%i\t%i\t|||%s|||\t"
            (string task) rating.Question1 rating.Question2 rating.Question3 rating.Question4 additionalTxt
    fullPath + infotxt
    //File.WriteAllText(fullPath,"Test")

module ServerPath =
    let workingDirectory =
        let currentAsm = Assembly.GetExecutingAssembly()
        let codeBaseLoc = currentAsm.CodeBase
        let localPath = Uri(codeBaseLoc).LocalPath
        Directory.GetParent(localPath).FullName

    let resolve segments =
        let paths = Array.concat [| [| workingDirectory |]; Array.ofList segments |]
        Path.GetFullPath(Path.Combine(paths))

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x
let publicPath = ServerPath.resolve [".."; "Client"; "public"]
let port = tryGetEnv "HTTP_PLATFORM_PORT" |> Option.map System.UInt16.Parse |> Option.defaultValue 8085us

let loggingOptions =
  { Literate.LiterateOptions.create() with
      getLogLevelText = function Verbose->"V" | Debug->"D" | Info->"I" | Warn->"W" | Error->"E" | Fatal->"F"
  }

let logger = LiterateConsoleTarget(
                name = [|"Suave";"Examples";"Example"|],
                minLevel = Verbose,
                options = loggingOptions,
                outputTemplate = "[{level}] {timestampUtc:o} {message} [{source}]{exceptions}"
              ) :> Logger

let config =
    { defaultConfig with
          homeFolder = Some publicPath
          bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port ]
          logger = logger }

let surveyApi = {
    WriteSurveyResult = fun (ratings,addTxt,task,pin) -> async { return (writeSurvey ratings addTxt task pin) }
    GetServertime = fun (ratings,addTxt,task,pin) -> async { return giveServerPath ratings addTxt task pin }
}

let errorHandler (ex: Exception) (routeInfo: RouteInfo<HttpContext>) : ErrorResult = 
    // do some logging
    printfn "Error at %s on method %s" routeInfo.path routeInfo.methodName
    // decide whether or not you want to propagate the error to the client
    match ex with
    | _ ->  Propagate ex

let webApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.withDiagnosticsLogger(fun x -> if x.Length < 10000 then printfn "%s" x else (printfn "omitting some of the serialized result [length is above 10000 characters]...%s" x.[0..10000]))
    |> Remoting.withErrorHandler errorHandler
    |> Remoting.fromValue surveyApi
    |> Remoting.buildWebPart

let webApp =
    choose [
        webApi
        path "/" >=> browseFileHome "index.html"
        browseHome
        RequestErrors.NOT_FOUND "Not found!"
    ]

startWebServer config webApp