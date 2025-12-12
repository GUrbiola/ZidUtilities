using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZidUtilities.CommonCode
{
    /// <summary>
    /// Represents an ordered collection of <see cref="Token"/> objects and maintains their text offsets and lengths.
    /// </summary>
    public class TokenList
    {
        /// <summary>
        /// The underlying list of tokens in their sequence order.
        /// </summary>
        public List<Token> List { get; set; }

        /// <summary>
        /// The start offsets (zero-based) of each token within the reconstructed full text.
        /// </summary>
        public List<int> StartOffsets { get; set; }

        /// <summary>
        /// The end offsets (zero-based) of each token within the reconstructed full text.
        /// </summary>
        public List<int> EndOffsets { get; set; }

        /// <summary>
        /// The length of each token's text.
        /// </summary>
        public List<int> TokenLengths { get; set; }

        /// <summary>
        /// Gets the number of tokens currently in the list.
        /// </summary>
        public int TokenCount { get { return List.Count; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenList"/> class.
        /// </summary>
        public TokenList()
        {
            List = new List<Token>();
            StartOffsets = new List<int>();
            EndOffsets = new List<int>();
            TokenLengths = new List<int>();
        }

        /// <summary>
        /// Adds a token to the list and updates the offset and length tracking collections.
        /// </summary>
        /// <param name="Current">The token to add. If null or has empty text the token is ignored.</param>
        /// <remarks>
        /// - For tokens of type <see cref="TokenType.WORD"/>, the method may change the token's type
        ///   (for example to RESERVED, DATATYPE, TEMPTABLE, VARIABLE, BLOCKSTART or BLOCKEND) based on text.
        /// - Start/End offsets and TokenLengths collections are kept in sync with the List.
        /// </remarks>
        public void AddToken(Token Current)
        {
            if (Current == null || Current.IsTextEmpty)
                return;

            switch (Current.Type)
            {
                default:
                //if this token is a variable is going to be determined in this method, initially the token must be marked as word
                case TokenType.VARIABLE:
                //if this token is a temptable is going to be determined in this method, initially the token must be marked as word
                case TokenType.TEMPTABLE:
                //if this token is a reserved word is going to be determined in this method, initially the token must be marked as word
                case TokenType.RESERVED:
                //this types of token are already processed before this, so we must only add it to the list of tokens without any further check                
                case TokenType.OPENBRACKET:
                case TokenType.CLOSEBRACKET:
                case TokenType.LINECOMMENT:
                case TokenType.BLOCKCOMMENT:
                case TokenType.STRING:
                case TokenType.EMPTYSPACE:
                case TokenType.COMMA:
                    if (List.Count == 0)
                    {
                        StartOffsets.Add(0);
                    }
                    else
                    {
                        StartOffsets.Add(StartOffsets.Last() + TokenLengths.Last());
                    }
                    EndOffsets.Add(StartOffsets.Last() + Current.Text.Length - 1);
                    TokenLengths.Add(Current.Text.Length);
                    List.Add(Current);
                    break;
                case TokenType.WORD:
                    if (Current.Text.IsReserved())
                    {
                        Current.Type = TokenType.RESERVED;
                    }
                    else if (Current.Text.IsSqlDataType())
                    {
                        Current.Type = TokenType.DATATYPE;
                    }
                    else if (Current.Text.StartsWith("#") && Current.Text.Length > 1)
                    {
                        Current.Type = TokenType.TEMPTABLE;
                    }
                    else if (Current.Text.StartsWith("@") && Current.Text.Length > 1)
                    {
                        Current.Type = TokenType.VARIABLE;
                    }
                    else if (Current.Text.Equals("begin", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Current.Type = TokenType.BLOCKSTART;
                    }
                    else if (Current.Text.Equals("end", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Current.Type = TokenType.BLOCKEND;
                    }


                    if (List.Count == 0)
                    {
                        StartOffsets.Add(0);
                    }
                    else
                    {
                        StartOffsets.Add(StartOffsets.Last() + TokenLengths.Last());
                    }
                    EndOffsets.Add(StartOffsets.Last() + Current.Text.Length - 1);
                    TokenLengths.Add(Current.Text.Length);
                    List.Add(Current);
                    break;
            }
        }

        /// <summary>
        /// Gets the start offset (zero-based) of the specified token.
        /// </summary>
        /// <param name="token">The token whose start offset will be returned.</param>
        /// <returns>The start offset of the token, or -1 if the token is not found in the list.</returns>
        public int GetStartOf(Token token)
        {
            int index = List.IndexOf(token);
            return GetStartOf(index);
        }

        /// <summary>
        /// Gets the start offset (zero-based) of the token at the specified index.
        /// </summary>
        /// <param name="tokenIndex">The index of the token in the list.</param>
        /// <returns>The start offset of the token, or -1 if the index is out of range.</returns>
        public int GetStartOf(int tokenIndex)
        {
            if (tokenIndex < 0)
                return -1;

            if (tokenIndex < List.Count)
                return StartOffsets[tokenIndex];
            return -1;
        }

        /// <summary>
        /// Gets the end offset (zero-based) of the specified token.
        /// </summary>
        /// <param name="token">The token whose end offset will be returned.</param>
        /// <returns>The end offset of the token, or -1 if the token is not found in the list.</returns>
        public int GetEndOf(Token token)
        {
            int index = List.IndexOf(token);
            return GetEndOf(index);
        }

        /// <summary>
        /// Gets the end offset (zero-based) of the token at the specified index.
        /// </summary>
        /// <param name="tokenIndex">The index of the token in the list.</param>
        /// <returns>The end offset of the token, or -1 if the index is out of range.</returns>
        public int GetEndOf(int tokenIndex)
        {
            if (tokenIndex < 0)
                return -1;

            if (tokenIndex < List.Count)
                return EndOffsets[tokenIndex];
            return -1;
        }

        /// <summary>
        /// Gets the length (number of characters) of the specified token's text.
        /// </summary>
        /// <param name="token">The token whose length will be returned.</param>
        /// <returns>The length of the token's text, or -1 if the token is not found.</returns>
        public int GetLengthOf(Token token)
        {
            int index = List.IndexOf(token);
            return GetLengthOf(index);
        }

        /// <summary>
        /// Gets the length (number of characters) of the token at the specified index.
        /// </summary>
        /// <param name="tokenIndex">The index of the token in the list.</param>
        /// <returns>The length of the token's text, or -1 if the index is out of range.</returns>
        public int GetLengthOf(int tokenIndex)
        {
            if (tokenIndex < 0)
                return -1;

            if (tokenIndex < List.Count)
                return TokenLengths[tokenIndex];
            return -1;
        }

        /// <summary>
        /// Clears all tokens and resets offset and length tracking collections.
        /// </summary>
        public void Clean()
        {
            List.Clear();
            StartOffsets.Clear();
            EndOffsets.Clear();
            TokenLengths.Clear();
        }

        /// <summary>
        /// Gets the token at the specified index.
        /// </summary>
        /// <param name="tokenIndex">The index of the token to retrieve.</param>
        /// <returns>The <see cref="Token"/> at the given index, or null if the index is out of range.</returns>
        public Token GetToken(int tokenIndex)
        {
            if(tokenIndex < 0)
                return null;
            return tokenIndex < List.Count ? List[tokenIndex] : null;
        }

        /// <summary>
        /// Removes the token at the specified index and updates offsets and lengths for subsequent tokens.
        /// </summary>
        /// <param name="tokenIndex">The index of the token to remove.</param>
        public void RemoveTokenAt(int tokenIndex)
        {
            if (tokenIndex < 0)
                return;

            if (tokenIndex < List.Count)
            {
                List.RemoveAt(tokenIndex);
                StartOffsets.RemoveAt(tokenIndex);
                EndOffsets.RemoveAt(tokenIndex);
                TokenLengths.RemoveAt(tokenIndex);
                if (List.Count > 0)
                {
                    for (int ind = tokenIndex; ind < List.Count; ind++)
                    {
                        if (tokenIndex == 0 && ind == 0)
                        {
                            StartOffsets[0] = 0;
                            EndOffsets[0] = StartOffsets[0] + List[0].Text.Length - 1;
                            TokenLengths[0] = EndOffsets[0] - StartOffsets[0] + 1;
                        }
                        else
                        {
                            StartOffsets[ind] = StartOffsets[ind - 1] + TokenLengths[ind - 1];
                            EndOffsets[ind] = StartOffsets[ind] + List[ind].Text.Length - 1;
                            TokenLengths[ind] = EndOffsets[ind] - StartOffsets[ind] + 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indexer to access tokens by integer index.
        /// </summary>
        /// <param name="index">The zero-based token index.</param>
        /// <returns>The <see cref="Token"/> at the specified index, or null if out of range.</returns>
        public Token this[int index]
        {
            get { return GetToken(index); }
        }

        /// <summary>
        /// Reconstructs and returns the full text represented by the sequence of tokens.
        /// </summary>
        /// <returns>A string containing the concatenated token texts in order.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Token t in List)
            {
                sb.Append(t.Text);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Finds the token that contains the specified character offset and returns it along with its index.
        /// </summary>
        /// <param name="offset">The zero-based character offset to locate.</param>
        /// <param name="index">When this method returns, contains the index of the token that contains the offset, or -1 if not found.</param>
        /// <returns>The <see cref="Token"/> that contains the offset, or null if no token covers that offset.</returns>
        public Token GetTokenAtOffset(int offset, out int index)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (StartOffsets[i] <= offset && offset <= EndOffsets[i])
                {
                    index = i;
                    return GetToken(i);
                }
            }
            index = -1;
            return null;
        }

        /// <summary>
        /// Finds the token that contains the specified character offset.
        /// </summary>
        /// <param name="offset">The zero-based character offset to locate.</param>
        /// <returns>The <see cref="Token"/> that contains the offset, or null if no token covers that offset.</returns>
        public Token GetTokenAtOffset(int offset)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (StartOffsets[i] <= offset && offset <= EndOffsets[i])
                {
                    return GetToken(i);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the tokens that have the specified <see cref="TokenType"/>.
        /// </summary>
        /// <param name="tokenType">The token type to filter by.</param>
        /// <returns>A list of tokens that match the specified type.</returns>
        /// <remarks>
        /// Special handling is performed for <see cref="TokenType.BLOCKEND"/> to avoid treating BLOCKEND tokens
        /// that close nested CASE blocks as top-level block ends.
        /// </remarks>
        public List<Token> GetByType(TokenType tokenType)
        {
            List<Token> Back = new List<Token>();
            if (tokenType == TokenType.BLOCKEND)
            {
                int Case = 0;
                foreach (Token t in List)
                {
                    if (t.Text.Equals("case", StringComparison.CurrentCultureIgnoreCase))
                        Case++;
                    if (t.Type == TokenType.BLOCKEND)
                    {
                        if (Case > 0)
                        {
                            Case--;
                        }
                        else
                        {
                            Back.Add(t);
                        }
                    }
                }
                return Back;
            }

            return List.Where(X => X.Type == tokenType).ToList();
        }

        /// <summary>
        /// Returns custom fold markers found in line comments.
        /// </summary>
        /// <param name="beginFold">If true, returns fold-begin markers (comments starting with "--fold"), otherwise returns fold-end markers ("--/fold").</param>
        /// <returns>A list of <see cref="Token"/> instances representing the requested fold markers.</returns>
        public List<Token> GetCustomFolders(bool beginFold)
        {
            if (beginFold)
                return List.Where(X => X.Type == TokenType.LINECOMMENT && X.Text.StartsWith("--fold", StringComparison.CurrentCultureIgnoreCase)).ToList();
            return List.Where(X => X.Type == TokenType.LINECOMMENT && X.Text.StartsWith("--/fold", StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Inserts a token at the specified logical index, rebuilding the internal lists to maintain offsets.
        /// </summary>
        /// <param name="i">The index at which to insert the token. If equal to <see cref="TokenCount"/> the token is appended.</param>
        /// <param name="token">The token to insert.</param>
        /// <remarks>
        /// This method builds a temporary <see cref="TokenList"/>, copies tokens and inserts the new token,
        /// then replaces internal lists with the rebuilt lists to ensure offsets and lengths are recalculated.
        /// </remarks>
        public void AddTokenAt(int i, Token token)
        {
            TokenList buff = new TokenList();
            for (int j = 0; j < this.TokenCount; j++)
            {
                if (j == i)
                    buff.AddToken(token);
                buff.AddToken(this[j]);
            }

            this.List = buff.List;
            this.StartOffsets = buff.StartOffsets;
            this.EndOffsets = buff.EndOffsets;
            this.TokenLengths = buff.TokenLengths;
        }

    }

    /// <summary>
    /// Represents a lexical token with a type and text content.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The type of the token.
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// The textual content of the token.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class with the specified type and text.
        /// </summary>
        /// <param name="Type">The token type.</param>
        /// <param name="Text">The token text.</param>
        public Token(TokenType Type, string Text)
        {
            this.Type = Type;
            this.Text = Text;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Text"/> property is null or empty.
        /// </summary>
        public bool IsTextEmpty { get { return String.IsNullOrEmpty(Text); } }
    }

    /// <summary>
    /// Enumeration of possible token types used by the tokenizer.
    /// </summary>
    public enum TokenType
    {
        EMPTYSPACE,
        RESERVED,
        COMMA,
        WORD,
        VARIABLE,
        TEMPTABLE,
        OPENBRACKET,
        CLOSEBRACKET,
        LINECOMMENT,
        BLOCKCOMMENT,
        STRING,
        OPERATOR,
        COMPARATOR,
        DATATYPE,
        BLOCKSTART,
        BLOCKEND,
        BEGINTRANSACTION,
        SEMMICOLON,
        TABLE,
        VIEW
    };

}
