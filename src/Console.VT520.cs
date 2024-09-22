using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Text;
using System;

using Unknown6656.Generics;
using Unknown6656.Runtime;

namespace Unknown6656.Console;


public static unsafe partial class Console
{
    /// <summary>
    /// Indicates whether the current console supports ANSI/VT100/VT520 escape sequences.
    /// </summary>
    public static bool SupportsVT520EscapeSequences => !OS.IsWindows || Environment.OSVersion.Version is { Major: >= 10, Build: >= 16257 };

#pragma warning disable CA1416 // Validate platform compatibility

    /// <summary>
    /// Indicates whether ANSI/VT100/VT520 escape sequences are enabled on the standard input stream.
    /// </summary>
    public static bool AreSTDInVT520EscapeSequencesEnabled => !OS.IsWindows || STDINConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);

    /// <summary>
    /// Indicates whether ANSI/VT100/VT520 escape sequences are enabled on the standard output stream.
    /// </summary>
    public static bool AreSTDOutVT520EscapeSequencesEnabled => !OS.IsWindows || STDOUTConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);

    /// <summary>
    /// Indicates whether ANSI/VT100/VT520 escape sequences are enabled on the standard error stream.
    /// </summary>
    public static bool AreSTDErrVT520EscapeSequencesEnabled => !OS.IsWindows || STDERRConsoleMode.HasFlag(ConsoleMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING);

