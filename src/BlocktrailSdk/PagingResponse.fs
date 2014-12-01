namespace BlocktrailSdk.Models

open System.Collections.ObjectModel
open BlocktrailSdk.Models
open System
open BlocktrailSdk.Helpers

/// <summary>
/// Processed Paging response. Added functionality to improve paging experience.
/// </summary>
type PagingResponse<'T>(id : string, sortDir : string, data : Paging<'T>, reqfunc) = 
    inherit ReadOnlyCollection<'T>(data.Data)
    member val private Id = id
    member val private Raw = data
    member val private SortDir = sortDir
    member val Limit = data.PerPage
    member val Total = data.Total
    member val Page = data.CurrentPage
    member x.PageAvailable page = page <= x.Pages
    member x.NextPageAvailable() = x.PageAvailable(x.Page + 1)
    member x.PrevPageAvailable() = x.PageAvailable(x.Page - 1)
    member x.Pages = int (x.Total / int64 x.Limit) + 1
    
    member x.GetPage page : PagingResponse<'T> = 
        if x.PageAvailable page = false then 
            raise (ArgumentOutOfRangeException("page", "Requested page is out of range"))
        let response = reqfunc x.Id (page, x.Raw.PerPage, x.SortDir)
        let convertedResponse = convertToObject<Paging<'T>> response
        if Array.isEmpty convertedResponse.Data then raise (NullReferenceException("No data returned."))
        new PagingResponse<'T>(x.Id, x.SortDir, convertedResponse, reqfunc)
    
    member x.PrevPage() : PagingResponse<'T> = x.GetPage(x.Raw.CurrentPage - 1)
    member x.NextPage() : PagingResponse<'T> = x.GetPage(x.Raw.CurrentPage + 1)