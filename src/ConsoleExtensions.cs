using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Linq;
using System;

using Unknown6656.Generics;
using Unknown6656.Runtime;
using Unknown6656.Common;

namespace Unknown6656.Console;





// WITH C#13, THIS WILL BE REPLACED BY SHAPES/EXTENSIONS
public static unsafe partial class ConsoleExtensions
{
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
    /// <exception cref="InvalidOperationException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static void* STDINHandle => OS.IsWindows ? NativeInterop.GetStdHandle(-10)
                                                    : throw new InvalidOperationException("This operation is not supported on non-Windows operating systems.");

    /// <summary>
    /// Returns the handle of the standard output stream.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static void* STDOUTHandle => OS.IsWindows ? NativeInterop.GetStdHandle(-11)
                                                     : throw new InvalidOperationException("This operation is not supported on non-Windows operating systems.");

    /// <summary>
    /// Returns the handle of the standard error stream.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static void* STDERRHandle => OS.IsWindows ? NativeInterop.GetStdHandle(-12)
                                                     : throw new InvalidOperationException("This operation is not supported on non-Windows operating systems.");

    /// <summary>
    /// Gets or sets the <see cref="ConsoleMode"/> of the standard input stream.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid console mode is encountered and <see cref="ThrowOnInvalidConsoleMode"/> is <see langword="true"/>.</exception>"
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleMode STDINConsoleMode
    {
        set
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Writing the STDIN console mode is not supported on non-Windows operating systems.");
            else if (!NativeInterop.SetConsoleMode(STDINHandle, value))
                if (ThrowOnInvalidConsoleMode)
                    throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Reading the STDIN console mode is not supported on non-Windows operating systems.");

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
    /// <exception cref="InvalidOperationException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid console mode is encountered and <see cref="ThrowOnInvalidConsoleMode"/> is <see langword="true"/>.</exception>"
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleMode STDOUTConsoleMode
    {
        set
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Writing the STDOUT console mode is not supported on non-Windows operating systems.");
            else if (!NativeInterop.SetConsoleMode(STDOUTHandle, value))
                if (ThrowOnInvalidConsoleMode)
                    throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Reading the STDOUT console mode is not supported on non-Windows operating systems.");

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
    /// <exception cref="InvalidOperationException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid console mode is encountered and <see cref="ThrowOnInvalidConsoleMode"/> is <see langword="true"/>.</exception>"
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleMode STDERRConsoleMode
    {
        set
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Writing the STDERR console mode is not supported on non-Windows operating systems.");
            else if (!NativeInterop.SetConsoleMode(STDERRHandle, value))
                if (ThrowOnInvalidConsoleMode)
                    throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Reading the STDERR console mode is not supported on non-Windows operating systems.");

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
    /// <exception cref="InvalidOperationException">Thrown, if this member is accessed on any non-Windows operating system.</exception>
    /// <exception cref="Win32Exception">Thrown, if an invalid <see cref="ConsoleFontInfo"/> is encountered or if any errors occurred whilst reading/writing to this property.</exception>
    [SupportedOSPlatform(OS.WIN)]
    public static ConsoleFontInfo FontInfo
    {
        set
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Changing the console font is not supported on non-Windows operating systems.");
            else if (!NativeInterop.SetCurrentConsoleFontEx(STDOUTHandle, false, ref value))
                throw NETRuntimeInterop.GetLastWin32Error();
        }
        get
        {
            if (!OS.IsWindows)
                throw new InvalidOperationException("Reading the console font is not supported on non-Windows operating systems.");

            ConsoleFontInfo font = new()
            {
                cbSize = Marshal.SizeOf<ConsoleFontInfo>()
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
    /// Sets or gets the console's current <see cref="ConsoleState"/>. This can be used to restore the console to an earlier state.
    /// </summary>
    public static ConsoleState CurrentConsoleState
    {
        get => SaveConsoleState();
        set => RestoreConsoleState(value);
    }

    /// <summary>
    /// Sets or gets the console's current graphic rendition.
    /// </summary>
    public static ConsoleGraphicRendition? CurrentGraphicRendition
    {
        get => GetRawVT100GraphicRenditions() is { } sgr ? new(sgr) : null;
        set
        {
            if (value?.FullVT100SGR() is { } sgr)
                sysconsole.Write($"\e[{string.Join(';', sgr)}m");
        }
    }

    /// <summary>
    /// Gets or sets the current cursor position.
    /// </summary>
    public static Point CursorPosition
    {
        get => new(sysconsole.CursorLeft, sysconsole.CursorTop);
        set => sysconsole.SetCursorPosition(value.X, value.Y);
    }

    ///   
    public static Point WindowPosition
    {
        get => new(sysconsole.WindowLeft, sysconsole.WindowTop);
        set
        {
            if (OS.IsWindows)
                sysconsole.SetWindowPosition(value.X, value.Y);
            else
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Gets or sets the size of the console window.
    /// This functionality is only partially supported by the Windows Terminal App (<c>wt.exe</c>).
    /// </summary>
    public static Size WindowSize
    {
        get => new(sysconsole.WindowWidth, sysconsole.WindowHeight);
        set
        {
            if (OS.IsWindows)
#pragma warning disable CA1416 // Validate platform compatibility
                sysconsole.SetWindowSize(value.Width, value.Height);
#pragma warning restore CA1416
            else
                sysconsole.Write($"\e[8;{value.Height};{value.Width}t");
        }
    }

    // TODO : add support for XTWINOPS
    // TODO : add support for Sixel drawing https://en.wikipedia.org/wiki/Sixel


    public static ConsoleCursorShape CursorShape
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write($"\e[{(int)value} q");
    }

    public static bool CursorVisible
    {
        get
        {
            if (OS.IsWindows)
                return sysconsole.CursorVisible;
            else
                throw new NotImplementedException(); // TODO
        }
        set
        {
            if (OS.IsWindows)
                sysconsole.CursorVisible = value;
            else
                sysconsole.Write(value ? "\e[?25h" : "\e[?25l");
        }
    }

    public static bool AlternateScreenEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?1049h" : "\e[?1049l");
    }

    public static bool BracketedPasteModeEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?2004h" : "\e[?2004l");
    }

    public static bool WindowAutoResizeModeEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?98h" : "\e[?98l");
    }


#warning TODO : verify if this is correct

    public static bool MouseEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?1000h" : "\e[?1000l");
    }

    public static bool MouseDraggingEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?1002h" : "\e[?1002l");
    }

    public static bool MouseAnyEventEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?1003h" : "\e[?1003l");
    }

    public static bool MouseFocusReportingEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?1004h" : "\e[?1004l");
    }

    public static bool MouseFocusEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?1005h" : "\e[?1005l");
    }

    public static bool MouseHighlightingEnabled
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?1006h" : "\e[?1006l");
    }



    public static bool DarkMode
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?5l" : "\e[?5h");
    }

    public static bool RightToLeft
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write(value ? "\e[?34h" : "\e[?34l");
    }

    public static (ConsoleColor Foreground, ConsoleColor Background) WindowFrameColors
    {
        get => throw new NotImplementedException();
        set => sysconsole.Write($"\e[2;{(int)value.Foreground};{(int)value.Background},|");
    }

    public static TextIntensityMode TextIntensity
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).Intensity;
        set => sysconsole.Write(value switch {
            TextIntensityMode.Bold => "\e[1m",
            TextIntensityMode.Dim => "\e[2m",
            _ => "\e[22m"
        });
    }

    public static TextBlinkMode TextBlink
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).Blink;
        set => sysconsole.Write(value switch
        {
            TextBlinkMode.Slow => "\e[5m",
            TextBlinkMode.Rapid => "\e[6m",
            _ => "\e[25m"
        });
    }

    public static bool InvertedColors
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).AreColorsInverted;
        set => sysconsole.Write(value ? "\e[7m" : "\e[27m");
    }

    public static TextUnderlinedMode TextUnderline
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).Underlined;
        set => sysconsole.Write($"\e[{(int)value}m");
    }

    public static bool CrossedOutText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsCrossedOut;
        set => sysconsole.Write(value ? "\e[9m" : "\e[29m");
    }

    public static bool OverlinedText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsOverlined;
        set => sysconsole.Write(value ? "\e[53m" : "\e[55m");
    }

    public static TextFrameMode TextFrame
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).TextFrame;
        set => sysconsole.Write($"\e[{(int)value}m");
    }

    public static bool ConcealedText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsTextConcealed;
        set => sysconsole.Write(value ? "\e[8m" : "\e[28m");
    }

    public static bool ItalicText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsItalic;
        set => sysconsole.Write(value ? "\e[3m" : "\e[23m");
    }

    public static bool GothicText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsGothic;
        set => sysconsole.Write(value ? "\e[20m" : "\e[23m");
    }

    public static TextTransformationMode TextTransformation
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).TextTransformation;
        set => sysconsole.Write($"\e[{(int)value}m");
    }



    // TODO : ^[[5n     ???
    // TODO : ^[[6n     cursor position report
    // TODO : -> ^[[c
    //        <- ^[[?61;6;7;21;22;23;24;28;32;42c



    /// <summary>
    /// Indicates whether the current console supports ANSI/VT100 escape sequences.
    /// </summary>
    public static bool SupportsVT100EscapeSequences => !OS.IsWindows || Environment.OSVersion.Version is { Major: >= 10, Build: >= 16257 };
