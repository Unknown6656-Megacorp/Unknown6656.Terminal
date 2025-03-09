using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System;

using Microsoft.Win32;

using Unknown6656.Runtime;
using System.CodeDom;

namespace Unknown6656.Terminal;


public enum TerminalType
{
    WindowsCMD,             // Windows
    PowerShell,             // Windows, macOS, Linux
    WindowsTerminal,        // Windows
    MacOSTerminalApp,       // macOS
    iTerm2,                 // macOS
    Alacritty,              // Windows, macOS, Linux
    Hyper,                  // Windows, macOS, Linux
    WezTerm,                // Windows, macOS, Linux
    XTerm,                  // Linux, BSD, Unix
    GNOMETerminal,          // Linux
    Konsole,                // Linux
    XFCETerminal,           // Linux
    Tilix,                  // Linux
    Guake,                  // Linux
    Yakuake,                // Linux
    Kitty,                  // Windows, macOS, Linux
    Cygwin,                 // Windows
    MSYS2,                  // Windows
    URXVT,                  // Linux, BSD
    SimpleTerminal,         // Linux, BSD
    Terminator,             // Linux
    Warp,                   // macOS
    Zellij,                 // macOS, Linux
    GenericLinux,           // Linux
}

public record TerminalEmulatorInfo(TerminalType Type, Version? Version, FileInfo? Path)
{
    public bool HasSixelSupport => Type switch
    {
        TerminalType.Alacritty or TerminalType.Kitty or TerminalType.URXVT or TerminalType.GNOMETerminal or TerminalType.GenericLinux => false,
        TerminalType.XTerm or TerminalType.Cygwin => true,
        TerminalType.Zellij when Version >= new Version(0, 31) => true,
        TerminalType.Konsole when Version >= new Version(22, 4) => true,
        TerminalType.WindowsTerminal when Version >= new Version(1, 22) => true,

        TerminalType.WindowsCMD when Version >= new Version(9999, 9999) => true, // TODO : check for actual version


        // https://www.arewesixelyet.com/
    };

    public override string ToString() => $"{Type} v.{Version} ({Path})";
}

public static unsafe partial class Console
{
    private static TerminalEmulatorInfo? _termnfo = null;

    public static TerminalEmulatorInfo CurrentTerminalEmulatorInfo => _termnfo ??= GetCurrentTerminalEmulatorInfo();


