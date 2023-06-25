using System;
using System.Collections.Generic;
using System.Net;

namespace BetWin.Game.API.Utils
{
    /// <summary>
    /// HttpClient 请求配置对象
    /// </summary>
    public class HttpClientOption
    {
        public HttpClientOption()
        {
            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 头部参数设定
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        /// <summary>
        /// 来路地址
        /// </summary>
        public string Referrer
        {
            get
            {
                return this.Headers?.Get(nameof(Referrer), string.Empty) ?? string.Empty;
            }
            set
            {
                if (this.Headers.ContainsKey(nameof(Referrer)))
                {
                    this.Headers[nameof(Referrer)] = value;
                }
                else
                {
                    this.Headers.Add(nameof(Referrer), value);
                }
            }
        }

        /// <summary>
        /// 使用代理
        /// </summary>
        public IWebProxy? Proxy { get; set; }

        public static implicit operator HttpClientOption(Dictionary<string, string> headers)
        {
            headers ??= new Dictionary<string, string>();
            HttpClientOption option = new HttpClientOption()
            {
                Headers = headers,
                ContentType = headers.Get("Content-Type", "application/x-www-form-urlencoded")
            };
            return option;
        }
    }
}
