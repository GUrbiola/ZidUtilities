using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZidUtilities.CommonCode.AvalonEdit.FoldingStrategies
{
    /// <summary>
    /// Generic folding strategy for AvalonEdit that creates foldings based on configurable start and end tokens.
    /// </summary>
    public class GenericFoldingStrategy
    {
        public List<string> StartFolding { get; set; }
        public List<string> EndFolding { get; set; }
        public List<string> SpamFolding { get; set; }
        public SyntaxLanguage Language { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFoldingStrategy"/> class
        /// with default empty folding token lists and the default language (<see cref="SyntaxLanguage.CSharp"/>).
        /// </summary>
        public GenericFoldingStrategy()
        {
            StartFolding = new List<string>();
            EndFolding = new List<string>();
            SpamFolding = new List<string>();
            Language = SyntaxLanguage.CSharp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFoldingStrategy"/> class
        /// with default empty folding token lists and the specified language.
        /// </summary>
        /// <param name="language">
        /// The <see cref="SyntaxLanguage"/> that this folding strategy should be associated with.
        /// </param>
        public GenericFoldingStrategy(SyntaxLanguage language)
        {
            StartFolding = new List<string>();
            EndFolding = new List<string>();
            SpamFolding = new List<string>();
            Language = language;
        }

        /// <summary>
        /// Generate fold markers for the provided document using the strategy's configured
        /// <see cref="StartFolding"/> and <see cref="EndFolding"/> token lists.
        /// </summary>
        /// <param name="document">The document to analyze for fold markers.</param>
        /// <returns>
        /// An enumerable of <see cref="NewFolding"/> instances representing regions in the document that can be folded.
        /// Returns an empty enumerable if no foldable regions are found or if the start/end token lists are not configured.
        /// </returns>
        public IEnumerable<NewFolding> GenerateFoldMarkers(TextDocument document)
        {
            return GenerateFoldMarkers(document, StartFolding, EndFolding);
        }

        private IEnumerable<NewFolding> GenerateFoldMarkers(TextDocument document, List<string> startFoldTokens, List<string> endFoldTokens)
        {
            List<NewFolding> foldings = new List<NewFolding>();
            List<FoldingPoint> starters = new List<FoldingPoint>();
            List<FoldingPoint> enders = new List<FoldingPoint>();

            if (startFoldTokens == null || endFoldTokens == null || startFoldTokens.Count == 0 || endFoldTokens.Count == 0)
                return foldings;

            int foldingCouples = Math.Min(startFoldTokens.Count, endFoldTokens.Count);
            for (int i = 0; i < foldingCouples; i++)
            {
                string startToken = startFoldTokens[i].ToLower();
                string endToken = endFoldTokens[i].ToLower();

                // Find start positions
                for (int lineNum = 1; lineNum <= document.LineCount; lineNum++)
                {
                    var line = document.GetLineByNumber(lineNum);
                    string curLine = document.GetText(line.Offset, line.Length).ToLower();
                    if (curLine.TrimStart().StartsWith(startToken))
                    {
                        int column = curLine.IndexOf(startToken);
                        starters.Add(new FoldingPoint
                        {
                            Line = lineNum,
                            Offset = line.Offset + column + startToken.Length,
                            FullLine = document.GetText(line.Offset, line.Length)
                        });
                    }
                }

                // Find end positions
                for (int lineNum = 1; lineNum <= document.LineCount; lineNum++)
                {
                    var line = document.GetLineByNumber(lineNum);
                    string curLine = document.GetText(line.Offset, line.Length).ToLower();
                    if (curLine.TrimEnd().EndsWith(endToken))
                    {
                        int column = curLine.IndexOf(endToken);
                        enders.Add(new FoldingPoint
                        {
                            Line = lineNum,
                            Offset = line.Offset + column,
                            FullLine = document.GetText(line.Offset, line.Length)
                        });
                    }
                }

                // Match them
                foreach (var end in enders)
                {
                    for (int rev = starters.Count - 1; rev >= 0; rev--)
                    {
                        if (starters[rev].Line > end.Line || (starters[rev].Line == end.Line && starters[rev].Offset >= end.Offset))
                            continue;

                        string afterLine = starters[rev].FullLine.Substring(
                            starters[rev].Offset - document.GetLineByNumber(starters[rev].Line).Offset);
                        if (afterLine.Length > 0)
                            afterLine = afterLine.Trim();
                        else
                            afterLine = "...";

                        foldings.Add(new NewFolding(starters[rev].Offset, end.Offset) { Name = afterLine });
                        starters.RemoveAt(rev);
                        break;
                    }
                }

                starters.Clear();
                enders.Clear();
            }

            // Handle spam folding (consecutive lines starting with the same token)
            if (SpamFolding != null && SpamFolding.Count > 0)
            {
                foreach (var spamToken in SpamFolding)
                {
                    string token = spamToken.ToLower();
                    for (int lineNum = 1; lineNum <= document.LineCount; lineNum++)
                    {
                        var line = document.GetLineByNumber(lineNum);
                        string curLine = document.GetText(line.Offset, line.Length).ToLower();

                        if (curLine.TrimStart().StartsWith(token))
                        {
                            if (lineNum + 1 > document.LineCount)
                                continue;

                            var nextLine = document.GetLineByNumber(lineNum + 1);
                            string nextLineText = document.GetText(nextLine.Offset, nextLine.Length).ToLower();
                            int endLine = lineNum;

                            if (nextLineText.TrimStart().StartsWith(token))
                            {
                                while (nextLineText.TrimStart().StartsWith(token))
                                {
                                    if (endLine + 1 > document.LineCount)
                                        break;

                                    endLine++;
                                    if (endLine + 1 <= document.LineCount)
                                    {
                                        nextLine = document.GetLineByNumber(endLine + 1);
                                        nextLineText = document.GetText(nextLine.Offset, nextLine.Length).ToLower();
                                    }
                                    else
                                        break;
                                }

                                var endLineSegment = document.GetLineByNumber(endLine);
                                foldings.Add(new NewFolding(line.Offset, endLineSegment.EndOffset) { Name = "..." });
                                lineNum = endLine;
                            }
                        }
                    }
                }
            }

            return foldings.OrderBy(f => f.StartOffset);
        }

        private class FoldingPoint
        {
            public int Line { get; set; }
            public int Offset { get; set; }
            public string FullLine { get; set; }
        }
    }
}
