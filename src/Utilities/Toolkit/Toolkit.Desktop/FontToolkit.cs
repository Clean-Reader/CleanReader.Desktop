// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using CleanReader.Toolkit.Interfaces;

namespace CleanReader.Toolkit.Desktop
{
    /// <summary>
    /// 字体处理工具.
    /// </summary>
    public class FontToolkit : IFontToolkit
    {
        /// <inheritdoc/>
        public List<string> GetSystemFontList()
        {
            try
            {
                using var col = new InstalledFontCollection();
                return col.Families.Select(p => p.Name).ToList();
            }
            catch (System.Exception)
            {
                return new List<string>
                {
                    "Segoe UI",
                };
            }
        }
    }
}
