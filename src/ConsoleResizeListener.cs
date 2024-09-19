using System.Threading.Tasks;
using System;

namespace Unknown6656.Runtime.Console;

using Console = System.Console;


public delegate void ConsoleSizeChangedEventHandler(int oldWidth, int oldHeight, int newWidth, int newHeight);

public sealed class ConsoleResizeListener
    : IAsyncDisposable
{
    private readonly Task _listener;


    public bool IsDisposed { get; private set; }

    public bool IsListening { get; private set; }

    public uint MinimumTimeoutMs { get; set; } = 100;

    public uint MaximumTimeoutMs { get; set; } = 1000;

    public float TimeoutMultiplier { get; set; } = 1.3f;


    public event ConsoleSizeChangedEventHandler? SizeChanged;


    public ConsoleResizeListener() => _listener = Task.Factory.StartNew(async delegate
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        uint timeout = MinimumTimeoutMs;

        while (!IsDisposed)
        {
            bool change = false;

            if (IsListening)
            {
                (int nw, int nh, int ow, int oh) = (Console.WindowWidth, Console.WindowHeight, width, height);

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

    public void Start() => IsListening = true;

    public void Stop() => IsListening = false;

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
