// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using GoogleTranslateFreeApi;
using Newtonsoft.Json;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 阅读器视图模型.
    /// </summary>
    public sealed partial class ReaderViewModel
    {
        private void ToggleCatalogVisibility()
        {
            IsCatalogShown = true;
            IsMenuShown = false;
            IsNotesShown = false;
        }

        private void ToggleNotesVisibility()
        {
            IsNotesShown = true;
            IsCatalogShown = false;
            IsMenuShown = false;
        }

        private async Task ShowInterfaceSettingsAsync()
        {
            IsMenuShown = IsCatalogShown = false;
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ReaderStyleOptionsDialog);
            var result = await dialog.ShowAsync();
            if (result == 0)
            {
                RequestInitializeStyle?.Invoke(this, EventArgs.Empty);
            }
        }

        private async Task ShowSearchDialogAsync(string text)
        {
            IsMenuShown = IsCatalogShown = false;
            SearchResult.Clear();
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.InternalSearchDialog);
            dialog.InjectData(text);
            await dialog.ShowAsync();
        }

        private void SearchInternal(string text)
        {
            IsSearcing = true;
            RequestSearch?.Invoke(this, text);
        }

        private void InitializeSearchResult(string resultStr)
        {
            SearchResult.Clear();
            var list = JsonConvert.DeserializeObject<List<ReaderSearchResult>>(resultStr);
            IsSearcing = false;
            list.ForEach(p => SearchResult.Add(p));
        }

        private async Task TranslateAsync(string source)
        {
            source = source.Trim();
            IsTranslateError = false;
            SourceText = "--";
            TranslatedText = "--";
            if (!string.IsNullOrEmpty(source) && !source.All(p => char.IsPunctuation(p)))
            {
                SourceText = source;
                var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var language = GoogleTranslator.GetLanguageByISO(lang);
                if (language == null || !GoogleTranslator.IsLanguageSupported(language))
                {
                    language = Language.ChineseSimplified;
                }

                var result = await _googleTranslator.TranslateLiteAsync(source, Language.Auto, language);
                TranslatedText = result.MergedTranslation;
            }
        }

        private async Task OnlineSearchAsync(string text)
        {
            var defaultSearchEngine = _settingsToolkit.ReadLocalSetting(SettingNames.SearchEngine, SearchEngine.Bing);
            var searchText = Uri.UnescapeDataString(text);
            var link = defaultSearchEngine switch
            {
                SearchEngine.Bing => $"https://cn.bing.com/search?q={searchText}",
                SearchEngine.Google => $"https://www.google.com/search?q={searchText}",
                SearchEngine.Baidu => $"https://www.baidu.com/s?wd={searchText}",
                SearchEngine.Sogou => $"https://www.sogou.com/web?query={searchText}",
                SearchEngine.DuckDuckGo => $"https://duckduckgo.com/?q={searchText}",
                _ => throw new NotImplementedException(),
            };

            await Windows.System.Launcher.LaunchUriAsync(new Uri(link));
        }

        private async Task ShowHighlightDialogAsync(ReaderContextMenuArgs args)
        {
            var dialog = ServiceLocator.Instance.GetService<ICustomDialog>(AppConstants.ReaderHighlightDialog);
            dialog.InjectData(args);
            await dialog.ShowAsync();
        }

        private void ClearPopup()
            => IsCatalogShown = IsNotesShown = IsMenuShown = false;

        private void DisplayTranslateError(Exception ex)
            => IsTranslateError = true;

        private void ChangeLocation(string location)
            => RequestChangeLocation?.Invoke(this, location);

        private void GoToNextChapter()
        {
            if (CurrentChapter == null)
            {
                return;
            }

            var currentIndex = Chapters.IndexOf(CurrentChapter);
            if (Chapters.Count == currentIndex + 1)
            {
                return;
            }
            else
            {
                var nextChapter = Chapters[currentIndex + 1];
                RequestChangeChapter?.Invoke(this, nextChapter.Chapter);
            }
        }

        private void GoToPreviousChapter()
        {
            if (CurrentChapter == null)
            {
                return;
            }

            var currentIndex = Chapters.IndexOf(CurrentChapter);
            if (currentIndex <= 0)
            {
                return;
            }
            else
            {
                var previousChapter = Chapters[currentIndex - 1];
                RequestChangeChapter?.Invoke(this, previousChapter.Chapter);
            }
        }

        private async Task RemoveHighlightAsync(Highlight highlight)
        {
            _libraryDbContext.Highlights.Remove(highlight);
            Highlights.Remove(highlight);
            await _libraryDbContext.SaveChangesAsync();
            RequestRemoveHighlight?.Invoke(this, highlight);
        }
    }
}
