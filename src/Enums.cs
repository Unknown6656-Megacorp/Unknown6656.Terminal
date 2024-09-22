using System.Collections.Generic;
using System.Runtime.Versioning;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Linq;
using System;

using Unknown6656.Generics;
using Unknown6656.Runtime;

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

    /// <summary>
    /// Creates a bounding <see cref="ConsoleArea"/> that encompasses all the specified areas.
    /// </summary>
    /// <param name="areas">The collection of <see cref="ConsoleArea"/> to create a bounding area for.</param>
    /// <returns>A <see cref="ConsoleArea"/> that represents the bounding area of the specified areas.</returns>
    public static ConsoleArea CreateBoundingArea(IEnumerable<ConsoleArea>? areas)
    {
        if (areas?.Count() is null or 0)
            return Empty;

        ConsoleArea first = areas.First();
        int x = first.X;
        int y = first.Y;
        int right = first.Right;
        int bottom = first.Bottom;

        foreach (ConsoleArea area in areas.Skip(1))
        {
            x = Math.Min(x, area.X);
            y = Math.Min(y, area.Y);
            right = Math.Max(right, area.Right);
            bottom = Math.Max(bottom, area.Bottom);
        }

        return new ConsoleArea(x, y, right - x, bottom - y);
    }

    /// <summary>
    /// Creates a bounding <see cref="ConsoleArea"/> that encompasses all the specified points.
    /// </summary>
    /// <param name="points">The collection of <see cref="Point"/> to create a bounding area for.</param>
    /// <returns>A <see cref="ConsoleArea"/> that represents the bounding area of the specified points.</returns>
    public static ConsoleArea CreateBoundingArea(IEnumerable<Point>? points) => CreateBoundingArea(points?.Select(p => (p.X, p.Y)));

    /// <summary>
    /// Creates a bounding <see cref="ConsoleArea"/> that encompasses all the specified points.
    /// </summary>
    /// <param name="points">The collection of points to create a bounding area for.</param>
    /// <returns>A <see cref="ConsoleArea"/> that represents the bounding area of the specified points.</returns>
    public static ConsoleArea CreateBoundingArea(IEnumerable<(int X, int Y)>? points)
    {
        if (points?.Count() is null or 0)
            return Empty;

        int x = int.MaxValue;
        int y = int.MaxValue;
        int right = int.MinValue;
        int bottom = int.MinValue;

        foreach ((int X, int Y) point in points)
        {
            x = Math.Min(x, point.X);
            y = Math.Min(y, point.Y);
            right = Math.Max(right, point.X);
            bottom = Math.Max(bottom, point.Y);
        }

        return new ConsoleArea(x, y, right - x, bottom - y);
    }


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
/// <param name="RawVT520SGRs">The raw VT520 SGR (Select Graphic Rendition) sequences.</param>
public record ConsoleGraphicRendition(string[] RawVT520SGRs)
{
    /// <summary>
    /// Gets the default console graphic rendition (<c>^[0m</c>).
    /// </summary>
    public static ConsoleGraphicRendition Default { get; } = new(["0"]);


    /// <summary>
    /// The intensity of the text (regular, bold, dim).
    /// A value of <see langword="null"/> indicates the default value (<see cref="TextIntensityMode.Regular"/>).
    /// </summary>
    public TextIntensityMode? Intensity { get; init; }

    /// <summary>
    /// The blink mode of the text (none, slow, rapid).
    /// A value of <see langword="null"/> indicates the default value (<see cref="TextBlinkMode.NotBlinking"/>).
    /// </summary>
    public TextBlinkMode? Blink { get; init; }

    /// <summary>
    /// The underline mode of the text (not underlined, single, double).
    /// A value of <see langword="null"/> indicates the default value (<see cref="TextUnderlinedMode.Single"/>).
    /// </summary>
    public TextUnderlinedMode? Underlined { get; init; }

    /// <summary>
    /// Indicates whether the text colors are inverted.
    /// A value of <see langword="null"/> indicates the default value (<see langword="false"/>);
    /// </summary>
    public bool? AreColorsInverted { get; init; }

    /// <summary>
    /// Indicates whether the text is italic.
    /// A value of <see langword="null"/> indicates the default value (<see langword="false"/>);
    /// </summary>
    public bool? IsItalic { get; init; }

