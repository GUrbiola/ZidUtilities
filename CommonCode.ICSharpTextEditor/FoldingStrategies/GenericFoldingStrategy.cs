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
        public GenericFoldingStrategy() 
        { 
            StartFolding = new List<string>();
            EndFolding = new List<string>();
            SpamFolding = new List<string>();
            Language = SyntaxLanguage.CSharp;
        }
        public GenericFoldingStrategy(SyntaxLanguage language)
        {
            StartFolding = new List<string>();
            EndFolding = new List<string>();
            SpamFolding = new List<string>();
            Language = language;
        }
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            return GenerateFoldMarkers(document, fileName, parseInformation, StartFolding, EndFolding);
        }
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
