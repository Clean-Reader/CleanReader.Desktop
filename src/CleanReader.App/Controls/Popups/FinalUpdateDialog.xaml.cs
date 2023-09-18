// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization;
using Windows.System;

namespace CleanReader.App.Controls.Popups
{
    /// <summary>
    /// 最终更新对话框.
    /// </summary>
    public sealed partial class FinalUpdateDialog : ContentDialog
    {
        private readonly string _zh = """
            亲！先别关这个提示！

            干净阅读已经有一段时间没有更新了，经过审慎的思考，我认为干净阅读不应仅局限于书籍，同时在 AI 浪潮下，干净阅读也应该搭上这股东风。

            所以我正在着手开发一个新的应用，该应用将会与我的另一个应用《RSS 追踪》合并，同时支持 PDF 阅读，并基于我的 AI 应用《小幻助理》来提供 AI 功能。

            新应用将会是收费买断制应用，为了感谢大家过去的支持，请点击下面的链接填写表单。我会在新应用发布后，将一次性激活码发送给大家，以避免二次付费。

            > 《阅读助理》激活码申请: https://forms.office.com/r/1cC8sju0Ci

            表单将会在 2023年 12月 31日 关闭，请大家尽快填写。

            再次感谢大家的支持！

            云之幻
            """;

        private readonly string _en = """
            Dear friends! Please don't close this prompt yet!

            Clean Reader has not been updated for long time. I haven't given up on this application, but after careful consideration, I believe that in the era of AI, book should have a new medium.

            Therefore, I am currently working on developing a new application that will merge with my other application: "Rss Track", and add PDF support. It will also incorporate AI features based on my AI application: "Fantasy Copilot".

            The new application will be a paid one-time purchase application. To thank everyone for past support, please fill out the form below. After the new application is released, I will send everyone a one-time activation code to avoid double payment.

            [《Reader Copilot》Activation code request](https://forms.office.com/r/1cC8sju0Ci)

            The form will be closed on December 31, 2023. Please fill it out as soon as possible.

            Thank you again for your support!

            Richasy
            """;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalUpdateDialog"/> class.
        /// </summary>
        public FinalUpdateDialog()
        {
            InitializeComponent();
            if (ApplicationLanguages.Languages.FirstOrDefault()?.StartsWith("zh") ?? false)
            {
                MainBlock.Text = _zh;
            }
            else
            {
                MainBlock.Text = _en;
            }
        }

        private async void OnLinkClickedAsync(object sender, CommunityToolkit.WinUI.UI.Controls.LinkClickedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(e.Link));
        }

        private async void ContentDialog_PrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var ne = _en;
            await Launcher.LaunchUriAsync(new Uri(" https://forms.office.com/r/1cC8sju0Ci"));
        }
    }
}
