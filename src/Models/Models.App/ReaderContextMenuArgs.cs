// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Models.App;

/// <summary>
/// 阅读器上下文菜单参数.
/// </summary>
public class ReaderContextMenuArgs
{
    /// <summary>
    /// 选中文本.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// X坐标.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Y坐标.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// 文本选中范围.
    /// </summary>
    public string Range { get; set; }
}
