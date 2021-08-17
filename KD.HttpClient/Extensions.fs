namespace System.Net.Http

open FSharp.Control.Tasks.ContextInsensitive
open Newtonsoft.Json
open System
open System.Runtime.CompilerServices
open System.Text
open System.Threading

module private Internal =
    let isDefault x =
        Object.Equals(x, Unchecked.defaultof<_>)

open Internal

[<Extension; Sealed; >]
type HttpClientExtensions =
    [<Extension>]
    static member PostJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri: Uri, value: 'TValue, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        task {
            use stringContent = new StringContent(JsonConvert.SerializeObject value, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(uri, stringContent, cancellationToken)
            response.EnsureSuccessStatusCode() |> ignore
            let! jsonResult = response.Content.ReadAsStringAsync()
            return JsonConvert.DeserializeObject<'TResult>(jsonResult)
        }

    [<Extension>]
    static member PostJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri, value) = 
        client.PostJsonAsync<'TResult, 'TValue> (uri, value, Unchecked.defaultof<_>)

    [<Extension>]
    static member PostJsonAsync<'TValue when 'TValue: not struct> (client: HttpClient, uri: Uri, value: 'TValue, cancellationToken: CancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        task {
            use stringContent = new StringContent(JsonConvert.SerializeObject value, Encoding.UTF8, "application/json")
            use! response = client.PostAsync(uri, stringContent, cancellationToken)
            response.EnsureSuccessStatusCode() |> ignore
            return! response.Content.ReadAsStringAsync()
        }

    [<Extension>]
    static member PostJsonAsync<'TValue when 'TValue: not struct> (client: HttpClient, uri, value) =
        client.PostJsonAsync<'TValue>(uri, value, Unchecked.defaultof<_>)

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