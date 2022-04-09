// Copyright (c) Richasy. All rights reserved.

using System.Collections.ObjectModel;

namespace CleanReader.Models.App
{
    /// <summary>
    /// 导航条目.
    /// </summary>
    public sealed class NavigationItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        public NavigationItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        /// <param name="type">类型.</param>
        /// <param name="position">位置.</param>
        /// <param name="name">显示名称.</param>
        /// <param name="icon">显示图标.</param>
        /// <param name="pageType">页面类型.</param>
        /// <param name="id">页面标识.</param>
        public NavigationItem(NavigationItemType type, NavigationItemPosition position, string name = null, string icon = null, Type pageType = null, string id = null)
        {
            Type = type;
            Position = position;
            Name = name;
            Icon = icon;
            PageType = pageType;
            Id = id ?? pageType?.FullName ?? $"{type}-{position}";
        }

        /// <summary>
        /// 显示名称.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示图标.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 关联的页面类型.
        /// </summary>
        public Type PageType { get; set; }

        /// <summary>
        /// 条目所在位置.
        /// </summary>
        public NavigationItemPosition Position { get; set; }

        /// <summary>
        /// 条目类型.
        /// </summary>
        public NavigationItemType Type { get; set; }

        /// <summary>
        /// 子项.
        /// </summary>
        public ObservableCollection<NavigationItem> Children { get; set; }

        /// <summary>
        /// 标识符.
        /// </summary>
        public string Id { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is NavigationItem item && Id == item.Id;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Id);
    }
}