#pragma warning restore CA1416

    /// <summary>
    /// Enables or disables the given VT520 <paramref name="mode"/>.
    /// </summary>
    /// <param name="mode">The mode to set.</param>
    /// <param name="bit">The bit value to set (<see langword="true"/> to enable, <see langword="false"/> to disable).</param>
    public static void SetVT520Bit(int mode, bool bit) => Write($"\e[?{mode.ToString(CultureInfo.InvariantCulture)}{(bit ? 'h' : 'l')}");

    /// <summary>
    /// Gets the VT520 private DEC mode.
    /// </summary>
    /// <param name="mode">The mode to get.</param>
    /// <returns>
    /// A nullable boolean indicating the mode status:
    /// <see langword="true"/> if the mode is enabled,
    /// <see langword="false"/> if the mode is disabled,
    /// <see langword="null"/> if the mode status is unknown or the <paramref name="mode"/> is not defined.
    /// </returns>
    public static bool? GetVT520PrivateDECMode(int mode)
    {
        if (GetRawVT520Report($"?{mode.ToString(CultureInfo.InvariantCulture)}$p", 'y') is ['\e', '[', '?', .. string response, '$']
                && response.Split(';') is [_, string value, ..])
            return value switch
            {
                "1" or "3" => true,
                "2" or "4" => false,
                _ => null
            };

        return null;
    }

    /// <summary>
    /// Gets the raw VT520 report.
    /// </summary>
    /// <param name="report_sequence">The report sequence to send. (the leading <c>\e</c> does not need to be included)</param>
    /// <param name="terminator">The character that terminates the report.</param>
    /// <returns>
    /// The raw VT520 report string, or <see langword="null"/> if an error occurs.
    /// <para/>
    /// Please note that the returned string does include the leading <c>\e</c>, but does <b>NOT</b> include the <paramref name="terminator"/> character.
    /// </returns>
    public static string? GetRawVT520Report(string report_sequence, char terminator)
    {
        Write($"\e{report_sequence.TrimStart('\e')}");

        try
        {
            string response = "";

            while (KeyAvailable && ReadKey(true).KeyChar is char c && c != terminator)
                response += c;

            return response;
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// Gets the raw DCS VT520 settings report.
    /// </summary>
    /// <param name="report_sequence">
    /// The report sequence to send. Note that this sequence must only the report-specific characters.
    /// Leading <c>\eP$q</c> and trailing <c>\e\</c> characters are to be omitted.
    /// </param>
    /// <param name="response_introducer">The response introducer character. If this value is set to <see langword="null"/>, the introducer character is ignored. The default value is <c>r</c>.</param>
    /// <returns>
    /// The raw DCS VT520 settings report string, or <see langword="null"/> if an error occurs.
    /// This string does <b>NOT</b> include the leading <c>\eP...$</c> and trailing <c>\e\</c> characters.
    /// </returns>
    public static string? GetRawVT520SettingsReport(string report_sequence, char? response_introducer = 'r')
    {
        if (GetRawVT520Report($"\eP$q{report_sequence}\e\\", '\\') is ['\e', 'P', _, '$', char ri, .. string response, '\e', '\\'] &&
            (response_introducer is null || ri == response_introducer))
            return response.TrimEnd(report_sequence);

        return null;
    }

    public static void ChangeVT520ForBufferArea(ConsoleArea area, IEnumerable<string> modes) => ChangeVT520ForBufferArea(area, modes.StringJoin(";"));

    public static void ChangeVT520ForBufferArea(ConsoleArea area, string modes) =>
        Write($"\e[{area.Top};{area.Left};{area.Bottom};{area.Right};{modes.Trim(';')}$r");

    public static string[]? GetRawVT520GraphicRenditions() => GetRawVT520SettingsReport("m")?.Split(';');

    /// <summary>
    /// Generates a VT100/VT520/ANSI color string for the given <paramref name="color"/> and foreground/background flag.
    /// <para/>
    /// <b>
    ///     Please do note that this string does NOT start with <c>\e[</c> and does NOT end with <c>m</c>.
    ///     This has still to be done by the caller. The reason for this is that <see cref="GenerateVT520ColorString"/> is intended to be used in combination with other VT520 escape sequences.
    /// </b>
    /// </summary>
    /// <param name="color">The color to be rendered as VT520 code. This may be an instance of <see cref="ConsoleColor"/> or <see cref="Color"/>.</param>
    /// <param name="foreground">
    ///     Indicates whether the given <paramref name="color"/> is a foreground color.
    ///     A value of <see langword="true"/> indicates a foreground color, while a value of <see langword="false"/> indicates a background color.
    ///     A value of <see langword="null"/> indicates a color for underline coloring.
    /// </param>
    /// <returns>The VT100/VT520/ANSI color string for the given <paramref name="color"/>.</returns>
    public static string GenerateVT520ColorString(Union<ConsoleColor, Color>? color, bool? foreground)
    {
        if (color?.Is(out ConsoleColor cc) ?? false)
        {
            (bool bright, ConsoleColor normalized) = cc switch
            {
                ConsoleColor.Black => (false, cc),
                ConsoleColor.DarkBlue => (false, cc),
                ConsoleColor.DarkGreen => (false, cc),
                ConsoleColor.DarkCyan => (false, cc),
                ConsoleColor.DarkRed => (false, cc),
                ConsoleColor.DarkMagenta => (false, cc),
                ConsoleColor.DarkYellow => (false, cc),
                ConsoleColor.Gray => (false, cc),
                ConsoleColor.DarkGray => (true, ConsoleColor.Black),
                ConsoleColor.Blue => (true, ConsoleColor.DarkBlue),
                ConsoleColor.Green => (true, ConsoleColor.DarkGreen),
                ConsoleColor.Cyan => (true, ConsoleColor.DarkCyan),
                ConsoleColor.Red => (true, ConsoleColor.DarkRed),
                ConsoleColor.Magenta => (true, ConsoleColor.DarkMagenta),
                ConsoleColor.Yellow => (true, ConsoleColor.DarkYellow),
                ConsoleColor.White => (true, ConsoleColor.Gray),
                _ => (false, cc),
            };

            return $"{(foreground, bright) switch
            {
                (true, true) => "9",
                (true, false) => "3",
                (false, true) => "10",
                (false, false) => "4",
            }}{(int)normalized}";
        }
        else if (color?.Is(out Color rgb) ?? false)
            return $"{foreground switch
            {
                true => "38",
                false => "48",
                _ => "58",
            }}:2:{rgb.R}:{rgb.G}:{rgb.B}";

        return foreground switch
        {
            true => "39",
            false => "49",
            _ => "59",
        };
    }
}

/// <summary>
/// Provides methods for working with VT100/VT520/ANSI escape sequences.
/// </summary>
public static partial class VT520
{
    // TODO : match sequences using the ST-token at the end

    [GeneratedRegex(
        @"(\x1b\[|\x9b)([\x30-\x3f]*[\x20-\x2f]*[\x40-\x7e]|([^\x40-\x5f]*|[\x30-\x3f\x60-\x7e]+[\x30-\x3f]+)[\x40-\x5f])|\x1b([\x20-\x7e]|[\x20-\x2f]{2,}[\x40-\x7e]|[\x20-\x2f]+[\x30-\x7e])",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    )]
    private static partial Regex GenerateVT520Regex();


