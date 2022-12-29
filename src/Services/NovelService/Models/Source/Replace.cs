// Copyright (c) Richasy. All rights reserved.

using CleanReader.Services.Novel.Enums;

namespace CleanReader.Services.Novel.Models;

/// <summary>
/// 指定字段替换配置.
/// </summary>
public class Replace
{
    /// <summary>
    /// 指定字段.
    /// </summary>
    public FieldType Field { get; set; }

    /// <summary>
    /// 待替换的内容，可以是正则表达式.
    /// </summary>
    public string Old { get; set; }

    /// <summary>
    /// 替换的内容.
    /// </summary>
    public string New { get; set; }
}
