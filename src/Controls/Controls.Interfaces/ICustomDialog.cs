// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Controls.Interfaces;

/// <summary>
/// 自定义对话框接口.
/// </summary>
public interface ICustomDialog
{
    /// <summary>
    /// 显示对话框.
    /// </summary>
    /// <returns><c>-1</c> 表示从代码中关闭，<c>0</c> 表示点击主按钮关闭，<c>1</c> 表示次按钮，<c>2</c> 表示关闭按钮.</returns>
    Task<int> ShowAsync();

    /// <summary>
    /// 隐藏对话框.
    /// </summary>
    void Hide();

    /// <summary>
    /// 插入数据.
    /// </summary>
    /// <param name="data">数据.</param>
    void InjectData(object data);

    /// <summary>
    /// 插入任务.
    /// </summary>
    /// <param name="task">任务.</param>
    /// <param name="cancellationTokenSource">终止令牌.</param>
    void InjectTask(Task task, CancellationTokenSource cancellationTokenSource = null);
}
