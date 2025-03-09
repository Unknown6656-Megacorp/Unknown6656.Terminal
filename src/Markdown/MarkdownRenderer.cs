using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System;

using Unknown6656.Generics;
using System.Drawing;

namespace Unknown6656.Terminal.Markdown;


// https://spec.commonmark.org/0.31.2/



public record MarkdownRenderOptions
{
    public static MarkdownRenderOptions Default { get; } = new()
    {
        BackgroundColor = ConsoleColor.Default,
        TextColor = 0xDDD,
        CodeColor = 0x889,
        BoldColor = ConsoleColor.White,
        HeaderColor = 0xFFD,
        LinkColor = ConsoleColor.Cyan,
        LinkUnderline = true,
        Indentation = 0,
        MaxWidth = null,
    };

    public required ConsoleColor TextColor { get; init; }
    public required ConsoleColor BoldColor { get; init; }
    public required ConsoleColor CodeColor { get; init; }
    public required ConsoleColor HeaderColor { get; init; }
    public required ConsoleColor LinkColor { get; init; }
    public required bool LinkUnderline { get; init; } 
    public required ConsoleColor BackgroundColor { get; init; }
    public int Indentation { get; init; } = 0;
    public int? MaxWidth { get; init; } = null;
}

static partial class MarkdownUtils
{
    [GeneratedRegex(@"[#_*\\<>~\[\]`\.]", RegexOptions.Compiled)]
    private static partial Regex EscapeRegex();

    [GeneratedRegex(@"&#((?<dec>\d+)|x(?<hex>[a-fA-F\d]+));", RegexOptions.Compiled)]
    private static partial Regex EntityRegex();

    private static readonly Dictionary<string, string> _entity_names = new()
    {
         ["&amp;"] = "&",
         ["&lt;"] = "<",
         ["&gt;"] = ">",
         ["&quot;"] = "\"",
         ["&apos;"] = "'",
         ["&nbsp;"] = " ",
        // TODO : add all entities from https://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references
    };



    public static string Escape(string content) => EscapeRegex().Replace(content, @"\$0");

    public static string ResolveHTMLEntity(string entity)
    {
        string repl = EntityRegex().Replace(entity, m =>
        {
            if (m.Groups["dec"].Success)
                return char.ConvertFromUtf32(int.Parse(m.Groups["dec"].Value));
            else if (m.Groups["hex"].Success)
                return char.ConvertFromUtf32(int.Parse(m.Groups["hex"].Value, NumberStyles.HexNumber));

            return m.Value;
        });

        if (entity == repl && !_entity_names.TryGetValue(entity, out repl!))
            repl ??= entity;

        return repl;
    }

    public static string ApplyFormatStyles(MarkdownTextNodeStyle styles, object? value)
    {
        string str = value?.ToString() ?? "";

        foreach ((MarkdownTextNodeStyle style, string pre, string post) in new[]
        {
            (MarkdownTextNodeStyle.Bold, "**", "**"),
            (MarkdownTextNodeStyle.Italic, "_", "_"),
            (MarkdownTextNodeStyle.Underlined, "<u>", "</u>"),
            (MarkdownTextNodeStyle.Strikethrough, "~~", "~~"),
            (MarkdownTextNodeStyle.Superscript, "<sup>", "</sup>"), // "^", "^"
            (MarkdownTextNodeStyle.Subscript, "<sub>", "</sub>"), // "~", "~"
            (MarkdownTextNodeStyle.InlineCode, "`", "`"),
        })
            if (styles.HasFlag(style))
                str = pre + str + post;

        return str;
    }
}

public abstract class MarkdownElement
{
    /// <summary>
    /// Exports the current section to a markdown string.
    /// </summary>
    /// <returns>The markdown string representing the current section.</returns>
    public abstract string Export();

    /// <summary>
    /// Renders the current section to a VT520 sequence string based on the given render options.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public abstract void Print(MarkdownRenderOptions options);
}

public abstract class MarkdownSection : MarkdownElement;

