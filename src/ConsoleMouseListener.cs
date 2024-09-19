using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Linq;
using System;

using Unknown6656.Generics;

namespace Unknown6656.Runtime.Console;


public delegate void ConsoleMouseEventHandler(int x, int y, MouseButtons buttons, ModifierKeysState modifiers);

// TODO : implement for POSIX

[SupportedOSPlatform(OS.WIN)]
public static class ConsoleMouseListener
{
    private static volatile bool _running = false;


    public static bool IsRunning => _running;

    public static event ConsoleMouseEventHandler? MouseMove;
    public static event ConsoleMouseEventHandler? MouseDoubleClick;
    public static event ConsoleMouseEventHandler? MouseHorizontalWheel;
    public static event ConsoleMouseEventHandler? MouseVerticalWheel;
    // TODO : key events


    public static void Start()
    {
        if (!_running)
        {
            _running = true;

            Task.Factory.StartNew(async delegate
            {
                nint handle;
                unsafe
                {
                    handle = (nint)ConsoleExtensions.STDINHandle;
                }
                ConsoleMode mode = ConsoleExtensions.STDINConsoleMode;
                ConsoleExtensions.STDINConsoleMode = (mode | ConsoleMode.ENABLE_MOUSE_INPUT
                                                           | ConsoleMode.ENABLE_WINDOW_INPUT
                                                           | ConsoleMode.ENABLE_EXTENDED_FLAGS)
                                                          & ~ConsoleMode.ENABLE_QUICK_EDIT_MODE;
                ConsoleExtensions.MouseEnabled = true;

                while (_running)
                    if (NativeInterop.GetNumberOfConsoleInputEvents(handle, out int count))
                        try
                        {
                            List<INPUT_RECORD> records = Enumerable.Repeat(new INPUT_RECORD(), count).ToList();
                            NativeInterop.ReadConsoleInput(handle, records.GetInternalArray(), count, out int read);

                            if (read < records.Count)
                                records.RemoveRange(read, records.Count - read);

                            for (int i = 0; i < records.Count; ++i)
                                if (records[i] is { EventType: EventType.MouseEvent, MouseEvent: { } @event })
                                {
                                    (@event.dwEventFlags switch
                                    {
                                        MouseActions.Movement => MouseMove,
                                        MouseActions.DoubleClick => MouseDoubleClick,
                                        MouseActions.Wheel => MouseVerticalWheel,
                                        MouseActions.HorizontalWheel => MouseHorizontalWheel,
                                        _ => null
                                    })?.Invoke(@event.wMousePositionX, @event.wMousePositionY, @event.dwButtonState, @event.dwControlKeyState);
                                    records.RemoveAt(i--);
                                }
                                // TODO : key event

                            if (records.Count > 0)
                                NativeInterop.WriteConsoleInput(handle, [.. records], records.Count, out _);
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }
                    else
                        await Task.Delay(10);

                ConsoleExtensions.STDINConsoleMode = mode;
            });
        }
    }

    public static void Stop() => _running = false;
}
