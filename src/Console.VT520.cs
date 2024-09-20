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
    // TODO : optimize this regex expression to be more efficient
    [GeneratedRegex(@"(\x1b\[|\x9b)([0-\?]*[\x20-\/]*[@-~]|[^@-_]*[@-_]|[\da-z]{1,2};\d{1,2}H)|\x1b([@-_0-\?\x60-~]|[\x20-\/]|[\x20-\/]{2,}[@-~]|[\x30-\x3f]|[\x20-\x2f]+[\x30-\x7e]|\[[\x30-\x3f]*[\x20-\x2f]*[\x40-\x7e])", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex GenerateVT520Regex();



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

    public static string StripVT520Sequences(this string raw_string) => GenerateVT520Regex().Replace(raw_string, "");

    public static MatchCollection MatchVT520Sequences(this string raw_string) => GenerateVT520Regex().Matches(raw_string);

    public static int CountVT520Sequences(this string raw_string) => GenerateVT520Regex().Count(raw_string);

    public static bool ContainsVT520Sequences(this string raw_string) => GenerateVT520Regex().IsMatch(raw_string);

    public static int LengthWithoutVT520Sequences(this string raw_string) => raw_string.Length - MatchVT520Sequences(raw_string).Sum(m => m.Length);


}
