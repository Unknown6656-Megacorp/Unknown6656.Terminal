using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Unknown6656.Generics;

namespace Unknown6656.Console;


public static unsafe partial class Console
{
    public static void SetVT520Bit(int mode, bool bit) => Write($"\e[?{mode.ToString(CultureInfo.InvariantCulture)}{(bit ? 'h' : 'l')}");

    public static string? GetRawVT520Report(string report_sequence, char terminator)
    {
        Write($"\e{report_sequence}");

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

    public static string? GetRawVT520SettingsReport(string report_sequence, char? response_introducer = 'r')
    {
        if (GetRawVT520Report($"\eP$q{report_sequence}\e\\", '\\') is ['\e', 'P', _, '$', char ri, .. string response, '\e', '\\'] &&
            (response_introducer is null || ri == response_introducer))
            return response.TrimEnd(report_sequence);

        return null;
    }

    public static void ChangeVT520ForArea(ConsoleArea area, IEnumerable<int> modes) => ChangeVT520ForArea(area, modes.StringJoin(";"));

    public static void ChangeVT520ForArea(ConsoleArea area, string modes) =>
        Write($"\e[{area.Top};{area.Left};{area.Bottom};{area.Right};{modes.Trim(';')}$r");

    public static string[]? GetRawVT520GraphicRenditions() => GetRawVT520SettingsReport("m")?.Split(';');
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
}
