using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace UtilCS
{
    public static class Util
    {
        public static string SendRequest(string uri, string method, string contentType, string body)
        {
            return SendRequest(uri, method, contentType, body, null);
        }
        public static string SendRequest(string uri, string method, string contentType, string body, Dictionary<string, string> headers)
        {
            string responseBody = null;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
            if (headers != null)
            {
                foreach (string headerName in headers.Keys)
                {
                    req.Headers[headerName] = headers[headerName];
                }
            }
            if (!String.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }

            if (body != null)
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
                req.GetRequestStream().Close();
            }

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }

            if (resp == null)
            {
                responseBody = null;
                Console.WriteLine("Response is null");
            }
            else
            {
                Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
                foreach (string headerName in resp.Headers.AllKeys)
                {
                    Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
                }
                Console.WriteLine();
                Stream respStream = resp.GetResponseStream();
                if (respStream != null)
                {
                    responseBody = new StreamReader(respStream).ReadToEnd();
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine("HttpWebResponse.GetResponseStream returned null");
                }
            }

            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();

            return responseBody;
        }
        public static void PrintBytes(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0 && ((i % 16) == 0))
                {
                    Console.WriteLine("   {0}", sb.ToString());
                    sb.Length = 0;
                }
                else if (i > 0 && ((i % 8) == 0))
                {
                    Console.Write(" ");
                    sb.Append(' ');
                }

                Console.Write(" {0:X2}", (int)bytes[i]);
                if (' ' <= bytes[i] && bytes[i] <= '~')
                {
                    sb.Append((char)bytes[i]);
                }
                else
                {
                    sb.Append('.');
                }
            }

            if ((bytes.Length % 16) > 0)
            {
                int spacesToPrint = 3 * (16 - (bytes.Length % 16));
                if ((bytes.Length % 16) <= 8)
                {
                    spacesToPrint++;
                }

                Console.Write(new string(' ', spacesToPrint));
            }

            Console.WriteLine("   {0}", sb.ToString());
        }
    }
}