    /// <summary>
    /// Indicates whether the text is concealed.
    /// A value of <see langword="null"/> indicates the default value (<see langword="false"/>);
    /// </summary>
    public bool? IsTextConcealed { get; init; }

    /// <summary>
    /// Indicates whether the text is crossed out.
    /// A value of <see langword="null"/> indicates the default value (<see langword="false"/>);
    /// </summary>
    public bool? IsCrossedOut { get; init; }

    /// <summary>
    /// Indicates whether the text is overlined.
    /// A value of <see langword="null"/> indicates the default value (<see langword="false"/>);
    /// </summary>
    public bool? IsOverlined { get; init; }

    /// <summary>
    /// Indicates whether the font is the default font (i.e., whether the <see cref="FontIndex"/> is equal to <c>0</c>).
    /// </summary>
    public bool IsDefaultFont => FontIndex is null or 0;

    /// <summary>
    /// The index of the font to use.
    /// A value of <see langword="null"/> indicates the default value (<c>0</c>);
    /// </summary>
    public int? FontIndex { get; init; }

    /// <summary>
    /// Indicates whether the font is monospace.
    /// A value of <see langword="null"/> indicates the default value (<see langword="true"/>);
    /// </summary>
    public bool? IsMonospace { get; init; }

    /// <summary>
    /// Indicates whether the font is gothic.
    /// A value of <see langword="null"/> indicates the default value (<see langword="false"/>);
    /// </summary>
    public bool? IsGothic { get; init; }

    /// <summary>
    /// The text frame mode.
    /// A value of <see langword="null"/> indicates the default value (<see cref="TextFrameMode.NotFramed"/>).
    /// </summary>
    public TextFrameMode? TextFrame { get; init; }

    /// <summary>
    /// The text transformation mode (regular, superscript, subscript).
    /// A value of <see langword="null"/> indicates the default value (<see cref="TextTransformationMode.Regular"/>).
    /// </summary>
    public TextTransformationMode? TextTransformation { get; init; }

    /// <summary>
    /// The foreground color of the text. A value of <see langword="null"/> indicates the default color.
    /// </summary>
    public ConsoleColor? ForegroundColor { get; init; }

    /// <summary>
    /// The background color of the text. A value of <see langword="null"/> indicates the default color.
    /// </summary>
    public ConsoleColor? BackgroundColor { get; init; }

    /// <summary>
    /// The color of the underline. A value of <see langword="null"/> indicates that the <see cref="UnderlineColor"/> is identical to the <see cref="ForegroundColor"/>.
    /// </summary>
    public ConsoleColor? UnderlineColor { get; init; }

    /// <summary>
    /// Indicates whether this console graphic rendition resets all previous SGRs.
    /// </summary>
    public bool ResetsAllPreviousSGRs => RawVT520SGRs.Contains("0");


    /// <summary>
    /// Returns this console graphic rendition as a VT520 SGR (Select Graphic Rendition) escape sequence.
    /// </summary>
    /// <returns>The VT520 SGR escape sequence.</returns>
    public override string ToString() => $"\e[{FullVT520SGR().StringJoin(";")}m";

    /// <summary>
    /// Returns the full VT520 SGR (Select Graphic Rendition) sequences for this console graphic rendition.
    /// </summary>
    /// <returns>An array of VT520 SGR sequences.</returns>
    public string[] FullVT520SGR()
    {
        List<string> modes = [.. RawVT520SGRs];

        void add_mode<T>(T? value) where T : struct, Enum
        {
            if (value is { } mode)
                modes.Add(((int)(object)mode).ToString(CultureInfo.InvariantCulture));
        }

        void add_flag(bool? flag, string hi, string lo)
        {
            if (flag is { } f)
                modes.Add(f ? hi : lo);
        }


        if (modes.LastIndexOf("0") is int reset and > 0)
            modes.RemoveRange(0, reset);

        add_mode(Intensity);
        add_mode(Blink);
        add_mode(Underlined);
        add_flag(AreColorsInverted, "7", "27");
        add_flag(IsItalic, "3", "23");
        add_flag(IsTextConcealed, "8", "28");
        add_flag(IsCrossedOut, "9", "29");
        add_flag(IsOverlined, "53", "55");

        if (FontIndex is { } fi and not 0)
            modes.Add($"1{fi}");

        add_flag(IsGothic, "20", "23");
        add_flag(IsMonospace, "50", "26");
        add_mode(TextFrame);
        add_mode(TextTransformation);

        if (ForegroundColor is { } fg)
            modes.Add(fg.ToVT520(ColorMode.Foreground));

        if (ForegroundColor is { } bg)
            modes.Add(bg.ToVT520(ColorMode.Background));

        if (UnderlineColor is { } ul)
            modes.Add(ul.ToVT520(ColorMode.Underline));

        return modes.Distinct().ToArray();
    }

