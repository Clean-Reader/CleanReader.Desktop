// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Enums;

/// <summary>
/// 在线书籍状态.
/// </summary>
public enum BookStatus
{
    /// <summary>
    /// 无效.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// 连载中.
    /// </summary>
    Writing = 1,

    /// <summary>
    /// 已完结.
    /// </summary>
    Finish = 2,
}
