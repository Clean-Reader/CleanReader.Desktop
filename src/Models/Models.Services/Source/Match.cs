// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Models.Services;

/// <summary>
/// 匹配项.
/// </summary>
public class Match : Attribute
{
    /// <summary>
    /// 匹配表达式. 如果筛选出的文本和该表达式匹配，则表示符合要求.
    /// </summary>
    public string MatchRule { get; set; }
}
