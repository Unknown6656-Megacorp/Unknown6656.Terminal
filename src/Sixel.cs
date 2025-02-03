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


public enum SixelPixelAspectRatio
{
    _1_to_1 = 7,
    _2_to_1 = 5,
    _3_to_1 = 2,
    _5_to_1 = 0,
}

public record SixelRenderSettings
{
    public SixelPixelAspectRatio PixelAspectRatio { get; init; } = SixelPixelAspectRatio._1_to_1;
}

public struct SixelColor
{
    // BIT LAYOUT:
    //
    //      fffi'iiii'iiir'rrrr'rrgg'gggg'gbbb'bbbb
    //      fff iiii'iiii -rrr'rrrr -ggg'gggg -bbb'bbbb
    //
    //      f = flags
    //      i = palette index (0-255)
    //      r = red (0-100)
    //      g = green (0-100)
    //      b = blue (0-100)

    private const uint _MASK_B = 0b_0000_0000_0000_0000_0000_0000_0111_1111u;
    private const uint _MASK_G = 0b_0000_0000_0000_0000_0011_1111_1000_0000u;
    private const uint _MASK_R = 0b_0000_0000_0001_1111_1100_0000_0000_0000u;
    private const uint _MASK_I = 0b_0001_1111_1110_0000_0000_0000_0000_0000u;
    private const uint _MASK_F = 0b_1110_0000_0000_0000_0000_0000_0000_0000u;

    private static readonly Dictionary<int, (float L, float a, float b)> _lab_cache = [];

    private uint _value;


    public float R
    {
        get => ((_value & _MASK_R) >> 14) * .01f;
        set => _value = (_value & ~_MASK_R) | ((uint)float.Round(float.Clamp(value, 0, 1) * 100f) << 14);
    }

    public float G
    {
        get => ((_value & _MASK_G) >> 7) * .01f;
        set => _value = (_value & ~_MASK_G) | ((uint)float.Round(float.Clamp(value, 0, 1) * 100f) << 7);
    }

    public float B
    {
        get => (_value & _MASK_B) * .01f;
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
        get => (byte)((_value & _MASK_I) >> 21);
        set => _value = (_value & ~_MASK_I) | ((uint)value << 21);
    }

    internal Flags ColorFlags
    {
        get => (Flags)((_value & _MASK_F) >> 29);
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

    private SixelColor(byte palette, float r, float g, float b, Flags flags)
    {
        ColorFlags = flags;
        PaletteIndex = palette;
        R = r;
        G = g;
        B = b;
    }

    public override string ToString() => $"{GetHashCode():x8}h: {R:P0}, {G:P0}, {B:P0}, 0x{PaletteIndex:x2} ({PaletteIndex}), {ColorFlags}";

    public override int GetHashCode() => (int)(_value & (_MASK_R | _MASK_G | _MASK_B));

    public override bool Equals(object? obj) => obj is SixelColor pixel && pixel.GetHashCode() == GetHashCode();

    public float LABDistanceTo(SixelColor other)
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

    public SixelColor FindClosest(SixelColor[] palette)
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

    private static char SixelValue(bool y0, bool y1, bool y2, bool y3, bool y4, bool y5) => SixelValue((byte)((y0 ? 1 : 0)
                                                                                                            | (y1 ? 2 : 0)
                                                                                                            | (y2 ? 4 : 0)
                                                                                                            | (y3 ? 8 : 0)
                                                                                                            | (y4 ? 16 : 0)
                                                                                                            | (y5 ? 32 : 0)));

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

            if (x + 1 < Width)
            {
                buffer[i + 1].R += r_err * 7;
                buffer[i + 1].G += g_err * 7;
                buffer[i + 1].B += b_err * 7;
            }

            if (y + 1 < Height)
            {
                buffer[i + Width].R += r_err * 5;
                buffer[i + Width].G += g_err * 5;
                buffer[i + Width].B += b_err * 5;

                if (x - 1 >= 0)
                {
                    buffer[i + Width - 1].R += r_err * 3;
                    buffer[i + Width - 1].G += g_err * 3;
                    buffer[i + Width - 1].B += b_err * 3;
                }

                if (x + 1 < Width)
                {
                    buffer[i + Width + 1].R += r_err;
                    buffer[i + Width + 1].G += g_err;
                    buffer[i + Width + 1].B += b_err;
                }
            }
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
        if (palette.Length <= 256)
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
            palette = new SixelColor[256]; // determine palette based on k-means clustering

            FloydSteinbergDithering(palette);
            OptimizeColorPalette(palette);
        }
    }

    public unsafe string GenerateVT340Sequence(SixelRenderSettings render_settings)
    {
        StringBuilder sb = new();

        sb.Append($"{Console._DCS}{(int)render_settings.PixelAspectRatio};1;;q"); // TODO : implement P3

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



    public static unsafe SixelImage FromFile(FileInfo path)
    {
        throw new NotImplementedException();
    }

    [SupportedOSPlatform(OS.WIN)]
    public static unsafe SixelImage FromBitmap(FileInfo path)
    {
        using (Bitmap bmp = new(path.FullName))
            return FromBitmap(bmp);
    }

    [SupportedOSPlatform(OS.WIN)]
    public static unsafe SixelImage FromBitmap(Stream stream)
    {
        using (Image img = Image.FromStream(stream))
        using (Bitmap bmp = new(img))
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
    public static void Write(SixelImage img) => Write(img, new());

    public static void Write(SixelImage img, SixelRenderSettings render_settings)
    {
        //ConsoleGraphicRendition? rendition = CurrentGraphicRendition;

        Write(img.GenerateVT340Sequence(render_settings));

        //CurrentGraphicRendition = rendition;
    }
}
