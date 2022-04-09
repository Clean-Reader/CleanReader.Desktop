// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Models.App
{
    /// <summary>
    /// 导航事件参数.
    /// </summary>
    public sealed class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgs"/> class.
        /// </summary>
        /// <param name="pageType">页面类型.</param>
        /// <param name="parameter">参数.</param>
        public NavigationEventArgs(Type pageType, object parameter = null)
        {
            PageType = pageType;
            Parameter = parameter;
            Id = pageType.FullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgs"/> class.
        /// </summary>
        /// <param name="navItem">导航条目.</param>
        /// <param name="parameter">参数.</param>
        public NavigationEventArgs(NavigationItem navItem, object parameter = null)
        {
            PageType = navItem.PageType;
            Parameter = parameter;
            Id = navItem.Id;
        }

        /// <summary>
        /// 页面类型.
        /// </summary>
        public Type PageType { get; set; }

        /// <summary>
        /// 导航附加参数.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 导航页面Id.
        /// </summary>
        public string Id { get; set; }
    }
}
