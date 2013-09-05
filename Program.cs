using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CH = NetTools.CmdletHelper;

namespace FUrl
{
    class Program
    {
        static string file = "";
        static string ip = "0.0.0.0";
        static List<string> urls = new List<string>();
        static string log = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            Dictionary<string, string> pList = CH.GetArguments(args);

            if (pList.Count == 0)
            {
                CH.ShowHelp();
                return;
            }

            foreach (string key in pList.Keys)
            {
                switch (key.ToLower())
                {
                    case "-ip":
                        ip = pList[key];
                        break;
                    case "-url":
                        urls.Add(pList[key]);
                        break;
                    case "-file":
                        file = pList[key];
                        if (!File.Exists(file))
                        {
                            CH.ShowMessage("Imported file doesn't exist.");
                            return;
                        }
                        break;
                    case "-h":
                        CH.ShowHelp();
                        return;
                    default:
                        CH.ShowError();
                        return;
                }
            }

            if (!string.IsNullOrEmpty(file))
            {
                urls.AddRange(File.ReadAllLines(file));
            }

            if (urls.Count > 0)
            {
                StringBuilder result = new StringBuilder();
                string furl = "";
                int index = 1;

                if (pList.ContainsKey("-file"))
                {
                    foreach (string url in urls)
                    {
                        furl = GetFinalURL(ip, url);
                        if (furl.IndexOf("?") > -1)
                        {
                            furl = furl.Substring(0, furl.IndexOf("?"));
                        }
                        File.AppendAllText(log, string.Format("{0}\t{1}\r\n", url, furl));

                        CH.ProgressStatus(index++, urls.Count);
                    }
                }
                else
                {
                    furl = GetFinalURL(ip, urls[0]);
                    if (furl.IndexOf("?") > -1)
                    {
                        furl = furl.Substring(0, furl.IndexOf("?"));
                    }
                    CH.ShowMessage(furl + "\n");
                }
            }
            else
            {
                CH.ShowError();
            }

            Console.CursorVisible = true;
        }

        private static string GetFinalURL(string ip, string url)
        {
            VHttpRequest vreq = new VHttpRequest();
            string header = vreq.GetHeader(ip, url);
            StringReader pageReader = new StringReader(header);
            string line = pageReader.ReadLine();
            string furl = url;

            while (line != null)
            {
                line = line.ToLower();
                if (line.StartsWith("location: "))
                {
                    url = line.Replace("location: ", "");
                    furl = url;
                    return GetFinalURL(ip, url);
                }
                line = pageReader.ReadLine();
            }

            return furl;
        }
    }
}
