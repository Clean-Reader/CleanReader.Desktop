// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Models.Constants
{
    /// <summary>
    /// 迁移的检查结果.
    /// </summary>
    public enum MigrationResult
    {
        /// <summary>
        /// 应用版本与数据库版本匹配.
        /// </summary>
        Matched,

        /// <summary>
        /// 应用版本较低，需要更新.
        /// </summary>
        ShouldUpdateApp,

        /// <summary>
        /// 数据库版本较低，需要更新数据库.
        /// </summary>
        ShouldUpdateDataBase,

        /// <summary>
        /// 不具备访问数据库的权限.
        /// </summary>
        AccessDenied,
    }
}
