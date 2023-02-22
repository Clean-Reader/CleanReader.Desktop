// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CleanReader.ViewModels.Desktop;

/// <summary>
/// ViewModel的基类.
/// </summary>
public class ViewModelBase : ObservableObject
{
    /// <summary>
    /// 对异步命令添加错误回调.
    /// </summary>
    /// <param name="handler">处理错误的回调.</param>
    /// <param name="commands">异步命令集.</param>
    protected static void AttachExceptionHandlerForAsyncCommand(Action<Exception> handler, params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AsyncRelayCommand.ExecutionTask) &&
                    ((IAsyncRelayCommand)s).ExecutionTask is Task task &&
                    task.Exception is AggregateException exception)
                {
                    exception.Handle(ex =>
                    {
                        handler(ex);
                        return true;
                    });
                }
            };
        }
    }

    /// <summary>
    /// 添加对 <see cref="AsyncRelayCommand.IsRunning"/> 属性的处理回调.
    /// </summary>
    /// <param name="handler">Callback.</param>
    /// <param name="commands">command set.</param>
    protected static void AttachIsRunningForAsyncCommand(Action<bool> handler, params IAsyncRelayCommand[] commands)
    {
        foreach (var command in commands)
        {
            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AsyncRelayCommand.IsRunning))
                {
                    handler(command.IsRunning);
                }
            };
        }
    }

    /// <summary>
    /// 尝试清除集合.
    /// </summary>
    /// <typeparam name="T"><see cref="{T}"/>.</typeparam>
    /// <param name="collection">集合.</param>
    /// <remarks>
    /// 在清理空集合时可能会出现错误.
    /// </remarks>
    protected static void TryClear<T>(ObservableCollection<T> collection)
    {
        if (collection?.Count > 0)
        {
            collection.Clear();
        }
    }
}
