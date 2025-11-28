using ICSharpCode.TextEditor.Document;
using ZidUtilities.CommonCode;


namespace CommonCode.ICSharpTextEditor.BracketMatching
{
    public class GenericBracketMatcher : DefaultFormattingStrategy
    {
        public GenericLanguage Language { get; set; }
        public GenericBracketMatcher(GenericLanguage language)
        {
            Language = language;
        }
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
