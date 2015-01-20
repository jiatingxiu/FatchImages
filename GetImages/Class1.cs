using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GetImages
{
    public class Class1
    {
        private string globePath;

        public Class1()
        {
            globePath = DealDir(System.IO.Path.Combine(Environment.CurrentDirectory, "images"));

            int num = 1;
            while (num < 10)
            {
                DoFetch(num);
                num++;
            }
            Console.WriteLine("=========================END==========================");
        }

        private void DoFetch(int pageNum)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.ipc.me/shoulu/pic-list-" + pageNum + ".html");
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    List<Uri> links = FetchLinksFromSource(sr.ReadToEnd());
                }
            }
        }

        private List<Uri> FetchLinksFromSource(string htmlSource)
        {
            List<Uri> links = new List<Uri>();
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImgSrc)
            {
                string href = m.Groups[1].Value;
                if (CheckIsUrlFormat(href))
                {
                    links.Add(new Uri(href));
                    Console.WriteLine(href);
                }
                else
                    continue;

                using (WebClient myWebClient = new WebClient())
                {
                    myWebClient.DownloadFileAsync(new Uri(href), System.IO.Path.Combine(globePath, System.IO.Path.GetRandomFileName() + System.IO.Path.GetExtension(href)));
                }
            }
            return links;
        }

        private string DealDir(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return path;
        }

        private bool CheckIsUrlFormat(string strValue)
        {
            return CheckIsFormat(@"http://?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", strValue);
        }

        private bool CheckIsFormat(string strRegex, string strValue)
        {
            if (strValue != null && strValue.Trim() != "")
            {
                Regex re = new Regex(strRegex);
                if (re.IsMatch(strValue))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