public class MarkdownDocument
    : MarkdownSection
    , IEnumerable<MarkdownSection>
{
    public List<MarkdownSection> Sections { get; }


    public MarkdownDocument() => Sections = [];

    public MarkdownDocument(params MarkdownSection[] sections) => Sections = [.. sections];

    public void Add(MarkdownSection section) => Sections.Add(section);

    public MarkdownDocument Clone() => new([.. Sections]);

    public MarkdownDocument Concat(MarkdownDocument other) => new([.. Sections, .. other.Sections]);

    /// <inheritdoc/>
    public override string Export() => Sections.Select(s => s.Export()).StringJoin("\n\n");

    /// <inheritdoc cref="MarkdownSection.Print(MarkdownRenderOptions)"/>
    public void Print() => Print(MarkdownRenderOptions.Default);

    /// <inheritdoc/>
    public override void Print(MarkdownRenderOptions options)
    {
        options = options with
        {
            MaxWidth = options.MaxWidth ?? Console.WindowWidth,
        };

        ConsoleGraphicRendition? renditions = Console.CurrentGraphicRendition;
        Console.BackgroundColor = options.BackgroundColor;

        foreach (MarkdownSection section in Sections)
        {
            section.Print(options);
            Console.WriteLine();
        }

        Console.CurrentGraphicRendition = renditions;
    }

    public static MarkdownDocument Parse(string markdown_source)
    {
        markdown_source = markdown_source.Replace("\r\n", "\n")
                                         .Replace('\t', ' ')
                                         .Replace("\r", "")
                                         .Replace('\0', '\ufffd');

        throw new NotImplementedException();
    }

    IEnumerator<MarkdownSection> IEnumerable<MarkdownSection>.GetEnumerator() => Sections.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Sections.GetEnumerator();
}

public enum MarkdownHeaderLevel
{
    H1,
    H2,
    H3,
    H4,
    H5,
    H6,
}

public sealed class MarkdownHeader
    : MarkdownSection
{
    public string Text { get; }
    public MarkdownHeaderLevel Level { get; }


    public MarkdownHeader(string text, MarkdownHeaderLevel level)
    {
        Text = VT520.StripVT520Sequences(text).Trim();
        Level = level;
    }

    /// <inheritdoc/>
    public override string Export() => $"{Level switch
    {
        MarkdownHeaderLevel.H1 => "#",
        MarkdownHeaderLevel.H2 => "##",
        MarkdownHeaderLevel.H3 => "###",
        MarkdownHeaderLevel.H4 => "####",
        MarkdownHeaderLevel.H5 => "#####",
        MarkdownHeaderLevel.H6 => "######",
        _ => throw new ArgumentOutOfRangeException(nameof(Level)),
    }} {MarkdownUtils.Escape(Text)}";

    /// <inheritdoc/>
    public override void Print(MarkdownRenderOptions options)
    {
        ConsoleGraphicRendition? renditions = Console.CurrentGraphicRendition;

        ConsoleColor color = options.HeaderColor | options.BoldColor | options.TextColor;
        Console.CurrentGraphicRendition = new()
        {
            Intensity = TextIntensityMode.Bold,
            Underlined = Level switch
            {
                MarkdownHeaderLevel.H1 or MarkdownHeaderLevel.H4 => TextUnderlinedMode.Double,
                MarkdownHeaderLevel.H2 or MarkdownHeaderLevel.H5 => TextUnderlinedMode.Single,
                _ => TextUnderlinedMode.NotUnderlined,
            },
            ForegroundColor = color,
            BackgroundColor = options.BackgroundColor,
        };

        Console.WriteLine();

        if (Level < MarkdownHeaderLevel.H4)
            Console.WriteDoubleSizeLine(Text);
        else
            Console.WriteLine(Text);

        Console.CurrentGraphicRendition = renditions;
    }
}

public sealed class MarkdownParagraph
    : MarkdownSection
{
    public MarkdownTextElement Content { get; }


    public MarkdownParagraph(IEnumerable<MarkdownTextElement> content) => Content = new MarkdownFormattedText(MarkdownTextNodeStyle.Regular, content);

    public override string Export() => Content.Export();

    public override void Print(MarkdownRenderOptions options) => Content.Print(options);
}

