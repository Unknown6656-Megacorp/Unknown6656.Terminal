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
    public static TextWriter Error
    {
        get => sysconsole.Error;
        set => SetError(value);
    }

    /// <inheritdoc cref="sysconsole.In"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static TextReader In
    {
        get => sysconsole.In;
        set => SetIn(value);
    }

    /// <inheritdoc cref="sysconsole.Out"/>
    public static TextWriter Out
    {
        get => sysconsole.Out;
        set => SetOut(value);
    }

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

    /// <inheritdoc cref="sysconsole.WindowWidth"/>
    public static int WindowWidth
    {
        get => sysconsole.WindowWidth;
        set
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                sysconsole.WindowWidth = value;
#pragma warning restore CA1416
            else
#warning TODO : implement this on non-Windows systems
                throw _unsupported_os;
        }
    }

    /// <inheritdoc cref="sysconsole.WindowHeight"/>
    public static int WindowHeight
    {
        get => sysconsole.WindowHeight;
        set
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                sysconsole.WindowHeight = value;
#pragma warning restore CA1416
            else
#warning TODO : implement this on non-Windows systems
                throw _unsupported_os;
        }
    }

    /// <inheritdoc cref="sysconsole.WindowLeft"/>
    public static int WindowLeft
    {
        get => sysconsole.WindowLeft;
        set
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                sysconsole.WindowLeft = value;
#pragma warning restore CA1416
            else
                SetWindowPosition(value, WindowTop);
        }
    }

    /// <inheritdoc cref="sysconsole.WindowTop"/>
    public static int WindowTop
    {
        get => sysconsole.WindowTop;
        set
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                sysconsole.WindowTop = value;
#pragma warning restore CA1416
            else
                SetWindowPosition(WindowLeft, value);
        }
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
        get => OS.IsUnix || OS.IsWindows ? sysconsole.CursorLeft : GetCursorPosition().Left;
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
        get => OS.IsUnix || OS.IsWindows ? sysconsole.CursorTop : GetCursorPosition().Top;
        set
        {
            if (OS.IsUnix || OS.IsWindows)
                sysconsole.CursorTop = value;
            else
                SetCursorPosition(CursorLeft, value);
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

    /// <inheritdoc cref="sysconsole.NumberLock"/>
    public static bool NumberLock
    {
        get
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                return sysconsole.NumberLock;
#pragma warning restore CA1416
            else
#warning TODO : implement this functionality on non-Windows systems
                throw _unsupported_os;
        }
        set => SetVT520Bit(108, value);
    }

    /// <inheritdoc cref="sysconsole.KeyAvailable"/>
    public static bool KeyAvailable => sysconsole.KeyAvailable;

    /// <inheritdoc cref="sysconsole.TreatControlCAsInput"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
#warning TODO : implement this on non-Windows systems
    public static bool TreatControlCAsInput
    {
        get => sysconsole.TreatControlCAsInput;
        set => sysconsole.TreatControlCAsInput = value;
    }

    #endregion
    #region METHODS: BUFFER/WINDOW/CURSOR SIZE AND POSITION

    /// <inheritdoc cref="sysconsole.GetCursorPosition"/>
    public static (int Left, int Top) GetCursorPosition()
    {
        if (OS.IsUnix || OS.IsWindows)
            return sysconsole.GetCursorPosition();
        else if (GetExtendedCursorPosition() is (int left, int top, _))
            return (left - 1, top - 1);
        else
            throw new InvalidOperationException("Failed to get cursor position.");
    }

    /// <inheritdoc cref="sysconsole.SetCursorPosition"/>
    public static void SetCursorPosition(int left, int top)
    {
        if (OS.IsUnix || OS.IsWindows)
            sysconsole.SetCursorPosition(left, top);
        else
            sysconsole.Write($"\e[{top + 1};{left + 1}H");
    }

    /// <inheritdoc cref="sysconsole.SetWindowPosition"/>
    public static void SetWindowPosition(int left, int top)
    {
        if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
            sysconsole.SetWindowPosition(left, top);
#pragma warning restore CA1416
        else
            Write($"\e[3;{left};{top}t");
    }

    /// <inheritdoc cref="sysconsole.SetWindowSize(int, int)"/>
    public static void SetWindowSize(int width, int height)
    {
        if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
            SetWindowSize(width, height);
#pragma warning restore CA1416
        else
            Write($"\e[8;{height};{width}t");
    }

    /// <inheritdoc cref="sysconsole.SetBufferSize(int, int)"/>
    [SupportedOSPlatform(OS.WIN)]
    public static void SetBufferSize(int width, int height) => sysconsole.SetBufferSize(width, height);

    #endregion
    #region METHODS: STREAMS

    /// <inheritdoc cref="sysconsole.OpenStandardInput()"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static Stream OpenStandardInput() => sysconsole.OpenStandardInput();

    /// <inheritdoc cref="sysconsole.OpenStandardInput(int)"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    public static Stream OpenStandardInput(int bufferSize) => sysconsole.OpenStandardInput(bufferSize);

    /// <inheritdoc cref="sysconsole.OpenStandardOutput()"/>
    public static Stream OpenStandardOutput() => sysconsole.OpenStandardOutput();

    /// <inheritdoc cref="sysconsole.OpenStandardOutput(int)"/>
    public static Stream OpenStandardOutput(int bufferSize) => sysconsole.OpenStandardOutput(bufferSize);

    /// <inheritdoc cref="sysconsole.OpenStandardError()"/>
    public static Stream OpenStandardError() => sysconsole.OpenStandardError();

    /// <inheritdoc cref="sysconsole.OpenStandardError(int)"/>
    public static Stream OpenStandardError(int bufferSize) => sysconsole.OpenStandardError(bufferSize);

    /// <inheritdoc cref="sysconsole.SetError"/>
    public static void SetError(TextWriter writer) => sysconsole.SetError(writer);

    /// <inheritdoc cref="sysconsole.SetIn"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static void SetIn(TextReader writer) => sysconsole.SetIn(writer);

    /// <inheritdoc cref="sysconsole.SetOut"/>
    public static void SetOut(TextWriter writer) => sysconsole.SetOut(writer);

    #endregion
    #region METHODS: READING

    /// <inheritdoc cref="sysconsole.Read"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    public static int Read() => sysconsole.Read();

    /// <inheritdoc cref="sysconsole.ReadKey()"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static ConsoleKeyInfo ReadKey() => sysconsole.ReadKey();

    /// <inheritdoc cref="sysconsole.ReadKey(bool)"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static ConsoleKeyInfo ReadKey(bool intercept) => sysconsole.ReadKey(intercept);

    /// <inheritdoc cref="sysconsole.ReadLine"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    public static string? ReadLine() => sysconsole.ReadLine();

    #endregion
    #region METHODS: WRITING

    /// <inheritdoc cref="sysconsole.Write(ulong)"/>
    public static void Write(ulong value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(bool)"/>
    public static void Write(bool value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(char)"/>
    public static void Write(char value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(char[])"/>
    public static void Write(char[]? buffer) => sysconsole.Write(buffer);

    /// <inheritdoc cref="sysconsole.Write(int)"/>
    public static void Write(int value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(long)"/>
    public static void Write(long value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(decimal)"/>
    public static void Write(decimal value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(object)"/>
    public static void Write(object? value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(float)"/>
    public static void Write(float value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(string)"/>
    public static void Write(string? value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(double)"/>
    public static void Write(double value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(uint)"/>
    public static void Write(uint value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(char[], int, int)"/>
    public static void Write(char[] buffer, int index, int count) => sysconsole.Write(buffer, index, count);


}
