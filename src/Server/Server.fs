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

let config =
    { defaultConfig with
          homeFolder = Some publicPath
          bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port ] }

let counterApi = {
    initialCounter = fun () -> async { return {Value = 42} }
}

let webApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue counterApi
    |> Remoting.buildWebPart
let webApp =
    choose [
        webApi
        path "/" >=> browseFileHome "index.html"
        browseHome
        RequestErrors.NOT_FOUND "Not found!"
    ]

startWebServer config webApp