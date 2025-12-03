using ICSharpCode.AvalonEdit.Document;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.AvalonEdit.BracketMatching
{
    /// <summary>
    /// SQL-aware bracket matcher for AvalonEdit.
    /// This class uses the SQL tokenizer to locate matching brackets while ignoring brackets that appear
    /// inside line comments, block comments or string literals.
    /// </summary>
    public class SqlBracketMatcher
    {
        /// <summary>
        /// Searches forward from the specified <paramref name="offset"/> for the matching closing bracket
        /// corresponding to <paramref name="openBracket"/>. Tokenization is used so brackets that appear
        /// inside comments or strings are ignored.
        /// </summary>
        /// <param name="document">The document to search in.</param>
        /// <param name="offset">The zero-based character offset in the document from which the search will start.</param>
        /// <param name="openBracket">The opening bracket character to match (for example '(').</param>
        /// <param name="closingBracket">The expected closing bracket character to find (for example ')').</param>
        /// <returns>
        /// The start offset (zero-based) of the matching closing bracket token if found; otherwise -1.
        /// If the offset is inside a line comment, block comment or string literal the method returns -1.
        /// </returns>
        public int SearchBracketForward(TextDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = 1;
            Token currentToken;

            // Get all the text
            string script = document.Text;
            // Tokenize text
            TokenList tokens = script.GetTokens();

            if (offset > 1)
            {
                Token checkToken = tokens.GetTokenAtOffset(offset - 1, out tokenIndex);
                if (checkToken.Type == TokenType.LINECOMMENT ||
                    checkToken.Type == TokenType.BLOCKCOMMENT ||
                    checkToken.Type == TokenType.STRING)
                    return -1;
            }

            Token startToken = tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {
                for (int i = tokenIndex; i < tokens.TokenCount; i++)
                {
                    currentToken = tokens.GetToken(i);
                    if (currentToken.Type == TokenType.CLOSEBRACKET && currentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (currentToken.Type == TokenType.OPENBRACKET && currentToken.Text == openBracket.ToString())
                    {
                        bracketTrick++;
                    }

                    if (bracketTrick == 0)
                    {
                        return tokens.GetStartOf(currentToken);
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
        /// <param name="document">The document to search in.</param>
        /// <param name="offset">The zero-based character offset in the document from which the backward search will start.</param>
        /// <param name="openBracket">The opening bracket character expected to match (for example '(').</param>
        /// <param name="closingBracket">The closing bracket character to match against (for example ')').</param>
        /// <returns>
        /// The start offset (zero-based) of the matching opening bracket token if found; otherwise -1.
        /// If the offset is inside a line comment, block comment or string literal the method returns -1.
        /// </returns>
        public int SearchBracketBackward(TextDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = -1;
            Token currentToken;

            // Get all the text
            string script = document.Text;
            // Tokenize text
            TokenList tokens = script.GetTokens();

            if (offset > 1)
            {
                Token checkToken = tokens.GetTokenAtOffset(offset + 1, out tokenIndex);
                if (checkToken.Type == TokenType.LINECOMMENT ||
                    checkToken.Type == TokenType.BLOCKCOMMENT ||
                    checkToken.Type == TokenType.STRING)
                    return -1;
            }

            tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {
                for (int i = tokenIndex; i >= 0; i--)
                {
                    currentToken = tokens.GetToken(i);
                    if (currentToken.Type == TokenType.CLOSEBRACKET && currentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (currentToken.Type == TokenType.OPENBRACKET && currentToken.Text == openBracket.ToString())
                    {
                        bracketTrick++;
                    }

                    if (bracketTrick == 0)
                    {
                        return tokens.GetStartOf(currentToken);
                    }
                }
            }
            return -1;
        }
    }
}
