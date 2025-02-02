using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

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

public struct SixelPixel
{
    public readonly byte R, G, B;
    internal byte _palette_index;
    internal Flags _flags;


    public SixelPixel(byte r, byte g, byte b)
    {
        _flags = Flags.UndefinedIndex;
        _palette_index = 0xff;
        R = r;
        G = g;
        B = b;
    }

    public SixelPixel(byte palette, byte r, byte g, byte b)
    {
        _flags = Flags.NONE;
        _palette_index = palette;
        R = r;
        G = g;
        B = b;
    }

    public SixelPixel(byte palette)
    {
        _flags = Flags.UsePalette;
        _palette_index = palette;
        R = G = B = 0;
    }

    public override string ToString() => $"#{GetHashCode():x6} ({R}, {G}, {B}), 0x{_palette_index:x2} ({_palette_index}), {_flags}";

    public override int GetHashCode() => (R << 16) | (G << 8) | B;

    public override bool Equals(object? obj) => obj is SixelPixel pixel && pixel.GetHashCode() == GetHashCode();

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

    private readonly SixelPixel[] _pixels;
    private readonly object _mutex = new();
    private volatile bool _optimized_palette = false;

    public int Width { get; }
    public int Height { get; }
    public int PixelCount => _pixels.Length;


    public SixelPixel this[int x, int y]
    {
        get => _pixels[y * Width + x];
        set
        {
            _pixels[y * Width + x] = value;
            _optimized_palette = false;
        }
    }

    public SixelImage(int width, int height)
        : this(width, height, new SixelPixel[width * height])
    {
    }

    public SixelImage(int width, int height, SixelPixel[] pixels)
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

    private static string Raster(int pan, int pad, int width, int height)
    {
        return $"{_SIXEL_RASTER}{pan};{pad};{height};{width}";
    }

    private static string ColorLUT(byte color_index) => $"{_SIXEL_COLOR}{color_index}";

    private static string ColorHLS(byte color_index, int hue, byte lightness, byte saturation) => ColorHLS(
        color_index,
        hue,
        Math.Min((int)lightness, 100) * .01,
        Math.Min((int)saturation, 100) * .01
    );

    private static string ColorHLS(byte color_index, double hue, double lightness, double saturation)
    {
        hue = Math.Round(hue % 360);
        lightness = Math.Round(100 * Math.Clamp(lightness, 0, 1));
        saturation = Math.Round(100 * Math.Clamp(saturation, 0, 1));

        return $"{_SIXEL_COLOR}{color_index};{1};{hue};{lightness};{saturation}";
    }

    private static string ColorRGB(byte color_index, byte red, byte green, byte blue) => ColorRGB(color_index, red / 255.0, green / 255.0, blue / 255.0);

    private static string ColorRGB(byte color_index, double r, double g, double b)
    {
        r = Math.Round(Math.Clamp(r, 0, 1) * 100);
        g = Math.Round(Math.Clamp(g, 0, 1) * 100);
        b = Math.Round(Math.Clamp(b, 0, 1) * 100);

        return $"{_SIXEL_COLOR}{color_index};2;{r};{g};{b}";
    }

    private static char SixelValue(bool y0, bool y1, bool y2, bool y3, bool y4, bool y5) => SixelValue((byte)((y0 ? 1 : 0)
                                                                                                            | (y1 ? 2 : 0)
                                                                                                            | (y2 ? 4 : 0)
                                                                                                            | (y3 ? 8 : 0)
                                                                                                            | (y4 ? 16 : 0)
                                                                                                            | (y5 ? 32 : 0)));

    private static char SixelValue(byte value) => (char)(value + 63);




    public void OptimizeColorPalette()
    {
        lock (_mutex)
        {
            if (_optimized_palette)
                return;

            int i = 0;
            Dictionary<SixelPixel, (int index, int count, bool used)> unique_colors = (from p in _pixels
                                                                                       group p by p.GetHashCode() into g
                                                                                       let count = g.Count()
                                                                                       orderby count descending
                                                                                       let index = i++
                                                                                       select (g.First(), (index, count, false))).ToDictionary();

            if (unique_colors.Count <= 255)
            {
                for (i = 0; i < _pixels.Length; ++i)
                {
                    ref SixelPixel pixel = ref _pixels[i];
                    (int index, int count, bool used) = unique_colors[pixel];

                    pixel._palette_index = (byte)index;
                    pixel._flags = used ? SixelPixel.Flags.UsePalette : SixelPixel.Flags.NONE;
                    unique_colors[pixel] = (index, count, true);
                }
            }
            else
            {
                // optimize based on dithering etc.

                throw new NotImplementedException();
            }

            _optimized_palette = true;
        }
    }

    public string GenerateVT340Sequence(SixelRenderSettings render_settings)
    {
        StringBuilder sb = new();


        sb.Append($"{Console._DCS}{(int)render_settings.PixelAspectRatio};1;;q"); // TODO : implement P2 and P3


        for (int y = 0; y < Height; ++y)
        {
            byte y_index = (byte)(1 << (y % 6));

            for (int x = 0; x < Width; )
            {
                SixelPixel pixel = this[x, y];
                int count = 1;

                if (pixel._flags.HasFlag(SixelPixel.Flags.UsePalette))
                    sb.Append(ColorLUT(pixel._palette_index));
                else
                    sb.Append(ColorRGB(pixel._palette_index, pixel.R, pixel.G, pixel.B));

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

        // TODO : fix the bg color bug

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
        public static implicit operator _BGR(SixelPixel pixel) => new(pixel.B, pixel.G, pixel.R);

        public static implicit operator SixelPixel(_BGR color) => new(color.R, color.G, color.B);
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
