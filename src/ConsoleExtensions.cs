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


/// <summary>
/// Specifies the visual intensity of the text (regular, bold, dim).
/// </summary>
public enum TextIntensityMode
    : byte
{
    /// <summary>
    /// Regular intensity.
    /// </summary>
    Regular = 22,
    /// <summary>
    /// Bold intensity.
    /// </summary>
    Bold = 1,
    /// <summary>
    /// Dim intensity.
    /// </summary>
    Dim = 2,
}

/// <summary>
/// Specifies the text underline mode.
/// </summary>
public enum TextUnderlinedMode
{
    /// <summary>
    /// Text is not underlined.
    /// </summary>
    NotUnderlined = 24,
    /// <summary>
    /// Text is underlined with a single line.
    /// </summary>
    Single = 4,
    /// <summary>
    /// Text is underlined with a double line.
    /// </summary>
    Double = 21,
}

/// <summary>
/// Specifies the text frame mode.
/// </summary>
public enum TextFrameMode
{
    /// <summary>
    /// Text is not framed.
    /// </summary>
    NotFramed = 54,
    /// <summary>
    /// Text is framed.
    /// </summary>
    Framed = 51,
    /// <summary>
    /// Text is encircled.
    /// </summary>
    Encircled = 52,
}

/// <summary>
/// Specifies the text transformation mode (sub-/superscript).
/// </summary>
public enum TextTransformationMode
{
    /// <summary>
    /// Regular text transformation.
    /// </summary>
    Regular = 75,
    /// <summary>
    /// Text is transformed to superscript.
    /// </summary>
    Superscript = 73,
    /// <summary>
    /// Text is transformed to subscript.
    /// </summary>
    Subscript = 74,
}

/// <summary>
/// Specifies the text blink mode (none, slow, rapid).
/// </summary>
public enum TextBlinkMode
    : byte
{
    NotBlinking = 25,
    Slow = 5,
    Rapid = 6,
}

/// <summary>
/// Specifies the text rendering mode.
/// </summary>
public enum LineRenderingMode
{
    /// <summary>
    /// Regular line rendering.
    /// </summary>
    Regular,
    /// <summary>
    /// Line is rendered with double width.
    /// </summary>
    DoubleWidth,
    /// <summary>
    /// Line is rendered with double height.
    /// </summary>
    DoubleHeight,
    /// <summary>
    /// Top half of the line is rendered with double height.
    /// </summary>
    DoubleHeight_Top,
    /// <summary>
    /// Bottom half of the line is rendered with double height.
    /// </summary>
    DoubleHeight_Bottom,
}

/// <summary>
/// Specifies the shape of the console cursor.
/// </summary>
public enum ConsoleCursorShape
{
    /// <summary>
    /// Default cursor shape.
    /// </summary>
    Default = 0,
    /// <summary>
    /// Blinking block cursor.
    /// </summary>
    BlinkingBlock = 1,
    /// <summary>
    /// Solid block cursor.
    /// </summary>
    SolidBlock = 2,
    /// <summary>
    /// Blinking underline cursor.
    /// <para/>
    /// This is the default in <c>conhost.exe</c>.
    /// </summary>
    BlinkingUnderline = 3,
    /// <summary>
    /// Solid underline cursor.
    /// </summary>
    SolidUnderline = 4,
    /// <summary>
    /// Blinking (vertical) cursor bar.
    /// <para/>
    /// This is the default in the Windows Terminal app (<c>wt.exe</c>).
    /// </summary>
    BlinkingBar = 5,
    /// <summary>
    /// Solid (vertical) cursor bar.
    /// </summary>
    SolidBar = 6,
}

