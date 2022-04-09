// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Toolkit.Interfaces
{
    /// <summary>
    /// <see cref="GraphToolkitState"/> 标识当前 Microsoft Graph 组件的授权状态.
    /// </summary>
    public enum GraphToolkitState
    {
        /// <summary>
        /// The user's status is not known.
        /// </summary>
        Loading,

        /// <summary>
        /// The user is signed-out.
        /// </summary>
        SignedOut,

        /// <summary>
        /// The user is signed-in.
        /// </summary>
        SignedIn,
    }
}