#pragma warning disable CA1416 // Validate platform compatibility

    /// <summary>
    /// Indicates whether ANSI/VT100 escape sequences are enabled on the standard input stream.
    /// </summary>
    public static bool AreSTDInVT100EscapeSequencesEnabled => !OS.IsWindows || STDINConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);

    /// <summary>
    /// Indicates whether ANSI/VT100 escape sequences are enabled on the standard output stream.
    /// </summary>
    public static bool AreSTDOutVT100EscapeSequencesEnabled => !OS.IsWindows || STDOUTConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);

    /// <summary>
    /// Indicates whether ANSI/VT100 escape sequences are enabled on the standard error stream.
    /// </summary>
    public static bool AreSTDErrVT100EscapeSequencesEnabled => !OS.IsWindows || STDERRConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
#pragma warning restore CA1416


    static ConsoleExtensions()
    {
        if (OS.IsWindows)
        {
            //LINQ.TryDo(() => STDINConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_INPUT);
            LINQ.TryDo(() => STDINConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            LINQ.TryDo(() => STDOUTConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            LINQ.TryDo(() => STDERRConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
    }

    public static string? GetRawVT100Report(string report_sequence, char terminator)
    {
        sysconsole.Write($"\e{report_sequence}");

        try
        {
            string response = "";

            while (sysconsole.KeyAvailable && sysconsole.ReadKey(true).KeyChar is char c && c != terminator)
                response += c;

            return response;
        }
        catch
        {
        }

        return null;
    }

    public static string? GetRawVT100SettingsReport(string report_sequence, char? response_introducer = 'r')
    {
        if (GetRawVT100Report($"\eP$q{report_sequence}\e\\", '\\') is ['\e', 'P', _, '$', char ri, ..string response, '\e', '\\'] &&
            (response_introducer is null || ri == response_introducer))
            return response.TrimEnd(report_sequence);

        return null;
    }

    public static (int Mode, int[] Attributes)? GetDeviceAttributes()
    {
        if (GetRawVT100Report("[c", 'c') is string response)
            try
            {
                response = response.TrimStart("\e[?");

                if (response.Split(';').ToArray(int.Parse) is [int mode, .. [] attributes])
                    return (mode, attributes);
            }
            catch
            {
            }

        return null;
    }

    public static void Beep(ConsoleTone tone, int duration, double volume = 1)
    {
        volume = Math.Clamp(volume, 0, 1);

        if (OS.IsWindows && volume == 1)
#pragma warning disable CA1416 // Validate platform compatibility
            sysconsole.Beep(tone switch
            {
                ConsoleTone.C5 => 523,
                ConsoleTone.CSharp5 => 554,
                ConsoleTone.D5 => 587,
                ConsoleTone.DSharp5 => 622,
                ConsoleTone.E5 => 659,
                ConsoleTone.F5 => 698,
                ConsoleTone.FSharp5 => 740,
                ConsoleTone.G5 => 784,
                ConsoleTone.GSharp5 => 81,
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
            sysconsole.Write($"\e[{(int)Math.Round(volume * 7)};{(int)tone};{(int)Math.Round(duration * .032)}\a");
    }

    public static void SoftReset() => sysconsole.Write("\e[!p");

    public static void ClearAndResetAll() => sysconsole.Write("\e[3J\ec\e[m");

    public static void ResetAllAttributes() => sysconsole.Write("\e[m");

    public static void ResetForegroundColor() => sysconsole.Write("\e[39m");

    public static void ResetBackgroundColor() => sysconsole.Write("\e[49m");

    public static void ScrollUp(int lines)
    {
        if (lines < 0)
            ScrollDown(-lines);
        else if (lines > 0)
            sysconsole.Write($"\e[{lines}S");
    }

    public static void ScrollDown(int lines)
    {
        if (lines < 0)
            ScrollUp(-lines);
        else if (lines > 0)
            sysconsole.Write($"\e[{lines}T");
    }

    public static void ClearArea(ConsoleArea area, bool selective = false) =>
        sysconsole.Write($"\e[{area.Top};{area.Left};{area.Bottom};{area.Right}${(selective ? 'z' : '{')}");

    public static void FillArea(ConsoleArea area, char @char) =>
        sysconsole.Write($"\e[{(int)@char};{area.Top};{area.Left};{area.Bottom};{area.Right}$z");

    public static void DuplicateArea(ConsoleArea source, (int X, int Y) destination) => DuplicateArea(source, 0, destination);

    public static void DuplicateArea(ConsoleArea source, int source_page, (int X, int Y) destination) =>
        DuplicateArea(source, source_page, (destination.X, destination.Y, source_page));

    public static void DuplicateArea(ConsoleArea source, int source_page, (int X, int Y, int Page) destination) =>
        sysconsole.Write($"\e[{source.Top};{source.Left};{source.Bottom};{source.Right};{source_page};{destination.X};{destination.Y};{destination.Page}$v");

    public static void ChangeVT100ForArea(ConsoleArea area, IEnumerable<int> modes) => ChangeVT100ForArea(area, modes.StringJoin(";"));

    public static void ChangeVT100ForArea(ConsoleArea area, string modes) =>
        sysconsole.Write($"\e[{area.Top};{area.Left};{area.Bottom};{area.Right};{modes.Trim(';')}$r");

    public static void GetCursorInformation()
    {
        if (GetRawVT100Report("[1$w", '\\') is ['\e', 'P', _, '$', 'u', .. string response, '\e', '\\'])
        {
            // TODO
        }
    }

    public static void GetTabStopInformation()
    {
        if (GetRawVT100Report("[2$w", '\\') is ['\e', 'P', _, '$', 'u', .. string response, '\e', '\\'])
        {
            // TODO
        }
    }

    // TODO : page 151 of https://web.mit.edu/dosathena/doc/www/ek-vt520-rm.pdf

    public static string[]? GetRawVT100GraphicRenditions() => GetRawVT100SettingsReport("m")?.Split(';');

    public static void GetCursorType()
    {
        if (GetRawVT100SettingsReport(" q") is string response)
        {
            // TODO
        }
    }

    public static void GetMargins()
    {
        if (GetRawVT100SettingsReport("s") is string left_right &&
            GetRawVT100SettingsReport("t") is string top_bottom)
        {
            // TODO
        }
    }

    public static void GetColor()
    {
        if (GetRawVT100SettingsReport(",|") is string response)
        {
            // TODO
        }
    }






    public static void WriteReverseIndex() => sysconsole.Write("\eM");

    public static void Write(object? value, int left, int top) => Write(value, (left, top));

    public static void Write(object? value, (int left, int top) starting_pos)
    {
        sysconsole.SetCursorPosition(starting_pos.left, starting_pos.top);
        sysconsole.Write(value);
    }

    public static void InsertLine(int count = 1) => sysconsole.Write($"\e[{count}L");

    public static void DeleteLine(int count = 1) => sysconsole.Write($"\e[{count}M");

    public static void InsertSpaceCharacter(int count = 1) => sysconsole.Write($"\e[{count}@");

    public static void DeleteCharacter(int count = 1) => sysconsole.Write($"\e[{count}P");

    public static void ChangeLineRendering(int line, LineRenderingMode mode)
    {
        if (mode is LineRenderingMode.DoubleHeight)
        {
            ChangeLineRendering(line, LineRenderingMode.DoubleHeight_Top);
            ChangeLineRendering(line + 1, LineRenderingMode.DoubleHeight_Bottom);
        }
        else
        {
            sysconsole.CursorTop = line;
            sysconsole.Write($"\e#{mode switch
            {
                LineRenderingMode.DoubleWidth => 6,
                LineRenderingMode.DoubleHeight_Top => 3,
                LineRenderingMode.DoubleHeight_Bottom => 4,
                _ => 5
            }}");
        }
    }

    public static void WriteDoubleWidthLine(object? value) => WriteDoubleWidthLine(value, (sysconsole.CursorLeft, sysconsole.CursorTop));

    public static void WriteDoubleWidthLine(object? value, int left, int top) => WriteDoubleWidthLine(value, (left, top));

    public static void WriteDoubleWidthLine(object? value, (int left, int top)? starting_pos)
    {
        if (starting_pos is (int x, int y))
            sysconsole.SetCursorPosition(x, y);

        sysconsole.WriteLine($"\e#5{value}");
    }

    public static void WriteDoubleSizeLine(object? value) => WriteDoubleSizeLine(value, (sysconsole.CursorLeft, sysconsole.CursorTop));

    public static void WriteDoubleSizeLine(object? value, int left, int top) => WriteDoubleSizeLine(value, (left, top));

    public static void WriteDoubleSizeLine(object? value, (int left, int top)? starting_pos)
    {
        int x = starting_pos?.left ?? sysconsole.CursorLeft;
        int y = starting_pos?.top ?? sysconsole.CursorTop;
        string text = value?.ToString() ?? "";

        sysconsole.SetCursorPosition(x, y);
        sysconsole.Write($"\e#3{text}");
        sysconsole.SetCursorPosition(x, y + 1);
        sysconsole.WriteLine($"\e#4{text}");
    }

    public static void FullClear()
    {
        sysconsole.Clear();
        sysconsole.Write("\e[3J");
    }

    public static (int max_line_length, int line_count) WriteBlock(string value, int left, int top) =>
        WriteBlock(value, (left, top));

    public static (int max_line_length, int line_count) WriteBlock(string value, (int left, int top) starting_pos) =>
        WriteBlock(value.SplitIntoLines(), starting_pos);

    public static (int max_line_length, int line_count) WriteBlock(IEnumerable<string> lines, int left, int top) => WriteBlock(lines, (left, top));

    public static (int max_line_length, int line_count) WriteBlock(IEnumerable<string> lines, (int left, int top) starting_pos) =>
        WriteBlock(lines, starting_pos, (0x0fffffff, 0x0fffffff), true);

    public static (int max_line_length, int line_count) WriteBlock(string value, int left, int top, int max_width, int max_height, bool wrap_overflow = true) =>
        WriteBlock(value, (left, top), (max_width, max_height), wrap_overflow);

    public static (int max_line_length, int line_count) WriteBlock(string value, (int left, int top) starting_pos, (int width, int height) max_size, bool wrap_overflow = true) =>
        WriteBlock(value.SplitIntoLines(), starting_pos, max_size, wrap_overflow);

    public static (int max_line_length, int line_count) WriteBlock(IEnumerable<string> lines, int left, int top, int max_width, int max_height, bool wrap_overflow = true) =>
        WriteBlock(lines, (left, top), (max_width, max_height), wrap_overflow);

    public static (int max_line_length, int line_count) WriteBlock(IEnumerable<string> lines, (int left, int top) starting_pos, (int width, int height) max_size, bool wrap_overflow = true)
    {
        List<string> cropped_lines = SplitLinesWithVT100(lines.ToList(), max_size.width, wrap_overflow);
        int line_no = 0;
        int max_width = 0;

        foreach (string line in cropped_lines.Take(max_size.height))
        {
            sysconsole.SetCursorPosition(starting_pos.left, starting_pos.top + line_no);
            sysconsole.Write(line);

            ++line_no;
            max_width = Math.Max(max_width, sysconsole.CursorLeft - starting_pos.left);
        }

        return (max_width, line_no);
    }

    public static void WriteVertical(object? value) => WriteVertical(value, sysconsole.CursorLeft, sysconsole.CursorTop);

    public static void WriteVertical(object? value, int left, int top) => WriteVertical(value, (left, top));

    public static void WriteVertical(object? value, (int left, int top) starting_pos)
    {
        string s = value?.ToString() ?? "";

        for (int i = 0; i < s.Length; i++)
        {
            sysconsole.CursorTop = starting_pos.top + i;
            sysconsole.CursorLeft = starting_pos.left;
            sysconsole.Write(s[i]);
        }
    }

    public static void WriteUnderlined(object? value) => sysconsole.Write($"\e[4m{value}\e[24m");

    public static void WriteInverted(object? value) => sysconsole.Write($"\e[7m{value}\e[27m");

    [SupportedOSPlatform(OS.WIN)]
    public static (ConsoleFontInfo before, ConsoleFontInfo after) SetCurrentFont(Font font)
    {
        ConsoleFontInfo before = FontInfo;
        ConsoleFontInfo set = new()
        {
            cbSize = Marshal.SizeOf<ConsoleFontInfo>(),
            FontIndex = 0,
            FontFamily = ConsoleFontInfo.FIXED_WIDTH_TRUETYPE,
            FontName = font.Name,
            FontWeight = font.Bold ? 700 : 400,
            FontSize = font.Size > 0 ? (default, (short)font.Size) : before.FontSize,
        };

        FontInfo = set;

        return (before, FontInfo);
    }

    // hexdump now moved to Unknown6656.Serialization

    public static ConsoleState SaveConsoleState()
    {
        ConsoleMode stdinmode = default;
        ConsoleMode stderrmode = default;
        ConsoleMode stdoutmode = default;
        int? cursor_size = null;

        if (OS.IsWindows)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            cursor_size = sysconsole.CursorSize;
            stdinmode = STDINConsoleMode;
            stderrmode = STDERRConsoleMode;
            stdoutmode = STDOUTConsoleMode;
#pragma warning restore CA1416
        }

        return new()
        {
            InputEncoding = sysconsole.InputEncoding,
            OutputEncoding = sysconsole.OutputEncoding,
            CursorSize = cursor_size,
            STDINMode = stdinmode,
            STDOUTMode = stdoutmode,
            STDERRMode = stderrmode,
            BufferSize = BufferSize,
            WindowSize = WindowSize,
            WindowPosition = WindowPosition,
            CursorPosition = CursorPosition,
            GraphicRendition = CurrentGraphicRendition,
        };
    }

    public static void RestoreConsoleState(ConsoleState? state)
    { 
        if (state is { })
        {
            sysconsole.InputEncoding = state.InputEncoding ?? Encoding.Default;
            sysconsole.OutputEncoding = state.OutputEncoding ?? Encoding.Default;

            if (OS.IsWindows)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                STDINConsoleMode = state.STDINMode;
                STDOUTConsoleMode = state.STDOUTMode;
                STDERRConsoleMode = state.STDERRMode;

                if (state.CursorSize is int sz)
                    LINQ.TryDo(() => sysconsole.CursorSize = sz);
#pragma warning restore CA1416
            }

            BufferSize = state.BufferSize;
            WindowSize = state.WindowSize;
            WindowPosition = state.WindowPosition;
            CursorPosition = state.CursorPosition;
            CurrentGraphicRendition = state.GraphicRendition;
        }
    }

    public static List<string> SplitLinesWithVT100(List<string> lines, int max_width, bool wrap_overflow = true)
    {
        return [..from line in lines
                  from processed in process(line, max_width)
                  select processed];

        List<string> process(string line, int max_width)
        {
            StringBuilder curr = new();
            List<string> result = [];
            int len = 0;

            for (int i = 0; i < line.Length; ++i)
            {
                char c = line[i];

                if (c is '\e' or '\x9b' && GenerateVT100Regex().Match(line, i) is { Success: true } match)
                {
                    curr.Append(line.AsSpan(i, match.Length));
                    i += match.Length - 1;
                }
                else
                {
                    curr.Append(c);
                    len += c is '\x7f'
                             or '\x81'
                             or '\x8d'
                             or '\x8f'
                             or '\x90'
                             or '\x9d'
                             or (>= '\x00' and <= '\x08')
                             or (>= '\x0b' and <= '\x0c')
                             or (>= '\x0e' and <= '\x1f') ? 0 : 1;

                    if (len >= max_width)
                    {
                        result.Add(curr.ToString());

                        if (!wrap_overflow)
                            return result;

                        curr.Clear();
                        len = 0;
                    }
                }
            }

            if (curr.Length > 0)
                result.Add(curr.ToString());

            return result;
        }
    }

    public static string StripVT100Sequences(this string raw_string) => GenerateVT100Regex().Replace(raw_string, "");

    public static MatchCollection MatchVT100Sequences(this string raw_string) => GenerateVT100Regex().Matches(raw_string);

    public static int CountVT100Sequences(this string raw_string) => GenerateVT100Regex().Count(raw_string);

    public static bool ContainsVT100Sequences(this string raw_string) => GenerateVT100Regex().IsMatch(raw_string);

    public static int LengthWithoutVT100Sequences(this string raw_string) => raw_string.Length - MatchVT100Sequences(raw_string).Sum(m => m.Length);


    // TODO : optimize this regex expression to be more efficient
    [GeneratedRegex(@"(\x1b\[|\x9b)([0-\?]*[\x20-\/]*[@-~]|[^@-_]*[@-_]|[\da-z]{1,2};\d{1,2}H)|\x1b([@-_0-\?\x60-~]|[\x20-\/]|[\x20-\/]{2,}[@-~]|[\x30-\x3f]|[\x20-\x2f]+[\x30-\x7e]|\[[\x30-\x3f]*[\x20-\x2f]*[\x40-\x7e])", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex GenerateVT100Regex();
}


// TODO : https://web.mit.edu/dosathena/doc/www/ek-vt520-rm.pdf

