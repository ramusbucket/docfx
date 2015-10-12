﻿namespace Microsoft.DocAsCode.MarkdownLite
{
    using System.Text.RegularExpressions;

    public class MarkdownParagraphBlockRule : IMarkdownRule
    {
        public string Name => "Paragraph";

        public virtual Regex Paragraph => Regexes.Block.Paragraph;

        public virtual IMarkdownToken TryMatch(MarkdownEngine engine, ref string source)
        {
            var match = Paragraph.Match(source);
            if (match.Length == 0)
            {
                return null;
            }
            source = source.Substring(match.Length);
            var content = match.Groups[1].Value[match.Groups[1].Value.Length - 1] == '\n'
                ? match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - 1)
                : match.Groups[1].Value;
            return new MarkdownParagraphBlockToken(this, content);
        }
    }
}