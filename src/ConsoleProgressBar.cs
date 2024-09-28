using System.Threading.Tasks;
using System.Text;
using System;

namespace Unknown6656.Console;


/// <summary>
/// Specifies the type of a <see cref="ConsoleProgressBar"/>.
/// </summary>
public enum ConsoleBarType
{
    /// <summary>
    /// A thick line progress bar. This mode uses the unicode character '<c>━</c>' (<c>U+2501</c>) for the progress bar and '<c>─</c>' (<c>U+2500</c>) for the background.
    /// Fractional progress is supported by usage of the unicode character '<c>╾</c>' (<c>U+257E</c>).
    /// </summary>
    ThickLine,
    /// <summary>
    /// A thin line progress bar. This mode uses the unicode character '<c>─</c>' (<c>U+2500</c>) for the progress bar and '<c>╌</c>' (<c>U+254C</c>) for the background.
    /// Fractional progress is supported by usage of the unicode character '<c>╴</c>' (<c>U+2574</c>).
    /// </summary>
    ThinLine,
    /// <summary>
    /// A solid block progress bar. This mode uses the unicode character '<c>█</c>' (<c>U+2588</c>) for the progress bar and '<c> </c>' (<c>U+0020</c>) for the background.
    /// Fractional progress is supported by usage of the unicode characters '<c>▏</c>' (<c>U+258F</c>), '<c>▎</c>' (<c>U+258E</c>), '<c>▍</c>' (<c>U+258D</c>), '<c>▌</c>' (<c>U+258C</c>), '<c>▋</c>' (<c>U+258B</c>), '<c>▊</c>' (<c>U+258A</c>), '<c>▉</c>' (<c>U+2589</c>).
    /// </summary>
    SolidBlock,
    /// <summary>
    /// A Braille block progress bar. This mode uses the unicode character '<c>⣿</c>' (<c>U+28FF</c>) for the progress bar and '<c>⣿</c>' (<c>U+28FF</c>) for the background.
    /// Fractional progress is supported by usage of the unicode characters '<c>⡀</c>' (<c>U+2840</c>), '<c>⡄</c>' (<c>U+2844</c>), '<c>⡆</c>' (<c>U+2846</c>), '<c>⡇</c>' (<c>U+2847</c>), '<c>⣇</c>' (<c>U+28C7</c>), '<c>⣧</c>' (<c>U+28E7</c>), '<c>⣷</c>' (<c>U+28F7</c>).
    /// </summary>
    BrailleBlock,
    /// <summary>
    /// A thick line progress bar using ASCII characters. This mode uses the characters '<c>=</c>' and '<c>-</c>' for the progress bar and background, respectively.
    /// Fractional progress is not supported.
    /// </summary>
    ASCII_ThickLine,
    /// <summary>
    /// A thin line progress bar using ASCII characters. This mode uses the characters '<c>-</c>' for the progress bar and background.
    /// Fractional progress is not supported.
    /// </summary>
    ASCII_ThinLine,
    /// <summary>
    /// A solid block progress bar using ASCII characters. This mode uses the characters '<c>#</c>' and '<c>-</c>' for the progress bar and background, respectively.
    /// Fractional progress is supported by usage of the character '<c>+</c>'.
    /// </summary>
    ASCII_SolidBlock,
}

/// <summary>
/// Represents the visual style of a <see cref="ConsoleProgressBar"/>.
/// </summary>
public record ConsoleProgressBarStyle
{
    /// <summary>
    /// Gets the color of the progress bar.
    /// </summary>
    public ConsoleColor BarColor { get; init; } = ConsoleColor.Green;

    public ConsoleColor TextColor { get; init; } = ConsoleColor.White;

    /// <summary>
    /// Gets the secondary color of the progress bar.
    /// </summary>
    public ConsoleColor BarSecondaryColor { get; init; } = ConsoleColor.DarkGray;

    /// <summary>
    /// Gets the background color of the progress bar.
    /// </summary>
    public ConsoleColor BackgroundColor { get; init; } = ConsoleColor.Default;

    /// <summary>
    /// Gets the type of the progress bar.
    /// </summary>
    public ConsoleBarType BarType { get; init; } = ConsoleBarType.ThickLine;