/// <summary>
/// An enumeration of musical notes that can be played by the console speaker.
/// </summary>
public enum ConsoleTone
{
    /// <summary>
    /// No sound.
    /// </summary>
    Silent = 0,
    /// <summary>
    /// Note C in the 5th octave.
    /// </summary>
    C5 = 1,
    /// <summary>
    /// Note C# (C sharp) in the 5th octave.
    /// </summary>
    CSharp5 = 2,
    /// <summary>
    /// Note D in the 5th octave.
    /// </summary>
    D5 = 3,
    /// <summary>
    /// Note D# (D sharp) in the 5th octave.
    /// </summary>
    DSharp5 = 4,
    /// <summary>
    /// Note E in the 5th octave.
    /// </summary>
    E5 = 5,
    /// <summary>
    /// Note F in the 5th octave.
    /// </summary>
    F5 = 6,
    /// <summary>
    /// Note F# (F sharp) in the 5th octave.
    /// </summary>
    FSharp5 = 7,
    /// <summary>
    /// Note G in the 5th octave.
    /// </summary>
    G5 = 8,
    /// <summary>
    /// Note G# (G sharp) in the 5th octave.
    /// </summary>
    GSharp5 = 9,
    /// <summary>
    /// Note A in the 5th octave.
    /// </summary>
    A5 = 10,
    /// <summary>
    /// Note A# (A sharp) in the 5th octave.
    /// </summary>
    ASharp5 = 11,
    /// <summary>
    /// Note B in the 5th octave.
    /// </summary>
    B5 = 12,
    /// <summary>
    /// Note C in the 6th octave.
    /// </summary>
    C6 = 13,
    /// <summary>
    /// Note C# (C sharp) in the 6th octave.
    /// </summary>
    CSharp6 = 14,
    /// <summary>
    /// Note D in the 6th octave.
    /// </summary>
    D6 = 15,
    /// <summary>
    /// Note D# (D sharp) in the 6th octave.
    /// </summary>
    DSharp6 = 16,
    /// <summary>
    /// Note E in the 6th octave.
    /// </summary>
    E6 = 17,
    /// <summary>
    /// Note F in the 6th octave.
    /// </summary>
    F6 = 18,
    /// <summary>
    /// Note F# (F sharp) in the 6th octave.
    /// </summary>
    FSharp6 = 19,
    /// <summary>
    /// Note G in the 6th octave.
    /// </summary>
    G6 = 20,
    /// <summary>
    /// Note G# (G sharp) in the 6th octave.
    /// </summary>
    GSharp6 = 21,
    /// <summary>
    /// Note A in the 6th octave.
    /// </summary>
    A6 = 22,
    /// <summary>
    /// Note A# (A sharp) in the 6th octave.
    /// </summary>
    ASharp6 = 23,
    /// <summary>
    /// Note B in the 6th octave.
    /// </summary>
    B6 = 24,
    /// <summary>
    /// Note C in the 7th octave.
    /// </summary>
    C7 = 25,
}

