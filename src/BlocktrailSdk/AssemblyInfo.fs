namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("BlocktrailSdk")>]
[<assembly: AssemblyProductAttribute("BlocktrailSdk")>]
[<assembly: AssemblyDescriptionAttribute("SDK for blocktrail.com")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