    /// <summary>
    /// Splits the given string into lines and wraps them at the given maximum width.
    /// This function is VT520-aware and will not split sequences nor include them in the line length.
    /// Furthermore, the function respects non-printable characters and will not count them towards the line length.
    /// </summary>
    /// <param name="lines">The input text lines (which may contain VT520 escape sequences).</param>
    /// <param name="max_width">The maximum line length.</param>
    /// <param name="wrap_overflow">Indicates whether wrapping overflow is enabled. If this value is set to <see langword="false"/>, the line will be cut at a maximum length of <paramref name="max_width"/> printable characters.</param>
    /// <returns>The list of split/wrapped/broken lines.</returns>
    public static List<string> SplitLinesWithVT520(List<string> lines, int max_width, bool wrap_overflow = true)
    {
        return [..from line in lines
                  from processed in process(line, max_width)
                  select processed];

        List<string> process(string line, int max_width)
        {
            StringBuilder curr = new();
            List<string> result = [];
            int len = 0;

            for (int i = 0; i < line.Length; ++i)
            {
                char c = line[i];

                if (c is '\e' or '\x9b' && GenerateVT520Regex().Match(line, i) is { Success: true } match)
                {
                    curr.Append(line.AsSpan(i, match.Length));
                    i += match.Length - 1;
                }
                else
                {
                    curr.Append(c);
                    len += c is '\x7f'
                             or '\x81'
                             or '\x8d'
                             or '\x8f'
                             or '\x90'
                             or '\x9d'
                             or (>= '\x00' and <= '\x08')
                             or (>= '\x0b' and <= '\x0c')
                             or (>= '\x0e' and <= '\x1f') ? 0 : 1;

                    if (len >= max_width)
                    {
                        result.Add(curr.ToString());

                        if (!wrap_overflow)
                            return result;

                        curr.Clear();
                        len = 0;
                    }
                }
            }

            if (curr.Length > 0)
                result.Add(curr.ToString());

            return result;
        }
    }

    /// <summary>
    /// Strips all (recognized) VT520 escape sequences from the given string.
    /// </summary>
    /// <param name="input">The raw input string</param>
    /// <returns>Returns the string with all VT520 escape sequences removed.</returns>
    public static string StripVT520Sequences(this string input) => GenerateVT520Regex().Replace(input, "");

    /// <summary>
    /// Matches all VT520 escape sequences in the given input string.
    /// </summary>
    /// <param name="input">The input string to search for VT520 escape sequences.</param>
    /// <returns>A collection of matches where each match represents a VT520 escape sequence.</returns>
    public static MatchCollection MatchVT520Sequences(this string input) => GenerateVT520Regex().Matches(input);

    /// <summary>
    /// Counts the number of VT520 escape sequences in the given input string.
    /// </summary>
    /// <param name="input">The input string to search for VT520 escape sequences.</param>
    /// <returns>The number of VT520 escape sequences found in the input string.</returns>
    public static int CountVT520Sequences(this string input) => GenerateVT520Regex().Count(input);

    /// <summary>
    /// Determines whether the given input string contains any VT520 escape sequences.
    /// </summary>
    /// <param name="input">The input string to search for VT520 escape sequences.</param>
    /// <returns><see langword="true"/> if the input string contains any VT520 escape sequences; otherwise, <see langword="false"/>.</returns>
    public static bool ContainsVT520Sequences(this string input) => GenerateVT520Regex().IsMatch(input);

    /// <summary>
    /// Calculates the length of the input string excluding any VT520 escape sequences.
    /// Please do note that any non-printable characters are still counted towards the string length.
    /// </summary>
    /// <param name="input">The input string to measure.</param>
    /// <returns>The length of the input string excluding VT520 escape sequences.</returns>
    public static int LengthWithoutVT520Sequences(this string input) => input.Length - MatchVT520Sequences(input).Sum(m => m.Length);

    public static bool StartsWithVT520Sequence(this string input, [NotNullWhen(true), MaybeNullWhen(false)] out string? sequence)
    {
        sequence = null;

        if (GenerateVT520Regex().Match(input) is { Success: true, Index: 0, Value: string seq })
            sequence = seq;

        return sequence is { };
    }
}