/// <summary>
/// Represents a rectangular area in the console.
/// </summary>
/// <param name="X">The zero-based X-coordinate of the top-left corner of the area (in characters).</param>
/// <param name="Y">The zero-based Y-coordinate of the top-left corner of the area (in characters).</param>
/// <param name="Width">The width of the area (in characters).</param>
/// <param name="Height">The height of the area (in characters).</param>
public readonly record struct ConsoleArea(int X, int Y, int Width, int Height)
{
    /// <summary>
    /// Returns the <see cref="ConsoleArea"/> that represents an empty area.
    /// </summary>
    public static ConsoleArea Empty { get; } = new(0, 0, 0, 0);

    /// <summary>
    /// Returns the <see cref="ConsoleArea"/> that represents the entire visible <see cref="sysconsole"/> screen.
    /// </summary>
    public static ConsoleArea FullWindow => new(0, 0, sysconsole.WindowWidth, sysconsole.WindowHeight);

    /// <summary>
    /// Returns the <see cref="ConsoleArea"/> that represents the entire <see cref="sysconsole"/> text buffer.
    /// </summary>
    public static ConsoleArea FullBuffer => new(0, 0, sysconsole.BufferWidth, sysconsole.BufferHeight);


    /// <summary>
    /// Gets the zero-based X-coordinate of the left edge of the area.
    /// <para/>
    /// This value is identical to <see cref="X"/> and is provided for compatibility with <see cref="Rectangle"/>.
    /// </summary>
    public readonly int Left => X;

    /// <summary>
    /// Gets the zero-based Y-coordinate of the top edge of the area.
    /// <para/>
    /// This value is identical to <see cref="Y"/> and is provided for compatibility with <see cref="Rectangle"/>.
    /// </summary>
    public readonly int Top => Y;

    /// <summary>
    /// Gets the zero-based X-coordinate of the right edge of the area.
    /// </summary>
    public readonly int Right => X + Width;

    /// <summary>
    /// Gets the zero-based Y-coordinate of the bottom edge of the area.
    /// </summary>
    public readonly int Bottom => Y + Height;

    /// <summary>
    /// Indicates whether the current <see cref="ConsoleArea"/> is contained within <see cref="FullWindow"/>.
    /// </summary>
    public readonly bool IsInsideWindow => FullWindow.Contains(this);

    /// <summary>
    /// Indicates whether the current <see cref="ConsoleArea"/> is contained within <see cref="FullBuffer"/>.
    /// </summary>
    public readonly bool IsInsideBuffer => FullBuffer.Contains(this);


    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleArea"/> struct from a <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="rectangle">The rectangle to initialize from.</param>
    public ConsoleArea(Rectangle rectangle)
        : this(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleArea"/> struct from column and row ranges.
    /// </summary>
    /// <param name="columns">The range of columns.</param>
    /// <param name="rows">The range of rows.</param>
    public ConsoleArea(Range columns, Range rows)
        : this(
            columns.Start.Value,
            rows.Start.Value,
            columns.End.Value - columns.Start.Value,
            rows.End.Value - rows.Start.Value
        )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleArea"/> struct from two points.
    /// </summary>
    /// <param name="from">The starting point.</param>
    /// <param name="to">The ending point.</param>
    public ConsoleArea((int X, int Y) from, (int X, int Y) to)
        : this(from.X, from.Y, to.X - from.X, to.Y - from.Y)
    {
    }

    /// <summary>
    /// Determines whether the specified coordinates are contained within this <see cref="ConsoleArea"/>.
    /// </summary>
    /// <param name="x">The zero-based X-coordinate to check.</param>
    /// <param name="y">The zero-based Y-coordinate to check.</param>
    /// <returns>
    /// <see langword="true"/> if the specified (<paramref name="x"/>, <paramref name="y"/>)-coordinates are within this <see cref="ConsoleArea"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public readonly bool Contains(int x, int y) => x >= X && x < Right && y >= Y && y < Bottom;

    /// <summary>
    /// Determines whether the specified <see cref="ConsoleArea"/> is entirely contained within this <see cref="ConsoleArea"/>.
    /// </summary>
    /// <param name="area">The <see cref="ConsoleArea"/> to check.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <paramref name="area"/> is contained within this <see cref="ConsoleArea"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public readonly bool Contains(ConsoleArea area) => area.X >= X && area.Right <= Right && area.Y >= Y && area.Bottom <= Bottom;

    // TODO : check if this is correct.
    public readonly bool Overlaps(ConsoleArea area) => area.X < Right && area.Right > X && area.Y < Bottom && area.Bottom > Y;

    /// <summary>
    /// Returns the intersection of this <see cref="ConsoleArea"/> with another <see cref="ConsoleArea"/>.
    /// </summary>
    /// <param name="area">The other <see cref="ConsoleArea"/> to intersect the current instance with.</param>
    /// <returns>The intersection between the two <see cref="ConsoleArea"/>s.</returns>
    public readonly ConsoleArea Intersect(ConsoleArea area)
    {
        int x = Math.Max(X, area.X);
        int y = Math.Max(Y, area.Y);
        int right = Math.Min(Right, area.Right);
        int bottom = Math.Min(Bottom, area.Bottom);

        return new ConsoleArea(x, y, right - x, bottom - y);
    }

    public static ConsoleArea CreateBoundingArea(IEnumerable<ConsoleArea> areas) => ;

    public static ConsoleArea CreateBoundingArea(IEnumerable<Point> points) => CreateBoundingArea(points.Select(p => (p.X, p.Y)));

    public static ConsoleArea CreateBoundingArea(IEnumerable<(int X, int Y)> points) => ;


    /// <summary>
    /// Converts a <see cref="Rectangle"/> to a <see cref="ConsoleArea"/>.
    /// </summary>
    /// <param name="rectangle">The rectangle to convert.</param>
    public static implicit operator ConsoleArea(Rectangle rectangle) => new(rectangle);

    /// <summary>
    /// Converts a tuple of column and row ranges to a <see cref="ConsoleArea"/>.
    /// </summary>
    /// <param name="area">The tuple of column and row ranges to convert.</param>
    public static implicit operator ConsoleArea((Range columns, Range rows) area) => new(area.columns, area.rows);

    /// <summary>
    /// Converts a tuple of two points to a <see cref="ConsoleArea"/>.
    /// </summary>
    /// <param name="area">The tuple of two points to convert.</param>
    public static implicit operator ConsoleArea(((int X, int Y) from, (int X, int Y) to) area) => new(area.from, area.to);

    /// <summary>
    /// Converts a tuple of left, top, width, and height to a <see cref="ConsoleArea"/>.
    /// </summary>
    /// <param name="area">The tuple of left, top, width, and height to convert.</param>
    public static implicit operator ConsoleArea((int Left, int Top, int Width, int Height) area) => new(area.Left, area.Top, area.Width, area.Height);

    public static ConsoleArea operator &(ConsoleArea a, ConsoleArea b) => a.Intersect(b);
}

/// <summary>
/// Represents a console graphic rendition, which includes various text attributes such as intensity, blink, underline, and colors.
/// </summary>
/// <param name="RawVT100SGRs">The raw VT100 SGR (Select Graphic Rendition) sequences.</param>
public record ConsoleGraphicRendition(string[] RawVT100SGRs)
{
    /// <summary>
    /// Gets the default console graphic rendition (<c>^[0m</c>).
    /// </summary>
    public static ConsoleGraphicRendition Default { get; } = new(["0"]);


    /// <summary>
    /// The intensity of the text (regular, bold, dim).
    /// </summary>
    public TextIntensityMode Intensity { get; init; } = TextIntensityMode.Regular;

    /// <summary>
    /// The blink mode of the text (none, slow, rapid).
    /// </summary>
    public TextBlinkMode Blink { get; init; } = TextBlinkMode.NotBlinking;

    /// <summary>
    /// The underline mode of the text (not underlined, single, double).
    /// </summary>
    public TextUnderlinedMode Underlined { get; init; } = TextUnderlinedMode.NotUnderlined;

    /// <summary>
    /// Indicates whether the text colors are inverted.
    /// </summary>
    public bool AreColorsInverted { get; init; } = false;

    /// <summary>
    /// Indicates whether the text is italic.
    /// </summary>
    public bool IsItalic { get; init; } = false;

    /// <summary>
    /// Indicates whether the text is concealed.
    /// </summary>
    public bool IsTextConcealed { get; init; } = false;

    /// <summary>
    /// Indicates whether the text is crossed out.
    /// </summary>
    public bool IsCrossedOut { get; init; } = false;

    /// <summary>
    /// Indicates whether the text is overlined.
    /// </summary>
    public bool IsOverlined { get; init; } = false;

    /// <summary>
    /// Indicates whether the font is the default font (i.e., whether the <see cref="FontIndex"/> is equal to <c>0</c>).
    /// </summary>
    public bool IsDefaultFont => FontIndex == 0;

    /// <summary>
    /// The index of the font to use.
    /// </summary>
    public int FontIndex { get; init; } = 0;

    /// <summary>
    /// Indicates whether the font is monospace.
    /// </summary>
    public bool IsMonospace { get; init; } = true;

    /// <summary>
    /// Indicates whether the font is gothic.
    /// </summary>
    public bool IsGothic { get; init; } = false;

    /// <summary>
    /// The text frame mode.
    /// </summary>
    public TextFrameMode TextFrame { get; init; } = TextFrameMode.NotFramed;

    /// <summary>
    /// The text transformation mode (regular, superscript, subscript).
    /// </summary>
    public TextTransformationMode TextTransformation { get; init; } = TextTransformationMode.Regular;

    /// <summary>
    /// The foreground color of the text. A value of <see langword="null"/> indicates the default color.
    /// </summary>
    public Union<ConsoleColor, Color>? ForegroundColor { get; init; } = null;

    /// <summary>
    /// The background color of the text. A value of <see langword="null"/> indicates the default color.
    /// </summary>
    public Union<ConsoleColor, Color>? BackgroundColor { get; init; } = null;

    /// <summary>
    /// The color of the underline. A value of <see langword="null"/> indicates that the <see cref="UnderlineColor"/> is identical to the <see cref="ForegroundColor"/>.
    /// </summary>
    public Color? UnderlineColor { get; init; } = null;


    /// <summary>
    /// Returns the full VT100 SGR (Select Graphic Rendition) sequences for this console graphic rendition.
    /// </summary>
    /// <returns>An array of VT100 SGR sequences.</returns>
    public string[] FullVT100SGR()
    {
        IEnumerable<string> modes = [
            .. RawVT100SGRs,
            ((int)Intensity).ToString(),
            ((int)Blink).ToString(),
            ((int)Underlined).ToString(),
            AreColorsInverted ? "7" : "27",
            IsItalic ? "3" : "23",
            IsTextConcealed ? "8" : "28",
            IsCrossedOut ? "9" : "29",
            IsOverlined ? "53" : "55",
            $"1{FontIndex}",
            IsGothic ? "20" : "23",
            IsMonospace ? "50" : "26",
            ((int)TextFrame).ToString(),
            ((int)TextTransformation).ToString(),
            generate_color(ForegroundColor, true),
            generate_color(BackgroundColor, false),
            generate_color(UnderlineColor, null),
        ];

        if (modes.LastIndexOf("0") is int reset and > 0)
            modes = modes.Skip(reset);

        return modes.Distinct().ToArray();

        string generate_color(Union<ConsoleColor, Color>? color, bool? foreground)
        {
            if (color?.Is(out ConsoleColor cc) ?? false)
            {
                (bool bright, ConsoleColor normalized) = cc switch
                {
                    ConsoleColor.Black => (false, cc),
                    ConsoleColor.DarkBlue => (false, cc),
                    ConsoleColor.DarkGreen => (false, cc),
                    ConsoleColor.DarkCyan => (false, cc),
                    ConsoleColor.DarkRed => (false, cc),
                    ConsoleColor.DarkMagenta => (false, cc),
                    ConsoleColor.DarkYellow => (false, cc),
                    ConsoleColor.Gray => (false, cc),
                    ConsoleColor.DarkGray => (true, ConsoleColor.Black),
                    ConsoleColor.Blue => (true, ConsoleColor.DarkBlue),
                    ConsoleColor.Green => (true, ConsoleColor.DarkGreen),
                    ConsoleColor.Cyan => (true, ConsoleColor.DarkCyan),
                    ConsoleColor.Red => (true, ConsoleColor.DarkRed),
                    ConsoleColor.Magenta => (true, ConsoleColor.DarkMagenta),
                    ConsoleColor.Yellow => (true, ConsoleColor.DarkYellow),
                    ConsoleColor.White => (true, ConsoleColor.Gray),
                    _ => (false, cc),
                };

                return $"{(foreground, bright) switch
                {
                    (true, true) => "9",
                    (true, false) => "3",
                    (false, true) => "10",
                    (false, false) => "4",
                }}{(int)normalized}";
            }
            else if (color?.Is(out Color rgb) ?? false)
                return $"{foreground switch {
                    true => "38",
                    false => "48",
                    _ => "58",
                }}:2:{rgb.R}:{rgb.G}:{rgb.B}";

            return foreground switch
            {
                true => "39",
                false => "49",
                _ => "59",
            };
        }
    }

    /// <summary>
    /// Parses the specified VT100 SGR (Select Graphic Rendition) sequences and returns the corresponding console graphic rendition.
    /// </summary>
    /// <param name="SGRs">Sequence of VT100 SGRs.</param>
    /// <returns>Parsed <see cref="ConsoleGraphicRendition"/>.</returns>
    public static ConsoleGraphicRendition TryParse(string[] SGRs)
    {
        ConsoleGraphicRendition rendition = new(SGRs);

        foreach (string sgr in SGRs)
            if (sgr is "0")
                rendition = Default with { RawVT100SGRs = SGRs };
            else if (sgr is "1")
                rendition = rendition with { Intensity = TextIntensityMode.Bold };
            else if (sgr is "2")
                rendition = rendition with { Intensity = TextIntensityMode.Dim };
            else if (sgr is "3")
                rendition = rendition with { IsItalic = true };
            else if (sgr is "4")
                rendition = rendition with { Underlined = TextUnderlinedMode.Single };
            else if (sgr is "5")
                rendition = rendition with { Blink = TextBlinkMode.Slow };
            else if (sgr is "6")
                rendition = rendition with { Blink = TextBlinkMode.Rapid };
            else if (sgr is "7")
                rendition = rendition with { AreColorsInverted = true };
            else if (sgr is "8")
                rendition = rendition with { IsTextConcealed = true };
            else if (sgr is "9")
                rendition = rendition with { IsCrossedOut = true };
            else if (sgr is ['1', char font_index])
                rendition = rendition with { FontIndex = font_index - '0' };
            else if (sgr is "20")
                rendition = rendition with { IsGothic = true };
            else if (sgr is "21")
                rendition = rendition with { Underlined = TextUnderlinedMode.Double };
            else if (sgr is "22")
                rendition = rendition with { Intensity = TextIntensityMode.Regular };
            else if (sgr is "23")
                rendition = rendition with { IsItalic = false, IsGothic = false };
            else if (sgr is "24")
                rendition = rendition with { Underlined = TextUnderlinedMode.NotUnderlined };
            else if (sgr is "25")
                rendition = rendition with { Blink = TextBlinkMode.NotBlinking };
            else if (sgr is "26")
                rendition = rendition with { IsMonospace = false };
            else if (sgr is "27")
                rendition = rendition with { AreColorsInverted = false };
            else if (sgr is "28")
                rendition = rendition with { IsTextConcealed = false };
            else if (sgr is "29")
                rendition = rendition with { IsCrossedOut = false };
            else if (sgr is ['3', '8', ..string fg_color])
                rendition = rendition with { ForegroundColor = parse_color(fg_color) };
            else if (sgr is ['3', char fg_color_index])
                rendition = rendition with { ForegroundColor = (ConsoleColor)(fg_color_index - '0') };
            else if (sgr is "39")
                rendition = rendition with { ForegroundColor = ConsoleColor.Gray };
            else if (sgr is ['4', '8', .. string bg_color])
                rendition = rendition with { BackgroundColor = parse_color(bg_color) };
            else if (sgr is ['4', char bg_color_index])
                rendition = rendition with { BackgroundColor = (ConsoleColor)(bg_color_index - '0') };
            else if (sgr is "49")
                rendition = rendition with { BackgroundColor = ConsoleColor.Black };
            else if (sgr is "50")
                rendition = rendition with { IsMonospace = true };
            else if (sgr is "51")
                rendition = rendition with { TextFrame = TextFrameMode.Framed };
            else if (sgr is "52")
                rendition = rendition with { TextFrame = TextFrameMode.Encircled };
            else if (sgr is "53")
                rendition = rendition with { IsOverlined = true };
            else if (sgr is "54")
                rendition = rendition with { TextFrame = TextFrameMode.NotFramed };
            else if (sgr is "55")
                rendition = rendition with { IsOverlined = false };
            else if (sgr is ['5', '8', .. string ul_color])
                rendition = rendition with { UnderlineColor = parse_color(ul_color) };
            else if (sgr is "59")
                rendition = rendition with { UnderlineColor = null };
            else if (sgr is "73")
                rendition = rendition with { TextTransformation = TextTransformationMode.Superscript };
            else if (sgr is "74")
                rendition = rendition with { TextTransformation = TextTransformationMode.Subscript };
            else if (sgr is "75")
                rendition = rendition with { TextTransformation = TextTransformationMode.Regular };
            else if (sgr is ['9', char fg_bcolor_index and >= '0' and <= '7'])
                rendition = rendition with { ForegroundColor = (ConsoleColor)(fg_bcolor_index - '0' + 7) };
            else if (sgr is ['1', '0', char bg_bcolor_index and >= '0' and <= '7'])
                rendition = rendition with { BackgroundColor = (ConsoleColor)(bg_bcolor_index - '0' + 7) };

        return rendition;

        Color parse_color(string vt100)
        {
            // TODO :  "2:..." or "5:..."

            throw new NotImplementedException();
        }
    }
}

