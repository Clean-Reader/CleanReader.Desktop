// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Toolkit.Interfaces;

/// <summary>
/// 日志记录模块.
/// </summary>
public interface ILoggerToolkit
{
    /// <summary>
    /// 记录信息.
    /// </summary>
    /// <param name="message">消息.</param>
    void LogInformation(string message);

    /// <summary>
    /// 记录信息.
    /// </summary>
    /// <param name="ex">错误.</param>
    /// <param name="isWarning">是否为警告.</param>
    void LogError(Exception ex, bool isWarning = false);
}
