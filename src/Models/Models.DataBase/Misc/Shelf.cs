// Copyright (c) Richasy. All rights reserved.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CleanReader.Models.DataBase
{
    /// <summary>
    /// 书架.
    /// </summary>
    [Index(nameof(Name))]
    public class Shelf
    {
        /// <summary>
        /// 书架名.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 书架 Id.
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 书架内的书籍.
        /// </summary>
        public List<ShelfBook> Books { get; set; }

        /// <summary>
        /// 排序序号.
        /// </summary>
        public int Order { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Shelf shelf && Id == shelf.Id;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Id);
    }
}
