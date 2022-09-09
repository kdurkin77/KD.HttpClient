namespace System.Net.Http

open System
open System.Runtime.CompilerServices
open System.Text
open System.Threading

open Newtonsoft.Json


[<AutoOpen>]
module private Internal =
    let isDefault x =
        Object.Equals(x, Unchecked.defaultof<_>)

    let CreateJsonContent value = fun () -> new StringContent(JsonConvert.SerializeObject value, Encoding.UTF8, "application/json") :> HttpContent

    let CreateOctetStreamContent value = fun () -> new ByteArrayContent(value) :> HttpContent

    let TypedResponseHandlerAsync<'TResult>(response: HttpResponseMessage, cancellationToken) = async {
        #if NET5_0_OR_GREATER
            let! text = response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(cancellationToken) |> Async.AwaitTask
            return JsonConvert.DeserializeObject<'TResult>(text)
        #else
            let! text = response.EnsureSuccessStatusCode().Content.ReadAsStringAsync() |> Async.AwaitTask
            return JsonConvert.DeserializeObject<'TResult>(text)
        #endif
        }

    let StringResponseHandlerAsync(response: HttpResponseMessage, cancellationToken) = async {
        #if NET5_0_OR_GREATER
            return! response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(cancellationToken) |> Async.AwaitTask
        #else
            return! response.EnsureSuccessStatusCode().Content.ReadAsStringAsync() |> Async.AwaitTask
        #endif
        }

    let SendHttpRequestAsync<'TPayload, 'TResult>(client: HttpClient, httpMethod, uri: Uri, createPayload': (unit -> HttpContent) option, responseHandlerAsync: _ -> Async<'TResult>, cancellationToken: CancellationToken) =
        async{
            use request = new HttpRequestMessage(httpMethod, uri)

            let content =
                createPayload'
                |> Option.map (fun f ->
                    request.Content <- f ()
                    request.Content
                    )
                |> Option.toObj
            try
                let! response = client.SendAsync(request, cancellationToken = cancellationToken) |> Async.AwaitTask
                return! responseHandlerAsync(response, cancellationToken)
            finally
                if not (isNull content) then
                    content.Dispose()
        } |> Async.StartAsTask


[<Extension; Sealed; AbstractClass;>]
type HttpClientExtensions =

    [<Extension>]
    static member PostJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri: Uri, value: 'TValue, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        SendHttpRequestAsync<'TValue, 'TResult>(client, HttpMethod.Post, uri, Some (CreateJsonContent value), TypedResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member PostJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri, value) = 
        client.PostJsonAsync<'TResult, 'TValue> (uri, value, CancellationToken.None)

    [<Extension>]
    static member PostJsonAsync<'TValue when 'TValue: not struct> (client: HttpClient, uri: Uri, value: 'TValue, cancellationToken: CancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        SendHttpRequestAsync(client, HttpMethod.Post, uri, Some (CreateJsonContent value), StringResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member PostJsonAsync<'TValue when 'TValue: not struct> (client: HttpClient, uri, value) =
        client.PostJsonAsync<'TValue>(uri, value, CancellationToken.None)

    [<Extension>]
    static member PostJsonAsync<'TResult> (client: HttpClient, uri: Uri, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)

        SendHttpRequestAsync<unit, 'TResult>(client, HttpMethod.Post, uri, None, TypedResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member PostJsonAsync<'TResult> (client: HttpClient, uri) = 
        client.PostJsonAsync<'TResult> (uri, CancellationToken.None)

    [<Extension>]
    static member PostOctetStreamAsync<'TResult> (client: HttpClient, uri: Uri, value, cancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        SendHttpRequestAsync<byte[], 'TResult>(client, HttpMethod.Post, uri, Some (CreateOctetStreamContent value), TypedResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member PostOctetStreamAsync<'TResult> (client: HttpClient, uri: Uri, value) =
        client.PostOctetStreamAsync<'TResult>(uri, value, CancellationToken.None)

    [<Extension>]
    static member PostOctetStreamAsync (client: HttpClient, uri: Uri, value, cancellationToken) =
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        SendHttpRequestAsync(client, HttpMethod.Post, uri, Some (CreateOctetStreamContent value), StringResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member PostOctetStreamAsync (client: HttpClient, uri, value) =
        client.PostOctetStreamAsync(uri, value, CancellationToken.None)

    [<Extension>]
    static member GetObjectAsync<'TResult> (client: HttpClient, uri: Uri, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)

        SendHttpRequestAsync<unit, 'TResult>(client, HttpMethod.Get, uri, None, TypedResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member GetObjectAsync<'TResult> (client: HttpClient, uri) = 
        client.GetObjectAsync<'TResult>(uri, CancellationToken.None)

    [<Extension>]
    static member GetStringAsync (client: HttpClient, uri: Uri, cancellationToken: CancellationToken) =
        if isNull uri then nullArg (nameof uri)

        SendHttpRequestAsync(client, HttpMethod.Get, uri, None, StringResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member GetStringAsync (client: HttpClient, uri: Uri) =
        client.GetStringAsync(uri, CancellationToken.None)

    [<Extension>]
    static member PutJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri: Uri, value: 'TValue, cancellationToken: CancellationToken) = 
        if isNull uri then nullArg (nameof uri)
        if isDefault value then nullArg (nameof value)

        SendHttpRequestAsync<'TValue, 'TResult>(client, HttpMethod.Put, uri, Some (CreateJsonContent value), TypedResponseHandlerAsync, cancellationToken)

    [<Extension>]
    static member PutJsonAsync<'TResult, 'TValue when 'TValue: not struct> (client: HttpClient, uri, value) = 
        client.PutJsonAsync<'TResult, 'TValue> (uri, value, CancellationToken.None)