/// <summary>
/// Represents the state of the console. This datatype can be used to restore the console to a previous state.
/// </summary>
public record ConsoleState
{
    public ConsoleMode STDINMode { get; init; }
    public ConsoleMode STDOUTMode { get; init; }
    public ConsoleMode STDERRMode { get; init; }
    public ConsoleGraphicRendition? GraphicRendition { get; init; }
    public Encoding? OutputEncoding { get; init; }
    public Encoding? InputEncoding { get; init; }
    [SupportedOSPlatform(OS.WIN)]
    public int? CursorSize { get; init; }
    public Size BufferSize { get; init; }
    public Size WindowSize { get; init; }
    public Point CursorPosition { get; init; }
    public Point WindowPosition { get; init; }

    // TODO : save console buffer / alternate buffer ?
}




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


    // TODO : implement saving SGRs and restoring them
    public static ConsoleState CurrentConsoleState
    {
        get => SaveConsoleState();
        set => RestoreConsoleState(value);
    }

    public static ConsoleGraphicRendition? CurrentGraphicRendition
    {
        get => GetRawVT100GraphicRenditions() is { } sgr ? new(sgr) : null;
        set
        {
            if (value?.FullVT100SGR() is { } sgr)
                sysconsole.Write($"\e[{string.Join(';', sgr)}m");
        }
    }



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



    public static bool SupportsVT100EscapeSequences => !OS.IsWindows || Environment.OSVersion.Version is { Major: >= 10, Build: >= 16257 };
#pragma warning disable CA1416 // Validate platform compatibility
    public static bool AreSTDInVT100EscapeSequencesEnabled => !OS.IsWindows || STDINConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);

    public static bool AreSTDOutVT100EscapeSequencesEnabled => !OS.IsWindows || STDOUTConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);

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

