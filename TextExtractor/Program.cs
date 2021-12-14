using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TextExtractor
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        private static StreamReader sr;
        static void Main(string[] args)
        {
            GetTest("https://www.prepostseo.com/frontend/extractImgText");
            Console.ReadKey();
        }

        private async static void GetTest(string url)
        {
            Console.WriteLine("starting");
            var values = new Dictionary<string, string>
            {
                { "submit", "true" },
                { "imgUrl", url }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://www.prepostseo.com/frontend/extractImgText", content);

            var v = await response.Content.ReadAsStringAsync();
            Console.WriteLine(v.ToString());
            Console.WriteLine("ending");
        }
    }
}
