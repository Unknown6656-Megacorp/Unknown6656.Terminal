// #define ALLOW_VARIOUS_PIXEL_RATIOS

using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System;

using Unknown6656.Generics;
using Unknown6656.Runtime;

namespace Unknown6656.Console;


#if ALLOW_VARIOUS_PIXEL_RATIOS
public enum SixelPixelAspectRatio
{
    _1_to_1 = 7,
    _2_to_1 = 5,
    _3_to_1 = 2,
    _5_to_1 = 0,
}
#endif

public record SixelRenderSettings
{
#if ALLOW_VARIOUS_PIXEL_RATIOS
    public SixelPixelAspectRatio PixelAspectRatio { get; init; } = SixelPixelAspectRatio._1_to_1;
#endif
    public bool RestoreConsoleRenditions { get; set; } = false;
    public bool RelativePosition { get; init; } = true;
    public int XPosition { get; init; } = 0;
    public int YPosition { get; init; } = 0;
}

public struct SixelColor
{
    private const uint _MASK_B = 0b_0000_0000_0000_0000_0000_0000_0111_1111u;
    private const uint _MASK_G = 0b_0000_0000_0000_0000_0011_1111_1000_0000u;
    private const uint _MASK_R = 0b_0000_0000_0001_1111_1100_0000_0000_0000u;
    private const uint _MASK_I = 0b_0001_1111_1110_0000_0000_0000_0000_0000u;
    private const uint _MASK_F = 0b_1110_0000_0000_0000_0000_0000_0000_0000u;

    private static readonly Dictionary<int, (float L, float a, float b)> _lab_cache = [];

    // BIT LAYOUT:
    //  (sorry for the shitty struct layout, I'm just trying to fit everying into 32 bits)
    //
    //      fffi iiii iiir rrrr rrgg gggg gbbb bbbb
    //
    //      f = flags
    //      i = palette index (0-255)
    //      r = red (0-100)
    //      g = green (0-100)
    //      b = blue (0-100)
    private uint _value;


    public float R
    {
        readonly get => ((_value & _MASK_R) >> 14) * .01f;
        set => _value = (_value & ~_MASK_R) | ((uint)float.Round(float.Clamp(value, 0, 1) * 100f) << 14);
    }

    public float G
    {
        readonly get => ((_value & _MASK_G) >> 7) * .01f;
        set => _value = (_value & ~_MASK_G) | ((uint)float.Round(float.Clamp(value, 0, 1) * 100f) << 7);
    }

    public float B
    {
        readonly get => (_value & _MASK_B) * .01f;
        set => _value = (_value & ~_MASK_B) | (uint)float.Round(float.Clamp(value, 0, 1) * 100f);
    }

    public (float L, float a, float b) LAB
    {
        get
        {
            int hc = GetHashCode();

            if (!_lab_cache.TryGetValue(hc, out (float L, float a, float b) lab))
            {
                float r = R;
                float g = G;
                float b = B;

                if (r > .04045f)
                    r = float.Pow((r + .055f) / 1.055f, 2.4f);
                else
                    r /= 12.92f;

                if (g > .04045f)
                    g = float.Pow((g + .055f) / 1.055f, 2.4f);
                else
                    g /= 12.92f;

                if (b > .04045f)
                    b = float.Pow((b + .055f) / 1.055f, 2.4f);
                else
                    b /= 12.92f;

                r *= 100f;
                g *= 100f;
                b *= 100f;

                float x = ((r * .412453f) + (g * .357580f) + (b * .180423f)) / 95.047f;
                float y = ((r * .212671f) + (g * .715160f) + (b * .072169f)) / 100f;
                float z = ((r * .019334f) + (g * .119193f) + (b * .950227f)) / 108.883f;

                if (x > .008856f)
                    x = float.Pow(x, .333333333333333f);
                else
                    x = (x * 7.787f) + .13793103448275862f;

                if (y > .008856f)
                    y = float.Pow(y, .333333333333333f);
                else
                    y = (y * 7.787f) + .13793103448275862f;

                if (z > .008856f)
                    z = float.Pow(z, .333333333333333f);
                else
                    z = (z * 7.787f) + .13793103448275862f;

                _lab_cache[hc] = lab = (
                    (116f * y) - 16f,
                    500f * (x - y),
                    200f * (y - z)
                );
            }

            return lab;
        }
    }

    internal byte PaletteIndex
    {
        readonly get => (byte)((_value & _MASK_I) >> 21);
        set => _value = (_value & ~_MASK_I) | ((uint)value << 21);
    }

    internal Flags ColorFlags
    {
        readonly get => (Flags)((_value & _MASK_F) >> 29);
        set => _value = (_value & ~_MASK_F) | ((uint)value << 29);
    }


    public SixelColor(float r, float g, float b)
        : this(0xff, r, g, b, Flags.UndefinedIndex)
    {
    }

    public SixelColor(byte palette, float r, float g, float b)
        : this(palette, r, g, b, Flags.NONE)
    {
    }

