using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.AvalonEdit.FoldingStrategies
{
    /// <summary>
    /// Provides a folding strategy for SQL-like scripts by scanning tokenized text and producing fold markers.
    /// </summary>
    /// <remarks>
    /// This strategy produces fold regions for:
    /// - Block constructs identified by BEGIN ... END tokens,
    /// - Block comments,
    /// - Custom line-comment folders using markers like "--fold" and "--/fold".
    /// The produced <see cref="NewFolding"/> instances can be used by the editor to collapse and expand regions.
    /// </remarks>
    public class SqlFoldingStrategy
    {
        /// <summary>
        /// Scans the provided <paramref name="document"/> and generates a list of fold markers representing
        /// foldable regions found in the text.
        /// </summary>
        /// <param name="document">
        /// The <see cref="TextDocument"/> instance whose text will be analyzed.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{NewFolding}"/> containing fold markers discovered in the document.
        /// </returns>
        public IEnumerable<NewFolding> GenerateFoldMarkers(TextDocument document)
        {
            List<NewFolding> foldings = new List<NewFolding>();

            // Get all the text
            string script = document.Text;
            // Tokenize text
            TokenList tokens = script.GetTokens();

            #region Calculate folding for BEGIN <----> END
            List<Token> foldStarters = tokens.GetByType(TokenType.BLOCKSTART);
            List<Token> foldEnders = tokens.GetByType(TokenType.BLOCKEND);

            for (int i = 0; i < foldEnders.Count; i++)
            {
                for (int j = foldStarters.Count - 1; j >= 0; j--)
                {
                    if (tokens.GetStartOf(foldStarters[j]) < tokens.GetStartOf(foldEnders[i]))
                    {
                        Token starter = foldStarters[j];
                        Token ender = foldEnders[i];

                        int startOffset = tokens.GetStartOf(starter) + "begin".Length;
                        int endOffset = tokens.GetStartOf(ender);

                        var startLocation = document.GetLocation(tokens.GetStartOf(starter));
                        var line = document.GetLineByNumber(startLocation.Line);
                        string lineText = document.GetText(line.Offset, line.Length);
                        int columnInLine = startLocation.Column - 1;

                        string afterStarter = lineText.Length > columnInLine
                            ? lineText.Substring(columnInLine).Trim(' ', '\t', '\n', '\r')
                            : "";

                        afterStarter = afterStarter.Length > 5 ? afterStarter.Substring(5) : "";
                        if (string.IsNullOrEmpty(afterStarter) || afterStarter.Trim().Length == 0)
                            afterStarter = "...";

                        foldings.Add(new NewFolding(startOffset, endOffset) { Name = afterStarter });

                        foldStarters.RemoveAt(j);
                        break;
                    }
                }
            }
            #endregion

            #region Calculate folding for block comments
            List<Token> blockCommentFolds = tokens.GetByType(TokenType.BLOCKCOMMENT);

            foreach (var blockComment in blockCommentFolds)
            {
                int startOffset = tokens.GetStartOf(blockComment) + 2;
                int endOffset = tokens.GetEndOf(blockComment) - 1;

                if (endOffset > startOffset)
                {
                    foldings.Add(new NewFolding(startOffset, endOffset) { Name = "***" });
                }
            }
            #endregion

            #region Calculate folding for custom folders --fold <----> --/fold
            foldStarters = tokens.GetCustomFolders(true);
            foldEnders = tokens.GetCustomFolders(false);

            for (int i = 0; i < foldEnders.Count; i++)
            {
                for (int j = foldStarters.Count - 1; j >= 0; j--)
                {
                    if (tokens.GetStartOf(foldStarters[j]) < tokens.GetStartOf(foldEnders[i]))
                    {
                        Token starter = foldStarters[j];
                        Token ender = foldEnders[i];

                        int startOffset = tokens.GetStartOf(starter) + "--fold".Length;
                        int endOffset = tokens.GetStartOf(ender);

                        var startLocation = document.GetLocation(tokens.GetStartOf(starter));
                        var line = document.GetLineByNumber(startLocation.Line);
                        string lineText = document.GetText(line.Offset, line.Length);
                        int columnInLine = startLocation.Column - 1;

                        string afterStarter = lineText.Length > columnInLine
                            ? lineText.Substring(columnInLine).Trim(' ', '\t', '\n', '\r')
                            : "";

                        afterStarter = afterStarter.Length > 6 ? afterStarter.Substring(6) : "";
                        if (string.IsNullOrEmpty(afterStarter) || afterStarter.Trim().Length == 0)
                            afterStarter = "...";

                        foldings.Add(new NewFolding(startOffset, endOffset) { Name = afterStarter });

                        foldStarters.RemoveAt(j);
                        break;
                    }
                }
            }
            #endregion

            return foldings.OrderBy(f => f.StartOffset);
        }
    }
}
