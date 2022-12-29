// Copyright (c) Richasy. All rights reserved.
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;

namespace CleanReader.Services.Epub;

/// <summary>
/// EPUB 服务的处理 EPUB 文件生成的部分.
/// </summary>
public sealed partial class EpubService
{
    private static void CreateTempFolder()
    {
        if (!Directory.Exists(TEMP_FOLDER_PATH))
        {
            Directory.CreateDirectory(TEMP_FOLDER_PATH);
        }
    }

    private static void CopyStyleSheet()
    {
        if (!Directory.Exists(TEMP_CSS_FOLDER_PATH))
        {
            Directory.CreateDirectory(TEMP_CSS_FOLDER_PATH);
        }

        File.Copy(ASSETS_CSS_FILE_PATH, TEMP_CSS_FILE_PATH, true);
    }

    private static void CopyContainerFile()
    {
        if (!Directory.Exists(TEMP_META_FOLDER_PATH))
        {
            Directory.CreateDirectory(TEMP_META_FOLDER_PATH);
        }

        File.Copy(ASSETS_CONTAINER_FILE_PATH, TEMP_CONTAINER_FILE_PATH, true);
    }

    private static Task CreateMimeTypeFileAsync()
        => File.WriteAllTextAsync(TEMP_MIME_TYPE_FILE_PATH, MIME_TYPE);

    private void GetFiles()
        => _files = Directory.GetFiles(Configuration.SourceFolderPath, "*.html");

    private async Task CreateOpfFileAsync()
    {
        var opfTempleate = await File.ReadAllTextAsync(ASSETS_OPF_FILE_PATH);
        opfTempleate = opfTempleate.Replace("{{title}}", Configuration.Title)
                                   .Replace("{{language}}", Configuration.Language)
                                   .Replace("{{author}}", Configuration.Author)
                                   .Replace("{{date}}", DateTime.Now.ToString())
                                   .Replace("{{titlePage}}", $"Pages/{Path.GetFileName(Configuration.TitlePagePath)}");

        var manifestBuilder = new StringBuilder();
        var spineBuilder = new StringBuilder();
        foreach (var file in _files.Where(p => !p.Equals(Configuration.TitlePagePath, StringComparison.OrdinalIgnoreCase)))
        {
            var pathName = Path.GetFileNameWithoutExtension(file);
            if (!_chapterNames.ContainsKey(pathName))
            {
                continue;
            }

            var fileName = NormalizeTitle(_chapterNames[pathName]);
            manifestBuilder.AppendLine(new string(' ', 12) + $"<item id=\"ch{pathName}\" href=\"Pages/{pathName}.html\" media-type=\"application/xhtml+xml\"/>");
            spineBuilder.AppendLine(new string(' ', 12) + $"<itemref idref=\"ch{pathName}\"/>");
        }

        opfTempleate = opfTempleate.Replace("{{manifest}}", manifestBuilder.ToString())
                                   .Replace("{{spine}}", spineBuilder.ToString());
        manifestBuilder.Clear();
        spineBuilder.Clear();

        await File.WriteAllTextAsync(TEMP_CONTENT_OPF_PATH, opfTempleate);
    }

    private async Task CreateNcxFileAsync()
    {
        var ncxTemplate = await File.ReadAllTextAsync(ASSETS_NCX_FILE_PATH);
        ncxTemplate = ncxTemplate.Replace("{{title}}", Configuration.Title)
                                 .Replace("{{author}}", Configuration.Author);

        var mapBuilder = new StringBuilder();
        var tocFiles = _files.Where(p => !p.Equals(Configuration.TitlePagePath, StringComparison.OrdinalIgnoreCase));
        foreach (var file in tocFiles)
        {
            var pathName = Path.GetFileNameWithoutExtension(file);
            var fileName = NormalizeTitle(_chapterNames[pathName]);
            try
            {
                mapBuilder.AppendLine(new string(' ', 8) + $"<navPoint class=\"chapter\" id=\"ch{pathName}\" playOrder=\"{Convert.ToInt32(pathName) + 1}\">");
                mapBuilder.AppendLine(new string(' ', 12) + $"<navLabel>");
                mapBuilder.AppendLine(new string(' ', 16) + $"<text>{fileName}</text>");
                mapBuilder.AppendLine(new string(' ', 12) + $"</navLabel>");
                mapBuilder.AppendLine(new string(' ', 12) + $"<content src=\"Pages/{pathName}.html\" />");
                mapBuilder.AppendLine(new string(' ', 8) + $"</navPoint>");
            }
            catch (Exception)
            {
                continue;
            }
        }

        ncxTemplate = ncxTemplate.Replace("{{navMap}}", mapBuilder.ToString());
        mapBuilder.Clear();

        await File.WriteAllTextAsync(TEMP_TOC_NCX_PATH, ncxTemplate);
    }

    private void CreateZipFile()
    {
        using var zipFile = new ZipFile(Encoding.UTF8);
        zipFile.EmitTimesInWindowsFormatWhenSaving = false;
        zipFile.Encryption = EncryptionAlgorithm.None;
        zipFile.CompressionLevel = CompressionLevel.None;
        zipFile.AddFile(TEMP_MIME_TYPE_FILE_PATH, string.Empty);
        zipFile.CompressionLevel = CompressionLevel.BestCompression;
        zipFile.AddFile(TEMP_CONTENT_OPF_PATH, "OEBPS");
        zipFile.AddFile(TEMP_CSS_FILE_PATH, "OEBPS\\Style");
        zipFile.AddFile(TEMP_TOC_NCX_PATH, "OEBPS");
        zipFile.AddFile(Configuration.TitlePagePath, "OEBPS\\Pages");
        zipFile.AddFile(TEMP_CONTAINER_FILE_PATH, "META-INF");

        if (File.Exists(Path.Combine(Configuration.SourceFolderPath, "crn.json")))
        {
            zipFile.AddFile(Path.Combine(Configuration.SourceFolderPath, "crn.json"), "OEBPS\\Pages");
        }

        zipFile.AddFiles(_files.Where(p => Path.GetFileName(p) != Path.GetFileName(Configuration.TitlePagePath)), "OEBPS\\Pages");
        var outputPath = Path.Combine(Configuration.OutputFolderPath, Configuration.OutputFileName);
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        zipFile.Save(outputPath);
    }
}
