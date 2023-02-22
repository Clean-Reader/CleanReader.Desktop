// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Controls.Interfaces;

/// <summary>
/// 主窗口的接口定义.
/// </summary>
public interface IMainWindow
{
    /// <summary>
    /// 显示顶层视图.
    /// </summary>
    /// <param name="element">要显示的元素.</param>
    void ShowOnHolder(object element);

    /// <summary>
    /// 从顶层视图中移除元素.
    /// </summary>
    /// <param name="element">UI元素.</param>
    void RemoveFromHolder(object element);
}
