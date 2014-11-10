namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("BlocktrailSdk.Sample")>]
[<assembly: AssemblyProductAttribute("BlocktrailSdk")>]
[<assembly: AssemblyDescriptionAttribute("SDK for blocktrail.com")>]
[<assembly: AssemblyVersionAttribute("0.0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.1"
