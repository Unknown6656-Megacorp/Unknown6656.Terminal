using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Text;
using System.Linq;
using System;

using Unknown6656.Generics;
using Unknown6656.Runtime;
using Unknown6656.Common;

namespace Unknown6656.Console;


// TODO : add support for XTWINOPS
// TODO : add support for Sixel drawing https://en.wikipedia.org/wiki/Sixel



// WITH C#13, THIS WILL BE REPLACED BY SHAPES/EXTENSIONS
public static unsafe partial class Console
{
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
        get => GetRawVT520GraphicRenditions() is { } sgr ? new(sgr) : null;
        set
        {
            if (value?.FullVT520SGR() is { } sgr)
                Write($"\e[{string.Join(';', sgr)}m");
        }
    }

    /// <summary>
    /// Gets or sets the current cursor position.
    /// </summary>
    public static Point CursorPosition
    {
        get => new(CursorLeft, CursorTop);
        set => SetCursorPosition(value.X, value.Y);
    }

    /// <summary>
    /// Gets or sets the position of the console window.
    /// </summary>
    public static Point WindowPosition
    {
        get => new(WindowLeft, WindowTop);
        set => SetWindowPosition(value.X, value.Y);
    }

    /// <summary>
    /// Gets or sets the size of the console window.
    /// This functionality is only partially supported by the Windows Terminal App (<c>wt.exe</c>).
    /// </summary>
    public static Size WindowSize
    {
        get => new(WindowWidth, WindowHeight);
        set => SetWindowSize(value.Width, value.Height);
    }

    /// <summary>
    /// Gets or sets the size of the console buffer.
    /// Changing the value of this member is currently not supported on non-Windows operating systems.
    /// </summary>
    public static Size BufferSize
    {
        get => new(BufferWidth, BufferHeight);
#pragma warning disable CA1416 // Validate platform compatibility
        set => SetBufferSize(value.Width, value.Height);
#pragma warning restore CA1416
    }



    public static ConsoleCursorShape CursorShape
    {
        set => Write($"\e[{(int)value} q");
    }

    public static bool AlternateScreenEnabled
    {
        set => SetVT520Bit(1049, value);
    }

    public static bool BracketedPasteModeEnabled
    {
        set => SetVT520Bit(2004, value);
    }

    public static bool WindowAutoResizeModeEnabled
    {
        set => SetVT520Bit(98, value);
    }


