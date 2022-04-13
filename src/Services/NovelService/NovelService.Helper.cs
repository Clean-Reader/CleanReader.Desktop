// Copyright (c) Richasy. All rights reserved.

using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CleanReader.Services.Novel.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace CleanReader.Services.Novel
{
    /// <summary>
    /// 在线小说服务.
    /// </summary>
    public partial class NovelService
    {
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.99 Safari/537.36 Edg/97.0.1072.69";
        private static HttpClient _httpClient;

        /// <summary>
        /// 解码Base64字符串.
        /// </summary>
        /// <param name="text">Base64字符串.</param>
        /// <returns>原始字符串.</returns>
        public static string DecodingBase64ToString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var bytes = Convert.FromBase64String(text);
            return Encoding.Default.GetString(bytes);
        }

        private static async Task<HtmlDocument> GetHtmlDocumentAsync(string url, Encoding encoding = null, RequestConfig config = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (config?.Headers?.Any() ?? false)
            {
                foreach (var header in config.Headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var content = await _httpClient.GetStreamAsync(url, cancellationTokenSource == null ? new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token : cancellationTokenSource.Token);
            var doc = new HtmlDocument();
            doc.Load(content, encoding ?? Encoding.UTF8);
            return doc;
        }

        private static async Task<HtmlDocument> GetHtmlDocumentAsync(string url, string keyword, RequestConfig config, CancellationTokenSource cancellationTokenSource = null)
        {
            if (config?.Headers?.Any() ?? false)
            {
                foreach (var header in config.Headers)
                {
                    _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var method = config.Method.Equals("post", StringComparison.OrdinalIgnoreCase) ? HttpMethod.Post : HttpMethod.Get;
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(config.Body))
            {
                if (config.DataType == "form")
                {
                    var sp = config.Body.Split(',');
                    var dict = new Dictionary<string, string>();
                    foreach (var item in sp)
                    {
                        var text = item.Replace("{{keyword}}", keyword);
                        var kv = text.Split('=');
                        if (kv.Length > 1)
                        {
                            dict.Add(kv[0].Trim(), text.Replace(kv[0], string.Empty).Trim().TrimStart('='));
                        }
                    }

                    request.Content = new FormUrlEncodedContent(dict);
                }
                else if (config.DataType == "raw")
                {
                    request.Content = new StringContent(config.Body, Encoding.UTF8, "text/xml");
                }
                else if (config.DataType == "json")
                {
                    request.Content = new StringContent(config.Body, Encoding.UTF8, "application/json");
                }
            }

            var response = await _httpClient.SendAsync(request, cancellationTokenSource == null ? CancellationToken.None : cancellationTokenSource.Token);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            return doc;
        }

        private static HttpClient GetHttpClient()
        {
            var handler = new HttpClientHandler() { UseCookies = true };
            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3");
            httpClient.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
            return httpClient;
        }

        private static string FormatString(string text, string? regex)
        {
            if (!string.IsNullOrEmpty(regex) && !string.IsNullOrEmpty(text))
            {
                try
                {
                    text = HttpUtility.HtmlDecode(text);
                    return Regex.Replace(text, regex, string.Empty).Trim();
                }
                catch (Exception)
                {
                    // TODO: 记录日志.
                }
            }

            return text;
        }

        private static string ReplaceString(string input, string pattern, string replacement)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (replacement.Contains("{{") && replacement.Contains("}}"))
            {
                var match = Regex.Match(input, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    for (var i = 1; i < match.Groups.Count; i++)
                    {
                        var v = match.Groups[i].Value;
                        replacement = replacement.Replace($"{{{{{i}}}}}", v);
                    }

                    return replacement;
                }
            }

            return Regex.Replace(input, pattern, replacement);
        }

        private static string RepairString(string input, bool isLeft, string addon)
            => isLeft ? addon + input : input + addon;

        private static string EncodingStringToBase64(string text, bool needCheck = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (needCheck && IsBase64String(text))
            {
                return text;
            }

            var bytes = Encoding.Default.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        private static HtmlNode? GetSingleNodeFromRule(HtmlNode parent, string? rule)
            => string.IsNullOrEmpty(rule) || parent == null || !parent.HasChildNodes ? null : parent.QuerySelector(rule);

        private static string NormalizeContent(string html)
            => Regex.Replace(html.Replace("</p>", "\n").Replace("<br>", "\n"), "<[^>]+>", string.Empty);

        private static bool IsBase64String(string str)
        {
            str = str.Trim();
            return (str.Length % 4 == 0) && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
    }
}
