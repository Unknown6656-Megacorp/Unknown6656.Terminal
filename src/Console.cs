using System.Collections.Generic;
using System.Runtime.Versioning;
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
// TODO : progress bars
// TODO : markdown rendering/formatting


/// <summary>
/// The <see cref="Console"/> class provides a set of static methods and properties for interacting with the console.
/// This class is a wrapper around the <see cref="System.Console"/> class and provides additional functionality for controlling the console's appearance and behavior.
/// </summary>
/// <inheritdoc cref="sysconsole"/>
public static unsafe partial class Console
{
    /// <summary>
    /// Sets or gets the console's current <see cref="ConsoleState"/>.
    /// This can be used to restore the console to an earlier state.
    /// <para/>
    /// Please note that this property is merely a thin wrapper for the <see cref="SaveConsoleState"/> and <see cref="RestoreConsoleState"/> methods, respectively.
    /// </summary>
    public static ConsoleState CurrentConsoleState
    {
        get => SaveConsoleState();
        set => RestoreConsoleState(value);
    }

    #region BUFFER/WINDOW/SCREEN/CURSOR POSITIONING PROPERTIES

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

    public static bool WindowAutoResizeModeEnabled
    {
        set => SetVT520Bit(98, value);
    }

    public static bool IsWindowFramed
    {
        // get maybe via private DEC mode?
        set => SetVT520Bit(111, value);
    }

    public static ConsoleColor WindowFrameBackgroundColor
    {
        set => SetWindowFrameColor(value);
    }

    [SupportedOSPlatform(OS.LNX)]
    [SupportedOSPlatform(OS.MAC)]
    [SupportedOSPlatform(OS.MACC)]
    public static (ConsoleColor Foreground, ConsoleColor Background) WindowFrameColors
    {
        set => SetWindowFrameColor(value.Foreground, value.Background);
    }

    public static ConsoleCursorShape CursorShape
    {
        set => Write($"\e[{(int)value} q");
    }

    #endregion
    #region TEXT COLORING/STYLING PROPERTIES

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

    public static TextIntensityMode TextIntensity
    {
        get => CurrentGraphicRendition?.Intensity ?? TextIntensityMode.Regular;
        set => Write(value switch
        {
            TextIntensityMode.Bold => "\e[1m",
            TextIntensityMode.Dim => "\e[2m",
            _ => "\e[22m"
        });
    }

    public static TextBlinkMode TextBlink
    {
        get => CurrentGraphicRendition?.Blink ?? TextBlinkMode.NotBlinking;
        set => Write(value switch
        {
            TextBlinkMode.Slow => "\e[5m",
            TextBlinkMode.Rapid => "\e[6m",
            _ => "\e[25m"
        });
    }

    public static bool InvertedColors
    {
        get => CurrentGraphicRendition?.AreColorsInverted ?? false;
        set => Write(value ? "\e[7m" : "\e[27m");
    }

    public static TextUnderlinedMode TextUnderline
    {
        get => CurrentGraphicRendition?.Underlined ?? TextUnderlinedMode.NotUnderlined;
        set => Write($"\e[{(int)value}m");
    }

    public static bool OverlinedText
    {
        get => CurrentGraphicRendition?.IsOverlined ?? false;
        set => Write(value ? "\e[53m" : "\e[55m");
    }

    public static bool CrossedOutText
    {
        get => CurrentGraphicRendition?.IsCrossedOut ?? false;
        set => Write(value ? "\e[9m" : "\e[29m");
    }

    public static TextFrameMode TextFrame
    {
        get => CurrentGraphicRendition?.TextFrame ?? TextFrameMode.NotFramed;
        set => Write($"\e[{(int)value}m");
    }

    public static bool ConcealedText
    {
        get => CurrentGraphicRendition?.IsTextConcealed ?? false;
        set => Write(value ? "\e[8m" : "\e[28m");
    }

    public static bool ItalicText
    {
        get => CurrentGraphicRendition?.IsItalic ?? false;
        set => Write(value ? "\e[3m" : "\e[23m");
    }

    public static bool GothicText
    {
        get => CurrentGraphicRendition?.IsGothic ?? false;
        set => Write(value ? "\e[20m" : "\e[23m");
    }

    public static TextTransformationMode TextTransformation
    {
        get => CurrentGraphicRendition?.TextTransformation ?? TextTransformationMode.Regular;
        set => Write($"\e[{(int)value}m");
    }

    public static bool RightToLeft
    {
        set => SetVT520Bit(34, value);
    }

    public static bool ControlCharactersVisible
    {
        set => SetVT520Bit(3, value);
    }

    #endregion

