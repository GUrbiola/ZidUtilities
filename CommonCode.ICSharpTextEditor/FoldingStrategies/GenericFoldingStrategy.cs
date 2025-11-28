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

        public GenericFoldingStrategy() 
        { 
            StartFolding = new List<string>();
            EndFolding = new List<string>();
        }

        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            return GenerateFoldMarkers(document, fileName, parseInformation, StartFolding, EndFolding);
        }
        private List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation, List<string> StartFoldTokens, List<string> EndFoldTokens)
        {
            List<FoldMarker> list = new List<FoldMarker>();
            List<Point> Starters = new List<Point>();
            int linea, columna, FoldingCouples;
            string buffer, after_line;

            if (StartFoldTokens == null || EndFoldTokens == null || StartFoldTokens.Count == 0 || EndFoldTokens.Count == 0)
                return list;
            FoldingCouples = Math.Min(StartFoldTokens.Count, EndFoldTokens.Count);
            for (int i = 0; i < FoldingCouples; i++)
            {
                for (linea = 0; linea < document.TotalNumberOfLines; linea++)
                {
                    buffer = document.GetText(document.GetLineSegment(linea)).ToLower();
                    columna = 0;
                    columna = buffer.IndexOf(StartFoldTokens[i], columna);
                    while (columna >= 0)
                    {
                        Starters.Add(new Point(linea, columna));
                        columna += StartFoldTokens[i].Length;
                        columna = buffer.IndexOf(StartFoldTokens[i], columna);
                    }
                }

                for (linea = 0; linea < document.TotalNumberOfLines; linea++)
                {
                    buffer = document.GetText(document.GetLineSegment(linea)).ToLower();
                    columna = 0;
                    columna = buffer.IndexOf(EndFoldTokens[i], columna);
                    while (columna >= 0)
                    {
                        for (int rev = Starters.Count - 1; rev >= 0; rev--)
                        {
                            if (Starters[rev].X > linea || (Starters[rev].X == linea && Starters[rev].Y > columna))
                                continue;
                            else
                            {
                                after_line = document.GetText(document.GetLineSegment(Starters[rev].X)).Substring(Starters[rev].Y);
                                if (after_line.Length > StartFoldTokens[i].Length)
                                    after_line = after_line.Substring(StartFoldTokens[i].Length).Trim();
                                else
                                    after_line = "...";
                                list.Add(new FoldMarker(document, Starters[rev].X, Starters[rev].Y + StartFoldTokens[i].Length, linea, columna, FoldType.Region, after_line, false));
                                Starters.RemoveAt(rev);
                                break;
                            }
                        }
                        columna += EndFoldTokens[i].Length;
                        columna = buffer.IndexOf(EndFoldTokens[i], columna);
                    }
                }

                Starters.Clear();
            }

            return list;
        }
    }
}
