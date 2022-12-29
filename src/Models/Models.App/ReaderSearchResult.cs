// Copyright (c) Richasy. All rights reserved.

using Newtonsoft.Json;

namespace CleanReader.Models.App;

/// <summary>
/// 阅读器搜索结果.
/// </summary>
public class ReaderSearchResult
{
    /// <summary>
    /// 定位.
    /// </summary>
    [JsonProperty("cfi")]
    public string Cfi { get; set; }

    /// <summary>
    /// 片段.
    /// </summary>
    [JsonProperty("excerpt")]
    public string Excerpt { get; set; }

    /// <summary>
    /// 章节名.
    /// </summary>
    [JsonProperty("chapter")]
    public string Chapter { get; set; }
}
