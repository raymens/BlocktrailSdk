namespace System
open System.Reflection
open System.Runtime.CompilerServices

[<assembly: AssemblyTitleAttribute("BlocktrailSdk")>]
[<assembly: AssemblyProductAttribute("BlocktrailSdk")>]
[<assembly: InternalsVisibleToAttribute("BlocktrailSdk.Tests")>]
[<assembly: AssemblyDescriptionAttribute("SDK for blocktrail.com")>]
[<assembly: AssemblyVersionAttribute("0.0.1")>]
[<assembly: AssemblyFileVersionAttribute("0.0.1")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.0.1"
