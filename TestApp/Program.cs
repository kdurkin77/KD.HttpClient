using System;
using System.Net.Http;
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
        }
    }
}
