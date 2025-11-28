using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{
    public static class ExtensionsTemp
    {

        /// <summary>
        /// A set of SQL reserved words (upper-cased). Used to detect reserved tokens when parsing SQL.
        /// </summary>
        public static HashSet<string> ReservedWords = new HashSet<string>{"@@FETCH_STATUS", "@@IDENTITY","ADD","ALL","ALTER","AND","ANY ","AS","ASC","AUTHORIZATION","AVG","BACKUP",
            "BETWEEN","BREAK","BROWSE","BULK","BY","CASCADE","CASE","CATCH", "CHECK","CHECKPOINT","CLOSE",
            "CLUSTERED","COALESCE","COLLATE","COLUMN","COMMIT","COMPUTE","CONSTRAINT","CONTAINS","CONTAINSTABLE",
            "CONTINUE","CONVERT","COUNT","CREATE","CROSS","CURRENT","CURRENT_DATE","CURRENT_TIME","CURRENT_TIMESTAMP",
            "CURRENT_USER","CURSOR","DATABASE","DATABASEPASSWORD","DATEADD","DATEDIFF","DATENAME","DATEPART",
            "DBCC","DEALLOCATE","DECLARE","DEFAULT","DELAY","DELETE","DENY","DESC","DISK","DISTINCT","DISTRIBUTED",
            "DOUBLE","DROP","DUMP","ELSE","ENCRYPTION","ERRLVL","ESCAPE","EXCEPT","EXEC","EXECUTE",
            "EXISTS","EXIT","EXPRESSION","FETCH","FILE","FILLFACTOR","FOR","FOREIGN","FREETEXT","FREETEXTTABLE",
            "FROM","FULL","FUNCTION","GOTO","GRANT","GROUP","HAVING","HOLDLOCK","IDENTITY","IDENTITY_INSERT",
            "IDENTITYCOL","IF","IN","INDEX","INNER","INSERT","INTERSECT","INTO","IS","JOIN","KEY",
            "KILL","LEFT","LIKE","LINENO","LOAD","MAX","MIN","NATIONAL","NOCHECK","NONCLUSTERED",
            "NOT","NULL","NULLIF","OF","OFF","OFFSETS","ON","OPEN","OPENDATASOURCE","OPENQUERY",
            "OPENROWSET","OPENXML","OPTION","OR","ORDER","OUTER","OVER","PERCENT","PLAN","PRECISION",
            "PRIMARY","PRINT","PROC","PROCEDURE","PUBLIC","RAISERROR","READ","READTEXT","RECONFIGURE","REFERENCES",
            "REPLICATION","RESTORE","RESTRICT","RETURN","REVOKE","RIGHT","ROLLBACK","ROWCOUNT","ROWGUIDCOL",
            "RULE","SAVE","SCHEMA","SELECT","SESSION_USER","SET","SETUSER","SHUTDOWN","SOME","STATISTICS",
            "SUM","SYSTEM_USER","TABLE","TEXTSIZE","THEN","TO","TOP","TRAN","TRANSACTION","TRIGGER","TRUNCATE", "TRY",
            "TSEQUAL","UNION","UNIQUE","UPDATE","UPDATETEXT","USE","USER","VALUES","VARYING","VIEW","WAITFOR",
            "WHEN","WHERE","WHILE","WITH","WRITETEXT"};

        /// <summary>
        /// A set of SQL data type names (upper-cased). Used to identify SQL data types while parsing.
        /// </summary>
        public static HashSet<string> SqlDataTypes = new HashSet<string>{
            "BIGINT", "NUMERIC","BIT","INT","SMALLINT","TINYINT","SMALLMONEY","MONEY","DECIMAL", //exact numeric type
            "FLOAT","REAL",//aproximate numeric type
            "DATE","DATETIME","DATETIME2","DATETIMEOFFSET","TIME","SMALLDATETIME",//date type
            "CHAR","VARCHAR","TEXT",//char type
            "NCHAR","NVARCHAR","NTEXT",//unicode char types
            "BINARY","VARBINARY","IMAGE",//binary types
            "XML","CURSOR","TIMESTAMP","UNIQUEIDENTIFIER","HIERARCHYID","SQL_VARIANT","TABLE"//other data types
        };

        #region Extensions related to EzSql
        /// <summary>
        /// Parses a text string and converts it into a list of tokens, these tokens are have different types like: words, operators, comparators, comments, strings, brackets, commas, empty spaces.
        /// It is meant to be used in T-SQL parsing. Makes easier to analyze and manipulate T-SQL code programmatically.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <returns>A list of tokens extracted from the input text.</returns>
        public static TokenList GetTokens(this string Text)
        {
            TokenList Back = new TokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            Token Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                char CurChar = Text[index];
                char[] wordTokenBreaker;
                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        ////can not be a comma, because a comma is a 1 char token
                        //case TokenType.COMMA:
                        //    break;
                        ////can not be aa oppenbracket, because a oppenbracket is a 1 char token
                        //case TokenType.OPENBRACKET:
                        //    break;
                        ////can not be a closebracket, because a closebracket is a 1 char token
                        //case TokenType.CLOSEBRACKET:
                        //    break;
                        //can not be any of the next either, because this is decided when the token is added to the list
                        //case TokenType.RESERVED:
                        //case TokenType.VARIABLE:
                        //case TokenType.TEMPTABLE:
                        //    break;
                        case TokenType.EMPTYSPACE:
                            #region Code to process an empty space token + something
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsComma())
                                {
                                    Back.AddToken(new Token(TokenType.COMMA, ","));
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                                else
                                {
                                    Current = new Token(TokenType.WORD, CurChar.ToString());
                                }
                            }
                            #endregion
                            break;
                        case TokenType.WORD:
                            wordTokenBreaker = new char[] { ' ', '\t', '\r', '\n', '\'', ',', '(', '[', '{', '}', ']', ')', '-', '*', '+', '/', '>', '<', '=', ';' };
                            #region Code to process a word token + something
                            if (wordTokenBreaker.Contains(CurChar))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsWhiteSpace())
                                {
                                    Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsComma())
                                {
                                    Back.AddToken(new Token(TokenType.COMMA, ","));
                                }
                                else if (CurChar.IsSemmiColon())
                                {
                                    Back.AddToken(new Token(TokenType.SEMMICOLON, ";"));
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.LINECOMMENT:
                            #region Code to process a char inside a linecomment, a token breaker are "\n" and "\r\n"
                            if (CurChar == '\r')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\n')
                                {
                                    Current.Text += "\r\n";
                                    Back.AddToken(Current);
                                    index++;
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text += "\r";
                                }
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text += "\n";
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.BLOCKCOMMENT:
                            #region Code to process a char inside a block comment, the only token breaker is "*/"
                            if (CurChar == '*')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '/')
                                {
                                    Current.Text += "*/";
                                    Back.AddToken(Current);
                                    Current = null;
                                    index++;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.STRING:
                            #region Code to process a char inside a string, only token breaker is "'", but not "''"
                            if (CurChar == '\'')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\'')
                                {//double '', so is an escaped ', it is a string still then
                                    Current.Text += "''";
                                    index++;
                                }
                                else
                                {
                                    Current.Text += "'";
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;

                    }
                }
                else if (Current == null)
                {
                    #region no previous token, must check if create a new one, or let the current stay null and add a new instance of token
                    if (IsWhiteSpace(CurChar))
                    {
                        Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                    }
                    else if (CurChar.IsComma())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.COMMA, CurChar.ToString()));
                    }
                    else if (CurChar.IsStringOperator())
                    {
                        Current = new Token(TokenType.STRING, CurChar.ToString());
                    }
                    else if (CurChar.IsOpenBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsCloseBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsOperator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        //must check if this chars means the start of comment
                        if (CurChar == '-' && nextc == '-')
                        {
                            Current = new Token(TokenType.LINECOMMENT, "--");
                            index++;
                        }
                        else if (CurChar == '/' && nextc == '*')
                        {
                            Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                        }
                    }
                    else if (CurChar.IsComparator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        if (nextc.IsComparator())
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                        }
                    }
                    else
                    {
                        Current = new Token(TokenType.WORD, CurChar.ToString());
                    }
                    #endregion
                }
            }
            if (Current != null)
                Back.AddToken(Current);

            List<int> ToReplace = new List<int>();
            for (int i = 0; i < Back.TokenCount - 2; i++)
            {
                if (
                    Back[i].Type == TokenType.BLOCKSTART &&
                Back[i + 1].Type == TokenType.EMPTYSPACE &&
                Back[i + 2].Type == TokenType.RESERVED &&
                (Back[i + 2].Text.Equals("tran", StringComparison.CurrentCultureIgnoreCase) ||
                Back[i + 2].Text.Equals("transaction", StringComparison.CurrentCultureIgnoreCase)))
                {
                    ToReplace.Add(i);
                }
            }

            if (ToReplace.Count > 0)
            {
                int found = 0;
                foreach (int i in ToReplace)
                {
                    int buffIndex = i - (found * 2);

                    Token buff = new Token(TokenType.BEGINTRANSACTION, String.Format("{0}{1}{2}", Back[buffIndex].Text, Back[buffIndex + 1].Text, Back[buffIndex + 2].Text));
                    Back.RemoveTokenAt(buffIndex);
                    Back.RemoveTokenAt(buffIndex);
                    Back.RemoveTokenAt(buffIndex);
                    Back.AddTokenAt(buffIndex, buff);

                    found++;
                }
            }

            return Back;
        }

        /// <summary>
        /// Initiates the whole parsing process but stops at the first token found. Done this way for efficiency, instead of parsing the whole string when only the first token is needed.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <returns>A token extracted from the input text.</returns>
        public static Token GetFirstToken(this string Text)
        {
            TokenList Back = new TokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            Token Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                if (Back.TokenCount > 1)
                    break;

                char CurChar = Text[index];
                char[] wordTokenBreaker;
                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        ////can not be a comma, because a comma is a 1 char token
                        //case TokenType.COMMA:
                        //    break;
                        ////can not be aa oppenbracket, because a oppenbracket is a 1 char token
                        //case TokenType.OPENBRACKET:
                        //    break;
                        ////can not be a closebracket, because a closebracket is a 1 char token
                        //case TokenType.CLOSEBRACKET:
                        //    break;
                        //can not be any of the next either, because this is decided when the token is added to the list
                        //case TokenType.RESERVED:
                        //case TokenType.VARIABLE:
                        //case TokenType.TEMPTABLE:
                        //    break;
                        case TokenType.EMPTYSPACE:
                            #region Code to process an empty space token + something
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsComma())
                                {
                                    Back.AddToken(new Token(TokenType.COMMA, ","));
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                                else
                                {
                                    Current = new Token(TokenType.WORD, CurChar.ToString());
                                }
                            }
                            #endregion
                            break;
                        case TokenType.WORD:
                            wordTokenBreaker = new char[] { ' ', '\t', '\r', '\n', '\'', ',', '(', '[', '{', '}', ']', ')', '-', '*', '+', '/', '>', '<', '=' };
                            #region Code to process a word token + something
                            if (wordTokenBreaker.Contains(CurChar))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                if (CurChar.IsWhiteSpace())
                                {
                                    Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                                }
                                else if (CurChar.IsStringOperator())
                                {
                                    Current = new Token(TokenType.STRING, "'");
                                }
                                else if (CurChar.IsComma())
                                {
                                    Current = new Token(TokenType.STRING, ",");
                                }
                                else if (CurChar.IsOpenBracket())
                                {
                                    Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsCloseBracket())
                                {
                                    Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                                }
                                else if (CurChar.IsOperator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    //must check if this chars means the start of comment
                                    if (CurChar == '-' && nextc == '-')
                                    {
                                        Current = new Token(TokenType.LINECOMMENT, "--");
                                        index++;
                                    }
                                    else if (CurChar == '/' && nextc == '*')
                                    {
                                        Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                                    }
                                }
                                else if (CurChar.IsComparator())
                                {
                                    char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                    if (nextc.IsComparator())
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                                        index++;
                                    }
                                    else
                                    {
                                        Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.LINECOMMENT:
                            #region Code to process a char inside a linecomment, a token breaker are "\n" and "\r\n"
                            if (CurChar == '\r')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\n')
                                {
                                    Current.Text += "\r\n";
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text += "\r";
                                }
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text += "\n";
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.BLOCKCOMMENT:
                            #region Code to process a char inside a block comment, the only token breaker is "*/"
                            if (CurChar == '*')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '/')
                                {
                                    Current.Text += "*/";
                                    Back.AddToken(Current);
                                    Current = null;
                                    index++;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;
                        case TokenType.STRING:
                            #region Code to process a char inside a string, only token breaker is "'", but not "''"
                            if (CurChar == '\'')
                            {
                                char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                                if (nextc == '\'')
                                {//double '', so is an escaped ', it is a string still then
                                    Current.Text += "''";
                                    index++;
                                }
                                else
                                {
                                    Current.Text += "'";
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            #endregion
                            break;

                    }
                }
                else if (Current == null)
                {
                    #region no previous token, must check if create a new one, or let the current stay null and add a new instance of token
                    if (IsWhiteSpace(CurChar))
                    {
                        Current = new Token(TokenType.EMPTYSPACE, CurChar.ToString());
                    }
                    else if (CurChar.IsComma())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.COMMA, CurChar.ToString()));
                    }
                    else if (CurChar.IsStringOperator())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.STRING, CurChar.ToString()));
                    }
                    else if (CurChar.IsOpenBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.OPENBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsCloseBracket())
                    {//1 char token, so Current stays null
                        Back.AddToken(new Token(TokenType.CLOSEBRACKET, CurChar.ToString()));
                    }
                    else if (CurChar.IsOperator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        //must check if this chars means the start of comment
                        if (CurChar == '-' && nextc == '-')
                        {
                            Current = new Token(TokenType.LINECOMMENT, "--");
                            index++;
                        }
                        else if (CurChar == '/' && nextc == '*')
                        {
                            Current = new Token(TokenType.BLOCKCOMMENT, "/*");
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.OPERATOR, CurChar.ToString()));
                        }
                    }
                    else if (CurChar.IsComparator())
                    {
                        char nextc = Text.Length > index + 1 ? Text[index + 1] : ' ';
                        if (nextc.IsComparator())
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString() + nextc.ToString()));
                            index++;
                        }
                        else
                        {
                            Back.AddToken(new Token(TokenType.COMPARATOR, CurChar.ToString()));
                        }
                    }
                    else
                    {
                        Current = new Token(TokenType.WORD, CurChar.ToString());
                    }
                    #endregion
                }
            }
            if (Current != null)
                Back.AddToken(Current);

            return Back.TokenCount > 0 ? Back[0] : new Token(TokenType.EMPTYSPACE, "");
        }

        /// <summary>
        /// Parses the whole string and returns the last token found.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <returns>A token extracted from the input text.</returns>
        public static Token GetLastToken(this string Text)
        {
            return Text.GetTokens().List.LastOrDefault() ?? new Token(TokenType.EMPTYSPACE, "");
        }

        /// <summary>
        /// Checks if the provided word is a reserved SQL keyword.
        /// </summary>
        /// <param name="Word">The word to check.</param>
        /// <returns>True if the word is a reserved SQL keyword; otherwise, false.</returns>
        public static bool IsReserved(this string Word)
        {
            return ReservedWords.Contains(Word.ToUpper());
        }

        /// <summary>
        /// Determines whether the specified string represents a valid SQL data type.
        /// </summary>
        /// <remarks>The method checks the string against a predefined list of SQL data types. The
        /// comparison is case-insensitive.</remarks>
        /// <param name="Word">The string to evaluate. This value is case-insensitive.</param>
        /// <returns><see langword="true"/> if the specified string matches a valid SQL data type; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsSqlDataType(this string Word)
        {
            return SqlDataTypes.Contains(Word.ToUpper());
        }

        /// <summary>
        /// Parses a text string and converts it into a list of generic tokens based on the specified programming language.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <param name="language">The programming language to use for parsing rules.</param>
        /// <returns>A GenericTokenList containing the parsed tokens.</returns>
        public static GenericTokenList GetTokens(this string Text, GenericLanguage language)
        {
            LanguageConfig config = GetLanguageConfig(language);
            GenericTokenList Back = new GenericTokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            GenericToken Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                char CurChar = Text[index];
                char nextChar = Text.Length > index + 1 ? Text[index + 1] : '\0';

                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        case GenericTokenType.EMPTYSPACE:
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            break;

                        case GenericTokenType.WORD:
                            if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.NUMBER:
                            if (char.IsDigit(CurChar) || CurChar == '.' || (config.AllowUnderscoreInNumbers && CurChar == '_'))
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                // Number followed by letter = switch to WORD
                                Current.Type = GenericTokenType.WORD;
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.LINECOMMENT:
                            if (CurChar == '\r' && nextChar == '\n')
                            {
                                Current.Text += "\r\n";
                                Back.AddToken(Current);
                                Current = null;
                                index++;
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text = Current.Text.AppendChar('\n');
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.BLOCKCOMMENT:
                            if (config.BlockCommentEnd != null && CurChar == config.BlockCommentEnd[0])
                            {
                                bool isEnd = true;
                                for (int i = 1; i < config.BlockCommentEnd.Length && index + i < StringLength; i++)
                                {
                                    if (Text[index + i] != config.BlockCommentEnd[i])
                                    {
                                        isEnd = false;
                                        break;
                                    }
                                }
                                if (isEnd)
                                {
                                    Current.Text += config.BlockCommentEnd;
                                    Back.AddToken(Current);
                                    Current = null;
                                    index += config.BlockCommentEnd.Length - 1;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.STRING:
                            if (config.StringEscapeChar.HasValue && CurChar == config.StringEscapeChar.Value && nextChar != '\0')
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                                Current.Text = Current.Text.AppendChar(nextChar);
                                index++;
                            }
                            else if (config.StringDelimiters.Contains(CurChar))
                            {
                                // Check if this is the closing delimiter
                                if (Current.Text.Length > 0 && Current.Text[0] == CurChar)
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;
                    }
                }
                else // Current == null
                {
                    index = ProcessNewToken(Text, index, ref Current, Back, config);
                }
            }

            if (Current != null)
                Back.AddToken(Current);

            // Post-processing: identify RESERVED words and DATATYPE
            for (int i = 0; i < Back.TokenCount; i++)
            {
                if (Back[i].Type == GenericTokenType.WORD)
                {
                    if (config.ReservedKeywords.Contains(Back[i].Text))
                    {
                        Back[i].Type = GenericTokenType.RESERVED;
                    }
                    else if (config.DataTypes.Contains(Back[i].Text))
                    {
                        Back[i].Type = GenericTokenType.DATATYPE;
                    }
                }
            }

            return Back;
        }

        /// <summary>
        /// Parses a text string and returns the first token found based on the specified programming language.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <param name="language">The programming language to use for parsing rules.</param>
        /// <returns>The first GenericToken found, or an empty EMPTYSPACE token if no token is found.</returns>
        public static GenericToken GetFirstToken(this string Text, GenericLanguage language)
        {
            LanguageConfig config = GetLanguageConfig(language);
            GenericTokenList Back = new GenericTokenList();
            int StringLength = String.IsNullOrEmpty(Text) ? 0 : Text.Length;
            GenericToken Current = null;

            for (int index = 0; index < StringLength; index++)
            {
                if (Back.TokenCount > 1)
                    break;

                char CurChar = Text[index];
                char nextChar = Text.Length > index + 1 ? Text[index + 1] : '\0';

                if (Current != null)
                {
                    switch (Current.Type)
                    {
                        case GenericTokenType.EMPTYSPACE:
                            if (CurChar.IsWhiteSpace())
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            break;

                        case GenericTokenType.WORD:
                            if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.NUMBER:
                            if (char.IsDigit(CurChar) || CurChar == '.' || (config.AllowUnderscoreInNumbers && CurChar == '_'))
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            else if (IsWordBreaker(CurChar, config))
                            {
                                Back.AddToken(Current);
                                Current = null;
                                index = ProcessNewToken(Text, index, ref Current, Back, config);
                            }
                            else
                            {
                                Current.Type = GenericTokenType.WORD;
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.LINECOMMENT:
                            if (CurChar == '\r' && nextChar == '\n')
                            {
                                Current.Text += "\r\n";
                                Back.AddToken(Current);
                                Current = null;
                                index++;
                            }
                            else if (CurChar == '\n')
                            {
                                Current.Text = Current.Text.AppendChar('\n');
                                Back.AddToken(Current);
                                Current = null;
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.BLOCKCOMMENT:
                            if (config.BlockCommentEnd != null && CurChar == config.BlockCommentEnd[0])
                            {
                                bool isEnd = true;
                                for (int i = 1; i < config.BlockCommentEnd.Length && index + i < StringLength; i++)
                                {
                                    if (Text[index + i] != config.BlockCommentEnd[i])
                                    {
                                        isEnd = false;
                                        break;
                                    }
                                }
                                if (isEnd)
                                {
                                    Current.Text += config.BlockCommentEnd;
                                    Back.AddToken(Current);
                                    Current = null;
                                    index += config.BlockCommentEnd.Length - 1;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;

                        case GenericTokenType.STRING:
                            if (config.StringEscapeChar.HasValue && CurChar == config.StringEscapeChar.Value && nextChar != '\0')
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                                Current.Text = Current.Text.AppendChar(nextChar);
                                index++;
                            }
                            else if (config.StringDelimiters.Contains(CurChar))
                            {
                                if (Current.Text.Length > 0 && Current.Text[0] == CurChar)
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                    Back.AddToken(Current);
                                    Current = null;
                                }
                                else
                                {
                                    Current.Text = Current.Text.AppendChar(CurChar);
                                }
                            }
                            else
                            {
                                Current.Text = Current.Text.AppendChar(CurChar);
                            }
                            break;
                    }
                }
                else
                {
                    index = ProcessNewToken(Text, index, ref Current, Back, config);
                }
            }

            if (Current != null)
                Back.AddToken(Current);

            // Post-processing for the first token
            if (Back.TokenCount > 0)
            {
                GenericToken firstToken = Back[0];
                if (firstToken.Type == GenericTokenType.WORD)
                {
                    if (config.ReservedKeywords.Contains(firstToken.Text))
                    {
                        firstToken.Type = GenericTokenType.RESERVED;
                    }
                    else if (config.DataTypes.Contains(firstToken.Text))
                    {
                        firstToken.Type = GenericTokenType.DATATYPE;
                    }
                }
                return firstToken;
            }

            return new GenericToken(GenericTokenType.EMPTYSPACE, "");
        }

        /// <summary>
        /// Parses a text string and returns the last token found based on the specified programming language.
        /// </summary>
        /// <param name="Text">The input text to tokenize.</param>
        /// <param name="language">The programming language to use for parsing rules.</param>
        /// <returns>The last GenericToken found, or an empty EMPTYSPACE token if no token is found.</returns>
        public static GenericToken GetLastToken(this string Text, GenericLanguage language)
        {
            GenericTokenList tokens = Text.GetTokens(language);
            return tokens.List.LastOrDefault() ?? new GenericToken(GenericTokenType.EMPTYSPACE, "");
        }

        private static int ProcessNewToken(string Text, int index, ref GenericToken Current, GenericTokenList Back, LanguageConfig config)
        {
            char CurChar = Text[index];
            char nextChar = Text.Length > index + 1 ? Text[index + 1] : '\0';

            if (CurChar.IsWhiteSpace())
            {
                Current = new GenericToken(GenericTokenType.EMPTYSPACE, CurChar.ToString());
            }
            else if (CurChar == ',')
            {
                Back.AddToken(new GenericToken(GenericTokenType.COMMA, ","));
            }
            else if (CurChar == '.')
            {
                // Check if it's a decimal number starting with a dot
                if (char.IsDigit(nextChar))
                {
                    Current = new GenericToken(GenericTokenType.NUMBER, ".");
                }
                else
                {
                    Back.AddToken(new GenericToken(GenericTokenType.DOT, "."));
                }
            }
            else if (CurChar == ';')
            {
                Back.AddToken(new GenericToken(GenericTokenType.SEMICOLON, ";"));
            }
            else if (CurChar == ':')
            {
                Back.AddToken(new GenericToken(GenericTokenType.COLON, ":"));
            }
            else if (config.OpenBrackets.Contains(CurChar))
            {
                Back.AddToken(new GenericToken(GenericTokenType.OPENBRACKET, CurChar.ToString()));
            }
            else if (config.CloseBrackets.Contains(CurChar))
            {
                Back.AddToken(new GenericToken(GenericTokenType.CLOSEBRACKET, CurChar.ToString()));
            }
            else if (config.StringDelimiters.Contains(CurChar))
            {
                Current = new GenericToken(GenericTokenType.STRING, CurChar.ToString());
            }
            else if (config.LineCommentStart != null && CurChar == config.LineCommentStart[0])
            {
                bool isComment = true;
                for (int i = 1; i < config.LineCommentStart.Length && index + i < Text.Length; i++)
                {
                    if (Text[index + i] != config.LineCommentStart[i])
                    {
                        isComment = false;
                        break;
                    }
                }
                if (isComment)
                {
                    Current = new GenericToken(GenericTokenType.LINECOMMENT, config.LineCommentStart);
                    return index + config.LineCommentStart.Length - 1;
                }
                else
                {
                    // It's an operator
                    Current = new GenericToken(GenericTokenType.OPERATOR, CurChar.ToString());
                }
            }
            else if (config.BlockCommentStart != null && CurChar == config.BlockCommentStart[0])
            {
                bool isComment = true;
                for (int i = 1; i < config.BlockCommentStart.Length && index + i < Text.Length; i++)
                {
                    if (Text[index + i] != config.BlockCommentStart[i])
                    {
                        isComment = false;
                        break;
                    }
                }
                if (isComment)
                {
                    Current = new GenericToken(GenericTokenType.BLOCKCOMMENT, config.BlockCommentStart);
                    return index + config.BlockCommentStart.Length - 1;
                }
                else if (config.Operators.Contains(CurChar.ToString()))
                {
                    Current = new GenericToken(GenericTokenType.OPERATOR, CurChar.ToString());
                }
                else
                {
                    Current = new GenericToken(GenericTokenType.WORD, CurChar.ToString());
                }
            }
            else if (char.IsDigit(CurChar))
            {
                Current = new GenericToken(GenericTokenType.NUMBER, CurChar.ToString());
            }
            else if (config.Operators.Contains(CurChar.ToString()))
            {
                // Check for multi-character operators
                string twoCharOp = CurChar.ToString() + nextChar.ToString();
                if (config.Operators.Contains(twoCharOp))
                {
                    Back.AddToken(new GenericToken(GenericTokenType.OPERATOR, twoCharOp));
                    return index + 1;
                }
                else
                {
                    Back.AddToken(new GenericToken(GenericTokenType.OPERATOR, CurChar.ToString()));
                }
            }
            else
            {
                Current = new GenericToken(GenericTokenType.WORD, CurChar.ToString());
            }

            return index;
        }

        private static bool IsWordBreaker(char c, LanguageConfig config)
        {
            return c.IsWhiteSpace() || c == ',' || c == '.' || c == ';' || c == ':' ||
                   config.OpenBrackets.Contains(c) || config.CloseBrackets.Contains(c) ||
                   config.StringDelimiters.Contains(c) || config.Operators.Contains(c.ToString());
        }

        private static LanguageConfig GetLanguageConfig(GenericLanguage language)
        {
            switch (language)
            {
                case GenericLanguage.CSharp:
                    return new LanguageConfig
                    {
                        ReservedKeywords = new HashSet<string>
                        {
                            "abstract", "as", "base", "break", "case", "catch", "checked",
                            "class", "const", "continue", "default", "delegate", "do", "else",
                            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "for",
                            "foreach", "goto", "if", "implicit", "in", "interface", "internal", "is", "lock",
                            "namespace", "new", "null", "operator", "out", "override", "params",
                            "private", "protected", "public", "readonly", "ref", "return", "sealed",
                            "sizeof", "stackalloc", "static", "struct", "switch", "this", "throw",
                            "true", "try", "typeof", "unchecked", "unsafe", "using",
                            "virtual", "volatile", "while", "add", "alias", "ascending", "async", "await",
                            "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let",
                            "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield"
                        },
                        DataTypes = new HashSet<string>
                        {
                            "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint",
                            "long", "ulong", "short", "ushort", "object", "string", "void"
                        },
                        LineCommentStart = "//",
                        BlockCommentStart = "/*",
                        BlockCommentEnd = "*/",
                        StringDelimiters = new HashSet<char> { '"', '\'' },
                        StringEscapeChar = '\\',
                        Operators = new HashSet<string>
                        {
                            "+", "-", "*", "/", "%", "=", "==", "!=", ">", "<", ">=", "<=",
                            "&&", "||", "!", "&", "|", "^", "~", "<<", ">>", "++", "--",
                            "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=",
                            "??", "?.", "=>", "??"
                        },
                        OpenBrackets = new HashSet<char> { '(', '[', '{', '<' },
                        CloseBrackets = new HashSet<char> { ')', ']', '}', '>' },
                        AllowUnderscoreInNumbers = true
                    };

                case GenericLanguage.JavaScript:
                    return new LanguageConfig
                    {
                        ReservedKeywords = new HashSet<string>
                        {
                            "abstract", "arguments", "await", "break", "case", "catch",
                            "class", "const", "continue", "debugger", "default", "delete", "do", "else",
                            "enum", "eval", "export", "extends", "false", "final", "finally", "for",
                            "function", "goto", "if", "implements", "import", "in", "instanceof", "interface",
                            "let", "native", "new", "null", "package", "private", "protected", "public",
                            "return", "static", "super", "switch", "synchronized", "this", "throw",
                            "throws", "transient", "true", "try", "typeof", "var", "volatile", "while",
                            "with", "yield", "async", "of"
                        },
                        DataTypes = new HashSet<string>
                        {
                            "boolean", "byte", "char", "double", "float", "int", "long", "short", "void"
                        },
                        LineCommentStart = "//",
                        BlockCommentStart = "/*",
                        BlockCommentEnd = "*/",
                        StringDelimiters = new HashSet<char> { '"', '\'', '`' },
                        StringEscapeChar = '\\',
                        Operators = new HashSet<string>
                        {
                            "+", "-", "*", "/", "%", "=", "==", "===", "!=", "!==", ">", "<", ">=", "<=",
                            "&&", "||", "!", "&", "|", "^", "~", "<<", ">>", ">>>", "++", "--",
                            "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=", ">>>=",
                            "??", "?.", "=>", "**", "**="
                        },
                        OpenBrackets = new HashSet<char> { '(', '[', '{' },
                        CloseBrackets = new HashSet<char> { ')', ']', '}' },
                        AllowUnderscoreInNumbers = true
                    };

                case GenericLanguage.Html:
                    return new LanguageConfig
                    {
                        ReservedKeywords = new HashSet<string>(), // HTML doesn't have keywords in the traditional sense
                        DataTypes = new HashSet<string>(), // HTML doesn't have datatypes
                        LineCommentStart = null,
                        BlockCommentStart = "<!--",
                        BlockCommentEnd = "-->",
                        StringDelimiters = new HashSet<char> { '"', '\'' },
                        StringEscapeChar = null,
                        Operators = new HashSet<string> { "=", "/" },
                        OpenBrackets = new HashSet<char> { '<', '(' },
                        CloseBrackets = new HashSet<char> { '>', ')' },
                        AllowUnderscoreInNumbers = false
                    };

                case GenericLanguage.Css:
                    return new LanguageConfig
                    {
                        ReservedKeywords = new HashSet<string>
                        {
                            "important", "inherit", "initial", "unset", "auto", "none", "normal", "all"
                        },
                        DataTypes = new HashSet<string>(), // CSS doesn't have traditional datatypes
                        LineCommentStart = null,
                        BlockCommentStart = "/*",
                        BlockCommentEnd = "*/",
                        StringDelimiters = new HashSet<char> { '"', '\'' },
                        StringEscapeChar = '\\',
                        Operators = new HashSet<string> { ":", ">", "+", "~", "*", "=" },
                        OpenBrackets = new HashSet<char> { '{', '(', '[' },
                        CloseBrackets = new HashSet<char> { '}', ')', ']' },
                        AllowUnderscoreInNumbers = false
                    };

                case GenericLanguage.Xml:
                    return new LanguageConfig
                    {
                        ReservedKeywords = new HashSet<string>(),
                        DataTypes = new HashSet<string>(), // XML doesn't have datatypes
                        LineCommentStart = null,
                        BlockCommentStart = "<!--",
                        BlockCommentEnd = "-->",
                        StringDelimiters = new HashSet<char> { '"', '\'' },
                        StringEscapeChar = null,
                        Operators = new HashSet<string> { "=", "/" },
                        OpenBrackets = new HashSet<char> { '<', '(' },
                        CloseBrackets = new HashSet<char> { '>', ')' },
                        AllowUnderscoreInNumbers = false
                    };

                case GenericLanguage.Json:
                    return new LanguageConfig
                    {
                        ReservedKeywords = new HashSet<string> { "true", "false", "null" },
                        DataTypes = new HashSet<string>(), // JSON doesn't have datatypes
                        LineCommentStart = null,
                        BlockCommentStart = null,
                        BlockCommentEnd = null,
                        StringDelimiters = new HashSet<char> { '"' },
                        StringEscapeChar = '\\',
                        Operators = new HashSet<string> { ":" },
                        OpenBrackets = new HashSet<char> { '{', '[' },
                        CloseBrackets = new HashSet<char> { '}', ']' },
                        AllowUnderscoreInNumbers = false
                    };

                default:
                    throw new ArgumentException($"Unsupported language: {language}");
            }
        }

        private class LanguageConfig
        {
            public HashSet<string> ReservedKeywords { get; set; }
            public HashSet<string> DataTypes { get; set; }
            public string LineCommentStart { get; set; }
            public string BlockCommentStart { get; set; }
            public string BlockCommentEnd { get; set; }
            public HashSet<char> StringDelimiters { get; set; }
            public char? StringEscapeChar { get; set; }
            public HashSet<string> Operators { get; set; }
            public HashSet<char> OpenBrackets { get; set; }
            public HashSet<char> CloseBrackets { get; set; }
            public bool AllowUnderscoreInNumbers { get; set; }
        }
        #endregion

        #region Extensions for char
        /// <summary>
        /// Converts a hexadecimal character (0-9, A-F, a-f) to its decimal integer equivalent.
        /// </summary>
        /// <param name="c">The hexadecimal character to convert. Valid characters are '0'-'9', 'A'-'F', 'a'-'f'.</param>
        /// <returns>
        /// The integer value represented by the hex character in the range 0 to 15.
        /// Returns -1 if the character is not a valid hexadecimal digit.
        /// </returns>
        public static int FromHexCharToDecInt(this char c)
        {
            switch (c)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the character is a closing bracket.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a closing bracket: ')', ']' or '}'.</returns>
        private static bool IsCloseBracket(this char c)
        {
            return (c == ')' || c == ']' || c == '}');
        }

        /// <summary>
        /// Determines whether the character is a comma.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a comma ','; otherwise false.</returns>
        private static bool IsComma(this char c)
        {
            return c == ',';
        }

        /// <summary>
        /// Determines whether the character is a comparator symbol.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is one of '>', '<' or '='; otherwise false.</returns>
        private static bool IsComparator(this char c)
        {
            return (c == '>' || c == '<' || c == '=');
        }

        /// <summary>
        /// Determines whether the character is an opening bracket.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is an opening bracket: '(', '[' or '{'. Returns false otherwise.</returns>
        private static bool IsOpenBracket(this char c)
        {
            return (c == '(' || c == '[' || c == '{');
        }

        /// <summary>
        /// Determines whether the character is an arithmetic operator or division symbol used in token parsing.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is one of '+', '/', '-', '*' ; otherwise false.</returns>
        private static bool IsOperator(this char c)
        {
            return (c == '+' || c == '/' || c == '-' || c == '*');
        }

        /// <summary>
        /// Determines whether the character is a semicolon ';'.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is ';'; otherwise false.</returns>
        private static bool IsSemmiColon(this char c)
        {
            return c == ';';
        }

        /// <summary>
        /// Determines whether the character is a SQL string delimiter (single quote).
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a single quote '\''; otherwise false.</returns>
        private static bool IsStringOperator(this char c)
        {
            return c == '\'';
        }

        /// <summary>
        /// Determines whether the character is whitespace used in parsing.
        /// </summary>
        /// <param name="c">The character to test.</param>
        /// <returns>True if the character is a space, tab, carriage return or newline; otherwise false.</returns>
        private static bool IsWhiteSpace(this char c)
        {
            return (c == ' ' || c == '\t' || c == '\r' || c == '\n');
        }
        #endregion

    }
}
