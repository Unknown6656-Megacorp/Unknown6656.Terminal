using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Linq;
using System;

using Unknown6656.Generics;
using Unknown6656.Runtime;

namespace Unknown6656.Console;


/// <summary>
/// Represents the method that will handle mouse events in the console.
/// </summary>
/// <param name="x">The x-coordinate of the mouse cursor.</param>
/// <param name="y">The y-coordinate of the mouse cursor.</param>
/// <param name="buttons">The state of the mouse buttons.</param>
/// <param name="modifiers">The state of the modifier keys.</param>
public delegate void ConsoleMouseEventHandler(int x, int y, MouseButtons buttons, ModifierKeysState modifiers);

// TODO : implement for POSIX


/// <summary>
/// A listening service for handling mouse events in the console.
/// <para/>
/// <i>Please note that this class is currently only supported on the Microsoft Windows operating system. POSIX support will follow in due course.</i>
/// </summary>
[SupportedOSPlatform(OS.WIN)]
public static class ConsoleMouseListener
{
    private static volatile bool _running = false;

    /// <summary>
    /// Indicates whether the listening service is currently running.
    /// </summary>
    public static bool IsRunning => _running;


    /// <summary>
    /// Occurs when the mouse is moved.
    /// </summary>
    public static event ConsoleMouseEventHandler? MouseMove;

    /// <summary>
    /// Occurs when the mouse is double-clicked.
    /// </summary>
    public static event ConsoleMouseEventHandler? MouseDoubleClick;

    /// <summary>
    /// Occurs when the mouse wheel is scrolled horizontally.
    /// </summary>
    public static event ConsoleMouseEventHandler? MouseHorizontalWheel;

    /// <summary>
    /// Occurs when the mouse wheel is scrolled vertically.
    /// </summary>
    public static event ConsoleMouseEventHandler? MouseVerticalWheel;

    // TODO : key events


    /// <summary>
    /// Starts the mouse event listening service.
    /// The following events will be raised when the corresponding action is performed:
    /// <list type="bullet">
    ///     <item><see cref="MouseMove"/> when the mouse is moved.</item>
    ///     <item><see cref="MouseDoubleClick"/> when the mouse is double-clicked.</item>
    ///     <item><see cref="MouseHorizontalWheel"/> when the mouse wheel is scrolled horizontally.</item>
    ///     <item><see cref="MouseVerticalWheel"/> when the mouse wheel is scrolled vertically.</item>
    /// </list>
    /// </summary>
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

    /// <summary>
    /// Stops the mouse event listening service.
    /// </summary>
    public static void Stop() => _running = false;
}
