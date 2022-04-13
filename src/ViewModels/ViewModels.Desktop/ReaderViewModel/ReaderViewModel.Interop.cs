// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Models.DataBase;
using CleanReader.Models.Resources;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CleanReader.ViewModels.Desktop
{
    /// <summary>
    /// 阅读器视图模型.
    /// </summary>
    public sealed partial class ReaderViewModel
    {
        /// <summary>
        /// 保存进度.
        /// </summary>
        /// <param name="progressString">进度字符串.</param>
        /// <returns><see cref="Task"/>.</returns>
        private async Task SaveProgressAsync(string progressString)
        {
            if (!string.IsNullOrEmpty(progressString))
            {
                var endTime = DateTime.Now;
                var data = JsonConvert.DeserializeObject<ReaderProgress>(progressString);
                ClearPopup();

                if (!string.IsNullOrEmpty(data.ChapterId) || !string.IsNullOrEmpty(data.ChapterHref))
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        CurrentChapter = !string.IsNullOrEmpty(data.ChapterId)
                            ? _chapterList.Where(p => p.Chapter.Id.Equals(data.ChapterId)).FirstOrDefault()
                            : _chapterList.Where(p => p.Chapter.Href.Contains(data.ChapterHref)).FirstOrDefault();
                        CurrentChapterTitle = CurrentChapter?.ToString() ?? string.Empty;
                        SetChapterSelection();
                    });
                }

                if (!string.IsNullOrEmpty(data.StartCfi))
                {
                    var hasCache = true;
                    var sourceHistory = await _libraryDbContext.Histories.Include(p => p.ReadSections).FirstOrDefaultAsync(p => p.BookId == _book.Id);
                    if (sourceHistory == null)
                    {
                        hasCache = false;
                        sourceHistory = new History
                        {
                            BookId = _book.Id,
                        };
                    }

                    sourceHistory.Position = data.StartCfi;
                    sourceHistory.Progress = data.CurrentPage > 0 && data.TotalPage > 0
                        ? data.CurrentPage * 100.0 / data.TotalPage
                        : 0;

                    _book.Status = sourceHistory.Progress < 99.9 ? BookStatus.Reading : BookStatus.Finish;

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        Progresss = sourceHistory.Progress.ToString("0.0") + "%";
                    });

                    AddReadDuration(sourceHistory, endTime);

                    if (!hasCache)
                    {
                        _libraryDbContext.Histories.Add(sourceHistory);
                    }
                    else
                    {
                        _libraryDbContext.Histories.Update(sourceHistory);
                    }

                    _libraryDbContext.Books.Update(_book);
                    await _libraryDbContext.SaveChangesAsync();
                    _startTime = endTime;
                }
            }
        }

        private async Task SaveLocationsAsync(string locations)
        {
            if (string.IsNullOrEmpty(locations))
            {
                return;
            }

            var hasCache = true;
            var history = await _libraryDbContext.Histories.FirstOrDefaultAsync(p => p.BookId == _book.Id);
            if (history == null)
            {
                hasCache = false;
                history = new History()
                {
                    BookId = _book.Id,
                };
            }

            history.Locations = locations;

            if (!hasCache)
            {
                _libraryDbContext.Histories.Add(history);
            }
            else
            {
                _libraryDbContext.Histories.Update(history);
            }

            await _libraryDbContext.SaveChangesAsync();
        }

        private void InitializeToc(string chapterString)
        {
            Chapters.Clear();
            if (string.IsNullOrEmpty(chapterString))
            {
                return;
            }

            var list = JsonConvert.DeserializeObject<List<ReaderChapter>>(chapterString);

            DispatcherQueue.TryEnqueue(() =>
            {
                list.ForEach(p =>
                {
                    p.Label = p.Label.Trim();
                    if (p.Children?.Any() ?? false)
                    {
                        p.Children.ForEach(j => j.Label = j.Label.Trim());
                    }

                    Chapters.Add(new ReaderChapterViewModel(p));
                });

                _chapterList = new List<ReaderChapterViewModel>();
                foreach (var chapter in Chapters)
                {
                    AddAllChapters(chapter, _chapterList);
                }
            });

            static void AddAllChapters(ReaderChapterViewModel chapter, List<ReaderChapterViewModel> chapters)
            {
                chapters.Add(chapter);
                if (chapter.Children?.Any() ?? false)
                {
                    foreach (var item in chapter.Children)
                    {
                        AddAllChapters(item, chapters);
                    }
                }
            }
        }

        private void DisplayInitializeError(string type)
        {
            IsInitializeFailed = true;
            switch (type)
            {
                case VMConstants.Reader.Toc:
                    InitializeErrorText = StringResources.LoadTocFailed;
                    break;
                case VMConstants.Reader.Book:
                    InitializeErrorText = StringResources.BookNotExist;
                    break;
                default:
                    break;
            }
        }

        private void AddReadDuration(History source, DateTime endTime)
        {
            if (source != null)
            {
                if (_startTime == DateTime.MinValue)
                {
                    _startTime = endTime.AddSeconds(-1);
                }

                source.ReadDuration += (endTime - _startTime).TotalSeconds;
                var isSectionAdded = false;
                if (source.ReadSections?.Any() ?? false)
                {
                    var last = source.ReadSections.Last();
                    if (last.EndTime.ToString("yyyy/MM/dd/HH/mm/ss") == _startTime.ToString("yyyy/MM/dd/HH/mm/ss"))
                    {
                        last.EndTime = endTime;
                        isSectionAdded = true;
                    }
                }

                if (!isSectionAdded)
                {
                    if (source.ReadSections == null)
                    {
                        source.ReadSections = new List<ReadSection>();
                    }

                    source.ReadSections.Add(new ReadSection()
                    {
                        Id = _startTime.ToString("yyyy/MM/dd/HH/mm/ss"),
                        StartTime = _startTime,
                        EndTime = endTime,
                    });
                }
            }
        }
    }
}