    private static TerminalEmulatorInfo GetCurrentTerminalEmulatorInfo()
    {
        TerminalType? type = null;
        Process? parent_proc = ProcessTree.GetParentProcess();
        string? parent_procname = parent_proc?.ProcessName?.ToLowerInvariant();
        string? term = Environment.GetEnvironmentVariable("TERM")?.ToLowerInvariant();
        string? uname_o = OS.GetUname('o');


        bool any(Func<bool> cond) => type is null && cond();
        bool win(Func<bool> cond) => type is null && OS.IsWindows && cond();
        bool nix(Func<bool> cond) => type is null && OS.IsUnix && cond();
        bool mac(Func<bool> cond) => type is null && OS.IsOSX && cond();
        bool lin(Func<bool> cond) => type is null && OS.IsLinux && cond();


        // check for mintty before cygwin/msys2

        if (any(() => Environment.GetEnvironmentVariable("CYGWIN") != null || Directory.Exists("/cygdrive")))
            type = TerminalType.Cygwin;

        if (any(() => Environment.GetEnvironmentVariable("MSYSTEM") != null || (uname_o?.Contains("MSYS") ?? false)))
            type = TerminalType.MSYS2;

        if (win(() => Environment.GetEnvironmentVariable("WT_SESSION") != null || Environment.GetEnvironmentVariable("WT_PROFILE_ID") != null || parent_procname is "windowsterminal" or "wt"))
            type = TerminalType.WindowsTerminal;

        if (win(() => parent_procname is { } && (parent_procname.Contains("powershell") || parent_procname.Contains("pwsh"))))
            type = TerminalType.PowerShell;

        if (mac(() => Environment.GetEnvironmentVariable("TERM_PROGRAM") is "Apple_Terminal"))
            type = TerminalType.MacOSTerminalApp;

        if (mac(() => Environment.GetEnvironmentVariable("TERM_PROGRAM") is "iTerm.app" || !string.IsNullOrWhiteSpace(GetRawVT520Report($"{_DCS}+q6E616D65;544E{_ST}", '\0'))))
            type = TerminalType.iTerm2;

        if (nix(() => term is "xterm" or "xterm-256color" || Environment.GetEnvironmentVariable("XTERM_VERSION") != null))
            type = TerminalType.XTerm;



        if (term is "alacritty")
            type = TerminalType.Alacritty;
        else if (Environment.GetEnvironmentVariable("TERM_PROGRAM")?.Equals("WezTerm", StringComparison.OrdinalIgnoreCase) ?? false)
            type = TerminalType.WezTerm;
        else if (Environment.GetEnvironmentVariable("KITTY_WINDOW_ID") != null)
            type = TerminalType.Kitty;
        else if (Environment.GetEnvironmentVariable("COLORTERM") is "gnome-terminal")
            type = TerminalType.GNOMETerminal;
        else if (File.Exists("/proc/sys/fs/binfmt_misc/WSLInterop") || (OS.GetUname('r')?.Contains("microsoft") ?? false) || Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") != null)
        {
            // running inside WSL. check for parent process to determine terminal type.
            if (parent_procname is { } par)
                if (parent_procname is "conhost")
                    type = TerminalType.WindowsCMD;

            if (type is null)
            {
                // ????
            }

            type ??= TerminalType.GenericLinux;
        }

        // Black Box
        // Bobcat
        // ConEmu
        // Contour
        // ctx terminal
        // DarkTile
        // DomTerm
        // Eat
        // Elementary Terminal
        // foot
        // Hyper
        // LaTerminal
        // MacTerm
        // mintty
        // mlterm
        // mobaxterm
        // PuTTY
        // rio terminal
        // rlogin
        // sixel-tmux
        // suckless st
        // swift term
        // SyncTERM
        // TeraTerm
        // Terminology
        // Termite
        // termux
        // Tilda
        // toyterm
        // U++
        // vscode
        // xfce-terminal
        // yaft
        // Yakuake
        // Zellij
        // URXVT
        // SimpleTerminal
        // Terminator
        // Konsole
        // XFCETerminal
        // Tilix
        // Guake
        // Yakuake
        // tmux?
        // Warp

        if (type is null)
            if (OS.IsWindows)
                type = TerminalType.WindowsCMD; // highly likely
            else if (OS.IsOSX)
                type = TerminalType.MacOSTerminalApp; // highly likely
            else
                type = TerminalType.GenericLinux;

        (Version? version, FileInfo? path) = type switch
        {
            TerminalType.WindowsCMD => (GetWindowsCMDVersion(), GetWindowsCMDPath()),
            TerminalType.WindowsTerminal => (GetWindowsTerminalVersion(), GetWindowsTerminalPath()),

            _ => (null, null),
        };

        if (OS.IsUnix && path is null && parent_proc?.Id is int ppid && new FileInfo($"/proc/{ppid}/exe").LinkTarget is string tgt)
            path = new(tgt);

        return new(type ?? TerminalType.GenericLinux, version, path);
    }

    /// <summary>
    /// Gets the version of the Windows Terminal.
    /// </summary>
    /// <returns>The version of the Windows Terminal if found; otherwise, <see langword="null"/>.</returns>
    public static Version? GetWindowsTerminalVersion()
    {
        if (GetWindowsTerminalPath() is FileInfo path)
        {
            string? raw_version = path.FullName;

            try
            {
                raw_version = FileVersionInfo.GetVersionInfo(path.FullName).FileVersion;
            }
            catch
            {
                raw_version = REGEX_VersionString().Match(raw_version ?? "")?.Value;
            }

            if (Version.TryParse(raw_version, out Version? version))
                return version;
        }

        return null;
    }

    /// <summary>
    /// Gets the version of the Windows CMD.
    /// </summary>
    /// <returns>The version of the CMD if found; otherwise, <see langword="null"/>.</returns>
    public static Version? GetWindowsCMDVersion()
    {
        if (GetWindowsCMDPath() is FileInfo path)
        {
            string? raw_version = FileVersionInfo.GetVersionInfo(path.FullName).FileVersion;

            if (Version.TryParse(raw_version, out Version? version))
                return version;
        }

        return null;
    }

    /// <summary>
    /// Gets the file path of the Windows Terminal executable (usually the full path to `<c>wt.exe</c> or <c>WindowsTerminal.exe</c>`).
    /// </summary>
    /// <returns>The file path of the Windows Terminal executable if found; otherwise, <see langword="null"/>.</returns>
    public static FileInfo? GetWindowsTerminalPath()
    {
        if (OS.IsWindows)
            try
            {
                string[] appdirs = [
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                ];

#pragma warning disable CA1416 // Validate platform compatibility
                if (Registry.CurrentUser.OpenSubKey(@"\Software\Microsoft\Windows\CurrentVersion\App Paths\wt.exe") is RegistryKey key
                    && key.GetValue(null) is string path
                    && new FileInfo(path) is { Exists: true } file)
                    return file;

                foreach (Process proc in Process.GetProcesses())
                    try
                    {
                        if (proc.MainModule is { } module && module.ModuleName.ToLowerInvariant() is "wt.exe" or "windowsterminal.exe")
                        {
                            path = module.FileName;

                            if (appdirs.Any(dir => path.StartsWith(dir, StringComparison.OrdinalIgnoreCase)))
                                return new(path);
                        }
                    }
                    catch
                    {
                    }
#pragma warning restore CA1416
            }
            catch
            {
            }

        return null;
    }

    /// <summary>
    /// Gets the file path of the Windows Command Prompt executable (usually the full path to `<c>cmd.exe</c>`).
    /// </summary>
    /// <param name="allow_windows_terminal">If <see langword="true"/>, allows returning the Windows Terminal path if the Command Prompt path is not found.</param>
    /// <returns>The file path of the Windows Command Prompt executable if found; otherwise, <see langword="null"/>.</returns>
    public static FileInfo? GetWindowsCMDPath(bool allow_windows_terminal = false)
    {
        if (Environment.GetEnvironmentVariable("ComSpec") is { Length: >= 9 } path)
            return new(path);
        else if (allow_windows_terminal)
            return GetWindowsTerminalPath();
        else
            return null;
    }



    [GeneratedRegex(@"(\d+\.\d+\.\d+\.\d+)", RegexOptions.Compiled)]
    private static partial Regex REGEX_VersionString();
}
