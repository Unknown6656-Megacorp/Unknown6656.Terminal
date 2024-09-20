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

}
