using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;

using Unknown6656.Generics;

namespace Unknown6656.Console;


/// <summary>
/// Specifies the color mode for the rendering of <see cref="ConsoleColor"/>.
/// </summary>
[Flags]
public enum ColorMode
    : byte
{
    // None = 0,

    /// <summary>
    /// Specifies the foreground color.
    /// </summary>
    Foreground = 1,
    /// <summary>
    /// Specifies the background color.
    /// </summary>
    Background = 2,
    /// <summary>
    /// Specifies the underline color.
    /// </summary>
    Underline = 4,
    /// <summary>
    /// Specifies any color mode.
    /// </summary>
    Any = Foreground | Background | Underline,
}

/// <summary>
/// Represents a console color that can be a system color (<see cref="sysconsolecolor"/>) or an RGB color (<see cref="Color"/>).
/// </summary>
public readonly record struct ConsoleColor
    : IFormattable
{
    private static readonly Dictionary<byte, ConsoleColor> _256colorLUT = new()
    {
        [0] = Black,
        [1] = DarkRed,
        [2] = DarkGreen,
        [3] = DarkYellow,
        [4] = DarkBlue,
        [5] = DarkMagenta,
        [6] = DarkCyan,
        [7] = Gray,
        [8] = DarkGray,
        [9] = Red,
        [10] = Green,
        [11] = Yellow,
        [12] = Blue,
        [13] = Magenta,
        [14] = Cyan,
        [15] = White,
        [16] = Color.FromArgb(0, 0, 0), // #000000
        [17] = Color.FromArgb(0, 0, 95), // #00005f
        [18] = Color.FromArgb(0, 0, 135), // #000087
        [19] = Color.FromArgb(0, 0, 175), // #0000af
        [20] = Color.FromArgb(0, 0, 215), // #0000d7
        [21] = Color.FromArgb(0, 0, 255), // #0000ff
        [22] = Color.FromArgb(0, 95, 0), // #005f00
        [23] = Color.FromArgb(0, 95, 95), // #005f5f
        [24] = Color.FromArgb(0, 95, 135), // #005f87
        [25] = Color.FromArgb(0, 95, 175), // #005faf
        [26] = Color.FromArgb(0, 95, 215), // #005fd7
        [27] = Color.FromArgb(0, 95, 255), // #005fff
        [28] = Color.FromArgb(0, 135, 0), // #008700
        [29] = Color.FromArgb(0, 135, 95), // #00875f
        [30] = Color.FromArgb(0, 135, 135), // #008787
        [31] = Color.FromArgb(0, 135, 175), // #0087af
        [32] = Color.FromArgb(0, 135, 215), // #0087d7
        [33] = Color.FromArgb(0, 135, 255), // #0087ff
        [34] = Color.FromArgb(0, 175, 0), // #00af00
        [35] = Color.FromArgb(0, 175, 95), // #00af5f
        [36] = Color.FromArgb(0, 175, 135), // #00af87
        [37] = Color.FromArgb(0, 175, 175), // #00afaf
        [38] = Color.FromArgb(0, 175, 215), // #00afd7
        [39] = Color.FromArgb(0, 175, 255), // #00afff
        [40] = Color.FromArgb(0, 215, 0), // #00d700
        [41] = Color.FromArgb(0, 215, 95), // #00d75f
        [42] = Color.FromArgb(0, 215, 135), // #00d787
        [43] = Color.FromArgb(0, 215, 175), // #00d7af
        [44] = Color.FromArgb(0, 215, 215), // #00d7d7
        [45] = Color.FromArgb(0, 215, 255), // #00d7ff
        [46] = Color.FromArgb(0, 255, 0), // #00ff00
        [47] = Color.FromArgb(0, 255, 95), // #00ff5f
        [48] = Color.FromArgb(0, 255, 135), // #00ff87
        [49] = Color.FromArgb(0, 255, 175), // #00ffaf
        [50] = Color.FromArgb(0, 255, 215), // #00ffd7
        [51] = Color.FromArgb(0, 255, 255), // #00ffff
        [52] = Color.FromArgb(95, 0, 0), // #5f0000
        [53] = Color.FromArgb(95, 0, 95), // #5f005f
        [54] = Color.FromArgb(95, 0, 135), // #5f0087
        [55] = Color.FromArgb(95, 0, 175), // #5f00af
        [56] = Color.FromArgb(95, 0, 215), // #5f00d7
        [57] = Color.FromArgb(95, 0, 255), // #5f00ff
        [58] = Color.FromArgb(95, 95, 0), // #5f5f00
        [59] = Color.FromArgb(95, 95, 95), // #5f5f5f
        [60] = Color.FromArgb(95, 95, 135), // #5f5f87
        [61] = Color.FromArgb(95, 95, 175), // #5f5faf
        [62] = Color.FromArgb(95, 95, 215), // #5f5fd7
        [63] = Color.FromArgb(95, 95, 255), // #5f5fff
        [64] = Color.FromArgb(95, 135, 0), // #5f8700
        [65] = Color.FromArgb(95, 135, 95), // #5f875f
        [66] = Color.FromArgb(95, 135, 135), // #5f8787
        [67] = Color.FromArgb(95, 135, 175), // #5f87af
        [68] = Color.FromArgb(95, 135, 215), // #5f87d7
        [69] = Color.FromArgb(95, 135, 255), // #5f87ff
        [70] = Color.FromArgb(95, 175, 0), // #5faf00
        [71] = Color.FromArgb(95, 175, 95), // #5faf5f
        [72] = Color.FromArgb(95, 175, 135), // #5faf87
        [73] = Color.FromArgb(95, 175, 175), // #5fafaf
        [74] = Color.FromArgb(95, 175, 215), // #5fafd7
        [75] = Color.FromArgb(95, 175, 255), // #5fafff
        [76] = Color.FromArgb(95, 215, 0), // #5fd700
        [77] = Color.FromArgb(95, 215, 95), // #5fd75f
        [78] = Color.FromArgb(95, 215, 135), // #5fd787
        [79] = Color.FromArgb(95, 215, 175), // #5fd7af
        [80] = Color.FromArgb(95, 215, 215), // #5fd7d7
        [81] = Color.FromArgb(95, 215, 255), // #5fd7ff
        [82] = Color.FromArgb(95, 255, 0), // #5fff00
        [83] = Color.FromArgb(95, 255, 95), // #5fff5f
        [84] = Color.FromArgb(95, 255, 135), // #5fff87
        [85] = Color.FromArgb(95, 255, 175), // #5fffaf
        [86] = Color.FromArgb(95, 255, 215), // #5fffd7
        [87] = Color.FromArgb(95, 255, 255), // #5fffff
        [88] = Color.FromArgb(135, 0, 0), // #870000
        [89] = Color.FromArgb(135, 0, 95), // #87005f
        [90] = Color.FromArgb(135, 0, 135), // #870087
        [91] = Color.FromArgb(135, 0, 175), // #8700af
        [92] = Color.FromArgb(135, 0, 215), // #8700d7
        [93] = Color.FromArgb(135, 0, 255), // #8700ff
        [94] = Color.FromArgb(135, 95, 0), // #875f00
        [95] = Color.FromArgb(135, 95, 95), // #875f5f
        [96] = Color.FromArgb(135, 95, 135), // #875f87
        [97] = Color.FromArgb(135, 95, 175), // #875faf
        [98] = Color.FromArgb(135, 95, 215), // #875fd7
        [99] = Color.FromArgb(135, 95, 255), // #875fff
        [100] = Color.FromArgb(135, 135, 0), // #878700
        [101] = Color.FromArgb(135, 135, 95), // #87875f
        [102] = Color.FromArgb(135, 135, 135), // #878787
        [103] = Color.FromArgb(135, 135, 175), // #8787af
        [104] = Color.FromArgb(135, 135, 215), // #8787d7
        [105] = Color.FromArgb(135, 135, 255), // #8787ff
        [106] = Color.FromArgb(135, 175, 0), // #87af00
        [107] = Color.FromArgb(135, 175, 95), // #87af5f
        [108] = Color.FromArgb(135, 175, 135), // #87af87
        [109] = Color.FromArgb(135, 175, 175), // #87afaf
        [110] = Color.FromArgb(135, 175, 215), // #87afd7
        [111] = Color.FromArgb(135, 175, 255), // #87afff
        [112] = Color.FromArgb(135, 215, 0), // #87d700
        [113] = Color.FromArgb(135, 215, 95), // #87d75f
        [114] = Color.FromArgb(135, 215, 135), // #87d787
        [115] = Color.FromArgb(135, 215, 175), // #87d7af
        [116] = Color.FromArgb(135, 215, 215), // #87d7d7
        [117] = Color.FromArgb(135, 215, 255), // #87d7ff
        [118] = Color.FromArgb(135, 255, 0), // #87ff00
        [119] = Color.FromArgb(135, 255, 95), // #87ff5f
        [120] = Color.FromArgb(135, 255, 135), // #87ff87
        [121] = Color.FromArgb(135, 255, 175), // #87ffaf
        [122] = Color.FromArgb(135, 255, 215), // #87ffd7
        [123] = Color.FromArgb(135, 255, 255), // #87ffff
        [124] = Color.FromArgb(175, 0, 0), // #af0000
        [125] = Color.FromArgb(175, 0, 95), // #af005f
        [126] = Color.FromArgb(175, 0, 135), // #af0087
        [127] = Color.FromArgb(175, 0, 175), // #af00af
        [128] = Color.FromArgb(175, 0, 215), // #af00d7
        [129] = Color.FromArgb(175, 0, 255), // #af00ff
        [130] = Color.FromArgb(175, 95, 0), // #af5f00
        [131] = Color.FromArgb(175, 95, 95), // #af5f5f
        [132] = Color.FromArgb(175, 95, 135), // #af5f87
        [133] = Color.FromArgb(175, 95, 175), // #af5faf
        [134] = Color.FromArgb(175, 95, 215), // #af5fd7
        [135] = Color.FromArgb(175, 95, 255), // #af5fff
        [136] = Color.FromArgb(175, 135, 0), // #af8700
        [137] = Color.FromArgb(175, 135, 95), // #af875f
        [138] = Color.FromArgb(175, 135, 135), // #af8787
        [139] = Color.FromArgb(175, 135, 175), // #af87af
        [140] = Color.FromArgb(175, 135, 215), // #af87d7
        [141] = Color.FromArgb(175, 135, 255), // #af87ff
        [142] = Color.FromArgb(175, 175, 0), // #afaf00
        [143] = Color.FromArgb(175, 175, 95), // #afaf5f
        [144] = Color.FromArgb(175, 175, 135), // #afaf87
        [145] = Color.FromArgb(175, 175, 175), // #afafaf
        [146] = Color.FromArgb(175, 175, 215), // #afafd7
        [147] = Color.FromArgb(175, 175, 255), // #afafff
        [148] = Color.FromArgb(175, 215, 0), // #afd700
        [149] = Color.FromArgb(175, 215, 95), // #afd75f
        [150] = Color.FromArgb(175, 215, 135), // #afd787
        [151] = Color.FromArgb(175, 215, 175), // #afd7af
        [152] = Color.FromArgb(175, 215, 215), // #afd7d7
        [153] = Color.FromArgb(175, 215, 255), // #afd7ff
        [154] = Color.FromArgb(175, 255, 0), // #afff00
        [155] = Color.FromArgb(175, 255, 95), // #afff5f
        [156] = Color.FromArgb(175, 255, 135), // #afff87
        [157] = Color.FromArgb(175, 255, 175), // #afffaf
        [158] = Color.FromArgb(175, 255, 215), // #afffd7
        [159] = Color.FromArgb(175, 255, 255), // #afffff
        [160] = Color.FromArgb(215, 0, 0), // #d70000
        [161] = Color.FromArgb(215, 0, 95), // #d7005f
        [162] = Color.FromArgb(215, 0, 135), // #d70087
        [163] = Color.FromArgb(215, 0, 175), // #d700af
        [164] = Color.FromArgb(215, 0, 215), // #d700d7
        [165] = Color.FromArgb(215, 0, 255), // #d700ff
        [166] = Color.FromArgb(215, 95, 0), // #d75f00
        [167] = Color.FromArgb(215, 95, 95), // #d75f5f
        [168] = Color.FromArgb(215, 95, 135), // #d75f87
        [169] = Color.FromArgb(215, 95, 175), // #d75faf
        [170] = Color.FromArgb(215, 95, 215), // #d75fd7
        [171] = Color.FromArgb(215, 95, 255), // #d75fff
        [172] = Color.FromArgb(215, 135, 0), // #d78700
        [173] = Color.FromArgb(215, 135, 95), // #d7875f
        [174] = Color.FromArgb(215, 135, 135), // #d78787
        [175] = Color.FromArgb(215, 135, 175), // #d787af
        [176] = Color.FromArgb(215, 135, 215), // #d787d7
        [177] = Color.FromArgb(215, 135, 255), // #d787ff
        [178] = Color.FromArgb(215, 175, 0), // #d7af00
        [179] = Color.FromArgb(215, 175, 95), // #d7af5f
        [180] = Color.FromArgb(215, 175, 135), // #d7af87
        [181] = Color.FromArgb(215, 175, 175), // #d7afaf
        [182] = Color.FromArgb(215, 175, 215), // #d7afd7
        [183] = Color.FromArgb(215, 175, 255), // #d7afff
        [184] = Color.FromArgb(215, 215, 0), // #d7d700
        [185] = Color.FromArgb(215, 215, 95), // #d7d75f
        [186] = Color.FromArgb(215, 215, 135), // #d7d787
        [187] = Color.FromArgb(215, 215, 175), // #d7d7af
        [188] = Color.FromArgb(215, 215, 215), // #d7d7d7
        [189] = Color.FromArgb(215, 215, 255), // #d7d7ff
        [190] = Color.FromArgb(215, 255, 0), // #d7ff00
        [191] = Color.FromArgb(215, 255, 95), // #d7ff5f
        [192] = Color.FromArgb(215, 255, 135), // #d7ff87
        [193] = Color.FromArgb(215, 255, 175), // #d7ffaf
        [194] = Color.FromArgb(215, 255, 215), // #d7ffd7
        [195] = Color.FromArgb(215, 255, 255), // #d7ffff
        [196] = Color.FromArgb(255, 0, 0), // #ff0000
        [197] = Color.FromArgb(255, 0, 95), // #ff005f
        [198] = Color.FromArgb(255, 0, 135), // #ff0087
        [199] = Color.FromArgb(255, 0, 175), // #ff00af
        [200] = Color.FromArgb(255, 0, 215), // #ff00d7
        [201] = Color.FromArgb(255, 0, 255), // #ff00ff
        [202] = Color.FromArgb(255, 95, 0), // #ff5f00
        [203] = Color.FromArgb(255, 95, 95), // #ff5f5f
        [204] = Color.FromArgb(255, 95, 135), // #ff5f87
        [205] = Color.FromArgb(255, 95, 175), // #ff5faf
        [206] = Color.FromArgb(255, 95, 215), // #ff5fd7
        [207] = Color.FromArgb(255, 95, 255), // #ff5fff
        [208] = Color.FromArgb(255, 135, 0), // #ff8700
        [209] = Color.FromArgb(255, 135, 95), // #ff875f
        [210] = Color.FromArgb(255, 135, 135), // #ff8787
        [211] = Color.FromArgb(255, 135, 175), // #ff87af
        [212] = Color.FromArgb(255, 135, 215), // #ff87d7
        [213] = Color.FromArgb(255, 135, 255), // #ff87ff
        [214] = Color.FromArgb(255, 175, 0), // #ffaf00
        [215] = Color.FromArgb(255, 175, 95), // #ffaf5f
        [216] = Color.FromArgb(255, 175, 135), // #ffaf87
        [217] = Color.FromArgb(255, 175, 175), // #ffafaf
        [218] = Color.FromArgb(255, 175, 215), // #ffafd7
        [219] = Color.FromArgb(255, 175, 255), // #ffafff
        [220] = Color.FromArgb(255, 215, 0), // #ffd700
        [221] = Color.FromArgb(255, 215, 95), // #ffd75f
        [222] = Color.FromArgb(255, 215, 135), // #ffd787
        [223] = Color.FromArgb(255, 215, 175), // #ffd7af
        [224] = Color.FromArgb(255, 215, 215), // #ffd7d7
        [225] = Color.FromArgb(255, 215, 255), // #ffd7ff
        [226] = Color.FromArgb(255, 255, 0), // #ffff00
        [227] = Color.FromArgb(255, 255, 95), // #ffff5f
        [228] = Color.FromArgb(255, 255, 135), // #ffff87
        [229] = Color.FromArgb(255, 255, 175), // #ffffaf
        [230] = Color.FromArgb(255, 255, 215), // #ffffd7
        [231] = Color.FromArgb(255, 255, 255), // #ffffff
        [232] = Color.FromArgb(8, 8, 8), // #080808
        [233] = Color.FromArgb(18, 18, 18), // #121212
        [234] = Color.FromArgb(28, 28, 28), // #1c1c1c
        [235] = Color.FromArgb(38, 38, 38), // #262626
        [236] = Color.FromArgb(48, 48, 48), // #303030
        [237] = Color.FromArgb(58, 58, 58), // #3a3a3a
        [238] = Color.FromArgb(68, 68, 68), // #444444
        [239] = Color.FromArgb(78, 78, 78), // #4e4e4e
        [240] = Color.FromArgb(88, 88, 88), // #585858
        [241] = Color.FromArgb(98, 98, 98), // #626262
        [242] = Color.FromArgb(108, 108, 108), // #6c6c6c
        [243] = Color.FromArgb(118, 118, 118), // #767676
        [244] = Color.FromArgb(128, 128, 128), // #808080
        [245] = Color.FromArgb(138, 138, 138), // #8a8a8a
        [246] = Color.FromArgb(148, 148, 148), // #949494
        [247] = Color.FromArgb(158, 158, 158), // #9e9e9e
        [248] = Color.FromArgb(168, 168, 168), // #a8a8a8
        [249] = Color.FromArgb(178, 178, 178), // #b2b2b2
        [250] = Color.FromArgb(188, 188, 188), // #bcbcbc
        [251] = Color.FromArgb(198, 198, 198), // #c6c6c6
        [252] = Color.FromArgb(208, 208, 208), // #d0d0d0
        [253] = Color.FromArgb(218, 218, 218), // #dadada
        [254] = Color.FromArgb(228, 228, 228), // #e4e4e4
        [255] = Color.FromArgb(238, 238, 238), // #eeeeee
    };
    private readonly Union<sysconsolecolor, Color>? _color;


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

    /// <summary>
    /// Indicates whether the current <see cref="ConsoleColor"/> is the default color, i.e., whether it is equal to <see cref="Default"/> or <see langword="null"/>.
    /// </summary>
    public bool IsDefault => _color is null;

    /// <summary>
    /// Indicates whether the current <see cref="ConsoleColor"/> is a RGB color, i.e., whether it is an instance of <see cref="Color"/>.
    /// </summary>
    public bool IsRGB => _color?.IsCase1 ?? false;

    /// <summary>
    /// Indicates whether the current <see cref="ConsoleColor"/> is a system color, i.e., whether it is an instance of <see cref="sysconsolecolor"/>.
    /// </summary>
    public bool IsSystemColor => _color?.IsCase0 ?? false;


    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the default color.
    /// </summary>
    public ConsoleColor()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the specified <see cref="Color"/>.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to initialize the <see cref="ConsoleColor"/> with.</param>
    public ConsoleColor(Color color) => _color = color;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the specified <see cref="KnownColor"/>.
    /// </summary>
    /// <param name="color">The <see cref="KnownColor"/> to initialize the <see cref="ConsoleColor"/> with.</param>
    public ConsoleColor(KnownColor color)
        : this(Color.FromKnownColor(color))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the specified <see cref="sysconsolecolor"/>.
    /// </summary>
    /// <param name="color">The <see cref="sysconsolecolor"/> to initialize the <see cref="ConsoleColor"/> with.</param>
    public ConsoleColor(sysconsolecolor? color) => _color = color is sysconsolecolor cc ? new Union<sysconsolecolor, Color>.Case0(cc) : null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the specified gray value.
    /// This value is clamped to the range of <c>[0..1]</c>.
    /// </summary>
    /// <param name="gray">The gray value in the range of <c>[0..1]</c> to initialize the <see cref="ConsoleColor"/> with.</param>
    public ConsoleColor(double gray)
        : this(gray, gray, gray)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the specified RGB values.
    /// Each value is clamped to the range of <c>[0..1]</c>.
    /// </summary>
    /// <param name="R">The red component value in the range of <c>[0..1]</c>.</param>
    /// <param name="G">The green component value in the range of <c>[0..1]</c>.</param>
    /// <param name="B">The blue component value in the range of <c>[0..1]</c>.</param>
    public ConsoleColor(double R, double G, double B)
        : this(
              (byte)Math.Round(double.Clamp(R, 0, 1) * 255),
              (byte)Math.Round(double.Clamp(G, 0, 1) * 255),
              (byte)Math.Round(double.Clamp(B, 0, 1) * 255)
        )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the specified gray value.
    /// This value is expected to be inside the range of <c>[0..255]</c>.
    /// </summary>
    /// <param name="gray">The gray value to initialize the <see cref="ConsoleColor"/> with.</param>
    public ConsoleColor(byte gray)
        : this(gray, gray, gray)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleColor"/> struct with the specified RGB values.
    /// Each value is expected to be inside the range of <c>[0..255]</c>.
    /// </summary>
    /// <param name="R">The red component value.</param>
    /// <param name="G">The green component value.</param>
    /// <param name="B">The blue component value.</param>
    public ConsoleColor(byte R, byte G, byte B)
        : this(Color.FromArgb(R, G, B))
    {
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (_color is null)
            return 0;
        else if (_color.Is(out sysconsolecolor color))
            return color.GetHashCode();
        else if (_color.Is(out Color rgb))
            return rgb.GetHashCode();
        else
            throw new InvalidOperationException($"Invalid color type '{_color}'.");
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="ConsoleColor"/>.
    /// Please note that this is <b>NOT</b> a VT520 escape sequence. Use the function <see cref="ToVT520(ColorMode)"/> for generating VT520 escape sequences.
    /// </summary>
    /// <returns>A string that represents the current <see cref="ConsoleColor"/>.</returns>
    public override string ToString() => _color is null ? "(Default)" : _color.Match(c => c.ToString(), rgb => $"#{rgb.ToArgb():x8}: {rgb}");

    /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
    public string ToString(string? format) => ToString(format, null);

    /// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
    public string ToString(string? format, IFormatProvider? provider) => new string([.. format?.ToLower()?.Where(char.IsAsciiLetter) ?? []]) switch
    {
        "f" or "fore" or "fg" or "foreground" => ToVT520(ColorMode.Foreground),
        "b" or "back" or "bg" or "background" => ToVT520(ColorMode.Background),
        "u" or "ud" or "ul" or "underline" => ToVT520(ColorMode.Underline),
        _ => ToString(),
    };

    /// <summary>
    /// Converts the current <see cref="ConsoleColor"/> to a <see cref="sysconsolecolor"/> if possible.
    /// </summary>
    /// <returns>Returns the <see cref="sysconsolecolor"/> if the current <see cref="ConsoleColor"/> is a system color; otherwise, <see langword="null"/>.</returns>
    public sysconsolecolor? ToSystemColor()
    {
        if (_color?.Is(out sysconsolecolor color) ?? false)
            return color;
        else
            return null;
    }

    /// <summary>
    /// Converts the current <see cref="ConsoleColor"/> to a VT520 escape sequence string for the specified <see cref="ColorMode"/>.
    /// The generated VT520 escape sequence contains the SGR code for the current <see cref="ConsoleColor"/> and starts with <c>\e[</c> and ends with <c>m</c>.
    /// </summary>
    /// <param name="mode">The <see cref="ColorMode"/> to use for the conversion.</param>
    /// <returns>A VT520 escape sequence string that represents the current <see cref="ConsoleColor"/>.</returns>
    public string ToVT520(ColorMode mode) => GetVT520SGRCode(mode) is { Length: > 0 } sgr ? $"{Console._CSI}{sgr}m" : "";

    /// <summary>
    /// Generates a VT100/VT520/ANSI color string for the current <see cref="ConsoleColor"/> instance and given color mode flag (foreground, background, underline).
    /// <para/>
    /// Please do note that this string does <b>NOT</b> start with <c>\e[</c> and does <b>NOT</b> end with <c>m</c>.
    /// This has still to be done by the caller. The reason for this is that <see cref="GetVT520SGRCode"/> is intended to be used in combination with other VT520 escape sequences.
    /// </summary>
    /// <param name="mode">The color mode to use (foreground, background, underline).</param>
    /// <returns>The VT100/VT520/ANSI color string for the current color.</returns>
    public string GetVT520SGRCode(ColorMode mode)
    {
        if (mode is not (ColorMode.Foreground or ColorMode.Background or ColorMode.Underline))
            return Enumerable.Where([ColorMode.Foreground, ColorMode.Background, ColorMode.Underline], m => mode.HasFlag(m))
                             .Select(GetVT520SGRCode)
                             .StringJoin(";");

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
        if (_color is null)
            return mode switch
            {
                ColorMode.Foreground => "39",
                ColorMode.Background => "49",
                ColorMode.Underline => "59",
            };
        else if (_color.Is(out sysconsolecolor color))
        {
            (string bg, string fg) = color switch
            {
                sysconsolecolor.Black => ("40", "30"),
                sysconsolecolor.DarkBlue => ("44", "34"),
                sysconsolecolor.DarkGreen => ("42", "32"),
                sysconsolecolor.DarkCyan => ("46", "36"),
                sysconsolecolor.DarkRed => ("41", "31"),
                sysconsolecolor.DarkMagenta => ("45", "35"),
                sysconsolecolor.DarkYellow => ("43", "33"),
                sysconsolecolor.Gray => ("47", "37"),
                sysconsolecolor.DarkGray => ("100", "90"),
                sysconsolecolor.Blue => ("104", "94"),
                sysconsolecolor.Green => ("102", "92"),
                sysconsolecolor.Cyan => ("106", "96"),
                sysconsolecolor.Red => ("101", "91"),
                sysconsolecolor.Magenta => ("105", "95"),
                sysconsolecolor.Yellow => ("103", "93"),
                sysconsolecolor.White => ("107", "97"),
            };

            return mode switch
            {
                ColorMode.Foreground => fg,
                ColorMode.Background => bg,
            };
        }
        else if (_color.Is(out Color rgb))
            return $"{mode switch
            {
                ColorMode.Foreground => "38",
                ColorMode.Background => "48",
                ColorMode.Underline => "58",
            }};2;{rgb.R};{rgb.G};{rgb.B}";
        else
            throw new InvalidOperationException($"Invalid color type '{_color}'.");
#pragma warning restore CS8524
#pragma warning restore CS8509
    }

    /// <summary>
    /// Parses a VT100/VT500/VT520/ANSI escape sequence string to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="vt520_color">The VT100/VT500/VT520/ANSI escape sequence string to be parsed as a color.</param>
    /// <param name="color">Returns the <see cref="ConsoleColor"/> instance that represents the specified given color string (or <see cref="Default"/> if the parsing was unsuccessful).</param>
    /// <returns>Returns <see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string vt520_color, out ConsoleColor color)
    {
        try
        {
            color = FromVT520(vt520_color);

            return true;
        }
        catch
        {
            color = Default;

            return false;
        }
    }

    /// <summary>
    /// Converts a VT100/VT500/VT520/ANSI escape sequence string to a <see cref="ConsoleColor"/> instance.
    /// <para/>
    /// Note that the given escape sequence string is not required to start with <c>\e[</c> and end with <c>m</c>.
    /// </summary>
    /// <param name="vt520_color">The VT100/VT500/VT520/ANSI escape sequence string.</param>
    /// <returns>A <see cref="ConsoleColor"/> instance that represents the specified given color string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the VT520 color code is invalid.</exception>
    public static ConsoleColor FromVT520(string vt520_color) => FromVT520(vt520_color, out _);

    /// <summary>
    /// Converts a VT100/VT500/VT520/ANSI escape sequence string to a <see cref="ConsoleColor"/> instance and outputs the <see cref="ColorMode"/>.
    /// <para/>
    /// Note that the given escape sequence string is not required to start with <c>\e[</c> and end with <c>m</c>.
    /// </summary>
    /// <param name="vt520_color">The VT100/VT500/VT520/ANSI escape sequence string.</param>
    /// <param name="mode">The <see cref="ColorMode"/> that represents the color mode of the parsed color.</param>
    /// <returns>A <see cref="ConsoleColor"/> instance that represents the specified given color string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the VT520 color code is invalid.</exception>
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
            ['3', '8', ':' or ';', '5', ':' or ';', .. string num] => (ColorMode.Foreground, From256ColorCode(byte.Parse(num))),
            ['3', '8', ':' or ';', '2', ':' or ';', .. string rgb] => (ColorMode.Foreground, parse_rgb(rgb)),
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
            ['5', '8', ':' or ';', '5', .. string num] => (ColorMode.Underline, From256ColorCode(byte.Parse(num))),
            ['5', '8', ':' or ';', '2', ':' or ';', .. string rgb] => (ColorMode.Underline, parse_rgb(rgb)),
            "59" => (ColorMode.Underline, Default),
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

            return FromColor(Color.FromArgb(rgb[0], rgb[1], rgb[2]));
        }
    }

    /// <summary>
    /// Converts a VT100/VT500/VT520/ANSI 256-color code to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="color_code">The VT100/VT500/VT520/ANSI 256-color code.</param>
    /// <returns>A <see cref="ConsoleColor"/> instance that represents the specified 256-color code.</returns>
    public static ConsoleColor From256ColorCode(byte color_code) => _256colorLUT[color_code];

    /// <summary>
    /// Converts a <see cref="Color"/> to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to convert.</param>
    /// <returns>A <see cref="ConsoleColor"/> instance that represents the specified color.</returns>
    public static ConsoleColor FromColor(Color color) => new(color);

    /// <summary>
    /// Converts a <see cref="KnownColor"/> to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="KnownColor"/> to convert.</param>
    /// <returns>A <see cref="ConsoleColor"/> instance that represents the specified known color.</returns>
    public static ConsoleColor FromKnownColor(KnownColor color) => new(color);

    /// <summary>
    /// Converts a <see cref="sysconsolecolor"/> to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="sysconsolecolor"/> to convert.</param>
    /// <returns>A <see cref="ConsoleColor"/> instance that represents the specified console color.</returns>
    public static ConsoleColor FromConsoleColor(sysconsolecolor? color) => new(color);

    /// <summary>
    /// Implicitly converts a <see cref="Color"/> to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to convert.</param>
    public static implicit operator ConsoleColor(Color color) => FromColor(color);

    /// <summary>
    /// Implicitly converts a <see cref="KnownColor"/> to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="KnownColor"/> to convert.</param>
    public static implicit operator ConsoleColor(KnownColor color) => FromKnownColor(color);

    /// <summary>
    /// Implicitly converts a <see cref="sysconsolecolor"/> to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="sysconsolecolor"/> to convert.</param>
    public static implicit operator ConsoleColor(sysconsolecolor? color) => FromConsoleColor(color);

    /// <summary>
    /// Converts a VT520 color string to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="vt520_color">The VT520 color string.</param>
    public static explicit operator ConsoleColor(string vt520_color) => FromVT520(vt520_color);

    /// <summary>
    /// Converts an RGB tuple to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="rgb">The RGB tuple.</param>
    public static explicit operator ConsoleColor((byte r, byte g, byte b) rgb) => new(rgb.r, rgb.g, rgb.b);

    /// <summary>
    /// Converts an RGB tuple to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="rgb">The RGB tuple.</param>
    public static explicit operator ConsoleColor((int r, int g, int b) rgb) => new((byte)rgb.r, (byte)rgb.g, (byte)rgb.b);

    /// <summary>
    /// Converts an RGB tuple to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="rgb">The RGB tuple.</param>
    public static explicit operator ConsoleColor((float r, float g, float b) rgb) => new(rgb.r, rgb.g, rgb.b);

    /// <summary>
    /// Converts an RGB tuple to a <see cref="ConsoleColor"/> instance.
    /// </summary>
    /// <param name="rgb">The RGB tuple.</param>
    public static explicit operator ConsoleColor((double r, double g, double b) rgb) => new(rgb.r, rgb.g, rgb.b);

    /// <summary>
    /// Converts a <see cref="ConsoleColor"/> instance to a nullable <see cref="sysconsolecolor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="ConsoleColor"/> instance.</param>
    public static explicit operator sysconsolecolor?(ConsoleColor color) => color.ToSystemColor();

    /// <summary>
    /// Converts a <see cref="ConsoleColor"/> instance to a <see cref="sysconsolecolor"/> instance.
    /// </summary>
    /// <param name="color">The <see cref="ConsoleColor"/> instance.</param>
    /// <exception cref="ArgumentException">Thrown when the conversion is not possible.</exception>
    public static explicit operator sysconsolecolor(ConsoleColor color) =>
        color.ToSystemColor() ?? throw new ArgumentException($"Unable to convert '{color}' to a valid instance of {typeof(sysconsolecolor)}.", nameof(color));
}