    public SixelColor(byte palette)
        : this(palette, 0, 0, 0, Flags.UsePalette)
    {
    }

    public SixelColor(byte palette, SixelColor color)
    {
        _value = color._value;
        PaletteIndex = palette;
    }

    private SixelColor(byte palette, float r, float g, float b, Flags flags)
    {
        ColorFlags = flags;
        PaletteIndex = palette;
        R = r;
        G = g;
        B = b;
    }

    public override readonly string ToString() => $"0x{GetHashCode():x6}: {R:P0}, {G:P0}, {B:P0}, 0x{PaletteIndex:x2} ({PaletteIndex}), {ColorFlags}";

    public override readonly int GetHashCode() => (int)(_value & (_MASK_R | _MASK_G | _MASK_B));

    public override readonly bool Equals(object? obj) => obj is SixelColor pixel && pixel.GetHashCode() == GetHashCode();

    public readonly float LABDistanceTo(SixelColor other)
    {
        (float L1, float a1, float b1) = LAB;
        (float L2, float a2, float b2) = other.LAB;

        float deltaL = L1 - L2;
        float deltaA = a1 - a2;
        float deltaB = b1 - b2;
        float c1 = float.Sqrt(a1 * a1 + b1 * b1);
        float c2 = float.Sqrt(a2 * a2 + b2 * b2);
        float deltaC = c1 - c2;
        float deltaH = float.Sqrt(float.Max(0, deltaA * deltaA + deltaB * deltaB - deltaC * deltaC));

        deltaC /= 1 + c1 * .045f;
        deltaH /= 1 + c1 * .015f;

        return float.Sqrt(float.Max(0, deltaL * deltaL + deltaC * deltaC + deltaH * deltaH));
    }

    public readonly SixelColor FindClosest(SixelColor[] palette)
    {
        float minDist = float.MaxValue;
        int index = 0;

        for (int i = 0; i < palette.Length; ++i)
        {
            float dist = LABDistanceTo(palette[i]);

            if (dist < minDist)
                (index, minDist) = (i, dist);
        }

        return palette[index];
    }

    public static implicit operator Color(SixelColor color) => Color.FromArgb(
        (int)float.Round(color.R * 255),
        (int)float.Round(color.G * 255),
        (int)float.Round(color.B * 255)
    );

    public static implicit operator SixelColor(Color color) => new(color.R / 255f, color.G / 255f, color.B / 255f);


    [Flags]
    internal enum Flags
        : byte
    {
        NONE =              0b_0000_0000,
        UsePalette =        0b_0000_0001,
        UndefinedIndex =    0b_0000_0010,
    }
}

public class SixelImage
{
    private const int _MAX_PALETTE_SIZE = 256;
    private const char _SIXEL_CR = '$';
    private const char _SIXEL_LF = '-';
    private const char _SIXEL_COLOR = '#';
    private const char _SIXEL_RASTER = '"';
    private const char _SIXEL_EMPTY = '?';

    private readonly SixelColor[] _pixels;
    private readonly object _mutex = new();
    private volatile bool _optimized_palette = false;


    public int Width { get; }
    public int Height { get; }
    public int PixelCount => _pixels.Length;

    public SixelColor[] UniversalColorPalette
    {
        get => field;
        set
        {
            if (value.Length != _MAX_PALETTE_SIZE)
                throw new ArgumentException($"The given color palette must have an exact size of {_MAX_PALETTE_SIZE} entries.", nameof(value));

            byte index = 0;

            field = value.ToArray(c => new SixelColor(index++, c));
        }
    }

    public SixelColor this[int x, int y]
    {
        get => _pixels[y * Width + x];
        set
        {
            _pixels[y * Width + x] = value;
            _optimized_palette = false;
        }
    }


    public SixelImage(int width, int height)
        : this(width, height, new SixelColor[width * height])
    {
    }

