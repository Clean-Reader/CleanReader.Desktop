// Copyright (c) Richasy. All rights reserved.

using CleanReader.Models.Constants;

namespace CleanReader.Models.Services;

/// <summary>
/// 指定字段左/右进行的补充内容.
/// </summary>
public class Repair
{
    /// <summary>
    /// 指定字段.
    /// </summary>
    public FieldType Field { get; set; }

    /// <summary>
    /// 在左边增补还是在右边增补，以'l'和'r'来分别，默认为'l'.
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// 增补内容.
    /// </summary>
    public string Value { get; set; }
}
