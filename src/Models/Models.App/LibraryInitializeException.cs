// Copyright (c) Richasy. All rights reserved.

using CleanReader.Models.Constants;

namespace CleanReader.Models.App
{
    /// <summary>
    /// 书库初始化异常.
    /// </summary>
    public class LibraryInitializeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryInitializeException"/> class.
        /// </summary>
        public LibraryInitializeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryInitializeException"/> class.
        /// </summary>
        /// <param name="message">消息.</param>
        public LibraryInitializeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryInitializeException"/> class.
        /// </summary>
        /// <param name="message">消息.</param>
        /// <param name="result">错误信息.</param>
        public LibraryInitializeException(string message, MigrationResult result)
            : base(message) => Result = result;

        /// <summary>
        /// 检查结果.
        /// </summary>
        public MigrationResult Result { get; set; }
    }
}
