﻿#r "System.Xml.Linq"

open System
open System.IO
open System.Xml.Linq
open System.Text.RegularExpressions

printf "Enter solution path: "
let solutionPath = Console.ReadLine()
let solutionDirPath = Path.GetDirectoryName(solutionPath)

let solutionText = File.ReadAllText(solutionPath)
let projectPaths  = 
    Regex.Matches(solutionText, "Project.+?, \"(.+?\.csproj)")
    |> Seq.cast<Match>
    |> Seq.map (fun m -> Path.Combine(solutionDirPath, m.Groups.[1].Value))

let codeFilePaths = 
    projectPaths
    |> Seq.map (fun path -> 
        let document = XDocument.Load(path)
        document.Descendants()
            |> Seq.where (fun desc -> desc.Name.LocalName = "Compile")
            |> Seq.map (fun desc -> Path.Combine(Path.GetDirectoryName(path), desc.FirstAttribute.Value)))
        |> Seq.concat

let totalLoc = 
    codeFilePaths
    |> Seq.sumBy (fun path ->
        File.ReadLines(path)
        |> Seq.length)

printfn "Total Loc: %i" totalLoc
Console.ReadKey() |> ignore