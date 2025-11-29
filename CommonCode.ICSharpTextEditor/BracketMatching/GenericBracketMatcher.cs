using ICSharpCode.TextEditor.Document;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.ICSharpTextEditor.BracketMatching
{
    /// <summary>
    /// Provides bracket matching functionality for a generic language.
    /// Uses tokenization provided by <see cref="GenericLanguage"/> and related token types
    /// to search forward and backward for matching bracket characters while ignoring
    /// brackets that appear inside comments or strings.
    /// </summary>
    public class GenericBracketMatcher : DefaultFormattingStrategy
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
        /// <remarks>
        /// The method first tokenizes the entire document text using the configured <see cref="Language"/>.
        /// If the bracket at the initial offset appears inside a line comment, block comment or string,
        /// the method returns -1 to indicate no match should be reported. The algorithm counts nested
        /// opening and closing brackets using a counter (bracketTrick) until it reaches zero, at which
        /// point it returns the start offset of the matching bracket token.
        /// </remarks>
        /// <param name="document">The document to search within. Must implement <see cref="IDocument"/>.</param>
        /// <param name="offset">The character offset in the document from which to start the forward search.
        /// Note: in practice the offset passed may point to the character immediately after the bracket,
        /// so the code checks the token at offset - 1 when verifying whether the bracket is inside a comment/string.</param>
        /// <param name="openBracket">The opening bracket character to match (e.g., '(' or '{').</param>
        /// <param name="closingBracket">The corresponding closing bracket character to find (e.g., ')' or '}').</param>
        /// <returns>
        /// The zero-based start offset of the matching closing bracket token if found; otherwise -1.
        /// Returns -1 also when the initial bracket appears inside a comment or string.
        /// </returns>
        public override int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = 1;
            GenericToken CurrentToken;
            //get all the text
            string Script = document.GetText(0, document.TextLength);
            //tokenize text
            GenericTokenList Tokens = Script.GetTokens(Language);

            if (offset > 1)
            {//for some reason the offset received is from the word after the bracket, so to check if the bracket is in a comment or a string is necesary to take the offset - 1
                GenericToken XXX = Tokens.GetTokenAtOffset(offset - 1);
                if (XXX.Type == GenericTokenType.LINECOMMENT || XXX.Type == GenericTokenType.BLOCKCOMMENT || XXX.Type == GenericTokenType.STRING)
                    return -1;
            }
            GenericToken ttt = Tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {//token found at offset
                for (int i = tokenIndex; i < Tokens.TokenCount; i++)
                {
                    CurrentToken = Tokens.GetToken(i);
                    if (CurrentToken.Type == GenericTokenType.CLOSEBRACKET && CurrentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (CurrentToken.Type == GenericTokenType.OPENBRACKET && CurrentToken.Text == openBracket.ToString())
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
        /// Searches backward from a given offset in the provided document for the matching opening bracket.
        /// </summary>
        /// <remarks>
        /// The method tokenizes the entire document text using the configured <see cref="Language"/>.
        /// If the bracket at or near the initial offset appears inside a line comment, block comment or string,
        /// the method returns -1. The algorithm walks tokens backwards counting nested brackets until the
        /// bracket counter reaches zero, then returns the start offset of the matching opening bracket token.
        /// </remarks>
        /// <param name="document">The document to search within. Must implement <see cref="IDocument"/>.</param>
        /// <param name="offset">The character offset in the document from which to start the backward search.
        /// Note: code checks offset + 1 to determine whether the position is within a comment or string.</param>
        /// <param name="openBracket">The opening bracket character to find (e.g., '(' or '{').</param>
        /// <param name="closingBracket">The closing bracket character that initiated the backward search (e.g., ')' or '}').</param>
        /// <returns>
        /// The zero-based start offset of the matching opening bracket token if found; otherwise -1.
        /// Returns -1 also when the bracket at the provided location appears inside a comment or string.
        /// </returns>
        public override int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
        {
            int tokenIndex, bracketTrick = -1;
            GenericToken CurrentToken;
            //get all the text
            string Script = document.GetText(0, document.TextLength);
            //tokenize text
            GenericTokenList Tokens = Script.GetTokens(Language);
            if (offset > 1)
            {
                GenericToken XXX = Tokens.GetTokenAtOffset(offset + 1, out tokenIndex);
                if (XXX.Type == GenericTokenType.LINECOMMENT || XXX.Type == GenericTokenType.BLOCKCOMMENT || XXX.Type == GenericTokenType.STRING)
                    return -1;
            }
            Tokens.GetTokenAtOffset(offset, out tokenIndex);
            if (tokenIndex >= 0)
            {//token found at offset
                for (int i = tokenIndex; i >= 0; i--)
                {
                    CurrentToken = Tokens.GetToken(i);
                    if (CurrentToken.Type == GenericTokenType.CLOSEBRACKET && CurrentToken.Text == closingBracket.ToString())
                    {
                        bracketTrick--;
                    }
                    else if (CurrentToken.Type == GenericTokenType.OPENBRACKET && CurrentToken.Text == openBracket.ToString())
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
