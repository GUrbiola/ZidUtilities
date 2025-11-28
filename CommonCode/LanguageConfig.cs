using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{


    public class LanguageConfig
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
        
        public static LanguageConfig GetLanguageConfig(GenericLanguage language)
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
                            "??", "?.", "=>", "?"
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
    }
}
