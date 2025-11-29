using ICSharpCode.TextEditor.Document;
using ZidUtilities.CommonCode;


namespace ZidUtilities.CommonCode.ICSharpTextEditor.BracketMatching
{
    /// <summary>
    /// SQL-aware bracket matcher used by the ICSharpCode.TextEditor integration.
    /// This class extends <see cref="DefaultFormattingStrategy"/> and uses the SQL tokenizer
    /// (via <c>GetTokens()</c>) to locate matching brackets while ignoring brackets that appear
    /// inside line comments, block comments or string literals.
    /// </summary>
    public class SqlBracketMatcher : DefaultFormattingStrategy
    {
        /// <summary>
        /// Searches forward from the specified <paramref name="offset"/> for the matching closing bracket
        /// corresponding to <paramref name="openBracket"/>. Tokenization is used so brackets that appear
        /// inside comments or strings are ignored.
        /// </summary>
        /// <param name="document">
        /// The document to search in. The method reads the full document text via <c>document.GetText(0, document.TextLength)</c>.
        /// </param>
        /// <param name="offset">
        /// The zero-based character offset in the document from which the search will start.
        /// Note: the implementation adjusts the offset by -1 to correctly detect whether the bracket is inside a comment or string,
        /// because the editor may supply an offset that points to the character after the bracket.
        /// </param>
        /// <param name="openBracket">
        /// The opening bracket character to match (for example '(').
        /// </param>
        /// <param name="closingBracket">
        /// The expected closing bracket character to find (for example ')').
        /// </param>
        /// <returns>
        /// The start offset (zero-based) of the matching closing bracket token if found; otherwise -1.
        /// If the offset is inside a line comment, block comment or string literal the method returns -1.
        /// </returns>
        public override int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = 1;
            Token CurrentToken;
            //get all the text
            string Script = document.GetText(0, document.TextLength);
            //tokenize text
            TokenList Tokens = Script.GetTokens();

            if (offset > 1)
            {//for some reason the offset received is from the word after the bracket, so to check if the bracket is in a comment or a string is necesary to take the offset - 1
                Token XXX = Tokens.GetTokenAtOffset(offset-1, out tokenIndex);
                if (XXX.Type == TokenType.LINECOMMENT || XXX.Type == TokenType.BLOCKCOMMENT || XXX.Type == TokenType.STRING)
                    return -1;
            }
            Token ttt = Tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {//token found at offset
                for (int i = tokenIndex; i < Tokens.TokenCount; i++)
                {
                    CurrentToken = Tokens.GetToken(i);
                    if (CurrentToken.Type == TokenType.CLOSEBRACKET && CurrentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (CurrentToken.Type == TokenType.OPENBRACKET && CurrentToken.Text == openBracket.ToString())
                    {
                        bracketTrick++;
                    }

                    if (bracketTrick == 0)
                    {
                        return Tokens.GetStartOf(CurrentToken);
                    }
                }
            }
            return -1;            
        }

        /// <summary>
        /// Searches backward from the specified <paramref name="offset"/> for the matching opening bracket
        /// corresponding to <paramref name="closingBracket"/>. Tokenization is used so brackets that appear
        /// inside comments or strings are ignored.
        /// </summary>
        /// <param name="document">
        /// The document to search in. The full document text is obtained using <c>document.GetText(0, document.TextLength)</c>.
        /// </param>
        /// <param name="offset">
        /// The zero-based character offset in the document from which the backward search will start.
        /// Note: the implementation checks <c>offset + 1</c> to determine if the position is inside a comment or string,
        /// since the editor-provided offset may be adjacent to the bracket.
        /// </param>
        /// <param name="openBracket">
        /// The opening bracket character expected to match (for example '(').
        /// </param>
        /// <param name="closingBracket">
        /// The closing bracket character to match against (for example ')').
        /// </param>
        /// <returns>
        /// The start offset (zero-based) of the matching opening bracket token if found; otherwise -1.
        /// If the offset is inside a line comment, block comment or string literal the method returns -1.
        /// </returns>
        public override int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = -1;
            Token CurrentToken;
            //get all the text
            string Script = document.GetText(0, document.TextLength);
            //tokenize text
            TokenList Tokens = Script.GetTokens();
            if (offset > 1)
            {
                Token XXX = Tokens.GetTokenAtOffset(offset + 1, out tokenIndex);
                if (XXX.Type == TokenType.LINECOMMENT || XXX.Type == TokenType.BLOCKCOMMENT || XXX.Type == TokenType.STRING)
                    return -1;
            }
            Tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {//token found at offset
                for (int i = tokenIndex; i >= 0; i--)
                {
                    CurrentToken = Tokens.GetToken(i);
                    if (CurrentToken.Type == TokenType.CLOSEBRACKET && CurrentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (CurrentToken.Type == TokenType.OPENBRACKET && CurrentToken.Text == openBracket.ToString())
                    {
                        bracketTrick++;
                    }

                    if (bracketTrick == 0)
                    {
                        return Tokens.GetStartOf(CurrentToken);
                    }                    
                }
            }
            return -1;  
        }
    }
}
