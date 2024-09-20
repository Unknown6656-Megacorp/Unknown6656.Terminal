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
    #region PROPERTIES: I/E/O STREAMS

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

    /// <inheritdoc cref="sysconsole.InputEncoding"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static Encoding InputEncoding
    {
        get => sysconsole.InputEncoding;
        set => sysconsole.InputEncoding = value;
    }

    /// <inheritdoc cref="sysconsole.OutputEncoding"/>
    public static Encoding OutputEncoding
    {
        get => sysconsole.OutputEncoding;
        set => sysconsole.OutputEncoding = value;
    }

    /// <inheritdoc cref="sysconsole.IsInputRedirected"/>
    public static bool IsInputRedirected => sysconsole.IsInputRedirected;

    /// <inheritdoc cref="sysconsole.IsOutputRedirected"/>
    public static bool IsOutputRedirected => sysconsole.IsOutputRedirected;

    /// <inheritdoc cref="sysconsole.IsErrorRedirected"/>
    public static bool IsErrorRedirected => sysconsole.IsErrorRedirected;

    #endregion
    #region PROPERTIES: BUFFER/WINDOW SIZE AND POSITION

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

    #endregion
    #region PROPERTIES: CURSOR SIZE, POSITION, VISIBILITY

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

    /// <inheritdoc cref="sysconsole.CursorSize"/>
    [SupportedOSPlatform(OS.WIN)]
    public static int CursorSize
    {
        get => sysconsole.CursorSize;
        set => sysconsole.CursorSize = value;
    }

    #endregion
    #region PROPERTIES: KEYBOARD

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




    #endregion
    #region METHOS: BUFFER/WINDOW/CURSOR SIZE AND POSITION

    /// <inheritdoc cref="GetCursorPosition"/>
    public static (int Left, int Top) GetCursorPosition()
    {
        if (OS.IsUnix || OS.IsWindows)
            return sysconsole.GetCursorPosition();
        else if (GetExtendedCursorPosition() is (int left, int top, _))
            return (left - 1, top - 1);
        else
            throw new InvalidOperationException("Failed to get cursor position.");
    }

    /// <inheritdoc cref="SetWindowPosition"/>
    public static void SetWindowPosition(int left, int top)
    {
        if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
            sysconsole.SetWindowPosition(left, top);
#pragma warning restore CA1416
        else
            Write($"\e[3;{left};{top}t");
    }

    /// <inheritdoc cref="SetWindowSize(int, int)"/>
    public static void SetWindowSize(int width, int height)
    {
        if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
            SetWindowSize(width, height);
#pragma warning restore CA1416
        else
            Write($"\e[8;{height};{width}t");
    }

    /// <inheritdoc cref="SetBufferSize(int, int)"/>
    [SupportedOSPlatform(OS.WIN)]
    public static void SetBufferSize(int width, int height) => sysconsole.SetBufferSize(width, height);

    #endregion





    //public static int CursorSize { get; set; }


}
