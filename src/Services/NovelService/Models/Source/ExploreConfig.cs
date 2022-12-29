// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Services.Novel.Models;

/// <summary>
/// 发现模块配置.
/// </summary>
public class ExploreConfig : BookInformationConfigBase
{
    /// <summary>
    /// 分类列表.
    /// </summary>
    public List<Category> Categories { get; set; }

    /// <summary>
    /// 是否需要进入书籍详情获取内容.
    /// </summary>
    public bool NeedDetail { get; set; }

    /// <summary>
    /// 请求配置.
    /// </summary>
    public RequestConfig Request { get; set; }
}