    /// <summary>
    /// Parses the specified VT520 SGR (Select Graphic Rendition) sequences and returns the corresponding console graphic rendition.
    /// </summary>
    /// <param name="SGRs">Sequence of VT520 SGRs.</param>
    /// <returns>Parsed <see cref="ConsoleGraphicRendition"/>.</returns>
    public static ConsoleGraphicRendition TryParse(string[] SGRs)
    {
        ConsoleGraphicRendition rendition = new(SGRs);

        foreach (string sgr in SGRs)
            if (sgr is "0")
                rendition = Default with { RawVT520SGRs = SGRs };
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
            else if (sgr is ['3' or '4' or '9', _] or ['1', '0', _] or "59" or ['5', '8', _, ..])
            {
                ConsoleColor color = ConsoleColor.FromVT520(sgr, out ColorMode mode);

                if (mode.HasFlag(ColorMode.Foreground))
                    rendition = rendition with { ForegroundColor = color };

                if (mode.HasFlag(ColorMode.Background))
                    rendition = rendition with { BackgroundColor = color };

                if (mode.HasFlag(ColorMode.Underline))
                    rendition = rendition with { UnderlineColor = color };
            }
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
            else if (sgr is "73")
                rendition = rendition with { TextTransformation = TextTransformationMode.Superscript };
            else if (sgr is "74")
                rendition = rendition with { TextTransformation = TextTransformationMode.Subscript };
            else if (sgr is "75")
                rendition = rendition with { TextTransformation = TextTransformationMode.Regular };

        return rendition;
    }
}

/// <summary>
/// Represents the state of the console. This datatype can be used to restore the console to a previous state.
/// </summary>
public record ConsoleState
{
    /// <summary>
    /// Gets or sets the console mode of the standard input stream.
    /// </summary>
    public ConsoleMode STDINMode { get; init; }

    /// <summary>
    /// Gets or sets the console mode of the standard output stream.
    /// </summary>
    public ConsoleMode STDOUTMode { get; init; }

    /// <summary>
    /// Gets or sets the console mode of the standard error stream.
    /// </summary>
    public ConsoleMode STDERRMode { get; init; }

    /// <summary>
    /// Gets or sets the graphic rendition of the console.
    /// </summary>
    public ConsoleGraphicRendition? GraphicRendition { get; init; }

    /// <summary>
    /// Gets or sets the output encoding of the console.
    /// </summary>
    public Encoding? OutputEncoding { get; init; }

    /// <summary>
    /// Gets or sets the input encoding of the console.
    /// </summary>
    public Encoding? InputEncoding { get; init; }

    /// <summary>
    /// Gets or sets the size of the console cursor.
    /// </summary>
    [SupportedOSPlatform(OS.WIN)]
    public int? CursorSize { get; init; }

    /// <summary>
    /// Gets or sets the buffer size of the console.
    /// </summary>
    public Size BufferSize { get; init; }

    /// <summary>
    /// Gets or sets the window size of the console.
    /// </summary>
    public Size WindowSize { get; init; }

    /// <summary>
    /// Gets or sets the cursor position of the console.
    /// </summary>
    public Point CursorPosition { get; init; }

    /// <summary>
    /// Gets or sets the window position of the console.
    /// </summary>
    public Point WindowPosition { get; init; }

    public ConsoleColor? WindowFrameForeground { get; init; }

    public ConsoleColor? WindowFrameBackground { get; init; }

    // TODO : save console buffer / alternate buffer ?
}
