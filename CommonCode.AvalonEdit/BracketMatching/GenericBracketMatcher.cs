using ICSharpCode.AvalonEdit.Document;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.AvalonEdit.BracketMatching
{
    /// <summary>
    /// Provides bracket matching functionality for a generic language in AvalonEdit.
    /// Uses tokenization provided by <see cref="GenericLanguage"/> and related token types
    /// to search forward and backward for matching bracket characters while ignoring
    /// brackets that appear inside comments or strings.
    /// </summary>
    public class GenericBracketMatcher
    {
        /// <summary>
        /// Gets or sets the language definition used for tokenization and bracket detection.
        /// </summary>
        public GenericLanguage Language { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericBracketMatcher"/> class.
        /// </summary>
        /// <param name="language">The <see cref="GenericLanguage"/> that will be used to tokenize documents
        /// and determine token types such as comments, strings and bracket tokens.</param>
        public GenericBracketMatcher(GenericLanguage language)
        {
            Language = language;
        }

        /// <summary>
        /// Searches forward from a given offset in the provided document for the matching closing bracket.
        /// </summary>
        /// <param name="document">The document to search within.</param>
        /// <param name="offset">The character offset in the document from which to start the forward search.</param>
        /// <param name="openBracket">The opening bracket character to match (e.g., '(' or '{').</param>
        /// <param name="closingBracket">The corresponding closing bracket character to find (e.g., ')' or '}').</param>
        /// <returns>
        /// The zero-based start offset of the matching closing bracket token if found; otherwise -1.
        /// Returns -1 also when the initial bracket appears inside a comment or string.
        /// </returns>
        public int SearchBracketForward(TextDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = 1;
            GenericToken currentToken;

            // Get all the text
            string script = document.Text;
            // Tokenize text
            GenericTokenList tokens = script.GetTokens(Language);

            if (offset > 1)
            {
                GenericToken checkToken = tokens.GetTokenAtOffset(offset - 1);
                if (checkToken.Type == GenericTokenType.LINECOMMENT ||
                    checkToken.Type == GenericTokenType.BLOCKCOMMENT ||
                    checkToken.Type == GenericTokenType.STRING)
                    return -1;
            }

            GenericToken startToken = tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {
                for (int i = tokenIndex; i < tokens.TokenCount; i++)
                {
                    currentToken = tokens.GetToken(i);
                    if (currentToken.Type == GenericTokenType.CLOSEBRACKET && currentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (currentToken.Type == GenericTokenType.OPENBRACKET && currentToken.Text == openBracket.ToString())
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
        /// Searches backward from a given offset in the provided document for the matching opening bracket.
        /// </summary>
        /// <param name="document">The document to search within.</param>
        /// <param name="offset">The character offset in the document from which to start the backward search.</param>
        /// <param name="openBracket">The opening bracket character to find (e.g., '(' or '{').</param>
        /// <param name="closingBracket">The closing bracket character that initiated the backward search (e.g., ')' or '}').</param>
        /// <returns>
        /// The zero-based start offset of the matching opening bracket token if found; otherwise -1.
        /// Returns -1 also when the bracket at the provided location appears inside a comment or string.
        /// </returns>
        public int SearchBracketBackward(TextDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = -1;
            GenericToken currentToken;

            // Get all the text
            string script = document.Text;
            // Tokenize text
            GenericTokenList tokens = script.GetTokens(Language);

            if (offset > 1)
            {
                GenericToken checkToken = tokens.GetTokenAtOffset(offset + 1, out tokenIndex);
                if (checkToken.Type == GenericTokenType.LINECOMMENT ||
                    checkToken.Type == GenericTokenType.BLOCKCOMMENT ||
                    checkToken.Type == GenericTokenType.STRING)
                    return -1;
            }

            tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {
                for (int i = tokenIndex; i >= 0; i--)
                {
                    currentToken = tokens.GetToken(i);
                    if (currentToken.Type == GenericTokenType.CLOSEBRACKET && currentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (currentToken.Type == GenericTokenType.OPENBRACKET && currentToken.Text == openBracket.ToString())
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