[Flags]
public enum MarkdownTextNodeStyle
    : byte
{
    Regular = 0b_0000_0000,
    Bold = 0b_0000_0001,
    Italic = 0b_0000_0010,
    Underlined = 0b_0000_0100,
    Strikethrough = 0b_0000_1000,
    Superscript = 0b_0001_0000,
    Subscript = 0b_0010_0000,
    InlineCode = 0b_0100_0000,
}

public abstract class MarkdownTextElement
    : MarkdownElement
{
    public abstract string Text { get; }


    public static implicit operator MarkdownTextElement(string text) => new MarkdownPlainText(text);
}

public sealed class MarkdownPlainText
    : MarkdownTextElement
{
    public override string Text { get; }


    public MarkdownPlainText(string text) => Text = text.Trim();

    /// <inheritdoc/>
    public override string Export() => MarkdownUtils.Escape(Text);

    /// <inheritdoc/>
    public override void Print(MarkdownRenderOptions options) => Console.Write(Text);

    /// <inheritdoc cref="MarkdownPlainText(string)"/>
    public static implicit operator MarkdownPlainText(string text) => new(text);

    public static implicit operator string(MarkdownPlainText text) => text.Text;
}

public sealed class MarkdownLineBreak
    : MarkdownTextElement
{
    public override string Text => "\n";

    public override string Export() => "<br/>";

    public override void Print(MarkdownRenderOptions options) => Console.WriteLine();
}

internal sealed class MarkdownSpace
    : MarkdownTextElement
{
    public override string Text => " ";

    public override string Export() => " ";

    public override void Print(MarkdownRenderOptions options) => Console.Write(' ');
}

public class MarkdownFormattedText
    : MarkdownTextElement
{
    public MarkdownTextElement[] Elements { get; }
    public MarkdownTextNodeStyle Style { get; }

    public override string Text => Elements.Select(e => e.Text).StringConcat();


    public MarkdownFormattedText(MarkdownTextNodeStyle style, string text)
    {
        Style = style;
        Elements = (text = text.Trim()).Length > 0 ? [new MarkdownPlainText(text)] : [];
    }

    public MarkdownFormattedText(MarkdownTextNodeStyle style, IEnumerable<MarkdownTextElement> elements)
    {
        List<MarkdownTextElement> elems = [];

        foreach (MarkdownTextElement elem in elements)
        {
            if (elems.Count > 0)
                if (elem is not MarkdownSpace)
                    elems.Add(new MarkdownSpace());
                else if (elem is MarkdownSpace && elems[^1] is MarkdownSpace)
                    continue;

            elems.Add(elem);
        }

        Elements = [..elems];
        Style = style;
    }

    /// <inheritdoc/>
    public override string Export()
    {
        string str = Elements.Select(e => e.Export()).StringJoin(" ");

        return MarkdownUtils.ApplyFormatStyles(Style, str);
    }

    /// <inheritdoc/>
    public override void Print(MarkdownRenderOptions options)
    {
        ConsoleGraphicRendition? curr, old = Console.CurrentGraphicRendition;
        ConsoleGraphicRendition sgr = ConsoleGraphicRendition.Default;

        if (Style.HasFlag(MarkdownTextNodeStyle.Bold))
        {
            sgr = sgr with { Intensity = TextIntensityMode.Bold };

            if ((options.BoldColor | options.TextColor) is { IsDefault: false } fg)
                sgr = sgr with { ForegroundColor = fg };
        }

        if (Style.HasFlag(MarkdownTextNodeStyle.Italic))
            sgr = sgr with { IsItalic = true };

        if (Style.HasFlag(MarkdownTextNodeStyle.Underlined))
            sgr = sgr with { Underlined = TextUnderlinedMode.Single };

        if (Style.HasFlag(MarkdownTextNodeStyle.Strikethrough))
            sgr = sgr with { IsCrossedOut = true };

        if (Style.HasFlag(MarkdownTextNodeStyle.Superscript))
            throw new NotImplementedException();

        if (Style.HasFlag(MarkdownTextNodeStyle.Subscript))
            throw new NotImplementedException();

        if (Style.HasFlag(MarkdownTextNodeStyle.InlineCode))
        {
            if ((options.CodeColor | options.TextColor) is { IsDefault: false } fg)
                sgr = sgr with { ForegroundColor = fg };
        }

        Console.CurrentGraphicRendition = sgr;

        foreach (MarkdownTextElement elem in Elements)
        {
            curr = Console.CurrentGraphicRendition;
            elem.Print(options);

            Console.CurrentGraphicRendition = curr;
        }

        Console.CurrentGraphicRendition = old;
    }

    public static implicit operator MarkdownFormattedText(MarkdownPlainText text) => new(MarkdownTextNodeStyle.Regular, [text]);

    public static implicit operator MarkdownFormattedText(string text) => new(MarkdownTextNodeStyle.Regular, text);
}

