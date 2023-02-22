// Copyright (c) Richasy. All rights reserved.

using System.Text.RegularExpressions;
using CleanReader.Models.Services;

namespace CleanReader.Services.Interfaces;

/// <summary>
/// Epub文件相关的服务的接口定义.
/// </summary>
public interface IEpubService
{
    /// <summary>
    /// 根目录地址.
    /// </summary>
    string RootPath { get; set; }

    /// <summary>
    /// 包地址.
    /// </summary>
    string PackagePath { get; set; }

    /// <summary>
    /// 拆分 TXT 章节，并生成对应的文件列表.
    /// </summary>
    /// <param name="filePath">文件路径.</param>
    /// <param name="splitRegex">分章正则表达式.</param>
    /// <param name="cancellationTokenSource">终止令牌.</param>
    /// <returns>EPUB 服务配置项.</returns>
    /// <exception cref="ArgumentException">传入的文件无法打开.</exception>
    Task<EpubServiceConfiguration> SplitTxtFileAsync(string filePath, Regex splitRegex = null, CancellationTokenSource cancellationTokenSource = null);

    /// <summary>
    /// 使用指定分割正则表达式分割章节.
    /// </summary>
    /// <param name="filePath">TXT 文件路径.</param>
    /// <param name="splitRegex">分章正则表达式.</param>
    /// <returns>分割的章节列表，包含章节名和字数.</returns>
    /// <exception cref="ArgumentException">传入的文件路径有误.</exception>
    Task<List<Tuple<string, int>>> GenerateTxtChaptersAsync(string filePath, Regex splitRegex = null);

    /// <summary>
    /// 将已拆分的内容组合成Epub文件.
    /// </summary>
    /// <param name="data">拆分的数据.</param>
    /// <param name="configuration">书籍配置.</param>
    /// <returns>保存的文件路径.</returns>
    Task<EpubServiceConfiguration> InitializeSplitedBookAsync(List<Tuple<int, string, string>> data, EpubServiceConfiguration configuration);

    /// <summary>
    /// 初始化追加章节.
    /// </summary>
    /// <param name="sourceBookPath">来源书籍路径.</param>
    /// <param name="data">追加的章节内容.</param>
    /// <param name="configuration">配置.</param>
    /// <returns>配置文件.</returns>
    Task<EpubServiceConfiguration> InitializeAdditionalChaptersAsync(string sourceBookPath, List<Tuple<int, string, string>> data, EpubServiceConfiguration configuration);

    /// <summary>
    /// 设置配置.
    /// </summary>
    /// <param name="configuration">Epub生成配置.</param>
    void SetConfiguration(EpubServiceConfiguration configuration);

    /// <summary>
    /// 是否需要重新生成epub文件.
    /// </summary>
    /// <param name="sourceFile">源文件.</param>
    /// <returns>结果.</returns>
    bool NeedRegenerate(string sourceFile);

    /// <summary>
    /// 清理缓存.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// 清理已生成的文件.
    /// </summary>
    void ClearGenerated();

    /// <summary>
    /// 创建 Epub 文件.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task CreateAsync();
}
