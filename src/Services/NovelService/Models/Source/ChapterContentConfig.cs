// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models;

/// <summary>
/// 章节内容配置.
/// </summary>
public class ChapterContentConfig
{
    /// <summary>
    /// HTML 查询范围.
    /// </summary>
    public string Range { get; set; }

    /// <summary>
    /// 内容规则.
    /// </summary>
    public Attribute Content { get; set; }

    /// <summary>
    /// 下一页规则.
    /// </summary>
    public NextPageConfig NextPage { get; set; }
}
