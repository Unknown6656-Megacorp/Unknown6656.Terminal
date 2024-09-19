using System.Threading.Tasks;
using System;

namespace Unknown6656.Console;


public delegate void ConsoleSizeChangedEventHandler(int oldWidth, int oldHeight, int newWidth, int newHeight);

/// <summary>
/// Represents an event listener that monitors console window size changes.
/// </summary>
public sealed class ConsoleResizeListener
    : IAsyncDisposable
{
    private readonly Task _listener;


    /// <summary>
    /// Indicates whether the listener has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Indicates whether the listener is currently listening for size changes.
    /// </summary>
    public bool IsListening { get; private set; }

    /// <summary>
    /// Gets or sets the minimum timeout in milliseconds between size checks.
    /// </summary>
    public uint MinimumTimeoutMs { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum timeout in milliseconds between size checks.
    /// </summary>
    public uint MaximumTimeoutMs { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the multiplier used to increase the timeout when no size change is detected.
    /// </summary>
    public float TimeoutMultiplier { get; set; } = 1.3f;


    /// <summary>
    /// Occurs when the console window size changes.
    /// </summary>
    public event ConsoleSizeChangedEventHandler? SizeChanged;


    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleResizeListener"/> class.
    /// </summary>
    public ConsoleResizeListener() => _listener = Task.Factory.StartNew(async delegate
    {
        int width = sysconsole.WindowWidth;
        int height = sysconsole.WindowHeight;
        uint timeout = MinimumTimeoutMs;

        while (!IsDisposed)
        {
            bool change = false;

            if (IsListening)
            {
                (int nw, int nh, int ow, int oh) = (sysconsole.WindowWidth, sysconsole.WindowHeight, width, height);

                if (nw != ow || nh != oh)
                {
                    (width, height, change) = (nw, nh, true);

                    SizeChanged?.Invoke(ow, oh, nw, nh);
                }
            }

            if (change)
                timeout = MinimumTimeoutMs;
            else
            {
                await Task.Delay((int)timeout);

                if (timeout < MaximumTimeoutMs)
                    timeout = (uint)Math.Min(timeout * TimeoutMultiplier, MaximumTimeoutMs);
            }
        }
    });

    /// <summary>
    /// Starts listening for console window size changes.
    /// </summary>
    public void Start() => IsListening = true;

    /// <summary>
    /// Stops listening for console window size changes.
    /// </summary>
    public void Stop() => IsListening = false;

    /// <summary>
    /// Disposes the listener asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            Stop();

            IsDisposed = true;

            await _listener;
        }
    }
}
