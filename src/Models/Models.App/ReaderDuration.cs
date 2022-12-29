// Copyright (c) Richasy. All rights reserved.

using CleanReader.Models.DataBase;

namespace CleanReader.Models.App;

/// <summary>
/// 书籍阅读时长.
/// </summary>
public class ReaderDuration
{
    /// <summary>
    /// 书籍.
    /// </summary>
    public Book Book { get; set; }

    /// <summary>
    /// 总时长.
    /// </summary>
    public TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// 阅读时长占所有书籍总时长的百分比.
    /// </summary>
    public double Percentage { get; set; }
}
