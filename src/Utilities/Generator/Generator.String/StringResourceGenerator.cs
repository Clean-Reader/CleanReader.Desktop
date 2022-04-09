// Copyright (c) Richasy. All rights reserved.

using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Generator.String
{
    /// <summary>
    /// 文本资源生成器.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class StringResourceGenerator : IIncrementalGenerator
    {
        private static readonly Regex ResourceKeyValidateRegex = new Regex(@"^\w+(\.\w+)*$", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// 添加资源文件.
        /// </summary>
        /// <param name="context">项目上下文.</param>
        /// <param name="filePath">资源文件路径.</param>
        public static void Execute(SourceProductionContext context, string filePath)
        {
            var resDoc = XDocument.Load(filePath);
            context.AddSource("StringResources.g.cs", GenerateCode(resDoc));
        }

        /// <summary>
        /// 初始化.
        /// </summary>
        /// <param name="context">资源初始化上下文.</param>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var reswFilesProvider = context.AdditionalTextsProvider.Where(file => IsResourcesFile(file.Path) && File.Exists(file.Path));
            context.RegisterSourceOutput(reswFilesProvider, (c, f) => Execute(c, f.Path));
        }

        private static bool IsResourcesFile(string path) => "Resources.resw".Equals(Path.GetFileName(path));

        private static string GenerateCode(XDocument reswDoc)
        {
            var sb = new StringBuilder();

            sb.Append(@"
using CleanReader.Locator.Lib;
using CleanReader.Toolkit.Interfaces;

namespace CleanReader.Models.Resources
{
    public static class StringResources
    {");

            var stringDataElements = reswDoc.Root.Elements("data")
                .Where(elem => elem.Attribute("type") == null)
                .Where(elem => ResourceKeyValidateRegex.IsMatch(elem.Attribute("name").Value))
                .OrderBy(elem => elem.Attribute("name").Value);

            foreach (var dataElement in stringDataElements)
            {
                var propertyName = dataElement.Attribute("name").Value.Replace(".", string.Empty);
                var resourceKey = dataElement.Attribute("name").Value.Replace(".", "/");
                var zhCNTextValue = dataElement.Element("value").Value.Replace("\r", string.Empty).Replace("\n", "\\n ");
                var comment = HttpUtility.HtmlEncode(dataElement.Element("comment")?.Value.Replace("\r", string.Empty).Replace("\n", " ") ?? string.Empty);
                sb.Append(@"
        /// <summary>");
                if (!string.IsNullOrEmpty(comment))
                {
                    sb.Append($@"
        /// {comment}.");
                }

                sb.Append($@"
        /// <para>Text(zh-CN): ""<![CDATA[{zhCNTextValue}]]>"".</para>
        /// </summary>
        public static string {propertyName} => ServiceLocator.Instance.GetService<IResourceToolkit>().GetLocaleString(""{resourceKey}"");");
            }

            sb.Append(@"
    }
}");

            return sb.ToString();
        }
    }
}
