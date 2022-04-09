// Copyright (c) Richasy. All rights reserved.

using System;
using CleanReader.Locator.Lib;
using CleanReader.Models.App;
using CleanReader.Models.Constants;
using CleanReader.Toolkit.Interfaces;
using Microsoft.UI.Xaml;
using Windows.System;

namespace CleanReader.App.Controls
{
    /// <summary>
    /// Github更新时现实的对话框.
    /// </summary>
    public sealed partial class GithubUpdateDialog : CustomDialog
    {
        private Uri downloadUrl;
        private string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="GithubUpdateDialog"/> class.
        /// </summary>
        public GithubUpdateDialog()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public override void InjectData(object data)
        {
            if (data is GithubReleaseResponse response)
            {
                var releaseTitle = response.Name;
                var releaseDescription = response.Description;
                var publishTime = response.PublishTime.ToLocalTime();
                var isPreRelease = response.IsPreRelease;
                downloadUrl = new Uri(response.Url);
                version = response.TagName.Replace("v", string.Empty).Replace(".pre-release", string.Empty);

                TitleBlock.Text = releaseTitle;
                PreReleaseContainer.Visibility = isPreRelease ? Visibility.Visible : Visibility.Collapsed;
                PublishTimeBlock.Text = publishTime.ToString("yyyy/MM/dd HH:mm");
                MarkdownBlock.Text = releaseDescription;
            }
        }

        /// <inheritdoc/>
        public override async void OnPrimaryButtonClick()
            => await Launcher.LaunchUriAsync(downloadUrl);

        /// <inheritdoc/>
        public override void OnSecondaryButtonClick()
        {
            var settingToolkit = ServiceLocator.Instance.GetService<ISettingsToolkit>();
            settingToolkit.WriteLocalSetting(SettingNames.IgnoreVersion, version);
        }
    }
}