#warning TODO : verify if this is correct

    public static bool MouseEnabled
    {
        set => SetVT520Bit(1000, value);
    }

    public static bool MouseDraggingEnabled
    {
        set => SetVT520Bit(1002, value);
    }

    public static bool MouseAnyEventEnabled
    {
        set => SetVT520Bit(1003, value);
    }

    public static bool MouseFocusReportingEnabled
    {
        set => SetVT520Bit(1004, value);
    }

    public static bool MouseFocusEnabled
    {
        set => SetVT520Bit(1005, value);
    }

    public static bool MouseHighlightingEnabled
    {
        set => SetVT520Bit(1006, value);
    }



    public static bool DarkMode
    {
        set => SetVT520Bit(5, !value);
    }

    public static bool RightToLeft
    {
        set => SetVT520Bit(34, value);
    }

    public static (ConsoleColor Foreground, ConsoleColor Background) WindowFrameColors
    {
        get => throw new NotImplementedException();
        set => Write($"\e[2;{(int)value.Foreground};{(int)value.Background},|");
    }

    public static TextIntensityMode TextIntensity
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).Intensity;
        set => Write(value switch {
            TextIntensityMode.Bold => "\e[1m",
            TextIntensityMode.Dim => "\e[2m",
            _ => "\e[22m"
        });
    }

    public static TextBlinkMode TextBlink
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).Blink;
        set => Write(value switch
        {
            TextBlinkMode.Slow => "\e[5m",
            TextBlinkMode.Rapid => "\e[6m",
            _ => "\e[25m"
        });
    }

    public static bool InvertedColors
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).AreColorsInverted;
        set => Write(value ? "\e[7m" : "\e[27m");
    }

    public static TextUnderlinedMode TextUnderline
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).Underlined;
        set => Write($"\e[{(int)value}m");
    }

    public static bool CrossedOutText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsCrossedOut;
        set => Write(value ? "\e[9m" : "\e[29m");
    }

    public static bool OverlinedText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsOverlined;
        set => Write(value ? "\e[53m" : "\e[55m");
    }

    public static TextFrameMode TextFrame
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).TextFrame;
        set => Write($"\e[{(int)value}m");
    }

    public static bool ConcealedText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsTextConcealed;
        set => Write(value ? "\e[8m" : "\e[28m");
    }

    public static bool ItalicText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsItalic;
        set => Write(value ? "\e[3m" : "\e[23m");
    }

    public static bool GothicText
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).IsGothic;
        set => Write(value ? "\e[20m" : "\e[23m");
    }

    public static TextTransformationMode TextTransformation
    {
        get => (CurrentGraphicRendition ?? ConsoleGraphicRendition.Default).TextTransformation;
        set => Write($"\e[{(int)value}m");
    }



    // TODO : ^[[5n     ???
    // TODO : ^[[6n     cursor position report
    // TODO : -> ^[[c
    //        <- ^[[?61;6;7;21;22;23;24;28;32;42c




    static Console()
    {
        if (OS.IsWindows)
        {
            //LINQ.TryDo(() => STDINConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_INPUT);
            LINQ.TryDo(() => STDINConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            LINQ.TryDo(() => STDOUTConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            LINQ.TryDo(() => STDERRConsoleMode |= ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
    }



    public static (int X, int Y, int Page)? GetExtendedCursorPosition()
    {
        if (GetRawVT520Report("[?6n", 'R') is ['\e', '[', '?', .. string response])
        {
            int[] parts = response.Split(';').ToArray(int.Parse);

            return (parts[0], parts[1], parts[2]);
        }
        else
            return null;
    }






    public static (int Mode, int[] Attributes)? GetDeviceAttributes()
    {
        if (GetRawVT520Report("[c", 'c') is string response)
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
            Write($"\e[{(int)Math.Round(volume * 7)};{(int)tone};{(int)Math.Round(duration * .032)}\a");
    }

    public static void SoftReset() => Write("\e[!p");

    public static void ClearAndResetAll() => Write("\e[3J\ec\e[m");

    public static void ResetAllAttributes() => Write("\e[m");

    public static void ResetForegroundColor() => Write("\e[39m");

    public static void ResetBackgroundColor() => Write("\e[49m");

    public static void ScrollUp(int lines)
    {
        if (lines < 0)
            ScrollDown(-lines);
        else if (lines > 0)
            Write($"\e[{lines}S");
    }

    public static void ScrollDown(int lines)
    {
        if (lines < 0)
            ScrollUp(-lines);
        else if (lines > 0)
            Write($"\e[{lines}T");
    }

    public static void ClearArea(ConsoleArea area, bool selective = false) =>
        Write($"\e[{area.Top};{area.Left};{area.Bottom};{area.Right}${(selective ? 'z' : '{')}");

    public static void FillArea(ConsoleArea area, char @char) =>
        Write($"\e[{(int)@char};{area.Top};{area.Left};{area.Bottom};{area.Right}$z");

    public static void DuplicateArea(ConsoleArea source, (int X, int Y) destination) => DuplicateArea(source, 0, destination);

    public static void DuplicateArea(ConsoleArea source, int source_page, (int X, int Y) destination) =>
        DuplicateArea(source, source_page, (destination.X, destination.Y, source_page));

    public static void DuplicateArea(ConsoleArea source, int source_page, (int X, int Y, int Page) destination) =>
        Write($"\e[{source.Top};{source.Left};{source.Bottom};{source.Right};{source_page};{destination.X};{destination.Y};{destination.Page}$v");

    public static void GetCursorInformation()
    {
        if (GetRawVT520Report("[1$w", '\\') is ['\e', 'P', _, '$', 'u', .. string response, '\e', '\\'])
        {
            // TODO
        }
    }

    public static void GetTabStopInformation()
    {
        if (GetRawVT520Report("[2$w", '\\') is ['\e', 'P', _, '$', 'u', .. string response, '\e', '\\'])
        {
            // TODO
        }
    }

    // TODO : page 151 of https://web.mit.edu/dosathena/doc/www/ek-vt520-rm.pdf

    public static void GetCursorType()
    {
        if (GetRawVT520SettingsReport(" q") is string response)
        {
            // TODO
        }
    }

    public static void GetMargins()
    {
        if (GetRawVT520SettingsReport("s") is string left_right &&
            GetRawVT520SettingsReport("t") is string top_bottom)
        {
            // TODO
        }
    }

    public static void GetColor()
    {
        if (GetRawVT520SettingsReport(",|") is string response)
        {
            // TODO
        }
    }






    public static void WriteReverseIndex() => Write("\eM");

    public static void Write(object? value, int left, int top) => Write(value, (left, top));

    public static void Write(object? value, (int left, int top) starting_pos)
    {
        SetCursorPosition(starting_pos.left, starting_pos.top);
        Write(value);
    }

    public static void InsertLine(int count = 1) => Write($"\e[{count}L");

    public static void DeleteLine(int count = 1) => Write($"\e[{count}M");

    public static void InsertSpaceCharacter(int count = 1) => Write($"\e[{count}@");

    public static void DeleteCharacter(int count = 1) => Write($"\e[{count}P");

    public static void ChangeLineRendering(int line, LineRenderingMode mode)
    {
        if (mode is LineRenderingMode.DoubleHeight)
        {
            ChangeLineRendering(line, LineRenderingMode.DoubleHeight_Top);
            ChangeLineRendering(line + 1, LineRenderingMode.DoubleHeight_Bottom);
        }
        else
        {
            CursorTop = line;
            Write($"\e#{mode switch
            {
                LineRenderingMode.DoubleWidth => 6,
                LineRenderingMode.DoubleHeight_Top => 3,
                LineRenderingMode.DoubleHeight_Bottom => 4,
                _ => 5
            }}");
        }
    }

    public static void WriteDoubleWidthLine(object? value) => WriteDoubleWidthLine(value, (CursorLeft, CursorTop));

    public static void WriteDoubleWidthLine(object? value, int left, int top) => WriteDoubleWidthLine(value, (left, top));

    public static void WriteDoubleWidthLine(object? value, (int left, int top)? starting_pos)
    {
        if (starting_pos is (int x, int y))
            SetCursorPosition(x, y);

        WriteLine($"\e#5{value}");
    }

    public static void WriteDoubleSizeLine(object? value) => WriteDoubleSizeLine(value, (CursorLeft, CursorTop));

    public static void WriteDoubleSizeLine(object? value, int left, int top) => WriteDoubleSizeLine(value, (left, top));

    public static void WriteDoubleSizeLine(object? value, (int left, int top)? starting_pos)
    {
        int x = starting_pos?.left ?? CursorLeft;
        int y = starting_pos?.top ?? CursorTop;
        string text = value?.ToString() ?? "";

        SetCursorPosition(x, y);
        Write($"\e#3{text}");
        SetCursorPosition(x, y + 1);
        WriteLine($"\e#4{text}");
    }

    public static void FullClear()
    {
        Clear();
        Write("\e[3J");
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
        List<string> cropped_lines = SplitLinesWithVT520(lines.ToList(), max_size.width, wrap_overflow);
        int line_no = 0;
        int max_width = 0;

        foreach (string line in cropped_lines.Take(max_size.height))
        {
            SetCursorPosition(starting_pos.left, starting_pos.top + line_no);
            Write(line);

            ++line_no;
            max_width = Math.Max(max_width, CursorLeft - starting_pos.left);
        }

        return (max_width, line_no);
    }

    public static void WriteVertical(object? value) => WriteVertical(value, CursorLeft, CursorTop);

    public static void WriteVertical(object? value, int left, int top) => WriteVertical(value, (left, top));

    public static void WriteVertical(object? value, (int left, int top) starting_pos)
    {
        string s = value?.ToString() ?? "";

        for (int i = 0; i < s.Length; i++)
        {
            CursorTop = starting_pos.top + i;
            CursorLeft = starting_pos.left;
            Write(s[i]);
        }
    }

    public static void WriteUnderlined(object? value) => Write($"\e[4m{value}\e[24m");

    public static void WriteInverted(object? value) => Write($"\e[7m{value}\e[27m");

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
            cursor_size = CursorSize;
            stdinmode = STDINConsoleMode;
            stderrmode = STDERRConsoleMode;
            stdoutmode = STDOUTConsoleMode;
#pragma warning restore CA1416
        }

        return new()
        {
            InputEncoding = InputEncoding,
            OutputEncoding = OutputEncoding,
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
            InputEncoding = state.InputEncoding ?? Encoding.Default;
            OutputEncoding = state.OutputEncoding ?? Encoding.Default;

            if (OS.IsWindows)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                STDINConsoleMode = state.STDINMode;
                STDOUTConsoleMode = state.STDOUTMode;
                STDERRConsoleMode = state.STDERRMode;

                if (state.CursorSize is int sz)
                    LINQ.TryDo(() => CursorSize = sz);
#pragma warning restore CA1416
            }

            BufferSize = state.BufferSize;
            WindowSize = state.WindowSize;
            WindowPosition = state.WindowPosition;
            CursorPosition = state.CursorPosition;
            CurrentGraphicRendition = state.GraphicRendition;
        }
    }
}


// TODO : https://web.mit.edu/dosathena/doc/www/ek-vt520-rm.pdf

