// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Models.Services;

/// <summary>
/// 目录配置.
/// </summary>
public class ChapterConfig
{
    /// <summary>
    /// HTML 查询范围.
    /// </summary>
    public string Range { get; set; }

    /// <summary>
    /// 增补文本规则.
    /// </summary>
    public List<Repair> Repair { get; set; }

    /// <summary>
    /// 替换文本规则.
    /// </summary>
    public List<Replace> Replace { get; set; }

    /// <summary>
    /// 标题规则.
    /// </summary>
    public Attribute Title { get; set; }

    /// <summary>
    /// 地址规则.
    /// </summary>
    public Attribute Url { get; set; }

    /// <summary>
    /// 是否启用下级索引（将会获取查询范围内的下级子标签列表.）.
    /// </summary>
    public bool IsChildFiltered { get; set; }

    /// <summary>
    /// 下级索引需要查找的标签.
    /// </summary>
    public string ChildSelector { get; set; }
}
