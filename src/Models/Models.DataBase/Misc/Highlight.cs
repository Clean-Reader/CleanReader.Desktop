// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CleanReader.Models.DataBase;

/// <summary>
/// 高亮.
/// </summary>
[Index(nameof(BookId))]
public class Highlight
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 书籍Id.
    /// </summary>
    public string BookId { get; set; }

    /// <summary>
    /// 标识范围.
    /// </summary>
    public string CfiRange { get; set; }

    /// <summary>
    /// 划定的文本.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 高亮颜色.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新时间.
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    public string Comments { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Highlight highlight && Id == highlight.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