    public SixelImage(int width, int height, SixelColor[] pixels)
    {
        if (pixels.Length != width * height)
            throw new ArgumentException("The number of pixels does not match the specified width and height.");

        Width = width;
        Height = height;
        _pixels = pixels;
        _optimized_palette = false;
        UniversalColorPalette = [
            new(0, 0.0f, 0.0f, 0.0f),
            new(1, 0.0f, 0.0f, 0.4f),
            new(2, 0.0f, 0.0f, 0.8f),
            new(3, 0.0f, 0.09019607843137255f, 0.2f),
            new(4, 0.0f, 0.09019607843137255f, 0.6f),
            new(5, 0.0f, 0.09019607843137255f, 1.0f),
            new(6, 0.0f, 0.1803921568627451f, 0.0f),
            new(7, 0.0f, 0.1803921568627451f, 0.4f),
            new(8, 0.0f, 0.1803921568627451f, 0.8f),
            new(9, 0.0f, 0.27058823529411763f, 0.2f),
            new(10, 0.0f, 0.27058823529411763f, 0.6f),
            new(11, 0.0f, 0.27058823529411763f, 1.0f),
            new(12, 0.0f, 0.3607843137254902f, 0.0f),
            new(13, 0.0f, 0.3607843137254902f, 0.4f),
            new(14, 0.0f, 0.3607843137254902f, 0.8f),
            new(15, 0.0f, 0.45098039215686275f, 0.2f),
            new(16, 0.0f, 0.45098039215686275f, 0.6f),
            new(17, 0.0f, 0.45098039215686275f, 1.0f),
            new(18, 0.0f, 0.5450980392156862f, 0.0f),
            new(19, 0.0f, 0.5450980392156862f, 0.4f),
            new(20, 0.0f, 0.5450980392156862f, 0.8f),
            new(21, 0.0f, 0.6352941176470588f, 0.2f),
            new(22, 0.0f, 0.6352941176470588f, 0.6f),
            new(23, 0.0f, 0.6352941176470588f, 1.0f),
            new(24, 0.0f, 0.7254901960784313f, 0.0f),
            new(25, 0.0f, 0.7254901960784313f, 0.4f),
            new(26, 0.0f, 0.7254901960784313f, 0.8f),
            new(27, 0.0f, 0.8156862745098039f, 0.2f),
            new(28, 0.0f, 0.8156862745098039f, 0.6f),
            new(29, 0.0f, 0.8156862745098039f, 1.0f),
            new(30, 0.0f, 0.9058823529411765f, 0.0f),
            new(31, 0.0f, 0.9058823529411765f, 0.4f),
            new(32, 0.0f, 0.9058823529411765f, 0.8f),
            new(33, 0.0f, 1.0f, 0.2f),
            new(34, 0.0f, 1.0f, 0.6f),
            new(35, 0.0f, 1.0f, 1.0f),
            new(36, 0.16470588235294117f, 0.0f, 0.2f),
            new(37, 0.16470588235294117f, 0.0f, 0.6f),
            new(38, 0.16470588235294117f, 0.0f, 1.0f),
            new(39, 0.16470588235294117f, 0.09019607843137255f, 0.0f),
            new(40, 0.16470588235294117f, 0.09019607843137255f, 0.4f),
            new(41, 0.16470588235294117f, 0.09019607843137255f, 0.8f),
            new(42, 0.16470588235294117f, 0.1803921568627451f, 0.2f),
            new(43, 0.16470588235294117f, 0.1803921568627451f, 0.6f),
            new(44, 0.16470588235294117f, 0.1803921568627451f, 1.0f),
            new(45, 0.16470588235294117f, 0.27058823529411763f, 0.0f),
            new(46, 0.16470588235294117f, 0.27058823529411763f, 0.4f),
            new(47, 0.16470588235294117f, 0.27058823529411763f, 0.8f),
            new(48, 0.16470588235294117f, 0.3607843137254902f, 0.2f),
            new(49, 0.16470588235294117f, 0.3607843137254902f, 0.6f),
            new(50, 0.16470588235294117f, 0.3607843137254902f, 1.0f),
            new(51, 0.16470588235294117f, 0.45098039215686275f, 0.0f),
            new(52, 0.16470588235294117f, 0.45098039215686275f, 0.4f),
            new(53, 0.16470588235294117f, 0.45098039215686275f, 0.8f),
            new(54, 0.16470588235294117f, 0.5450980392156862f, 0.2f),
            new(55, 0.16470588235294117f, 0.5450980392156862f, 0.6f),
            new(56, 0.16470588235294117f, 0.5450980392156862f, 1.0f),
            new(57, 0.16470588235294117f, 0.6352941176470588f, 0.0f),
            new(58, 0.16470588235294117f, 0.6352941176470588f, 0.4f),
            new(59, 0.16470588235294117f, 0.6352941176470588f, 0.8f),
            new(60, 0.16470588235294117f, 0.7254901960784313f, 0.2f),
            new(61, 0.16470588235294117f, 0.7254901960784313f, 0.6f),
            new(62, 0.16470588235294117f, 0.7254901960784313f, 1.0f),
            new(63, 0.16470588235294117f, 0.8156862745098039f, 0.0f),
            new(64, 0.16470588235294117f, 0.8156862745098039f, 0.4f),
            new(65, 0.16470588235294117f, 0.8156862745098039f, 0.8f),
            new(66, 0.16470588235294117f, 0.9058823529411765f, 0.2f),
            new(67, 0.16470588235294117f, 0.9058823529411765f, 0.6f),
            new(68, 0.16470588235294117f, 0.9058823529411765f, 1.0f),
            new(69, 0.16470588235294117f, 1.0f, 0.0f),
            new(70, 0.16470588235294117f, 1.0f, 0.4f),
            new(71, 0.16470588235294117f, 1.0f, 0.8f),
            new(72, 0.3333333333333333f, 0.0f, 0.0f),
            new(73, 0.3333333333333333f, 0.0f, 0.4f),
            new(74, 0.3333333333333333f, 0.0f, 0.8f),
            new(75, 0.3333333333333333f, 0.09019607843137255f, 0.2f),
            new(76, 0.3333333333333333f, 0.09019607843137255f, 0.6f),
            new(77, 0.3333333333333333f, 0.09019607843137255f, 1.0f),
            new(78, 0.3333333333333333f, 0.1803921568627451f, 0.0f),
            new(79, 0.3333333333333333f, 0.1803921568627451f, 0.4f),
            new(80, 0.3333333333333333f, 0.1803921568627451f, 0.8f),
            new(81, 0.3333333333333333f, 0.27058823529411763f, 0.2f),
            new(82, 0.3333333333333333f, 0.27058823529411763f, 0.6f),
            new(83, 0.3333333333333333f, 0.27058823529411763f, 1.0f),
            new(84, 0.3333333333333333f, 0.3607843137254902f, 0.0f),
            new(85, 0.3333333333333333f, 0.3607843137254902f, 0.4f),
            new(86, 0.3333333333333333f, 0.3607843137254902f, 0.8f),
            new(87, 0.3333333333333333f, 0.45098039215686275f, 0.2f),
            new(88, 0.3333333333333333f, 0.45098039215686275f, 0.6f),
            new(89, 0.3333333333333333f, 0.45098039215686275f, 1.0f),
            new(90, 0.3333333333333333f, 0.5450980392156862f, 0.0f),
            new(91, 0.3333333333333333f, 0.5450980392156862f, 0.4f),
            new(92, 0.3333333333333333f, 0.5450980392156862f, 0.8f),
            new(93, 0.3333333333333333f, 0.6352941176470588f, 0.2f),
            new(94, 0.3333333333333333f, 0.6352941176470588f, 0.6f),
            new(95, 0.3333333333333333f, 0.6352941176470588f, 1.0f),
            new(96, 0.3333333333333333f, 0.7254901960784313f, 0.0f),
            new(97, 0.3333333333333333f, 0.7254901960784313f, 0.4f),
            new(98, 0.3333333333333333f, 0.7254901960784313f, 0.8f),
            new(99, 0.3333333333333333f, 0.8156862745098039f, 0.2f),
            new(100, 0.3333333333333333f, 0.8156862745098039f, 0.6f),
            new(101, 0.3333333333333333f, 0.8156862745098039f, 1.0f),
            new(102, 0.3333333333333333f, 0.9058823529411765f, 0.0f),
            new(103, 0.3333333333333333f, 0.9058823529411765f, 0.4f),
            new(104, 0.3333333333333333f, 0.9058823529411765f, 0.8f),
            new(105, 0.3333333333333333f, 1.0f, 0.2f),
            new(106, 0.3333333333333333f, 1.0f, 0.6f),
            new(107, 0.3333333333333333f, 1.0f, 1.0f),
            new(108, 0.4980392156862745f, 0.0f, 0.2f),
            new(109, 0.4980392156862745f, 0.0f, 0.6f),
            new(110, 0.4980392156862745f, 0.0f, 1.0f),
            new(111, 0.4980392156862745f, 0.09019607843137255f, 0.0f),
            new(112, 0.4980392156862745f, 0.09019607843137255f, 0.4f),
            new(113, 0.4980392156862745f, 0.09019607843137255f, 0.8f),
            new(114, 0.4980392156862745f, 0.1803921568627451f, 0.2f),
            new(115, 0.4980392156862745f, 0.1803921568627451f, 0.6f),
            new(116, 0.4980392156862745f, 0.1803921568627451f, 1.0f),
            new(117, 0.4980392156862745f, 0.27058823529411763f, 0.0f),
            new(118, 0.4980392156862745f, 0.27058823529411763f, 0.4f),
            new(119, 0.4980392156862745f, 0.27058823529411763f, 0.8f),
            new(120, 0.4980392156862745f, 0.3607843137254902f, 0.2f),
            new(121, 0.4980392156862745f, 0.3607843137254902f, 0.6f),
            new(122, 0.4980392156862745f, 0.3607843137254902f, 1.0f),
            new(123, 0.4980392156862745f, 0.45098039215686275f, 0.0f),
            new(124, 0.4980392156862745f, 0.45098039215686275f, 0.4f),
            new(125, 0.4980392156862745f, 0.45098039215686275f, 0.8f),
            new(126, 0.4980392156862745f, 0.5450980392156862f, 0.2f),
            new(127, 0.4980392156862745f, 0.5450980392156862f, 0.6f),
            new(128, 0.4980392156862745f, 0.5450980392156862f, 1.0f),
            new(129, 0.4980392156862745f, 0.6352941176470588f, 0.0f),
            new(130, 0.4980392156862745f, 0.6352941176470588f, 0.4f),
            new(131, 0.4980392156862745f, 0.6352941176470588f, 0.8f),
            new(132, 0.4980392156862745f, 0.7254901960784313f, 0.2f),
            new(133, 0.4980392156862745f, 0.7254901960784313f, 0.6f),
            new(134, 0.4980392156862745f, 0.7254901960784313f, 1.0f),
            new(135, 0.4980392156862745f, 0.8156862745098039f, 0.0f),
            new(136, 0.4980392156862745f, 0.8156862745098039f, 0.4f),
            new(137, 0.4980392156862745f, 0.8156862745098039f, 0.8f),
            new(138, 0.4980392156862745f, 0.9058823529411765f, 0.2f),
            new(139, 0.4980392156862745f, 0.9058823529411765f, 0.6f),
            new(140, 0.4980392156862745f, 0.9058823529411765f, 1.0f),
            new(141, 0.4980392156862745f, 1.0f, 0.0f),
            new(142, 0.4980392156862745f, 1.0f, 0.4f),
            new(143, 0.4980392156862745f, 1.0f, 0.8f),
            new(144, 0.6666666666666666f, 0.0f, 0.0f),
            new(145, 0.6666666666666666f, 0.0f, 0.4f),
            new(146, 0.6666666666666666f, 0.0f, 0.8f),
            new(147, 0.6666666666666666f, 0.09019607843137255f, 0.2f),
            new(148, 0.6666666666666666f, 0.09019607843137255f, 0.6f),
            new(149, 0.6666666666666666f, 0.09019607843137255f, 1.0f),
            new(150, 0.6666666666666666f, 0.1803921568627451f, 0.0f),
            new(151, 0.6666666666666666f, 0.1803921568627451f, 0.4f),
            new(152, 0.6666666666666666f, 0.1803921568627451f, 0.8f),
            new(153, 0.6666666666666666f, 0.27058823529411763f, 0.2f),
            new(154, 0.6666666666666666f, 0.27058823529411763f, 0.6f),
            new(155, 0.6666666666666666f, 0.27058823529411763f, 1.0f),
            new(156, 0.6666666666666666f, 0.3607843137254902f, 0.0f),
            new(157, 0.6666666666666666f, 0.3607843137254902f, 0.4f),
            new(158, 0.6666666666666666f, 0.3607843137254902f, 0.8f),
            new(159, 0.6666666666666666f, 0.45098039215686275f, 0.2f),
            new(160, 0.6666666666666666f, 0.45098039215686275f, 0.6f),
            new(161, 0.6666666666666666f, 0.45098039215686275f, 1.0f),
            new(162, 0.6666666666666666f, 0.5450980392156862f, 0.0f),
            new(163, 0.6666666666666666f, 0.5450980392156862f, 0.4f),
            new(164, 0.6666666666666666f, 0.5450980392156862f, 0.8f),
            new(165, 0.6666666666666666f, 0.6352941176470588f, 0.2f),
            new(166, 0.6666666666666666f, 0.6352941176470588f, 0.6f),
            new(167, 0.6666666666666666f, 0.6352941176470588f, 1.0f),
            new(168, 0.6666666666666666f, 0.7254901960784313f, 0.0f),
            new(169, 0.6666666666666666f, 0.7254901960784313f, 0.4f),
            new(170, 0.6666666666666666f, 0.7254901960784313f, 0.8f),
            new(171, 0.6666666666666666f, 0.8156862745098039f, 0.2f),
            new(172, 0.6666666666666666f, 0.8156862745098039f, 0.6f),
            new(173, 0.6666666666666666f, 0.8156862745098039f, 1.0f),
            new(174, 0.6666666666666666f, 0.9058823529411765f, 0.0f),
            new(175, 0.6666666666666666f, 0.9058823529411765f, 0.4f),
            new(176, 0.6666666666666666f, 0.9058823529411765f, 0.8f),
            new(177, 0.6666666666666666f, 1.0f, 0.2f),
            new(178, 0.6666666666666666f, 1.0f, 0.6f),
            new(179, 0.6666666666666666f, 1.0f, 1.0f),
            new(180, 0.8313725490196079f, 0.0f, 0.2f),
            new(181, 0.8313725490196079f, 0.0f, 0.6f),
            new(182, 0.8313725490196079f, 0.0f, 1.0f),
            new(183, 0.8313725490196079f, 0.09019607843137255f, 0.0f),
            new(184, 0.8313725490196079f, 0.09019607843137255f, 0.4f),
            new(185, 0.8313725490196079f, 0.09019607843137255f, 0.8f),
            new(186, 0.8313725490196079f, 0.1803921568627451f, 0.2f),
            new(187, 0.8313725490196079f, 0.1803921568627451f, 0.6f),
            new(188, 0.8313725490196079f, 0.1803921568627451f, 1.0f),
            new(189, 0.8313725490196079f, 0.27058823529411763f, 0.0f),
            new(190, 0.8313725490196079f, 0.27058823529411763f, 0.4f),
            new(191, 0.8313725490196079f, 0.27058823529411763f, 0.8f),
            new(192, 0.8313725490196079f, 0.3607843137254902f, 0.2f),
            new(193, 0.8313725490196079f, 0.3607843137254902f, 0.6f),
            new(194, 0.8313725490196079f, 0.3607843137254902f, 1.0f),
            new(195, 0.8313725490196079f, 0.45098039215686275f, 0.0f),
            new(196, 0.8313725490196079f, 0.45098039215686275f, 0.4f),
            new(197, 0.8313725490196079f, 0.45098039215686275f, 0.8f),
            new(198, 0.8313725490196079f, 0.5450980392156862f, 0.2f),
            new(199, 0.8313725490196079f, 0.5450980392156862f, 0.6f),
            new(200, 0.8313725490196079f, 0.5450980392156862f, 1.0f),
            new(201, 0.8313725490196079f, 0.6352941176470588f, 0.0f),
            new(202, 0.8313725490196079f, 0.6352941176470588f, 0.4f),
            new(203, 0.8313725490196079f, 0.6352941176470588f, 0.8f),
            new(204, 0.8313725490196079f, 0.7254901960784313f, 0.2f),
            new(205, 0.8313725490196079f, 0.7254901960784313f, 0.6f),
            new(206, 0.8313725490196079f, 0.7254901960784313f, 1.0f),
            new(207, 0.8313725490196079f, 0.8156862745098039f, 0.0f),
            new(208, 0.8313725490196079f, 0.8156862745098039f, 0.4f),
            new(209, 0.8313725490196079f, 0.8156862745098039f, 0.8f),
            new(210, 0.8313725490196079f, 0.9058823529411765f, 0.2f),
            new(211, 0.8313725490196079f, 0.9058823529411765f, 0.6f),
            new(212, 0.8313725490196079f, 0.9058823529411765f, 1.0f),
            new(213, 0.8313725490196079f, 1.0f, 0.0f),
            new(214, 0.8313725490196079f, 1.0f, 0.4f),
            new(215, 0.8313725490196079f, 1.0f, 0.8f),
            new(216, 1.0f, 0.0f, 0.0f),
            new(217, 1.0f, 0.0f, 0.4f),
            new(218, 1.0f, 0.0f, 0.8f),
            new(219, 1.0f, 0.09019607843137255f, 0.2f),
            new(220, 1.0f, 0.09019607843137255f, 0.6f),
            new(221, 1.0f, 0.09019607843137255f, 1.0f),
            new(222, 1.0f, 0.1803921568627451f, 0.0f),
            new(223, 1.0f, 0.1803921568627451f, 0.4f),
            new(224, 1.0f, 0.1803921568627451f, 0.8f),
            new(225, 1.0f, 0.27058823529411763f, 0.2f),
            new(226, 1.0f, 0.27058823529411763f, 0.6f),
            new(227, 1.0f, 0.27058823529411763f, 1.0f),
            new(228, 1.0f, 0.3607843137254902f, 0.0f),
            new(229, 1.0f, 0.3607843137254902f, 0.4f),
            new(230, 1.0f, 0.3607843137254902f, 0.8f),
            new(231, 1.0f, 0.45098039215686275f, 0.2f),
            new(232, 1.0f, 0.45098039215686275f, 0.6f),
            new(233, 1.0f, 0.45098039215686275f, 1.0f),
            new(234, 1.0f, 0.5450980392156862f, 0.0f),
            new(235, 1.0f, 0.5450980392156862f, 0.4f),
            new(236, 1.0f, 0.5450980392156862f, 0.8f),
            new(237, 1.0f, 0.6352941176470588f, 0.2f),
            new(238, 1.0f, 0.6352941176470588f, 0.6f),
            new(239, 1.0f, 0.6352941176470588f, 1.0f),
            new(240, 1.0f, 0.7254901960784313f, 0.0f),
            new(241, 1.0f, 0.7254901960784313f, 0.4f),
            new(242, 1.0f, 0.7254901960784313f, 0.8f),
            new(243, 1.0f, 0.8156862745098039f, 0.2f),
            new(244, 1.0f, 0.8156862745098039f, 0.6f),
            new(245, 1.0f, 0.8156862745098039f, 1.0f),
            new(246, 1.0f, 0.9058823529411765f, 0.0f),
            new(247, 1.0f, 0.9058823529411765f, 0.4f),
            new(248, 1.0f, 0.9058823529411765f, 0.8f),
            new(249, 1.0f, 1.0f, 0.2f),
            new(250, 1.0f, 1.0f, 0.6f),
            new(251, 1.0f, 1.0f, 1.0f),
            new(252, 0.8f, 0.8f, 0.8f),
            new(253, 0.6f, 0.6f, 0.6f),
            new(254, 0.4f, 0.4f, 0.4f),
            new(255, 0.2f, 0.2f, 0.2f),
        ];
    }

