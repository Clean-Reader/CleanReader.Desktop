// Copyright (c) Richasy. All rights reserved.

using CleanReader.Locator.Lib;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI.Xaml.Markup;

namespace CleanReader.Locator.Desktop
{
    /// <summary>
    /// Localized text extension.
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class LocaleLocatorExtension : MarkupExtension
    {
        /// <summary>
        /// Language name.
        /// </summary>
        public LanguageNames Name { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            return ServiceLocator.Instance.GetService<IResourceToolkit>()
                                          .GetLocaleString(this.Name);
        }
    }
}
