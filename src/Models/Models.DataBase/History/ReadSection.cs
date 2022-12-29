// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace CleanReader.Models.DataBase;

/// <summary>
/// 阅读区间.
/// </summary>
public class ReadSection
{
    /// <summary>
    /// 区间Id.
    /// </summary>
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// 开始阅读时间.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间.
    /// </summary>
    public DateTime EndTime { get; set; }
}
