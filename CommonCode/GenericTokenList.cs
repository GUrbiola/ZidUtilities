using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZidUtilities.CommonCode
{
    /// <summary>
    /// Represents an ordered collection of <see cref="Token"/> objects and maintains their text offsets and lengths.
    /// </summary>
    public class GenericTokenList
    {


        /// <summary>
        /// The underlying list of tokens in their sequence order.
        /// </summary>
        public List<GenericToken> List { get; set; }

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
        public GenericTokenList()
        {
            List = new List<GenericToken>();
            StartOffsets = new List<int>();
            EndOffsets = new List<int>();
            TokenLengths = new List<int>();
        }

        /// <summary>
        /// Adds a token to the list and updates the offset and length tracking collections.
        /// </summary>
        /// <param name="Current">The token to add. If null or has empty text the token is ignored.</param>
        public void AddToken(GenericToken Current)
        {
            if (Current == null || Current.IsTextEmpty)
                return;

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
        }

        /// <summary>
        /// Gets the start offset (zero-based) of the specified token.
        /// </summary>
        /// <param name="token">The token whose start offset will be returned.</param>
        /// <returns>The start offset of the token, or -1 if the token is not found in the list.</returns>
        public int GetStartOf(GenericToken token)
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
            if (tokenIndex < List.Count)
                return StartOffsets[tokenIndex];
            return -1;
        }

        /// <summary>
        /// Gets the end offset (zero-based) of the specified token.
        /// </summary>
        /// <param name="token">The token whose end offset will be returned.</param>
        /// <returns>The end offset of the token, or -1 if the token is not found in the list.</returns>
        public int GetEndOf(GenericToken token)
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
            if (tokenIndex < List.Count)
                return EndOffsets[tokenIndex];
            return -1;
        }

        /// <summary>
        /// Gets the length (number of characters) of the specified token's text.
        /// </summary>
        /// <param name="token">The token whose length will be returned.</param>
        /// <returns>The length of the token's text, or -1 if the token is not found.</returns>
        public int GetLengthOf(GenericToken token)
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
        public GenericToken GetToken(int tokenIndex)
        {
            return tokenIndex < List.Count ? List[tokenIndex] : null;
        }

        /// <summary>
        /// Removes the token at the specified index and updates offsets and lengths for subsequent tokens.
        /// </summary>
        /// <param name="tokenIndex">The index of the token to remove.</param>
        public void RemoveTokenAt(int tokenIndex)
        {
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
        public GenericToken this[int index]
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
            foreach (GenericToken t in List)
                sb.Append(t.Text);
            return sb.ToString();
        }

        /// <summary>
        /// Finds the token that contains the specified character offset and returns it along with its index.
        /// </summary>
        /// <param name="offset">The zero-based character offset to locate.</param>
        /// <param name="index">When this method returns, contains the index of the token that contains the offset, or -1 if not found.</param>
        /// <returns>The <see cref="Token"/> that contains the offset, or null if no token covers that offset.</returns>
        public GenericToken GetTokenAtOffset(int offset, out int index)
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
        public GenericToken GetTokenAtOffset(int offset)
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
        public List<GenericToken> GetByType(GenericTokenType tokenType)
        {
            return List.Where(X => X.Type == tokenType).ToList();
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
        public void AddTokenAt(int i, GenericToken token)
        {
            GenericTokenList buff = new GenericTokenList();
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
    public class GenericToken
    {
        /// <summary>
        /// The type of the token.
        /// </summary>
        public GenericTokenType Type { get; set; }

        /// <summary>
        /// The textual content of the token.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public GenericToken() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class with the specified type and text.
        /// </summary>
        /// <param name="Type">The token type.</param>
        /// <param name="Text">The token text.</param>
        public GenericToken(GenericTokenType Type, string Text)
        {
            this.Type = Type;
            this.Text = Text;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Text"/> property is null or empty.
        /// </summary>
        public bool IsTextEmpty { get { return string.IsNullOrEmpty(Text); } }
    }

    /// <summary>
    /// Enumeration of possible token types used by the tokenizer.
    /// </summary>
    public enum GenericTokenType
    {
        EMPTYSPACE,//spaces, tabs, new lines
        COMMA,//,
        DOT,//.
        SEMICOLON,//;
        COLON,//:
        DATATYPE, //depending on the language: int, string, bool, float, double, char, etc.
        WORD,//any text that is not identified as another type of token
        OPENBRACKET,// (, [, {,
        CLOSEBRACKET,//), ], },
        LINECOMMENT,//depending on the language: // comment , # comment , -- comment
        BLOCKCOMMENT,//depending on the language: /* comment */ , <!-- comment -->
        STRING,//depending on the language: "string", 'string'
        OPERATOR,//depending on the language: +, -, *, /, !, %, =, <, >, ==, <=, >=, <>, &&, ||, etc.
        NUMBER,//1234, 12.34, .34
        RESERVED //depending on the language: if, else, while, for, return, function, var, etc.
    };

    public enum GenericLanguage
    {
        CSharp,
        JavaScript,
        Html,
        Css,
        Xml,
        Json
    }
}
