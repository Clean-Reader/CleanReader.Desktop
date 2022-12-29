// Copyright (c) Richasy. All rights reserved.

using Newtonsoft.Json;

namespace CleanReader.Models.App;

/// <summary>
/// 阅读器章节.
/// </summary>
public class ReaderChapter
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonProperty("label")]
    public string Label { get; set; }

    /// <summary>
    /// 地址.
    /// </summary>
    [JsonProperty("href")]
    public string Href { get; set; }

    /// <summary>
    /// 子项.
    /// </summary>
    [JsonProperty("subitems")]
    public List<ReaderChapter> Children { get; set; }

    /// <summary>
    /// 父级 Id.
    /// </summary>
    [JsonProperty("parent")]
    public string ParentId { get; set; }

    /// <inheritdoc/>
    public override string ToString() => Label.Replace(".html", string.Empty).Trim();

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ReaderChapter chapter && Id == chapter.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
