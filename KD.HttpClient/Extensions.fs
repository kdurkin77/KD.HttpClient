namespace System.Net.Http

open FSharp.Control.Tasks.ContextInsensitive
open Newtonsoft.Json
open System
open System.Runtime.CompilerServices
open System.Text
open System.Threading

[<Extension>]
type HttpClientExtensions() =
    [<Extension>]
    static member PostJsonAsync<'TResult> (client: HttpClient, url: Uri, values: Object, ct: CancellationToken) = 
        task {
            if isNull url then nullArg "url"
            if isNull values then nullArg "values"

            use stringContent = new StringContent(JsonConvert.SerializeObject values, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(url, stringContent, ct)
            let! jsonResult = response.Content.ReadAsStringAsync()
            response.EnsureSuccessStatusCode() |> ignore
            return JsonConvert.DeserializeObject<'TResult>(jsonResult)
        }

    [<Extension>]
    static member PostJsonAsync<'TResult> (client: HttpClient, url: Uri, values: Object) = 
        client.PostJsonAsync<'TResult> (url, values, Unchecked.defaultof<CancellationToken>)

    [<Extension>]
    static member PostJsonAsync (client: HttpClient, url: Uri, values: Object, ct: CancellationToken) =
        task {
            if isNull url then nullArg "url"
            if isNull values then nullArg "values"

            use stringContent = new StringContent(JsonConvert.SerializeObject values, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(url, stringContent, ct)
            let! result = response.Content.ReadAsStringAsync()
            response.EnsureSuccessStatusCode() |> ignore
            return result
        }

    [<Extension>]
    static member PostJsonAsync (client: HttpClient, url: Uri, values: Object) =
        client.PostJsonAsync(url, values, Unchecked.defaultof<CancellationToken>)

    [<Extension>]
    static member GetObjectAsync<'T> (client: HttpClient, url: Uri, ct: CancellationToken) = 
        task {
            if isNull url then nullArg "url"

            use! response = client.GetAsync(url, ct)
            let! jsonResult = response.Content.ReadAsStringAsync()
            response.EnsureSuccessStatusCode() |> ignore
            return JsonConvert.DeserializeObject<'T>(jsonResult)
        }

    [<Extension>]
    static member GetObjectAsync<'T> (client: HttpClient, url: Uri) = 
        client.GetObjectAsync<'T>(url, Unchecked.defaultof<CancellationToken>)

    [<Extension>]
    static member GetStringAsync (client: HttpClient, url: Uri, ct: CancellationToken) =
        task{
            if isNull url then nullArg "url"

            use! response = client.GetAsync(url, ct)
            let! result = response.Content.ReadAsStringAsync()
            response.EnsureSuccessStatusCode() |> ignore
            return result
        }

    [<Extension>]
    static member GetStringAsync (client: HttpClient, url: Uri) =
        client.GetAsync(url, Unchecked.defaultof<CancellationToken>)