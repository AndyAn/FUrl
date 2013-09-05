using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace FUrl
{
    public class VHttpRequest
    {
        public enum RequestType
        {
            Header,
            Page
        }

        private string GetResponse(string ip, int port, string body, Encoding encode, RequestType rt)
        {
            StringBuilder result = new StringBuilder();
            byte[] bteSend = Encoding.ASCII.GetBytes(body);
            byte[] bteReceive = new byte[1024];
            int intLen = 0;
            IPAddress address = null;
            string res = "";

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    if (IPAddress.TryParse(ip, out address))
                    {
                        socket.Connect(address, port);
                    }
                    else
                    {
                        socket.Connect(ip, port);
                    }
                    if (socket.Connected)
                    {
                        socket.Send(bteSend, bteSend.Length, 0);
                        while ((intLen = socket.Receive(bteReceive, bteReceive.Length, 0)) > 0)
                        {
                            result.Append(encode.GetString(bteReceive, 0, intLen));
                            if (rt == RequestType.Header)
                            {
                                res = result.ToString().Replace("\r", "");
                                if (res.IndexOf("\n\n") > -1)
                                {
                                    result.Clear();
                                    result.Append(res.Substring(0, res.IndexOf("\n\n")));
                                    break;
                                }
                            }
                        }
                    }
                    socket.Close();
                }
                catch { }
            }

            return result.ToString();
        }

        struct UrlInfo
        {
            public string Host;
            public int Port;
            public string File;
            public string Body;
        }

        private UrlInfo ParseURL(string url)
        {
            UrlInfo urlInfo = new UrlInfo();
            string[] strTemp = null;
            urlInfo.Host = "";
            urlInfo.Port = 80;
            urlInfo.File = "/";
            urlInfo.Body = "";
            int intIndex = url.ToLower().IndexOf("http://");
            if (intIndex != -1)
            {
                url = url.Substring(7);
                intIndex = url.IndexOf("/");
                if (intIndex == -1)
                {
                    urlInfo.Host = url;
                }
                else
                {
                    urlInfo.Host = url.Substring(0, intIndex);
                    url = url.Substring(intIndex);
                    intIndex = urlInfo.Host.IndexOf(":");
                    if (intIndex != -1)
                    {
                        strTemp = urlInfo.Host.Split(':');
                        urlInfo.Host = strTemp[0];
                        int.TryParse(strTemp[1], out urlInfo.Port);
                    }
                    intIndex = url.IndexOf("?");
                    if (intIndex == -1)
                    {
                        urlInfo.File = url;
                    }
                    else
                    {
                        strTemp = url.Split('?');
                        urlInfo.File = strTemp[0];
                        urlInfo.Body = strTemp[1];
                    }
                }
            }
            return urlInfo;
        }

        public string GetHeader(string ip, string url)
        {
            return Get(ip, url, Encoding.UTF8, RequestType.Header);
        }

        public string Get(string ip, string url, Encoding encode)
        {
            return Get(ip, url, encode, RequestType.Page);
        }

        private string Get(string ip, string url, Encoding encode, RequestType rt)
        {
            UrlInfo urlInfo = ParseURL(url);
            string strRequest = string.Format("GET {0}?{1} HTTP/1.1\r\nHost:{2}:{3}\r\nConnection:Close\r\n\r\n", urlInfo.File, urlInfo.Body, urlInfo.Host, urlInfo.Port.ToString());
            ip = (ip == "0.0.0.0" ? urlInfo.Host : ip);
            return GetResponse(ip, urlInfo.Port, strRequest, encode, rt);
        }

        public string Post(string ip, string url, Encoding encode)
        {
            UrlInfo urlInfo = ParseURL(url);
            string strRequest = string.Format("POST {0} HTTP/1.1\r\nHost:{1}:{2}\r\nContent-Length:{3}\r\nContent-Type:application/x-www-form-urlencoded\r\nConnection:Close\r\n\r\n{4}", urlInfo.File, urlInfo.Host, urlInfo.Port.ToString(), urlInfo.Body.Length, urlInfo.Body);
            ip = (ip == "0.0.0.0" ? urlInfo.Host : ip);
            return GetResponse(ip, urlInfo.Port, strRequest, encode, RequestType.Page);
        }
    }
}