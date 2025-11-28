using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.TextEditor.Document;


namespace CommonCode.ICSharpTextEditor.FoldingStrategies
{
    /// <summary>
    /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
    /// </summary>
    public class CSharpFoldingStrategy : IFoldingStrategy
    {
        /// <summary>
        /// Generates the foldings for our document.
        /// </summary>
        /// <param name="document">The current document.</param>
        /// <param name="fileName">The filename of the document.</param>
        /// <param name="parseInformation">Extra parse information, not used in this sample.</param>
        /// <returns>A list of FoldMarkers.</returns>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            List<FoldMarker> list = new List<FoldMarker>();
            List<Point> Starters = new List<Point>();
            int line, column;
            List<string> StartFoldTokens = new List<string>();
            List<string> EndFoldTokens = new List<string>();
            string buffer, after_line;

            StartFoldTokens.Add("#region");
            EndFoldTokens.Add("#endregion");
            StartFoldTokens.Add("/*");
            EndFoldTokens.Add("*/");

            for (int i = 0; i < StartFoldTokens.Count; i++)
            {
                for (line = 0; line < document.TotalNumberOfLines; line++)
                {
                    buffer = document.GetText(document.GetLineSegment(line)).ToLower();
                    column = 0;
                    column = buffer.IndexOf(StartFoldTokens[i], column);
                    while (column >= 0)
                    {
                        Starters.Add(new Point(line, column));
                        column += StartFoldTokens[i].Length;
                        column = buffer.IndexOf(StartFoldTokens[i], column);
                    }
                }

                for (line = 0; line < document.TotalNumberOfLines; line++)
                {
                    buffer = document.GetText(document.GetLineSegment(line)).ToLower();
                    column = 0;
                    column = buffer.IndexOf(EndFoldTokens[i], column);
                    while (column >= 0)
                    {
                        for (int rev = Starters.Count - 1; rev >= 0; rev--)
                        {
                            if (Starters[rev].X > line || (Starters[rev].X == line && Starters[rev].Y > column))
                                continue;
                            else
                            {
                                after_line = document.GetText(document.GetLineSegment(Starters[rev].X)).Substring(Starters[rev].Y);
                                if (after_line.Length > StartFoldTokens[i].Length)
                                    after_line = after_line.Substring(StartFoldTokens[i].Length).Trim();
                                else
                                    after_line = "...";
                                list.Add(new FoldMarker(document, Starters[rev].X, Starters[rev].Y + StartFoldTokens[i].Length, line, column, FoldType.Region, after_line, false));
                                Starters.RemoveAt(rev);
                                break;
                            }
                        }
                        column += EndFoldTokens[i].Length;
                        column = buffer.IndexOf(EndFoldTokens[i], column);
                        if (Starters.Count == 0)
                            break;
                    }
                }

                Starters.Clear();
            }

            return list;
        }
    }

}

