using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.IO;
using System;

using Unknown6656.Runtime;

namespace Unknown6656.Console;


// This file only forwards the members of the System.Console class to the Unknown6656.Console.Console class.

public static unsafe partial class Console
{
    /// <inheritdoc cref="sysconsole.IsInputRedirected"/>
    public static bool IsInputRedirected => sysconsole.IsInputRedirected;

    /// <inheritdoc cref="sysconsole.IsOutputRedirected"/>
    public static bool IsOutputRedirected => sysconsole.IsOutputRedirected;

    /// <inheritdoc cref="sysconsole.IsErrorRedirected"/>
    public static bool IsErrorRedirected => sysconsole.IsErrorRedirected;

    /// <inheritdoc cref="sysconsole.BufferHeight"/>
    public static int BufferHeight 
    { 
        get => sysconsole.BufferHeight;
        set => BufferSize = BufferSize with { Height = value };
    }

    /// <inheritdoc cref="sysconsole.BufferWidth"/>
    public static int BufferWidth
    {
        get => sysconsole.BufferWidth;
        set => BufferSize = BufferSize with { Width = value };
    }

    /// <inheritdoc cref="sysconsole.CapsLock"/>
    public static bool CapsLock
    {
        get
        {
            if (OS.IsWindows)
                return sysconsole.CapsLock;
            else
                throw _unsupported_os; // TODO
        }
        set => SetVT100Bit(109, value);
    }

    /// <inheritdoc cref="sysconsole.CursorVisible"/>
    public static bool CursorVisible
    {
        get
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                return sysconsole.CursorVisible;
#pragma warning restore CA1416
            else
                throw _unsupported_os; // TODO
        }
        set
        {
            if (OS.IsWindows)
                sysconsole.CursorVisible = value;
            else
                SetVT100Bit(25, value);
        }
    }

}
