// Copyright (c) Richasy. All rights reserved.

using System.Text.RegularExpressions;

namespace CleanReader.Services.Epub;

/// <summary>
/// EPUB 服务所需的一些变量.
/// </summary>
public partial class EpubService
{
#pragma warning disable SA1310 // Field names should not contain underscore
    private const string MIME_TYPE = "application/epub+zip";

    private static readonly Regex CHAPTER_DIVISION_REGEX = new(@"(正文){0,1}(\s|\n)(第)([\u4e00-\u9fa5a-zA-Z0-9]{1,7})[章节卷集部篇回幕计](\s{0})(.*)($\s*)");
    private static readonly string[] CHAPTER_EXTRA_KEYS = new string[] { "序", "前言", "后记", "楔子", "附录", "外传" };

    /// <summary>
    /// 章节标题字符数限制.
    /// </summary>
    private static readonly int MAX_CHAPTER_TITLE_LENGTH = 50;

    /// <summary>
    /// 根目录地址.
    /// </summary>
    public static string RootPath { get; set; }

    /// <summary>
    /// 根目录地址.
    /// </summary>
    public static string PackagePath { get; set; }

    private static string TXT_FOLDER_PATH => RootPath + "\\.txt-temp\\";

    private static string GENERATE_FOLDER_PATH => RootPath + "\\.epub-generate\\";

    private static string TEMP_FOLDER_PATH => RootPath + "\\.epub-temp\\";

    private static string TEMP_CSS_FOLDER_PATH => TEMP_FOLDER_PATH + "css\\";

    private static string TEMP_CSS_FILE_PATH => TEMP_CSS_FOLDER_PATH + "main.css";

    private static string TEMP_META_FOLDER_PATH => TEMP_FOLDER_PATH + "META-INF\\";

    private static string TEMP_CONTAINER_FILE_PATH => TEMP_META_FOLDER_PATH + "container.xml";

    private static string TEMP_MIME_TYPE_FILE_PATH => TEMP_FOLDER_PATH + "mimetype";

    private static string TEMP_CONTENT_OPF_PATH => TEMP_FOLDER_PATH + "content.opf";

    private static string TEMP_TOC_NCX_PATH => TEMP_FOLDER_PATH + "toc.ncx";

    private static string ASSETS_FOLDER_PATH => PackagePath + "\\Assets\\Epub\\";

    private static string ASSETS_CSS_FILE_PATH => ASSETS_FOLDER_PATH + "css\\main.css";

    private static string ASSETS_META_FOLDER_PATH => ASSETS_FOLDER_PATH + "META-INF\\";

    private static string ASSETS_OPS_FOLDER_PATH => ASSETS_FOLDER_PATH + "OPS\\";

    private static string ASSETS_CONTAINER_FILE_PATH => ASSETS_META_FOLDER_PATH + "container.xml";

    private static string ASSETS_OPF_FILE_PATH => ASSETS_OPS_FOLDER_PATH + "content.opf";

    private static string ASSETS_NCX_FILE_PATH => ASSETS_OPS_FOLDER_PATH + "toc.ncx";

    private static string ASSETS_PAGE_PATH => ASSETS_FOLDER_PATH + "page.html";

    private static string ASSETS_TITLE_PAGE_PATH => ASSETS_FOLDER_PATH + "title-page.html";
}