    /// <summary>
    /// Sets whether the console should use the alternate screen buffer.
    /// </summary>
    public static bool AlternateScreenEnabled
    {
        set => SetVT520Bit(1049, value);
    }

    public static bool BracketedPasteModeEnabled
    {
        set => SetVT520Bit(2004, value);
    }



#warning TODO : verify if this is correct
    public static bool MouseEnabled
    {
        set => SetVT520Bit(1000, value);
    }

    // TODO : 1001h

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


    /// <summary>
    /// Switches the console between light and dark mode.
    /// Depending on the terminal configuration, the "dark mode" might be the terminal's default appearance, while the "light mode" might be an inverted color scheme.
    /// </summary>
    public static bool DarkMode
    {
        set => SetVT520Bit(5, !value);
    }



    // TODO : ^[[5n     ???
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

    /// <summary>
    /// Saves the current state of the console and returns it as a <see cref="ConsoleState"/> object.
    /// </summary>
    /// <returns>The saved console state.</returns>
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
            WindowFrameForeground = null, // TODO : implement
            WindowFrameBackground = null, // TODO : implement
        };
    }

    /// <summary>
    /// Restores the console to the state specified by the given <see cref="ConsoleState"/> object.
    /// </summary>
    /// <param name="state">The console state to restore.</param>
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

            if (!(state.WindowFrameForeground is null && state.WindowFrameBackground is null))
                WindowFrameColors = (
                    state.WindowFrameForeground ?? ConsoleColor.White,
                    state.WindowFrameBackground ?? ConsoleColor.Black
                );
        }
    }

    #region RESET/CLEAR FUNCTIONS

    /// <summary>
    /// Performs a soft reset of the console. This will reset the following settings to their respective default values:
    /// <list type="bullet">
    ///     <item>Text cursor enable mode.</item>
    ///     <item>Insert/replacement mode.</item>
    ///     <item>Cursor origin.</item>
    ///     <item>Autowrapping mode.</item>
    ///     <item>Keyboard action mode.</item>
    ///     <item>Numeric keypad.</item>
    ///     <item>Cursor keys.</item>
    ///     <item>Top and bottom margins.</item>
    ///     <item>Graphic renditions (SGR).</item>
    ///     <item>Character attributes.</item>
    ///     <item>Cursor direction.</item>
    /// </list>
    /// </summary>
    public static void SoftReset() => Write("\e[!p");

    /// <summary>
    /// Clears the entire console screen and resets the cursor position to the top-left corner.
    /// Note that the behavior of this function is similar to the <see cref="Clear"/> method, although it delivers more consistent results across various terminals.
    /// </summary>
    public static void FullClear()
    {
        Clear();
        Write("\e[3J");
    }

    /// <summary>
    /// Clears the entire console screen and performs a hard reset of the console, as well as all cursor and graphic renditions and attributes.
    /// </summary>
    public static void HardResetAndFullClear() => Write("\e[m\e[3J\e[!p\ec");

    /// <summary>
    /// Resets all graphic renditions to their default values.
    /// </summary>
    public static void ResetGraphicRenditions() => Write("\e[m");

    /// <summary>
    /// Resets the text's foreground color to its default value.
    /// </summary>
    public static void ResetForegroundColor() => ForegroundColor = ConsoleColor.Default;

    /// <summary>
    /// Resets the text's background color to its default value.
    /// </summary>
    public static void ResetBackgroundColor() => BackgroundColor = ConsoleColor.Default;

    #endregion
    #region BUFFER AREA

    /// <summary>
    /// Clears the current line.
    /// </summary>
    public static void ClearLine() => Write("\e[K");

    /// <summary>
    /// Clears the specified <paramref name="area"/> of the console buffer.
    /// <para/>
    /// Note that this preserves all <b>original</b> colors and console rendition modes of the characters in the cleared area.
    /// This behavior is different from the <see cref="FillBufferArea"/> method when used with the parameters <c>(<paramref name="area"/>, ' ')</c>.
    /// </summary>
    /// <param name="area">The area of the console buffer to clear.</param>
    /// <param name="selective">If set to <see langword="true"/>, only the characters in the specified area will be cleared, leaving the attributes unchanged.</param>
    public static void ClearBufferArea(ConsoleArea area, bool selective = false) =>
        Write($"\e[{area.Top};{area.Left};{area.Bottom};{area.Right}${(selective ? 'z' : '{')}");

    /// <summary>
    /// Fills the specified <paramref name="area"/> of the console buffer with the given character.
    /// </summary>
    /// <param name="area">The area of the console buffer to fill.</param>
    /// <param name="char">The character to fill the area with.</param>
    public static void FillBufferArea(ConsoleArea area, char @char) =>
        Write($"\e[{(int)@char};{area.Top};{area.Left};{area.Bottom};{area.Right}$x");

    /// <summary>
    /// Duplicates the specified source area of the console buffer to the specified destination.
    /// </summary>
    /// <param name="source">The source area of the console buffer to duplicate.</param>
    /// <param name="destination">The destination coordinates where the source area will be duplicated.</param>
    public static void DuplicateBufferArea(ConsoleArea source, (int X, int Y) destination) => DuplicateBufferArea(source, 0, destination);

    /// <summary>
    /// Duplicates the specified source area of the console buffer to the specified destination on the specified page.
    /// </summary>
    /// <param name="source">The source area of the console buffer to duplicate.</param>
    /// <param name="source_page">The page number of the source area.</param>
    /// <param name="destination">The destination coordinates where the source area will be duplicated.</param>
    public static void DuplicateBufferArea(ConsoleArea source, int source_page, (int X, int Y) destination) =>
        DuplicateBufferArea(source, source_page, (destination.X, destination.Y, source_page));

    /// <summary>
    /// Duplicates the specified source area of the console buffer to the specified destination on the specified page.
    /// </summary>
    /// <param name="source">The source area of the console buffer to duplicate.</param>
    /// <param name="source_page">The page number of the source area.</param>
    /// <param name="destination">The destination coordinates and page where the source area will be duplicated.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any of the coordinates are negative or if the source area has zero width or height.</exception>
    public static void DuplicateBufferArea(ConsoleArea source, int source_page, (int X, int Y, int Page) destination)
    {
        if (source.Left < 0 || source.Top < 0 || source.Width < 0 || source.Height < 0 || destination.X < 0 || destination.Y < 0)
            throw new ArgumentOutOfRangeException("All coordinates must be non-negative.");
        else if (source.Width == 0 || source.Height == 0)
            return;
        //else if (sourceLeft + sourceWidth > BufferWidth || sourceTop + sourceHeight > BufferHeight)
        //    throw new ArgumentOutOfRangeException("The source area must fit into the buffer.");
        //else if (targetLeft + sourceWidth > BufferWidth || targetTop + sourceHeight > BufferHeight)
        //    throw new ArgumentOutOfRangeException("The target area must fit into the buffer.");


#warning TODO : check for off-by-one errors
        Write($"\e[{source.Top};{source.Left};{source.Bottom};{source.Right};{source_page};{destination.X};{destination.Y};{destination.Page}$v");
    }

    /// <summary>
    /// 
    /// Duplicates the specified source area of the console buffer to the specified destination.
    /// </summary>
    /// <param name="sourceLeft">The left coordinate of the source area.</param>
    /// <param name="sourceTop">The top coordinate of the source area.</param>
    /// <param name="sourceWidth">The width of the source area.</param>
    /// <param name="sourceHeight">The height of the source area.</param>
    /// <param name="targetLeft">The left coordinate of the target area.</param>
    /// <param name="targetTop">The top coordinate of the target area.</param>
    public static void DuplicateBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop) =>
        DuplicateBufferArea(new(sourceLeft, sourceTop, sourceWidth, sourceHeight), (targetLeft, targetTop));

    /// <summary>
    /// Modifies the specified area of the console buffer with the given graphic rendition.
    /// </summary>
    /// <param name="area">The area of the console buffer to modify.</param>
    /// <param name="sgr">The graphic rendition to apply to the specified area.</param>
    public static void ModifyBufferArea(ConsoleArea area, ConsoleGraphicRendition sgr) => ChangeVT520ForBufferArea(area, sgr.FullVT520SGR());

    /// <summary>
    /// Scrolls the console buffer up by the specified number of lines.
    /// Please note that the cursor position will not be changed by this operation.
    /// </summary>
    /// <param name="lines">The number of lines to scroll. If negative, the buffer will scroll down.</param>
    public static void MoveBufferUp(int lines)
    {
        if (lines < 0)
            MoveBufferDown(-lines);
        else if (lines > 0)
            Write($"\e[{lines}S");
    }

    /// <summary>
    /// Scrolls the console buffer down by the specified number of lines.
    /// Please note that the cursor position will not be changed by this operation.
    /// </summary>
    /// <param name="lines">The number of lines to scroll. If negative, the buffer will scroll up.</param>
    public static void MoveBufferDown(int lines)
    {
        if (lines < 0)
            MoveBufferUp(-lines);
        else if (lines > 0)
            Write($"\e[{lines}T");
    }

    #endregion
    #region CURSOR POSITION

    /// <summary>
    /// Gets the extended cursor position, including the page number.
    /// </summary>
    /// <returns>
    /// A tuple containing the X-coordinate, Y-coordinate, and page number of the cursor position,
    /// or <see langword="null"/> if the position could not be determined.
    /// </returns>
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


    #endregion