    /// <summary>
    /// Gets a value indicating whether to show the percentage on the progress bar.
    /// </summary>
    public bool ShowPercentage { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to animate the progress bar when it is indeterminate.
    /// </summary>
    public bool AnimatedIndeterminate { get; init; } = true;
}

/// <summary>
/// Represents a console progress bar that can be rendered asynchronously.
/// </summary>
public class ConsoleProgressBar
    : IAsyncDisposable
{
    private static readonly object _mutex = new();
    private const int _ADDITIONAL_WIDTH = 8;

    private readonly Task<Task> _render_task;
    private volatile bool _invalidated;
    private ConsoleProgressBarStyle _style = new();
    private double? _value;
    private int _width;
    private int _x;
    private int _y;


    /// <summary>
    /// Gets the maximum value of the progress bar.
    /// </summary>
    public double Maximum { get; }

    /// <summary>
    /// Gets the minimum value of the progress bar.
    /// </summary>
    public double Minimum { get; }

    /// <summary>
    /// Gets or sets the current value of the progress bar.
    /// This value is assumed to be in the range of <c>[<see cref="Minimum"/>...<see cref="Maximum"/>]</c>.
    /// A value of <see langword="null"/> or <see cref="double.NaN"/> indicates an indeterminate progress bar.
    /// </summary>
    public double? Value
    {
        get => _value;
        set
        {
            if (value != _value)
            {
                _value = value is double d && !double.IsNaN(d) ? double.Clamp(d, Minimum, Maximum) : null;

                Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets or sets the progress of the progress bar as a value between 0 and 1.
    /// A value of <see langword="null"/> or <see cref="double.NaN"/> indicates an indeterminate progress bar.
    /// </summary>
    public double? Progress
    {
        get => Value is double v ? (v - Minimum) / (Maximum - Minimum) : null;
        set => Value = value is double v && double.IsFinite(v) ? Minimum + (Maximum - Minimum) * double.Clamp(v, 0, 1) : null;
    }

    /// <summary>
    /// Indicates whether the progress bar is indeterminate.
    /// </summary>
    public bool IsIndeterminate => !Value.HasValue;

    /// <summary>
    /// Indicates whether the progress bar is running.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Gets or sets the X position of the progress bar in the console buffer (in characters).
    /// </summary>
    public int XPosition
    {
        get => _x;
        set
        {
            if (value != _x)
            {
                _x = value < 0 || value >= Console.BufferWidth - Width ? throw new ArgumentOutOfRangeException(nameof(XPosition)) : value;

                Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets or sets the Y position of the progress bar in the console buffer (in characters).
    /// </summary>
    public int YPosition
    {
        get => _y;
        set
        {
            if (value != _y)
            {
                _y = value < 0 || value >= Console.BufferHeight ? throw new ArgumentOutOfRangeException(nameof(YPosition)) : value;

                Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets or sets the width of the progress bar (in characters).
    /// </summary>
    public required int Width
    {
        get => _width;
        set
        {
            if (value != _width)
            {
                value -= _ADDITIONAL_WIDTH;
                _width = value < 0 || value >= Console.BufferWidth - XPosition ? throw new ArgumentOutOfRangeException(nameof(Width)) : value;

                Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets or sets the visual style of the progress bar.
    /// </summary>
    public ConsoleProgressBarStyle Style
    {
        get => _style;
        set
        {
            if (value != _style)
            {
                _style = value;

                Invalidate();
            }
        }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleProgressBar"/> class with the specified initial value.
    /// The value is assumed to be in the range of <c>[0...1]</c>.
    /// </summary>
    /// <param name="value">The initial value of the progress bar.</param>
    public ConsoleProgressBar(double value = 0)
        : this(0, 1, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleProgressBar"/> class with the specified minimum, maximum, and initial values.
    /// </summary>
    /// <param name="min">The minimum value of the progress bar.</param>
    /// <param name="max">The maximum value of the progress bar.</param>
    /// <param name="value">The initial value of the progress bar.</param>
    public ConsoleProgressBar(double min, double max, double? value = null)
    {
        Minimum = double.Min(min, max);
        Maximum = double.Max(min, max);
        XPosition = Console.CursorLeft;
        YPosition = Console.CursorTop;
        Value = value;
        IsRunning = true;

        Invalidate();

        _render_task = SpawnRenderTask();
    }

    private Task<Task> SpawnRenderTask() => Task.Factory.StartNew(async delegate
    {
        const int min_delay = 2;
        const int max_delay = 100;
        int delay = min_delay;

        while (IsRunning)
        {
            if (_invalidated)
            {
                Render();

                delay = min_delay;
            }
            else
                delay = Math.Min(max_delay, delay + min_delay);

            await Task.Delay(delay);
        }
    });

    /// <summary>
    /// Renders the progress bar to the console.
    /// Note that this is also automatically performed when rendering is invalidated (either through property updates or by calling <see cref="Invalidate"/>).
    /// </summary>
    public void Render()
    {
        StringBuilder sb = new(Style.BackgroundColor.ToVT520(ColorMode.Background));
        bool invalidated = false;
        (char bar_fg_char, char bar_bg_char) = Style.BarType switch
        {
            ConsoleBarType.ThickLine => ('━', '─'),
            ConsoleBarType.ThinLine => ('─', '╌'),
            ConsoleBarType.SolidBlock => ('█', ' '),
            ConsoleBarType.BrailleBlock => ('⣿', '⣿'),
            ConsoleBarType.ASCII_ThickLine => ('=', '-'),
            ConsoleBarType.ASCII_ThinLine => ('-', '-'),
            ConsoleBarType.ASCII_SolidBlock => ('#', '-'),
            _ => throw new ArgumentOutOfRangeException(nameof(Style.BarType)),
        };

        if (Progress is double progress && double.IsFinite(progress))
        {
            int bar_width = (int)(Width * progress);
            double fractional = progress * Width - bar_width;
            int remaining_width = Width - bar_width - double.Sign(fractional);

            sb.Append(Style.BarColor.ToVT520(ColorMode.Foreground))
              .Append(new string(bar_fg_char, bar_width));

            if (fractional > 0)
            {
                char[] bar_mid = Style.BarType switch
                {
                    ConsoleBarType.ThickLine => [bar_bg_char, '╾', bar_fg_char],
                    ConsoleBarType.ThinLine => [bar_bg_char, '╴', bar_fg_char],
                    ConsoleBarType.SolidBlock => [bar_bg_char, '▏', '▎', '▍', '▌', '▋', '▊', '▉', '█', bar_fg_char],
                    ConsoleBarType.BrailleBlock => [bar_bg_char, '⡀', '⡄', '⡆', '⡇', '⣇', '⣧', '⣷', bar_fg_char],
                    ConsoleBarType.ASCII_ThickLine => [bar_bg_char, bar_fg_char],
                    ConsoleBarType.ASCII_ThinLine => [bar_bg_char, bar_fg_char],
                    ConsoleBarType.ASCII_SolidBlock => [bar_bg_char, '+', bar_fg_char],
                    _ => throw new ArgumentOutOfRangeException(nameof(Style.BarType)),
                };

                sb.Append(bar_mid[(int)Math.Round(fractional * (bar_mid.Length - 1))]);
            }

            sb.Append(Style.BarSecondaryColor.ToVT520(ColorMode.Foreground))
              .Append(new string(bar_bg_char, remaining_width))
              .Append(Style.TextColor.ToVT520(ColorMode.Foreground))
              .Append($" {progress * 100,5:F1} %");
        }
        else
        {
            string block = new(bar_fg_char, Width / 4);
            int offs = (Width - block.Length) / 2;

            if (Style.AnimatedIndeterminate)
            {
                invalidated = true;
                offs = (int)((Math.Sin(DateTime.UtcNow.Ticks * .00_000_01) + 1) * .5 * (Width - block.Length + 1));
            }

            sb.Append(Style.BarSecondaryColor.ToVT520(ColorMode.Foreground))
              .Append(new string(bar_bg_char, offs))
              .Append(Style.BarColor.ToVT520(ColorMode.Foreground))
              .Append(block)
              .Append(Style.BarSecondaryColor.ToVT520(ColorMode.Foreground))
              .Append(new string(bar_bg_char, Width - offs - block.Length))
              .Append(Style.TextColor.ToVT520(ColorMode.Foreground))
              .Append(" ---.- %");
        }

        lock (_mutex)
        {
            Console.SetCursorPosition(XPosition, YPosition);
            Console.Write(sb);

            _invalidated = invalidated;
        }
    }

    /// <summary>
    /// Invalidates the progress bar, causing it to be re-rendered.
    /// </summary>
    public void Invalidate() => _invalidated = true;

    /// <summary>
    /// Disposes the progress bar asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        IsRunning = false;

        await await _render_task;
    }
}

// TODO : spinner (modal, non-modal)
// TODO : progress bar (modal)

