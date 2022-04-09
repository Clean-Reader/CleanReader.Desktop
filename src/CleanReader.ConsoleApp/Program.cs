// Copyright (c) Richasy. All rights reserved.

using CleanReader.Services.Novel;
using Newtonsoft.Json;

// var watch = new Stopwatch();
// watch.Start();
// var configuration = await EpubService.SplitTxtFileAsync(Directory.GetCurrentDirectory() + "\\启明1158.txt", new Regex(@"[零一二三四五六七八九十百千万]{1,10}[ ]{1}(.*)"));

// if (!Directory.Exists(configuration.OutputFolderPath))
// {
//    Directory.CreateDirectory(configuration.OutputFolderPath);
// }

// Console.WriteLine($"拆分TXT用时：{watch.Elapsed.TotalSeconds}s");

// var creator = new EpubService(configuration);
// await creator.CreateAsync();
// Console.WriteLine($"总用时：{watch.Elapsed.TotalSeconds}s");
// Console.WriteLine($"已导出：{Path.Combine(configuration.OutputFolderPath, configuration.OutputFileName)}");
var service = new NovelService();
await service.InitializeBookSourcesAsync("C:\\Users\\zar23\\Desktop\\测试书库\\.booksource");
var data = await service.SearchBookAsync("顶级");
Console.WriteLine($"{JsonConvert.SerializeObject(data.SelectMany(p => p.Value))}");
Console.ReadKey();
