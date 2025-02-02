using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Text;
using System.IO;
using System;

using Unknown6656.Runtime;

namespace Unknown6656.Console;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *                                                                                                               *
 *   This file only forwards the members of the System.Console class to the Unknown6656.Console.Console class.   *
 *   WITH C#14, THIS WILL HOPEFULLY BE REPLACED BY SHAPES/EXTENSIONS                                             *
 *                                                                                                               *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


public static unsafe partial class Console
{
    /// <inheritdoc cref="sysconsole.CancelKeyPress"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static event ConsoleCancelEventHandler? CancelKeyPress
    {
        add => sysconsole.CancelKeyPress += value;
        remove => sysconsole.CancelKeyPress -= value;
    }


    /// <inheritdoc cref="sysconsole.Title"/>
    public static string Title
    {
        get
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                return sysconsole.Title;
#pragma warning restore CA1416
            else
                return GetRawVT520Report("21t", '\x07') ?? throw new InvalidOperationException("Failed to get console title.");
        }
        set => sysconsole.Title = value;
    }

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

    /// <inheritdoc cref="sysconsole.LargestWindowHeight"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static int LargestWindowHeight => sysconsole.LargestWindowHeight;

    /// <inheritdoc cref="sysconsole.LargestWindowWidth"/>
    [UnsupportedOSPlatform(OS.ANDR)]
    [UnsupportedOSPlatform(OS.BROW)]
    [UnsupportedOSPlatform(OS.IOS)]
    [UnsupportedOSPlatform(OS.TVOS)]
    public static int LargestWindowWidth => sysconsole.LargestWindowWidth;

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
        set => SetVT520Bit(25, value);
    }

    /// <inheritdoc cref="sysconsole.CursorLeft"/>
    public static int CursorLeft
    {
        get => OS.IsUnix || OS.IsWindows ? sysconsole.CursorLeft : GetCursorPosition().Left;
        set => sysconsole.Write($"{_CSI}{value + 1}G");
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

    /// <inheritdoc cref="sysconsole.ForegroundColor"/>
    public static ConsoleColor ForegroundColor
    {
        [UnsupportedOSPlatform(OS.ANDR)]
        [UnsupportedOSPlatform(OS.BROW)]
        [UnsupportedOSPlatform(OS.IOS)]
        [UnsupportedOSPlatform(OS.TVOS)]
        get
        {
            if (OS.CurrentOS is KnownOS.Android or KnownOS.Browser or KnownOS.iOS or KnownOS.TvOS)
#warning TODO : implement this, probably by parsing the SGR?
                throw _unsupported_os;
            else
                return sysconsole.ForegroundColor;
        }
        set => Write(value.ToVT520(ColorMode.Foreground)); // That is actually faster than the original implementation in System.Console
    }

    /// <inheritdoc cref="sysconsole.BackgroundColor"/>
    public static ConsoleColor BackgroundColor
    {
        [UnsupportedOSPlatform(OS.ANDR)]
        [UnsupportedOSPlatform(OS.BROW)]
        [UnsupportedOSPlatform(OS.IOS)]
        [UnsupportedOSPlatform(OS.TVOS)]
        get
        {
            if (OS.CurrentOS is KnownOS.Android or KnownOS.Browser or KnownOS.iOS or KnownOS.TvOS)
#warning TODO : implement this, probably by parsing the SGR?
                throw _unsupported_os;
            else
                return sysconsole.BackgroundColor;
        }
        set => Write(value.ToVT520(ColorMode.Background)); // That is actually faster than the original implementation in System.Console
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

    // That is actually over 3x faster than the original implementation in System.Console AND works on all platforms!
    /// <inheritdoc cref="sysconsole.SetCursorPosition"/>
    public static void SetCursorPosition(int left, int top) => sysconsole.Write($"{_CSI}{top + 1};{left + 1}H");

    /// <inheritdoc cref="sysconsole.SetWindowPosition"/>
    public static void SetWindowPosition(int left, int top)
    {
        if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
            sysconsole.SetWindowPosition(left, top);
#pragma warning restore CA1416
        else
            sysconsole.Write($"{_CSI}3;{left};{top}t");
    }

    /// <inheritdoc cref="sysconsole.SetWindowSize(int, int)"/>
    public static void SetWindowSize(int width, int height) => sysconsole.Write($"{_CSI}8;{height};{width}t");

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

    /// <inheritdoc cref="sysconsole.Write(bool)"/>
    public static void Write(bool value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(char)"/>
    public static void Write(char value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(int)"/>
    public static void Write(int value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(uint)"/>
    public static void Write(uint value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(long)"/>
    public static void Write(long value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(ulong)"/>
    public static void Write(ulong value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(float)"/>
    public static void Write(float value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(double)"/>
    public static void Write(double value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(decimal)"/>
    public static void Write(decimal value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(string)"/>
    public static void Write(string? value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(object)"/>
    public static void Write(object? value) => sysconsole.Write(value);

    /// <inheritdoc cref="sysconsole.Write(char[])"/>
    public static void Write(char[]? buffer) => sysconsole.Write(buffer);

    /// <inheritdoc cref="sysconsole.Write(char[], int, int)"/>
    public static void Write(char[] buffer, int index, int count) => sysconsole.Write(buffer, index, count);

    /// <inheritdoc cref="sysconsole.Write(string, object)"/>
    public static void Write([StringSyntax("CompositeFormat")] string format, object? arg0) => sysconsole.Write(format, arg0);

    /// <inheritdoc cref="sysconsole.Write(string, object, object)"/>
    public static void Write([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1) => sysconsole.Write(format, arg0, arg1);

    /// <inheritdoc cref="sysconsole.Write(string, object, object, object)"/>
    public static void Write([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2) =>
        sysconsole.Write(format, arg0, arg1, arg2);

    /// <inheritdoc cref="sysconsole.Write(string, object[])"/>
    public static void Write([StringSyntax("CompositeFormat")] string format, params object?[]? arg) => sysconsole.Write(format, arg);

    /// <inheritdoc cref="sysconsole.Write(string, ReadOnlySpan{object})"/>
    public static void Write([StringSyntax("CompositeFormat")] string format, params ReadOnlySpan<object?> arg) => sysconsole.Write(format, arg);

    /// <inheritdoc cref="sysconsole.WriteLine()"/>
    public static void WriteLine() => sysconsole.WriteLine();

    /// <inheritdoc cref="sysconsole.WriteLine(bool)"/>
    public static void WriteLine(bool value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(char)"/>
    public static void WriteLine(char value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(long)"/>
    public static void WriteLine(long value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(ulong)"/>
    public static void WriteLine(ulong value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(int)"/>
    public static void WriteLine(int value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(uint)"/>
    public static void WriteLine(uint value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(float)"/>
    public static void WriteLine(float value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(double)"/>
    public static void WriteLine(double value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(decimal)"/>
    public static void WriteLine(decimal value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(string)"/>
    public static void WriteLine(string? value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(char[])"/>
    public static void WriteLine(char[]? buffer) => sysconsole.WriteLine(buffer);

    /// <inheritdoc cref="sysconsole.WriteLine(char[], int, int)"/>
    public static void WriteLine(char[] buffer, int index, int count) => sysconsole.WriteLine(buffer, index, count);

    /// <inheritdoc cref="sysconsole.WriteLine(object)"/>
    public static void WriteLine(object? value) => sysconsole.WriteLine(value);

    /// <inheritdoc cref="sysconsole.WriteLine(string, object)"/>
    public static void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0) => sysconsole.WriteLine(format, arg0);

    /// <inheritdoc cref="sysconsole.WriteLine(string, object, object)"/>
    public static void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1) => sysconsole.WriteLine(format, arg0, arg1);

    /// <inheritdoc cref="sysconsole.WriteLine(string, object, object, object)"/>
    public static void WriteLine([StringSyntax("CompositeFormat")] string format, object? arg0, object? arg1, object? arg2) =>
        sysconsole.WriteLine(format, arg0, arg1, arg2);

    /// <inheritdoc cref="sysconsole.WriteLine(string, object[])"/>
    public static void WriteLine([StringSyntax("CompositeFormat")] string format, params object?[]? arg) => sysconsole.WriteLine(format, arg);

    /// <inheritdoc cref="sysconsole.WriteLine(string, ReadOnlySpan{object})"/>
    public static void WriteLine([StringSyntax("CompositeFormat")] string format, params ReadOnlySpan<object?> arg) => sysconsole.WriteLine(format, arg);

    #endregion

    /// <inheritdoc cref="sysconsole.ResetColor"/>
    public static void ResetColor()
    {
        ResetForegroundColor();
        ResetBackgroundColor();
    }

    /// <inheritdoc cref="sysconsole.Clear"/>
    public static void Clear()
    {
        if (OS.CurrentOS is KnownOS.Android or KnownOS.iOS or KnownOS.TvOS)
            Write($"{_CSI}H{_CSI}2J");
        else
            sysconsole.Clear();
    }

    /// <inheritdoc cref="sysconsole.Beep()"/>
    public static void Beep()
    {
        if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
            Beep(800, 200);
#pragma warning restore CA1416
        else
            Beep(ConsoleTone.GSharp5, 200);
    }

    /// <inheritdoc cref="sysconsole.Beep(int, int)"/>
    [SupportedOSPlatform(OS.WIN)]
#warning TODO : implement this on non-Windows systems
    public static void Beep(int frequency, int duration) => sysconsole.Beep(frequency, duration);

    /// <summary>
    /// Plays a beep sound with the specified tone, duration, and volume.
    /// </summary>
    /// <param name="tone">The tone of the beep sound.</param>
    /// <param name="duration">The duration of the beep sound in milliseconds.</param>
    /// <param name="volume">The volume of the beep sound (0 to 1).</param>
    public static void Beep(ConsoleTone tone, int duration, double volume = 1)
    {
        volume = Math.Clamp(volume, 0, 1);

        if (OS.IsWindows && volume == 1)
#pragma warning disable CA1416 // Validate platform compatibility
            Beep(tone switch
            {
                ConsoleTone.C5 => 523,
                ConsoleTone.CSharp5 => 554,
                ConsoleTone.D5 => 587,
                ConsoleTone.DSharp5 => 622,
                ConsoleTone.E5 => 659,
                ConsoleTone.F5 => 698,
                ConsoleTone.FSharp5 => 740,
                ConsoleTone.G5 => 784,
                ConsoleTone.GSharp5 => 810,
                ConsoleTone.A5 => 880,
                ConsoleTone.ASharp5 => 932,
                ConsoleTone.B5 => 988,
                ConsoleTone.C6 => 1047,
                ConsoleTone.CSharp6 => 1109,
                ConsoleTone.D6 => 1175,
                ConsoleTone.DSharp6 => 1245,
                ConsoleTone.E6 => 1319,
                ConsoleTone.F6 => 1397,
                ConsoleTone.FSharp6 => 1480,
                ConsoleTone.G6 => 1568,
                ConsoleTone.GSharp6 => 1661,
                ConsoleTone.A6 => 1760,
                ConsoleTone.ASharp6 => 1865,
                ConsoleTone.B6 => 1976,
                ConsoleTone.C7 => 2093,
                _ => 0,
            }, duration);
#pragma warning restore CA1416
        else
            Write($"{_CSI}{(int)Math.Round(volume * 7)};{(int)tone};{(int)Math.Round(duration * .032)}\a");
    }


    /// <inheritdoc cref="sysconsole.MoveBufferArea(int, int, int, int, int, int)"/>
    [Obsolete($"The usage of this function is not recommended. Use {nameof(DuplicateBufferArea)} instead.")]
    public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop) =>
        MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, ' ', ConsoleColor.Black, BackgroundColor);

    /// <inheritdoc cref="sysconsole.MoveBufferArea(int, int, int, int, int, int, char, sysconsolecolor, sysconsolecolor)"/>
    [Obsolete($"The usage of this function is not recommended. Use {nameof(DuplicateBufferArea)} instead.")]
    public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
    {
        if (OS.IsWindows &&
            ForegroundColor.ToSystemColor() is sysconsolecolor fg &&
            BackgroundColor.ToSystemColor() is sysconsolecolor bg)
#pragma warning disable CA1416 // Validate platform compatibility
            sysconsole.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, fg, bg);
#pragma warning restore CA1416
        else
            DuplicateBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
    }
}