public sealed class MarkdownLink
    : MarkdownFormattedText
{
    public string URL { get; }
    public string? Title { get; }


    public MarkdownLink(string url, string text, string? title = null)
        : this(url, [new MarkdownPlainText(text)], title)
    {
    }

    public MarkdownLink(string url, MarkdownTextElement[] text, string? title = null)
        : base(MarkdownTextNodeStyle.Underlined, text) // <-- TODO : strip colors
    {
        URL = url;
        Title = title;
    }

    /// <inheritdoc/>
    public override string Export() => $"[{base.Export()}]({URL}{(Title is not null ? $" \"{MarkdownUtils.Escape(Title)}\"" : "")})";

    /// <inheritdoc/>
    public override void Print(MarkdownRenderOptions options) => Console.WriteFormatted(Text, new()
    {
        ForegroundColor = options.LinkColor,
        Underlined = TextUnderlinedMode.Single,
    });
}

public sealed class MarkdownHorizontalLine
    : MarkdownSection
{
    public override string Export() => "---";

    public override void Print(MarkdownRenderOptions options)
    {
        (int x, int y) = Console.GetCursorPosition();

        Console.WriteLine(new string('─', options.MaxWidth ?? Console.WindowWidth));
        Console.SetCursorPosition(x, y + 1);
    }
}

public sealed class MarkdownImage
    : MarkdownSection
{
    public string? URL { get; init; }
    public int? Width { get; }
    public int? Height { get; }
    public SixelImage Image { get; }


    public MarkdownImage(SixelImage image)
        : this(image, null, null)
    {
    }

    public MarkdownImage(SixelImage image, int? width, int? height)
    {
        if (width is int w)
            if (height is int h)
            {
                // TODO : resize image
            }
            else
            {
                // TODO : resize image
            }
        else if (height is int h)
        {
            // TODO : resize image
        }

        Image = image;
        Width = width ?? image.Width;
        Height = height ?? image.Height;
    }

    public override string Export() => $"<img width=\"{Width ?? Image.Width}\" height=\"{Height ?? Image.Height}\" src=\"{URL ?? "(internal)"}\"/>";

    public override void Print(MarkdownRenderOptions options)
    {
        (int x, int y) = Console.GetCursorPosition();
        SixelRenderSettings settings = SixelRenderSettings.Default; // TODO : change?
        Rectangle bounds = Image.Measure(settings);

        Console.Write(Image, settings);
        Console.SetCursorPosition(x, y + bounds.Height);
    }
}




//public sealed class MarkdownList
//    : MarkdownSection
//{
//    public List<MarkdownSection> Items { get; }

//    // TODO: unordered list
//    // TODO: unordered task list
//    // TODO: ordered list (with start index, also alphanumeric)

//}



//public class MarkdownTable : MarkdownSection;
//public class MarkdownBlockCode : MarkdownSection;
//public class MarkdownBlockQuote : MarkdownSection;


/*
─⍽─⎨
⍽⍽⍽⍽⍽⍽⍽⍽⍽⍽
  ╱╲    ┌─
 ╱──╲   ├─
╱    ╲  └─ 
⎧
⎪
⎨
⎪
⎩
⎰
⎱
*/