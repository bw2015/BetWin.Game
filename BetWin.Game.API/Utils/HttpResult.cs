﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using SP.StudioCore.Net.Http;

namespace BetWin.Game.API.Utils
{
    internal struct HttpResult
    {
        public HttpResult(Exception ex, string url) : this()
        {
            this.StatusCode = 0;
            this.Content = new
            {
                _url = url,
                _exception = ex.Message
            }.ToJson();
        }

        /// <summary>
        /// 发生网络错误
        /// </summary>
        public HttpResult(HttpRequestException ex, string url) : this()
        {
            this.StatusCode = HttpStatusCode.RequestTimeout;
            this.Content = new
            {
                _url = url,
                _exception = ex.Message
            }.ToJson();
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 返回的内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 二进制内容
        /// </summary>
        public byte[]? Data { get; set; }

        public HttpResponseHeaders? Headers { get; internal set; }

        public static implicit operator bool(HttpResult result)
        {
            return result.StatusCode == HttpStatusCode.OK;
        }

        public static implicit operator string(HttpResult result)
        {
            return result.Content;
        }

        public static implicit operator HttpResult(HttpClientResponse response)
        {
            return new HttpResult()
            {
                StatusCode = response.StatusCode,
                Content = response.Content,
                Headers = response.Headers
            };
        }
    }
}
