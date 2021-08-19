using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var values = new
            {
                Test = 31337,
                Data = ""
            };

            var uri = new Uri("");
            using (var client = new HttpClient())
            {
                var result = await client.PostJsonAsync<string, object>(uri, values);
                Console.WriteLine(result);
            }


            using (var authClient = new HttpClient())
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("username:password"));
                authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                authClient.Timeout = TimeSpan.FromSeconds(30);

                var bytes = new byte[0];
                var result = await authClient.PostOctetStreamAsync(uri, bytes);
            }
        }
    }
}
