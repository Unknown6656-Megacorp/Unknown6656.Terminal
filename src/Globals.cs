#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
global using sysconsole = System.Console;
global using sysconsolecolor = System.ConsoleColor;

using System;
using System.IO;

#pragma warning restore CS8981

using System.Runtime.CompilerServices;

using Unknown6656.Runtime;

namespace Unknown6656.Console;


public static class LibGDIPlus
{
    private static string[] LibraryCandidates = [
        "libgdiplus.so",
        "libgdiplus.so.0",
        "libgdiplus.so.0.0.0",
        "libgdiplus.dll.so",
        "libgdiplus.dll.so.0",
    ];
    private const string USR_LIB = "/usr/lib";


    public static bool IsAvailable => FileInfo is not null;

    public static FileInfo? FileInfo { get; private set; } = null;


#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void ResolveLibGDIPlus()
    {
        static FileInfo? resolve()
        {
            if (OS.IsWindows)
                return new("gdi32.dll");

            FileInfo? fi = null;

            try
            {
                foreach (string path in LibraryCandidates)
                    if ((fi = new(path)).Exists)
                        return fi;
            }
            catch
            {
            }

            try
            {
                foreach (string path in LibraryCandidates)
                    if ((fi = new($"{USR_LIB}/{path}")).Exists)
                        break;
            }
            catch
            {
            }

            if (fi?.Exists ?? false)
            {
                Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", USR_LIB);
                Environment.SetEnvironmentVariable("MONO_PATH", USR_LIB);
                Environment.SetEnvironmentVariable("PATH", $"{Environment.GetEnvironmentVariable("PATH")}:{USR_LIB}");

                return fi;
            }

            // TODO : ?

            return null;
        }

        FileInfo = resolve();
    }
}
