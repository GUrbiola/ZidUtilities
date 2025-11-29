using System;
using System.Collections.Generic;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ZidUtilities.CommonCode;


namespace CommonCode.ICSharpTextEditor.FoldingStrategies
{
    /// <summary>
    /// Provides a folding strategy for SQL-like scripts by scanning tokenized text and producing fold markers.
    /// </summary>
    /// <remarks>
    /// This strategy produces fold regions for:
    /// - Block constructs identified by BEGIN ... END tokens,
    /// - Block comments,
    /// - Custom line-comment folders using markers like "--fold" and "--/fold".
    /// The produced <see cref="FoldMarker"/> instances can be used by the editor to collapse and expand regions.
    /// </remarks>
    public class SqlFoldingStrategy : IFoldingStrategy
    {
        /// <summary>
        /// Scans the provided <paramref name="document"/> and generates a list of fold markers representing
        /// foldable regions found in the text.
        /// </summary>
        /// <param name="document">
        /// The <see cref="IDocument"/> instance whose text will be analyzed. The method reads the entire document
        /// text and converts character offsets to <see cref="TextLocation"/> values when creating fold markers.
        /// </param>
        /// <param name="fileName">
        /// The name of the file being analyzed. This parameter is accepted for interface compatibility and is not
        /// used by the current implementation.
        /// </param>
        /// <param name="parseInformation">
        /// Optional parse information that may be provided by a parser. This implementation does not rely on
        /// parse information and ignores this parameter.
        /// </param>
        /// <returns>
        /// A <see cref="List{FoldMarker}"/> containing fold markers discovered in the document. Each fold marker
        /// describes the start and end <see cref="TextLocation"/> of a foldable region, the fold type, and
        /// display text used when the region is collapsed.
        /// </returns>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            List<FoldMarker> Back = new List<FoldMarker>();
            //get all the text
            string Script = document.GetText(0, document.TextLength);
            //tokenize text
            TokenList Tokens = Script.GetTokens();

            #region Calculate folding for BEGIN <----> END
            List<Token> FoldStarter = Tokens.GetByType(TokenType.BLOCKSTART);
            List<Token> FoldEnder = Tokens.GetByType(TokenType.BLOCKEND);

            for (int i = 0; i < FoldEnder.Count; i++)
            {
                for (int j = FoldStarter.Count - 1; j >= 0; j--)
                {
                    if (Tokens.GetStartOf(FoldStarter[j]) < Tokens.GetStartOf(FoldEnder[i]))
                    {
                        Token Starter, Ender;
                        Ender = FoldEnder[i];
                        Starter = FoldStarter[j];
                        TextLocation Start, End, afterFoldHelper;

                        Start = document.OffsetToPosition(Tokens.GetStartOf(Starter) + "begin".Length);
                        afterFoldHelper = document.OffsetToPosition(Tokens.GetStartOf(Starter));
                        End = document.OffsetToPosition(Tokens.GetStartOf(Ender));

                        string afterStarter = document.GetText(document.GetLineSegment(afterFoldHelper.Line)).Substring(afterFoldHelper.Column).Trim(' ', '\t', '\n', '\r');
                        afterStarter = afterStarter.Length > 5 ? afterStarter.Substring(5) : "";
                        if (String.IsNullOrEmpty(afterStarter) || afterStarter.Trim().Length == 0)
                            afterStarter = "...";


                        Back.Add(new FoldMarker(document, Start.Line, Start.Column, End.Line, End.Column, FoldType.Region, afterStarter, false));

                        FoldStarter.RemoveAt(j);
                        break;
                    }
                }
            }
            #endregion

            #region Calculate folding for block comments
            List<Token> BlockCommentFolds = Tokens.GetByType(TokenType.BLOCKCOMMENT);

            for (int i = 0; i < BlockCommentFolds.Count; i++)
            {
                Token BlockComment = BlockCommentFolds[i];
                TextLocation Start, End;

                Start = document.OffsetToPosition(Tokens.GetStartOf(BlockComment) + 2);
                End = document.OffsetToPosition(Tokens.GetEndOf(BlockComment) - 1);

                Back.Add(new FoldMarker(document, Start.Line, Start.Column, End.Line, End.Column, FoldType.Region, "***", false));
            }
            #endregion

            #region Calculate folding for custom folders --fold <----> --/fold
            FoldStarter = Tokens.GetCustomFolders(true);
            FoldEnder = Tokens.GetCustomFolders(false);


            for (int i = 0; i < FoldEnder.Count; i++)
            {
                for (int j = FoldStarter.Count - 1; j >= 0; j--)
                {
                    if (Tokens.GetStartOf(FoldStarter[j]) < Tokens.GetStartOf(FoldEnder[i]))
                    {
                        Token Starter, Ender;
                        Ender = FoldEnder[i];
                        Starter = FoldStarter[j];
                        TextLocation Start, End, afterFoldHelper;

                        Start = document.OffsetToPosition(Tokens.GetStartOf(Starter) + "--fold".Length);
                        afterFoldHelper = document.OffsetToPosition(Tokens.GetStartOf(Starter));
                        End = document.OffsetToPosition(Tokens.GetStartOf(Ender));

                        string afterStarter = document.GetText(document.GetLineSegment(afterFoldHelper.Line)).Substring(afterFoldHelper.Column).Trim(' ', '\t', '\n', '\r');
                        afterStarter = afterStarter.Length > 6 ? afterStarter.Substring(6) : "";
                        if (String.IsNullOrEmpty(afterStarter) || afterStarter.Trim().Length == 0)
                            afterStarter = "...";

                        Back.Add(new FoldMarker(document, Start.Line, Start.Column, End.Line, End.Column, FoldType.Region, afterStarter, false));

                        FoldStarter.RemoveAt(j);
                        break;
                    }
                }
            }
            #endregion


            return Back;
        }
    }

}

