// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace CleanReader.Models.DataBase;

/// <summary>
/// 元数据.
/// </summary>
public class Meta
{
    /// <summary>
    /// 键.
    /// </summary>
    [Key]
    public string Key { get; set; }

    /// <summary>
    /// 值.
    /// </summary>
    public string Value { get; set; }
}
