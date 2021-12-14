using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScreenShotGrabber
{
    class Program
    {
        private static MixedRadix mr;
        private static string urlPath = "v2\\Urls.txt";
        private static string countPath = "v2\\Count.txt";
        private static string failedPath = "v2\\Failed.txt";
        private static int maxConcurrentFails = 5;
        private static int concurrentFails = 0;
        private static bool stop = false;
        static void Main(string[] args)
        {
            if (!File.Exists(urlPath))
            {
                File.Create(urlPath);
            }

            List<char> chars = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            List<char> digits = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            mr = new MixedRadix(new List<List<char>> { chars, chars, digits, digits, digits, digits }, countPath);

            if (!File.Exists(countPath))
            {
                File.Create(countPath);
            }
            else
            {
                mr.currentNum = Convert.ToInt32(File.ReadAllLines(countPath)[0]);
            }


            for (int i = 0; i < 6760000; i++)
            {
                if (stop)
                {
                    System.Threading.Thread.Sleep(10000);
                    return;
                }
                System.Threading.Thread.Sleep(240);
                string url = GetImageUrl();
                new Task(() => { CheckUrl(url); }).Start();
            }
        }

        private static string GetImageUrl()
        {
            return "http://prnt.sc/"+mr.Next();
        }

        private static void CheckUrl(string url)
        {
            try
            {
                Console.WriteLine($"STARTED CHECKING {url}");
                WebClient web1 = new WebClient();
                web1.Headers.Add("user-agent", "Only a test!");
                string page = web1.DownloadString(url);

                //string imageUrl = Regex.Match(page, @"https://image.prntscr.com" + "(.{0,30}).jpg").Value;
                string imageUrl = Regex.Match(page, "<img class=\"no-click screenshot-image\"[^>]*").Value;
                imageUrl = Regex.Match(imageUrl, "src=\"[^\"]*").Value;
                imageUrl = imageUrl.Replace("src=\"", "");

                imageUrl = (imageUrl == "" || imageUrl == "//st.prntscr.com/2021/10/22/2139/img/0_173a7b_211be8ff.png") ? "NO IMAGE" : imageUrl;
                Console.WriteLine($"FINISHED CHECKING {url}\t{imageUrl}");
                if (imageUrl != "NO IMAGE")
                {
                    File.AppendAllText(urlPath, imageUrl + "\n");
                }
                concurrentFails = concurrentFails == 0 ? 0 : concurrentFails - 1;
            }
            catch(Exception ex)
            {
                concurrentFails++;
                Console.WriteLine($"FAILED CHECK {url} \n\n Exceprion: {ex}");
                File.AppendAllText(failedPath, url + "\n");
                stop = true;
                if (concurrentFails > maxConcurrentFails)
                {
                    stop = true;
                }
            }
        }
    }
}
