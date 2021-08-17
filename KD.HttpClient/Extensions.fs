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
    static member PostJsonAsync<'TResult> (client: HttpClient, uri: Uri, value: Object, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)
        if isNull value then nullArg (nameof value)

        task {
            use stringContent = new StringContent(JsonConvert.SerializeObject value, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(uri, stringContent, cancellationToken)
            response.EnsureSuccessStatusCode() |> ignore
            let! jsonResult = response.Content.ReadAsStringAsync()
            return JsonConvert.DeserializeObject<'TResult>(jsonResult)
        }

    [<Extension>]
    static member PostJsonAsync<'TResult> (client: HttpClient, uri, value) = 
        client.PostJsonAsync<'TResult> (uri, value, Unchecked.defaultof<_>)

    [<Extension>]
    static member PostJsonAsync (client: HttpClient, uri: Uri, value: Object, cancellationToken: CancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isNull value then nullArg (nameof value)

        task {
            use stringContent = new StringContent(JsonConvert.SerializeObject value, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(uri, stringContent, cancellationToken)
            response.EnsureSuccessStatusCode() |> ignore
            return! response.Content.ReadAsStringAsync()
        }

    [<Extension>]
    static member PostJsonAsync (client: HttpClient, uri, value) =
        client.PostJsonAsync(uri, value, Unchecked.defaultof<_>)

    [<Extension>]
    static member GetObjectAsync<'T> (client: HttpClient, uri: Uri, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)

        task {
            use! response = client.GetAsync(uri, cancellationToken)
            response.EnsureSuccessStatusCode() |> ignore
            let! jsonResult = response.Content.ReadAsStringAsync()
            return JsonConvert.DeserializeObject<'T>(jsonResult)
        }

    [<Extension>]
    static member GetObjectAsync<'T> (client: HttpClient, uri) = 
        client.GetObjectAsync<'T>(uri, Unchecked.defaultof<_>)

    [<Extension>]
    static member GetStringAsync (client: HttpClient, uri: Uri, cancellationToken: CancellationToken) =
        if isNull uri then nullArg (nameof uri)

        task{
            use! response = client.GetAsync(uri, cancellationToken)
            response.EnsureSuccessStatusCode() |> ignore
            return! response.Content.ReadAsStringAsync()
        }

    [<Extension>]
    static member GetStringAsync (client: HttpClient, uri: Uri) =
        client.GetStringAsync(uri, Unchecked.defaultof<_>)