    [SupportedOSPlatform(OS.WIN)]
    public unsafe Bitmap ToBitmap()
    {
        Bitmap bmp = new(Width, Height, PixelFormat.Format24bppRgb);
        BitmapData dat = bmp.LockBits(new(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
        _BGR* ptr = (_BGR*)dat.Scan0;

        Parallel.For(0, PixelCount, i => ptr[i] = _pixels[i]);

        bmp.UnlockBits(dat);

        return bmp;
    }

    [SupportedOSPlatform(OS.WIN)]
    public void SaveAsBitmap(FileInfo path)
    {
        if (path.Extension.ToLower() is "six" or "sixel")
            SaveAs(path);
        else
            using (Bitmap bmp = ToBitmap())
                bmp.Save(path.FullName);
    }

    [SupportedOSPlatform(OS.WIN)]
    public void SaveAsBitmap(FileInfo path, ImageFormat format)
    {
        using Bitmap bmp = ToBitmap();

        bmp.Save(path.FullName, format);
    }

    [SupportedOSPlatform(OS.WIN)]
    public void SaveAsBitmap(Stream stream, ImageFormat format)
    {
        using Bitmap bmp = ToBitmap();

        bmp.Save(stream, format);
    }

    public void SaveAs(FileInfo path) => SaveAs(path, Encoding.UTF8);

    public void SaveAs(FileInfo path, Encoding encoding)
    {
        using FileStream fs = path.OpenWrite();
        using StreamWriter sw = new(fs, encoding);

        SaveAs(sw);
    }

    public void SaveAs(StreamWriter stream)
    {
        stream.Write(GenerateVT340Sequence(new()));

        throw new NotImplementedException();
    }

    public void Print() => Print(new());

    public void Print(SixelRenderSettings render_settings)
    {
        ConsoleGraphicRendition? rendition = render_settings.RestoreConsoleRenditions ? Console.CurrentGraphicRendition : null;
        (int x, int y) = Console.GetCursorPosition();

        if (render_settings.RelativePosition)
            Console.SetRelativeCursorPosition(render_settings.XPosition, render_settings.YPosition);
        else
            Console.SetCursorPosition(render_settings.XPosition, render_settings.YPosition);

        Console.Write(GenerateVT340Sequence(render_settings));
        Console.SetCursorPosition(x, y);

        if (render_settings.RestoreConsoleRenditions)
            Console.CurrentGraphicRendition = rendition;
    }


    public override string ToString() => throw new InvalidOperationException($"It looks like you want to write the Sixel image to the console using the '{typeof(SixelImage)}.{nameof(ToString)}()' method. Please use either the method '{typeof(SixelImage)}.{nameof(Print)}(*)' or '{typeof(Console)}.{nameof(Console.Write)}({typeof(SixelImage)}, *)'.");

    private static string Repeat(char sixel, int count)
    {
        string result = "";

        while (count > 255)
        {
            result += $"!255{sixel}";
            count -= 255;
        }

        if (count > 4)
            result += $"!{count}{sixel}";
        else if (count > 0)
            result += new string(sixel, count);

        return result;
    }

    private static string Raster(int pan, int pad, int width, int height) => $"{_SIXEL_RASTER}{pan};{pad};{height};{width}";

    private static string ColorLUT(SixelColor color) => $"{_SIXEL_COLOR}{color.PaletteIndex}";

    private static string ColorSet(SixelColor color) => $"{_SIXEL_COLOR}{color.PaletteIndex};2;{(int)float.Round(color.R * 100f)};{(int)float.Round(color.G * 100f)};{(int)float.Round(color.B * 100f)}";

    private static char SixelValue(byte value) => (char)(value + 63);

    private void FloydSteinbergDithering(SixelColor[] palette)
    {
#pragma warning disable IDE0305 // Simplify collection initialization
        SixelColor[] buffer = _pixels.ToArray();
#pragma warning restore IDE0305

        for (int i = 0; i < buffer.Length; ++i)
        {
            int x = i % Width;
            int y = i / Width;

            SixelColor prev = buffer[i];
            SixelColor curr = buffer[i] = prev.FindClosest(palette);
            float r_err = (prev.R - curr.R) / 16f;
            float g_err = (prev.G - curr.G) / 16f;
            float b_err = (prev.B - curr.B) / 16f;

            ref SixelColor c1 = ref buffer[(i + 1) % buffer.Length];
            ref SixelColor c2 = ref buffer[(i + Width) % buffer.Length];
            ref SixelColor c3 = ref buffer[(i + Width - 1) % buffer.Length];
            ref SixelColor c4 = ref buffer[(i + Width + 1) % buffer.Length];

            c1.R += r_err * 7;
            c1.G += g_err * 7;
            c1.B += b_err * 7;

            c2.R += r_err * 5;
            c2.G += g_err * 5;
            c2.B += b_err * 5;

            c3.R += r_err * 3;
            c3.G += g_err * 3;
            c3.B += b_err * 3;

            c4.R += r_err;
            c4.G += g_err;
            c4.B += b_err;
        }

        Array.Copy(buffer, _pixels, buffer.Length);
    }

    // TODO : optimize this code's performance. this shit is way too slow
    public void OptimizeColorPalette()
    {
        lock (_mutex)
        {
            if (_optimized_palette)
                return;

            Stopwatch sw = Stopwatch.StartNew();

            OptimizeColorPalette([.. _pixels.Distinct()]);

            sw.Stop();
            Console.WriteLine($"Optimized color palette in {sw.ElapsedMilliseconds}ms.");

            _optimized_palette = true;
        }
    }

    private void OptimizeColorPalette(SixelColor[] palette)
    {
        if (palette.Length <= _MAX_PALETTE_SIZE)
        {
            int i = 0;
            Dictionary<SixelColor, (int index, bool used)> palette_dict = palette.ToDictionary(LINQ.id, _ => (i++, false));

            for (i = 0; i < _pixels.Length; ++i)
            {
                ref SixelColor pixel = ref _pixels[i];
                (int index, bool used) = palette_dict[pixel];

                pixel.PaletteIndex = (byte)index;
                pixel.ColorFlags = used ? SixelColor.Flags.UsePalette : SixelColor.Flags.NONE;
                palette_dict[pixel] = (index, true);
            }
        }
        else
        {
            palette = UniversalColorPalette;

            FloydSteinbergDithering(palette);
            OptimizeColorPalette(palette);
        }
    }

    private unsafe string GenerateVT340Sequence(SixelRenderSettings render_settings)
    {
        StringBuilder sb = new();
        int pixel_ratio = 7;
#if ALLOW_VARIOUS_PIXEL_RATIOS
        pixel_ratio = (int)render_settings.PixelAspectRatio;
#endif
        sb.Append($"{Console._DCS}{pixel_ratio};1;;q"); // TODO : implement P3

        for (int y = 0; y < Height; ++y)
        {
            byte y_index = (byte)(1 << (y % 6));

            for (int x = 0; x < Width; )
            {
                SixelColor pixel = this[x, y];
                delegate*<SixelColor, string> func = pixel.ColorFlags.HasFlag(SixelColor.Flags.UsePalette) ? &ColorLUT : &ColorSet;

                sb.Append(func(pixel));

                int count = 1;

                while (x + count < Width && this[x + count, y].Equals(pixel))
                    ++count;

                sb.Append(Repeat(SixelValue(y_index), count));

                x += count;
            }

            sb.Append(_SIXEL_CR);

            if (y_index >= 0b_0010_0000)
                sb.Append(_SIXEL_LF);
        }

        sb.Append(Console._ST);

        return sb.ToString();
    }


    public static SixelImage Parse(string vt340_sequence)
    {
        for (int index = 0; index < vt340_sequence.Length; ++index)
        {
            char curr = vt340_sequence[index];



        }

        throw new NotImplementedException();
    }

    public static SixelImage Parse(StreamReader stream) => Parse(stream.ReadToEnd());

    public static unsafe SixelImage FromFile(FileInfo path)
    {
        if (OS.IsWindows && path.Extension.ToLowerInvariant() is ".png" or ".jpg" or ".jpeg" or ".gif" or ".emf" or ".tif" or ".tiff"
                                                              or ".webp" or ".wmf" or ".bmp" or ".heif" or ".exif" or ".exf" or ".ico")
            return FromBitmap(path);
        else
            using (FileStream fs = path.OpenRead())
            using (StreamReader rd = new(fs, Encoding.UTF8))
                return Parse(rd);
    }

    [SupportedOSPlatform(OS.WIN)]
    public static unsafe SixelImage FromBitmap(FileInfo path)
    {
        using Bitmap bmp = new(path.FullName);

        return FromBitmap(bmp);
    }

    [SupportedOSPlatform(OS.WIN)]
    public static unsafe SixelImage FromBitmap(Stream stream)
    {
        using Image img = Image.FromStream(stream);
        using Bitmap bmp = new(img);

        return FromBitmap(bmp);
    }

    [SupportedOSPlatform(OS.WIN)]
    public static unsafe SixelImage FromBitmap(Bitmap bitmap)
    {
        if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
        {
            Bitmap tmp = new(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);

            using Graphics g = Graphics.FromImage(tmp);

            g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
            bitmap = tmp;
        }

        SixelImage img = new(bitmap.Width, bitmap.Height);
        BitmapData dat = bitmap.LockBits(new(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
        _BGR* ptr = (_BGR*)dat.Scan0;

        Parallel.For(0, img.PixelCount, i => img._pixels[i] = ptr[i]);

        bitmap.UnlockBits(dat);
        img.OptimizeColorPalette();

        return img;
    }


    private readonly record struct _BGR(byte B, byte G, byte R)
    {
#if DEBUG
        public override string ToString() => $"#{R:x2}{G:x2}{B:x2} ({R}, {G}, {B})";
#endif
        public static implicit operator _BGR(SixelColor pixel) => new(
            (byte)Math.Round(pixel.B * 255),
            (byte)Math.Round(pixel.G * 255),
            (byte)Math.Round(pixel.R * 255)
        );

        public static implicit operator SixelColor(_BGR color) => new(color.R / 255f, color.G / 255f, color.B / 255f);
    }
}

public static partial class Console
{
    public static void Write(SixelImage img) => img.Print();

    public static void Write(SixelImage img, SixelRenderSettings render_settings) => img.Print(render_settings);
}
