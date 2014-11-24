(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
BlocktrailSdk
===================

Documentation

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The BlocktrailSdk library can be <a href="https://nuget.org/packages/BlocktrailSdk">installed from NuGet</a>:
      <pre>PM> Install-Package BlocktrailSdk</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Example
-------

This example demonstrates using a function defined in this sample library.

*)
#r "BlocktrailSdk.dll"
open BlocktrailSdk
open BlocktrailSdk.Models

BlocktrailSdk.Config.ApiKey <- "INSERT_YOUR_API_KEY";

let transaction = BlocktrailSdk.Client.GetTransaction "c326105f7fbfa4e8fe971569ef8858f47ee7e4aa5e8e7c458be8002be3d86aad"

printfn "Hash: %s" transaction.Hash
printfn "Confirmations: %i" transaction.Confirmations

(**
Samples & documentation
-----------------------

The library comes with comprehensible documentation. 
It can include tutorials automatically generated from `*.fsx` files in [the content folder][content]. 
The API reference is automatically generated from Markdown comments in the library implementation.

 * [Tutorial](tutorial.html) contains a further explanation of this sample library.

 * [API Reference](reference/index.html) contains automatically generated documentation for all types, modules
   and functions in the library. This includes additional brief samples on using most of the
   functions.
 
Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding a new public API, please also 
consider adding [samples][content] that can be turned into a documentation. You might
also want to read the [library design notes][readme] to understand how it works.

The library is available under the MIT license, which allows modification and 
redistribution for both commercial and non-commercial purposes. For more information see the 
[License file][license] in the GitHub repository. 

  [content]: https://github.com/raymens/BlocktrailSdk/tree/master/docs/content
  [gh]: https://github.com/raymens/BlocktrailSdk
  [issues]: https://github.com/raymens/BlocktrailSdk/issues
  [readme]: https://github.com/raymens/BlocktrailSdk/blob/master/README.md
  [license]: https://github.com/raymens/BlocktrailSdk/blob/master/LICENSE.txt
*)
