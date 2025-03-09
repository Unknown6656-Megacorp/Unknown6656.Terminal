﻿using System.Runtime.Versioning;
using System.ComponentModel;
using System.Drawing;
using System;

using Unknown6656.Runtime;

namespace Unknown6656.Terminal;


public static unsafe partial class Console
{
    private static readonly PlatformNotSupportedException _unsupported_os = new("This operation is not (yet) supported supported on the current operating system.");


    /// <summary>
    /// Indicates whether to throw an <see cref="Win32Exception"/> when an invalid console mode is encountered.
    /// <para/>
    /// This is only relevant on Windows operating systems for the following members:
    /// <list type="bullet">
    ///     <item><see cref="STDINConsoleMode"/></item>
    ///     <item><see cref="STDERRConsoleMode"/></item>
    ///     <item><see cref="STDOUTConsoleMode"/></item>
    /// </list>
    /// </summary>
    public static bool ThrowOnInvalidConsoleMode { get; set; } = false;

    /// <summary>
    /// Returns the handle of the standard input stream.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static void* STDINHandle => OS.IsWindows ? NativeInterop.GetStdHandle(-10) : throw _unsupported_os;

    /// <summary>
    /// Returns the handle of the standard output stream.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static void* STDOUTHandle => OS.IsWindows ? NativeInterop.GetStdHandle(-11) : throw _unsupported_os;

    /// <summary>
    /// Returns the handle of the standard error stream.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static void* STDERRHandle => OS.IsWindows ? NativeInterop.GetStdHandle(-12) : throw _unsupported_os;

    /// <summary>
    /// Gets or sets the <see cref="ConsoleMode"/> of the standard input stream.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid console mode is encountered and <see cref="ThrowOnInvalidConsoleMode"/> is <see langword="true"/>.</exception>"
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleMode STDINConsoleMode
    {
        set
        {
            if (!OS.IsWindows)
                throw _unsupported_os;
            else if (!NativeInterop.SetConsoleMode(STDINHandle, value))
                if (ThrowOnInvalidConsoleMode)
                    throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw _unsupported_os;

            ConsoleMode mode = default;

            if (NativeInterop.GetConsoleMode(STDINHandle, &mode))
                return mode;
            else if (ThrowOnInvalidConsoleMode)
                throw NETRuntimeInterop.GetLastWin32Error();
            else
                return default;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="ConsoleMode"/> of the standard output stream.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid console mode is encountered and <see cref="ThrowOnInvalidConsoleMode"/> is <see langword="true"/>.</exception>"
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleMode STDOUTConsoleMode
    {
        set
        {
            if (!OS.IsWindows)
                throw _unsupported_os;
            else if (!NativeInterop.SetConsoleMode(STDOUTHandle, value))
                if (ThrowOnInvalidConsoleMode)
                    throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw _unsupported_os;

            ConsoleMode mode = default;

            if (NativeInterop.GetConsoleMode(STDOUTHandle, &mode))
                return mode;
            else if (ThrowOnInvalidConsoleMode)
                throw NETRuntimeInterop.GetLastWin32Error();
            else
                return default;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="ConsoleMode"/> of the standard error stream.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid console mode is encountered and <see cref="ThrowOnInvalidConsoleMode"/> is <see langword="true"/>.</exception>"
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleMode STDERRConsoleMode
    {
        set
        {
            if (!OS.IsWindows)
                throw _unsupported_os;
            else if (!NativeInterop.SetConsoleMode(STDERRHandle, value))
                if (ThrowOnInvalidConsoleMode)
                    throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw _unsupported_os;

            ConsoleMode mode = default;

            if (NativeInterop.GetConsoleMode(STDERRHandle, &mode))
                return mode;
            else if (ThrowOnInvalidConsoleMode)
                throw NETRuntimeInterop.GetLastWin32Error();
            else
                return default;
        }
    }

    /// <summary>
    /// Sets or gets the current console font information.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid <see cref="ConsoleFontInfo"/> is encountered or if any errors occurred whilst reading/writing to this property.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleFontInfo FontInfo
    {
        set
        {
            if (!OS.IsWindows)
                throw _unsupported_os;
            else if (!NativeInterop.SetCurrentConsoleFontEx(STDOUTHandle, false, ref value))
                throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw _unsupported_os;

            ConsoleFontInfo font = new()
            {
                cbSize = sizeof(ConsoleFontInfo),
            };

            return NativeInterop.GetCurrentConsoleFontEx(STDOUTHandle, false, ref font) ? font : throw NETRuntimeInterop.GetLastWin32Error();
        }
    }

    /// <summary>
    /// Sets the current console font to the specified font.
    /// This is currently only supported on Windows operating systems.
    /// </summary>
    [SupportedOSPlatform(OS.WIN)]
    public static Font Font
    {
        set => SetCurrentFont(value);
        get
        {
            ConsoleFontInfo font = FontInfo;

            return new Font(font.FontName, font.FontSize.H, font.FontWeight > 550 ? FontStyle.Bold : FontStyle.Regular);
        }
    }

    /// <summary>
    /// Sets the current console font to the specified <see cref="System.Drawing.Font"/> and returns the font information before and after the change.
    /// </summary>
    /// <param name="font">The new font to set for the console.</param>
    /// <returns>A tuple containing the <see cref="ConsoleFontInfo"/> before and after the change.</returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if this method is called on a non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown if an error occurs while setting the console font.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static (ConsoleFontInfo before, ConsoleFontInfo after) SetCurrentFont(Font font)
    {
        ConsoleFontInfo before = FontInfo;
        ConsoleFontInfo set = new()
        {
            cbSize = sizeof(ConsoleFontInfo),
            FontIndex = 0,
            FontFamily = ConsoleFontInfo.FIXED_WIDTH_TRUETYPE,
            FontName = font.Name,
            FontWeight = font.Bold ? 700 : 400,
            FontSize = font.Size > 0 ? (default, (short)font.Size) : before.FontSize,
        };

        FontInfo = set;

        return (before, FontInfo);
    }
}
