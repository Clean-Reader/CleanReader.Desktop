// Copyright (c) Richasy. All rights reserved.

namespace CleanReader.Toolkit.Interfaces;

/// <summary>
/// <see cref="IGraphToolkit.StateChanged"/> event arguments.
/// </summary>
public class GraphToolkitStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphToolkitStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldState">Previous <see cref="GraphToolkitState"/>.</param>
    /// <param name="newState">Current <see cref="GraphToolkitState"/>.</param>
    public GraphToolkitStateChangedEventArgs(GraphToolkitState? oldState, GraphToolkitState? newState)
    {
        OldState = oldState;
        NewState = newState;
    }

    /// <summary>
    /// Gets the previous state of the <see cref="IProvider"/>.
    /// </summary>
    public GraphToolkitState? OldState { get; private set; }

    /// <summary>
    /// Gets the new state of the <see cref="IProvider"/>.
    /// </summary>
    public GraphToolkitState? NewState { get; private set; }
}
