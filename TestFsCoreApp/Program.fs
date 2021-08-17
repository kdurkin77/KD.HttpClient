open System
open System.Net.Http

[<EntryPoint>]
let main argv = 
    async {
        let values = {| Test = 31337; Data = ""; |}

        let uri = new Uri("")
        use client = new HttpClient()
        let! result = client.PostJsonAsync<string, Object>(uri, values) |> Async.AwaitTask
        printf "Results: %s " result
        return 0
    } |> Async.RunSynchronously