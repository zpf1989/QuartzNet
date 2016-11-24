using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Drawing;
using Common.Logging;
using System.Reflection;

namespace HttpTaskService
{
    public class HttpHelper
    {
        static int _defaultTimeout = 500;//单位s
        public static int TimeOut
        {
            get
            {
                var val = ConfigHelper.GetAppSetting("http_timeout", _defaultTimeout.ToString());
                int time = _defaultTimeout;
                int.TryParse(val, out time);

                return time;
            }
        }

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>  
        /// 创建GET方式的HTTP请求  
        /// </summary>  
        /// <param name="url">请求的URL</param>  
        /// <param name="timeout">请求的超时时间</param>  
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>  
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>  
        /// <param name="token">token</param>  
        /// <returns></returns>  
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies, string token = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            request.Timeout = timeout.HasValue ? timeout.Value : (TimeOut * 1000);
            //Log.Logger.Info("timeout:" + TimeOut);

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if (!string.IsNullOrEmpty(token))
            {
                AddToken(request, token);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 发起get请求，返回响应字符串数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string Get(string url, string token = "")
        {
            var reqEncoding = Encoding.UTF8;

            var response = CreateGetHttpResponse(url, null, "", null, token);
            using (var stream = response.GetResponseStream())
            {
                var bytes = GetBytes(stream);
                var strResponse = reqEncoding.GetString(bytes, 0, bytes.Length);
                return strResponse;
            }

        }

        private static void AddToken(HttpWebRequest request, string token)
        {
            request.Headers.Add("token", token);
        }
        private static byte[] GetBytes(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
