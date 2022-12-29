// Copyright (c) Richasy. All rights reserved.

using System;

namespace CleanReader.App.Controls;

/// <summary>
/// Card panel status change event arguments.
/// </summary>
public class CardPanelStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CardPanelStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="isPointerOver">Whether it is in the PointerOver state.</param>
    /// <param name="isPressed">Whether it is in a Pressed state.</param>
    public CardPanelStateChangedEventArgs(bool isPointerOver, bool isPressed)
    {
        IsPointerOver = isPointerOver;
        IsPressed = isPressed;
    }

    /// <summary>
    /// Whether it is in PointerOver state.
    /// </summary>
    public bool IsPointerOver { get; }

    /// <summary>
    /// Whether it is in a Pressed state.
    /// </summary>
    public bool IsPressed { get; }
}
