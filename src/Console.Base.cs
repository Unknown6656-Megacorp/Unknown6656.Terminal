using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Linq;
using System.IO;
using System;

using Unknown6656.Generics;
using Unknown6656.Runtime;

namespace Unknown6656.Console;


// This file only forwards the members of the System.Console class to the Unknown6656.Console.Console class.


public static unsafe partial class Console
{
    /// <inheritdoc cref="sysconsole.Error"/>
    public static TextWriter Error => sysconsole.Error;

    /// <inheritdoc cref="sysconsole.In"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static TextReader In => sysconsole.In;

    /// <inheritdoc cref="sysconsole.Out"/>
    public static TextWriter Out => sysconsole.Out;

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
#pragma warning disable CA1416 // Validate platform compatibility
                return sysconsole.CapsLock;
#pragma warning restore CA1416
            else
#warning TODO : implement capslock functionality on non-Windows systems
                throw _unsupported_os;
        }
        set => SetVT520Bit(109, value);
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
                SetVT520Bit(25, value);
        }
    }

    /// <inheritdoc cref="sysconsole.CursorLeft"/>
    public static int CursorLeft
    {
        get
        {
            if (OS.IsUnix || OS.IsWindows)
                return sysconsole.CursorLeft;
            else if (GetExtendedCursorPosition() is (int left, _, _))
                return left - 1;
            else
                throw new InvalidOperationException("Failed to get cursor position.");
        }
        set
        {
            if (OS.IsUnix || OS.IsWindows)
                sysconsole.CursorLeft = value;
            else
                sysconsole.Write($"\e[{value + 1}G");
        }
    }

    /// <inheritdoc cref="sysconsole.CursorTop"/>
    public static int CursorTop
    {
        get
        {
            if (OS.IsUnix || OS.IsWindows)
                return sysconsole.CursorTop;
            else if (GetExtendedCursorPosition() is (_, int top, _))
                return top - 1;
            else
                throw new InvalidOperationException("Failed to get cursor position.");
        }
        set
        {
            if (OS.IsUnix || OS.IsWindows)
                sysconsole.CursorTop = value;
            else
                sysconsole.Write($"\e[{value + 1};{CursorLeft}H");
        }
    }






    private static (int X, int Y, int Page)? GetVT100CursorPosition()
    {
        if (GetRawVT100Report("[?6n", 'R') is ['\e', '[', '?', .. string response])
        {
            int[] parts = response.Split(';').ToArray(int.Parse);

            return (parts[0], parts[1], parts[2]);
        }
        else
            return null;
    }

    //public static int CursorSize { get; set; }


}
