using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCode.ICSharpTextEditor.FoldingStrategies
{
    public class GenericFoldingStrategy : IFoldingStrategy
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
        /// This value is stored in the <see cref="Language"/> property and does not directly affect
        /// token matching unless callers configure the token lists accordingly.
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
        /// <param name="document">The document to analyze for fold markers. Must implement <see cref="IDocument"/>.</param>
        /// <param name="fileName">The name of the file being analyzed. This parameter is not used by the current implementation
        /// but is kept for compatibility with the folding strategy interface.</param>
        /// <param name="parseInformation">Optional parse information produced by a parser. This parameter is not used by the current implementation.</param>
        /// <returns>
        /// A list of <see cref="FoldMarker"/> instances representing regions in the document that can be folded.
        /// Returns an empty list if no foldable regions are found or if the start/end token lists are not configured.
        /// </returns>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            return GenerateFoldMarkers(document, fileName, parseInformation, StartFolding, EndFolding);
        }

        /// <summary>
        /// Internal implementation that generates fold markers for the provided document using the given
        /// start and end token lists. The method finds lines that start with any token in <paramref name="StartFoldTokens"/>
        /// and lines that end with any token in <paramref name="EndFoldTokens"/> and matches them to create fold regions.
        /// It also creates folding regions for consecutive "spam" lines configured via the <see cref="SpamFolding"/> list.
        /// </summary>
        /// <param name="document">The document to scan for folding regions.</param>
        /// <param name="fileName">The file name associated with the document; not used by the implementation but provided for compatibility.</param>
        /// <param name="parseInformation">Optional parser information; not used by this implementation.</param>
        /// <param name="StartFoldTokens">A list of tokens that indicate the start of a foldable region. Each entry is matched at the beginning of a trimmed line.</param>
        /// <param name="EndFoldTokens">A list of tokens that indicate the end of a foldable region. Each entry is matched at the end of a trimmed line.</param>
        /// <returns>
        /// A list of <see cref="FoldMarker"/> objects representing the detected foldable regions.
        /// The method returns an empty list if inputs are null/empty or no regions are found.
        /// </returns>
        private List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation, List<string> StartFoldTokens, List<string> EndFoldTokens)
        {
            List<FoldMarker> list = new List<FoldMarker>();
            List<Point> Starters = new List<Point>();
            List<Point> Enders = new List<Point>();
            int line, column, FoldingCouples;
            string after_line, curLine;

            

            if (StartFoldTokens == null || EndFoldTokens == null || StartFoldTokens.Count == 0 || EndFoldTokens.Count == 0)
                return list;
            FoldingCouples = Math.Min(StartFoldTokens.Count, EndFoldTokens.Count);
            for (int i = 0; i < FoldingCouples; i++)
            {
                //first lets get start positions
                for (line = 0; line < document.TotalNumberOfLines; line++)
                {
                    curLine = document.GetText(document.GetLineSegment(line)).ToLower();
                    if(curLine.TrimStart().StartsWith(StartFoldTokens[i]))
                    {
                        column = curLine.IndexOf(StartFoldTokens[i]);
                        Starters.Add(new Point(line, column + StartFoldTokens[i].Length));
                    }
                }

                //now get end positions
                for (line = 0; line < document.TotalNumberOfLines; line++)
                {
                    curLine = document.GetText(document.GetLineSegment(line)).ToLower();
                    if (curLine.TrimEnd().EndsWith(EndFoldTokens[i]))
                    {
                        column = curLine.IndexOf(EndFoldTokens[i]);
                        Enders.Add(new Point(line, column));
                    }
                }

                //finally match them    
                foreach (Point end in Enders)
                {
                    for (int rev = Starters.Count - 1; rev >= 0; rev--)
                    {
                        if (Starters[rev].X > end.X || (Starters[rev].X == end.X && Starters[rev].Y >= end.Y))
                            continue;
                        else
                        {
                            after_line = document.GetText(document.GetLineSegment(Starters[rev].X)).Substring(Starters[rev].Y);
                            if (after_line.Length > 0)
                                after_line = after_line.Trim();
                            else
                                after_line = "...";
                            list.Add(new FoldMarker(document, Starters[rev].X, Starters[rev].Y, end.X, end.Y, FoldType.Region, after_line, false));
                            Starters.RemoveAt(rev);
                            break;
                        }
                    }
                }

                //clear lists for next token couple, make sure to not mix them
                Starters.Clear();
                Enders.Clear();
            }

            if (SpamFolding != null && SpamFolding.Count > 0)
            {
                for (int i = 0; i < SpamFolding.Count; i++)
                {
                    var spamFold = SpamFolding[i];
                    for (line = 0; line < document.TotalNumberOfLines; line++)
                    {
                        curLine = document.GetText(document.GetLineSegment(line)).ToLower();
                        if (curLine.TrimStart().StartsWith(spamFold))
                        {
                            if(line + 1 >= document.TotalNumberOfLines)
                                continue;

                            string nextLine = document.GetText(document.GetLineSegment(line + 1)).ToLower();
                            int endLine = line, endCol = nextLine.Length;
                            if (nextLine.TrimStart().StartsWith(spamFold))
                            {
                                while (nextLine.TrimStart().StartsWith(spamFold))
                                {
                                    if (endLine + 1 >= document.TotalNumberOfLines)
                                        break;
                                    else
                                    {
                                        nextLine = document.GetText(document.GetLineSegment(endLine + 1)).ToLower();
                                        if(nextLine.TrimStart().StartsWith(spamFold))
                                            endLine++;
                                        else
                                            break;
                                    }
                                }

                                nextLine = document.GetText(document.GetLineSegment(endLine)).ToLower();
                                list.Add(new FoldMarker(document, line, curLine.IndexOf(spamFold), endLine, nextLine.Length, FoldType.Region, "...", false));
                                line = endLine;
                            }
                        }
                    }
                }
            }

            return list;
        }
    }
}
