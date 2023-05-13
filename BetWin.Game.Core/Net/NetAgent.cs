using BetWin.Game.Core.Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetWin.Game.Core.Net
{
    public static class NetAgent
    {
        private static HttpClient? _client;

        private static HttpClient Client => _client ??= new HttpClient();

        internal static string USER_AGENT
        {
            get
            {
                Assembly assembly = typeof(NetAgent).Assembly;
                AssemblyName assemblyName = assembly.GetName();
                return string.Concat(assemblyName.Name, "/", assemblyName.Version);
            }
        }

        /// <summary>
        /// 添加默认的Header头
        /// </summary>
        /// <param name="headers"></param>
        private static void AddDefaultHeader(this HttpHeaders headers, Dictionary<string, string>? header = null)
        {
            header ??= new Dictionary<string, string>();
            if (!header.ContainsKey("User-Agent")) header.Add("User-Agent", USER_AGENT);
            foreach (KeyValuePair<string, string> item in header)
            {
                if (new[] { "Content-Type" }.Contains(item.Key)) continue;
                headers.Add(item.Key, item.Value);
            }
        }

        public static async Task<HttpResult> SendAsync(string url, byte[]? data, HttpMethod method, Encoding? encoding, HttpClientOption? options = null, HttpClient? httpClient = null)
        {
            httpClient ??= Client;

            if (options?.Timeout != null) httpClient.Timeout = options.Timeout.Value;

            encoding ??= Encoding.UTF8;
            options ??= new HttpClientOption();
            if (!options.Headers.ContainsKey("Referer")) options.Referrer = url;
            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(method, url)
                {
                    Version = new Version(2, 0)
                })
                {
                    if (data != null)
                    {
                        StringContent content = new StringContent(encoding.GetString(data), encoding, options.ContentType);
                        request.Content = content;
                    }
                    request.Headers.AddDefaultHeader(options.Headers);
                    //request.Headers.AddDefaultHeader(header);


                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    byte[] resultData = await response.Content.ReadAsByteArrayAsync();
                    // 如果启用了gzip压缩
                    if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                    {
                        resultData = UnGZip(resultData);
                    }
                    return new HttpResult
                    {
                        StatusCode = response.StatusCode,
                        Headers = response.Headers,
                        Data = resultData,
                        Content = encoding.GetString(resultData)
                    };
                }
            }
            // 发生网络错误
            catch (HttpRequestException ex)
            {
                return new HttpResult(ex, url);
            }
            catch (Exception ex)
            {
                return new HttpResult(ex, url);
            }
        }

        /// <summary>
        /// 异步返回内容
        /// </summary>
        public static async Task<HttpResult> GetAsync(string url, Encoding? encoding = null, HttpClientOption? options = null)
        {
            return await SendAsync(url, null, HttpMethod.Get, encoding, options);
        }

        public static async Task<HttpResult> PostAsync(string url, string data, Encoding? encoding = null, HttpClientOption? headers = null)
        {
            encoding ??= Encoding.UTF8;
            headers ??= new Dictionary<string, string>();
            //if (!header.ContainsStringKey("Content-Type")) header.Add("Content-Type", "application/x-www-form-urlencoded");
            return await SendAsync(url, encoding.GetBytes(data), HttpMethod.Post, encoding, headers);
        }

        /// <summary>
        /// 进行gzip的解压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] UnGZip(byte[] data)
        {
            using (MemoryStream dms = new MemoryStream())
            {
                using (MemoryStream cms = new MemoryStream(data))
                {
                    using (System.IO.Compression.GZipStream gzip = new GZipStream(cms, System.IO.Compression.CompressionMode.Decompress))
                    {
                        byte[] bytes = new byte[1024];
                        int len = 0;
                        while ((len = gzip.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            dms.Write(bytes, 0, len);
                        }
                    }
                }
                return dms.ToArray();
            }
        }
    }
}
