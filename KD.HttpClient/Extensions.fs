namespace System.Net.Http

open Newtonsoft.Json
open System
open System.Net.Http.Headers
open System.Runtime.CompilerServices
open System.Text
open System.Threading

module private Internal =
    let isDefault x =
        Object.Equals(x, Unchecked.defaultof<_>)

open Internal

[<Extension; Sealed; AbstractClass;>]
type HttpClientExtensions =
    [<Extension>]
    static member PostJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri: Uri, value: 'TValue, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        async {
            use stringContent = new StringContent(JsonConvert.SerializeObject value, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(uri, stringContent, cancellationToken) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            let! jsonResult = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return JsonConvert.DeserializeObject<'TResult>(jsonResult)
            } |> Async.StartAsTask

    [<Extension>]
    static member PostJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri, value) = 
        client.PostJsonAsync<'TResult, 'TValue> (uri, value, Unchecked.defaultof<_>)

    [<Extension>]
    static member PostJsonAsync<'TValue when 'TValue: not struct> (client: HttpClient, uri: Uri, value: 'TValue, cancellationToken: CancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        async {
            use stringContent = new StringContent(JsonConvert.SerializeObject value, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(uri, stringContent, cancellationToken) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            return! response.Content.ReadAsStringAsync() |> Async.AwaitTask
            } |> Async.StartAsTask

    [<Extension>]
    static member PostJsonAsync<'TValue when 'TValue: not struct> (client: HttpClient, uri, value) =
        client.PostJsonAsync<'TValue>(uri, value, Unchecked.defaultof<_>)

    [<Extension>]
    static member PostOctetStreamAsync<'TResult> (client: HttpClient, uri: Uri, value, cancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        async {
            use content = new ByteArrayContent(value)
            content.Headers.ContentType <- MediaTypeHeaderValue("application/octet-stream")
            use! response = client.PostAsync(uri, content, cancellationToken) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            let! jsonResult = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return JsonConvert.DeserializeObject<'TResult>(jsonResult)
            } |> Async.StartAsTask

    [<Extension>]
    static member PostOctetStreamAsync<'TResult> (client: HttpClient, uri: Uri, value) =
        client.PostOctetStreamAsync<'TResult>(uri, value, Unchecked.defaultof<_>)

    [<Extension>]
    static member PostOctetStreamAsync (client: HttpClient, uri: Uri, value, cancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        async {
            use content = new ByteArrayContent(value)
            content.Headers.ContentType <- MediaTypeHeaderValue("application/octet-stream")
            use! response = client.PostAsync(uri, content, cancellationToken) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            return! response.Content.ReadAsStringAsync() |> Async.AwaitTask
            } |> Async.StartAsTask

    [<Extension>]
    static member PostOctetStreamAsync (client: HttpClient, uri, value) =
        client.PostOctetStreamAsync(uri, value, Unchecked.defaultof<_>)

    [<Extension>]
    static member GetObjectAsync<'T> (client: HttpClient, uri: Uri, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)

        async {
            use! response = client.GetAsync(uri, cancellationToken) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            let! jsonResult = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return JsonConvert.DeserializeObject<'T>(jsonResult)
        } |> Async.StartAsTask

    [<Extension>]
    static member GetObjectAsync<'T> (client: HttpClient, uri) = 
        client.GetObjectAsync<'T>(uri, Unchecked.defaultof<_>)

    [<Extension>]
    static member GetStringAsync (client: HttpClient, uri: Uri, cancellationToken: CancellationToken) =
        if isNull uri then nullArg (nameof uri)

        async{
            use! response = client.GetAsync(uri, cancellationToken) |> Async.AwaitTask
            response.EnsureSuccessStatusCode() |> ignore
            return! response.Content.ReadAsStringAsync() |> Async.AwaitTask
        } |> Async.StartAsTask

    [<Extension>]
    static member GetStringAsync (client: HttpClient, uri: Uri) =
        client.GetStringAsync(uri, Unchecked.defaultof<_>)