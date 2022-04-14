// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Models.Constants
{
    /// <summary>
    /// 视图模型用到的常量.
    /// </summary>
    public static class VMConstants
    {
#pragma warning disable SA1600 // Elements should be documented
        public static class User
        {
            public const string IdKey = "UserId";
            public const string DisplayNameKey = "DisplayName";
            public const string PrincipalNameKey = "PrincipalName";
            public const string InfoKey = "GraphUserInformation";
            public const string PhotoKey = "GraphUserPhoto";
        }

        public static class Library
        {
            public const string BookSourceFolder = ".booksource";
            public const string DbFile = "meta.db";
            public const string DbFile2 = "meta2.db";
            public const string ThemeFile = "theme.json";
            public const string BooksFolder = "books";
            public const string CoverFolder = ".covers";
        }

        public static class Shelf
        {
            public const string AllType = "All";
            public const string LocalType = "Local";
            public const string OnlineType = "Online";

            public const string SortTime = "SortByTimeAdded";
            public const string SortName = "SortByName";
            public const string SortType = "SortByType";
            public const string SortRead = "SortByLastRead";
            public const string SortProgress = "SortByProgress";
        }

        public static class Reader
        {
            public const string Toc = "Toc";
            public const string Book = "Book";
        }

        public static class Service
        {
            public const string AmbieServiceName = "com.jeniusapps.ambie";
            public const string AmbiePackageId = "43891JeniusApps.Ambie_jaj7tphbgjeh8";
            public const string AmbieShellId = "43891JeniusApps.Ambie_jaj7tphbgjeh8!App";
        }
#pragma warning restore SA1600 // Elements should be documented
    }
}