#pragma warning disable CA1416 // Validate platform compatibility
    /// <summary>
    /// Sets the foreground color of the window frame.
    /// </summary>
    /// <param name="background">The terminal window frame background color. The color must be a system color (i.e., one of <see cref="ConsoleColor"/>'s static members, or an instance of <see cref="sysconsolecolor"/>).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if a <paramref name="background"/> is not a system color.</exception>
    public static void SetWindowFrameColor(ConsoleColor background) => SetWindowFrameColor(ConsoleColor.Black, background);
#pragma warning restore CA1416

    /// <summary>
    /// Sets the foreground and background colors of the window frame.
    /// This method is currently only supported on Linux and macOS.
    /// </summary>
    /// <param name="foreground">The terminal window frame foreground color. The color must be a system color (i.e., one of <see cref="ConsoleColor"/>'s static members, or an instance of <see cref="sysconsolecolor"/>).</param>
    /// <param name="background">The terminal window frame background color. The color must be a system color (i.e., one of <see cref="ConsoleColor"/>'s static members, or an instance of <see cref="sysconsolecolor"/>).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if a <paramref name="foreground"/> and/or <paramref name="background"/> is not a system color.</exception>
    [SupportedOSPlatform(OS.LNX)]
    [SupportedOSPlatform(OS.MAC)]
    [SupportedOSPlatform(OS.MACC)]
    public static void SetWindowFrameColor(ConsoleColor foreground, ConsoleColor background)
    {
        if (background.ToSystemColor() is not sysconsolecolor bg)
            throw new ArgumentOutOfRangeException(nameof(background), $"The specified background color '{background}' is not supported.");
        else if (foreground.ToSystemColor() is not sysconsolecolor fg)
            throw new ArgumentOutOfRangeException(nameof(foreground), $"The specified foreground color '{foreground}' is not supported.");
        else
            Write($"\e[2;{(int)fg};{(int)bg},|");
    }


    #region WRITE FUNCTIONS

    public static void Write(object? value, int left, int top) => Write(value, (left, top));

    public static void Write(object? value, (int left, int top) starting_pos)
    {
        SetCursorPosition(starting_pos.left, starting_pos.top);
        Write(value);
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
        List<string> cropped_lines = VT520.SplitLinesWithVT520(lines.ToList(), max_size.width, wrap_overflow);
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

    /// <summary>
    /// Writes the specified value vertically down starting from the current cursor position.
    /// </summary>
    /// <param name="value">The value to write vertically.</param>
    public static void WriteVertical(object? value) => WriteVertical(value, CursorLeft, CursorTop);

    /// <summary>
    /// Writes the specified value vertically down starting from the specified position.
    /// </summary>
    /// <param name="value">The value to write vertically.</param>
    /// <param name="left">The left coordinate to start writing from.</param>
    /// <param name="top">The top coordinate to start writing from.</param>
    public static void WriteVertical(object? value, int left, int top) => WriteVertical(value, (left, top));

    /// <summary>
    /// Writes the specified value vertically down starting from the specified position.
    /// </summary>
    /// <param name="value">The value to write vertically.</param>
    /// <param name="starting_pos">The starting position to write from.</param>
    public static void WriteVertical(object? value, (int left, int top) starting_pos)
    {
        string s = value as string ?? value?.ToString() ?? "";

        for (int i = 0; i < s.Length; ++i)
            if (s[i] is '\e' or '\x9b' && VT520.StartsWithVT520Sequence(s[i..], out string? sequence))
            {
                Write(sequence);

                i += sequence.Length - 1;
            }
            else
            {
                CursorTop = starting_pos.top + i;
                CursorLeft = starting_pos.left;

                Write(s[i]);
            }
    }

    public static void WriteFormatted(object? value, ConsoleGraphicRendition format)
    {
        ConsoleGraphicRendition? current = CurrentGraphicRendition;

        CurrentGraphicRendition = format;

        Write(value);

        CurrentGraphicRendition = current;
    }

    public static void WriteUnderlined(object? value) => WriteFormatted(value, new() { Underlined = TextUnderlinedMode.Single });

    public static void WriteInverted(object? value) => WriteFormatted(value, new() { AreColorsInverted = true });

    public static void WriteBold(object? value) => WriteFormatted(value, new() { Intensity = TextIntensityMode.Bold });

    public static void WriteDim(object? value) => WriteFormatted(value, new() { Intensity = TextIntensityMode.Dim });

    public static void WriteBlinking(object? value) => WriteFormatted(value, new() { Blink = TextBlinkMode.Slow });

    public static void WriteItalic(object? value) => WriteFormatted(value, new() { IsItalic = true });


    // TODO : implement all other (temporary) formatting styles




    public static void WriteDoubleWidthLine(object? value) => WriteDoubleWidthLine(value, null);

    public static void WriteDoubleWidthLine(object? value, int left, int top) => WriteDoubleWidthLine(value, (left, top));

    // TODO : handle line wrapping/breaks for the following functions
    public static void WriteDoubleWidthLine(object? value, (int left, int top)? starting_pos)
    {
        if (starting_pos is (int x, int y))
            SetCursorPosition(x, y);

        WriteLine($"\e#5{value}");
    }

    public static void WriteDoubleSizeLine(object? value) => WriteDoubleSizeLine(value, null);

    public static void WriteDoubleSizeLine(object? value, int left, int top) => WriteDoubleSizeLine(value, (left, top));

    // TODO : handle line wrapping/breaks for the following functions
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

    /// <summary>
    /// Changes the line rendering mode of the current line.
    /// <para/>
    /// Please note that if the value of <paramref name="mode"/> is set to <see cref="LineRenderingMode.DoubleHeight"/>, the line below the current line will also be affected.
    /// </summary>
    /// <param name="mode">The new rendering mode for the current (and possibly following) line.</param>
    public static void ChangeLineRendering(LineRenderingMode mode) => ChangeLineRendering(CursorTop, mode);

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

    #endregion



    public static bool TryReadInt(out int value) => int.TryParse(ReadLine(), out value);

    public static bool TryReadDouble(out double value) => double.TryParse(ReadLine(), out value);

    public static bool TryReadFloat(out float value) => float.TryParse(ReadLine(), out value);

    public static bool TryReadLong(out long value) => long.TryParse(ReadLine(), out value);

    public static bool TryReadDecimal(out decimal value) => decimal.TryParse(ReadLine(), out value);

    public static bool TryReadBool(out bool value) => bool.TryParse(ReadLine(), out value) || (TryReadInt(out int i) && i != 0);








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

    //public static void GetCursorInformation()
    //{
    //    if (GetRawVT520Report("[1$w", '\\') is ['\e', 'P', _, '$', 'u', .. string response, '\e', '\\'])
    //    {
    //        // TODO
    //    }
    //}

    //public static void GetTabStopInformation()
    //{
    //    if (GetRawVT520Report("[2$w", '\\') is ['\e', 'P', _, '$', 'u', .. string response, '\e', '\\'])
    //    {
    //        // TODO
    //    }
    //}

    // TODO : page 151 of https://web.mit.edu/dosathena/doc/www/ek-vt520-rm.pdf

    //public static void GetCursorType()
    //{
    //    if (GetRawVT520SettingsReport(" q") is string response)
    //    {
    //        // TODO
    //    }
    //}

    //public static void GetMargins()
    //{
    //    if (GetRawVT520SettingsReport("s") is string left_right &&
    //        GetRawVT520SettingsReport("t") is string top_bottom)
    //    {
    //        // TODO
    //    }
    //}

    //public static void GetColor()
    //{
    //    if (GetRawVT520SettingsReport(",|") is string response)
    //    {
    //        // TODO
    //    }
    //}






    public static void WriteReverseIndex() => Write("\eM");

    public static void InsertLine(int count = 1) => Write($"\e[{count}L");

    /// <summary>
    /// Deletes the specified number of lines from the current cursor position.
    /// This will cause the lines below the cursor to be shifted up.
    /// </summary>
    /// <param name="count">The number of lines to be deleted.</param>
    public static void DeleteLines(int count = 1) => Write($"\e[{count}M");

    /// <summary>
    /// Inserts the specified number of space characters (<c>0x20</c>) at the current cursor position.
    /// This will cause the characters to the right of the cursor to be shifted to the right.
    /// </summary>
    /// <param name="count">The number of spaces to be inserted.</param>
    public static void InsertSpaceCharacter(int count = 1) => Write($"\e[{count}@");

    /// <summary>
    /// Deletes the specified number of characters from the current cursor position.
    /// This will cause the characters to the right of the cursor to be shifted to the left.
    /// </summary>
    /// <param name="count">The number of characters to be deleted.</param>
    public static void DeleteCharacters(int count = 1) => Write($"\e[{count}P");


}


// TODO : https://web.mit.edu/dosathena/doc/www/ek-vt520-rm.pdf

