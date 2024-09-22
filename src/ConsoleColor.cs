using System.Drawing;
using System.Linq;
using System;

using Unknown6656.Generics;

namespace Unknown6656.Console;


public enum ColorMode
{
    Foreground,
    Background,
    Underline,
    Any,
}

public readonly record struct ConsoleColor
{
    /// <summary>
    /// Represents the color that is used by default. This may vary between different console implementations, as well as <see cref="ColorMode"/>s.
    /// </summary>
    public static ConsoleColor Default { get; } = new();

    /// <summary>
    /// The color black.
    /// </summary>
    public static ConsoleColor Black { get; } = new(sysconsolecolor.Black);

    /// <summary>
    /// The color blue.
    /// </summary>
    public static ConsoleColor Blue { get; } = new(sysconsolecolor.Blue);

    /// <summary>
    /// The color cyan(blue-green).
    /// </summary>
    public static ConsoleColor Cyan { get; } = new(sysconsolecolor.Cyan);

    /// <summary>
    /// The color dark blue.
    /// </summary>
    public static ConsoleColor DarkBlue { get; } = new(sysconsolecolor.DarkBlue);

    /// <summary>
    /// The color dark cyan (dark blue-green).
    /// </summary>
    public static ConsoleColor DarkCyan { get; } = new(sysconsolecolor.DarkCyan);

    /// <summary>
    /// The color dark gray.
    /// </summary>
    public static ConsoleColor DarkGray { get; } = new(sysconsolecolor.DarkGray);

    /// <summary>
    /// The color dark green.
    /// </summary>
    public static ConsoleColor DarkGreen { get; } = new(sysconsolecolor.DarkGreen);

    /// <summary>
    /// The color dark magenta (dark purplish-red).
    /// </summary>
    public static ConsoleColor DarkMagenta { get; } = new(sysconsolecolor.DarkMagenta);

    /// <summary>
    /// The color dark red.
    /// </summary>
    public static ConsoleColor DarkRed { get; } = new(sysconsolecolor.DarkRed);

    /// <summary>
    /// The color dark yellow (ochre).
    /// </summary>
    public static ConsoleColor DarkYellow { get; } = new(sysconsolecolor.DarkYellow);

    /// <summary>
    /// The color gray.
    /// </summary>
    public static ConsoleColor Gray { get; } = new(sysconsolecolor.Gray);

    /// <summary>
    /// The color green.
    /// </summary>
    public static ConsoleColor Green { get; } = new(sysconsolecolor.Green);

    /// <summary>
    /// The color magenta (purplish-red).
    /// </summary>
    public static ConsoleColor Magenta { get; } = new(sysconsolecolor.Magenta);

    /// <summary>
    /// The color red.
    /// </summary>
    public static ConsoleColor Red { get; } = new(sysconsolecolor.Red);

    /// <summary>
    /// The color white.
    /// </summary>
    public static ConsoleColor White { get; } = new(sysconsolecolor.White);

    /// <summary>
    /// The color yellow.
    /// </summary>
    public static ConsoleColor Yellow { get; } = new(sysconsolecolor.Yellow);


    private readonly Union<sysconsolecolor, Color>? _color;


    public ConsoleColor()
        : this(null)
    {
    }

    public ConsoleColor(Color color) => _color = color;

    public ConsoleColor(KnownColor color)
        : this(Color.FromKnownColor(color))
    {
    }

    public ConsoleColor(sysconsolecolor? color) => _color = color is sysconsolecolor cc ? new Union<sysconsolecolor, Color>.Case0(cc) : null;

    public override string ToString() => _color is null ? "(Default)" : _color.Match(c => c.ToString(), rgb => $"#{rgb.ToArgb():x8}: {rgb}");

    public string ToVT520(ColorMode mode) => $"\e[{Console.GenerateVT520ColorString(this, mode)}m";

    public static ConsoleColor FromVT520(string vt520_color) => FromVT520(vt520_color, out _);

    public static ConsoleColor FromVT520(string vt520_color, out ColorMode mode)
    {
        if (vt520_color is ['\e', '[', .., 'm'])
            vt520_color = vt520_color[3..^1];

        (mode, ConsoleColor color) = vt520_color switch
        {
            "0" => (ColorMode.Any, Default),
            "30" => (ColorMode.Foreground, Black),
            "31" => (ColorMode.Foreground, DarkRed),
            "32" => (ColorMode.Foreground, DarkGreen),
            "33" => (ColorMode.Foreground, DarkYellow),
            "34" => (ColorMode.Foreground, DarkBlue),
            "35" => (ColorMode.Foreground, DarkMagenta),
            "36" => (ColorMode.Foreground, DarkCyan),
            "37" => (ColorMode.Foreground, Gray),
            ['3', '8', ':' or ';', '5', ':' or ';', ..string num] => (ColorMode.Foreground, From256ColorCode(byte.Parse(num))),
            ['3', '8', ':' or ';', '2', ':' or ';', ..string rgb] => (ColorMode.Foreground, parse_rgb(rgb)),
            "39" => (ColorMode.Foreground, Default),
            "40" => (ColorMode.Background, Black),
            "41" => (ColorMode.Background, DarkRed),
            "42" => (ColorMode.Background, DarkGreen),
            "43" => (ColorMode.Background, DarkYellow),
            "44" => (ColorMode.Background, DarkBlue),
            "45" => (ColorMode.Background, DarkMagenta),
            "46" => (ColorMode.Background, DarkCyan),
            "47" => (ColorMode.Background, Gray),
            ['4', '8', ':' or ';', '5', .. string num] => (ColorMode.Foreground, From256ColorCode(byte.Parse(num))),
            ['4', '8', ':' or ';', '2', ':' or ';', .. string rgb] => (ColorMode.Foreground, parse_rgb(rgb)),
            "49" => (ColorMode.Background, Default),
            "90" => (ColorMode.Foreground, DarkGray),
            "91" => (ColorMode.Foreground, Red),
            "92" => (ColorMode.Foreground, Green),
            "93" => (ColorMode.Foreground, Yellow),
            "94" => (ColorMode.Foreground, Blue),
            "95" => (ColorMode.Foreground, Magenta),
            "96" => (ColorMode.Foreground, Cyan),
            "97" => (ColorMode.Foreground, White),
            "100" => (ColorMode.Background, DarkGray),
            "101" => (ColorMode.Background, Red),
            "102" => (ColorMode.Background, Green),
            "103" => (ColorMode.Background, Yellow),
            "104" => (ColorMode.Background, Blue),
            "105" => (ColorMode.Background, Magenta),
            "106" => (ColorMode.Background, Cyan),
            "107" => (ColorMode.Background, White),
            _ => throw new ArgumentOutOfRangeException(nameof(vt520_color), vt520_color, "Invalid VT520 color code."),
        };

        return color;

        static ConsoleColor parse_rgb(string color)
        {
            byte[] rgb = color.Replace(';', ':').Split(':').ToArray(byte.Parse);

        }
    }

    public static ConsoleColor From256ColorCode(byte color_code) => throw new NotImplementedException();

    public static ConsoleColor FromColor(Color color) => new(color);

    public static ConsoleColor FromKnownColor(KnownColor color) => new(color);

    public static ConsoleColor FromConsoleColor(sysconsolecolor? color) => new(color);

    public static implicit operator ConsoleColor(Color color) => FromColor(color);

    public static implicit operator ConsoleColor(KnownColor color) => FromKnownColor(color);

    public static implicit operator ConsoleColor(sysconsolecolor? color) => FromConsoleColor(color);
}
