using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{
    /// <summary>
    /// Describes lexical and syntactic configuration information for a generic programming or markup language.
    /// Instances of this class provide sets of keywords, data types, comment delimiters, string delimiters,
    /// operator tokens and bracket characters to be used by tokenizers, highlighters and parsers.
    /// </summary>
    public class LanguageConfig
    {
        /// <summary>
        /// Gets or sets the set of reserved keywords for the language.
        /// These words are typically used to identify tokens that cannot be used as identifiers.
        /// The set may be empty for languages without traditional reserved keywords.
        /// </summary>
        public HashSet<string> ReservedKeywords { get; set; }

        /// <summary>
        /// Gets or sets the set of built-in data type names for the language.
        /// This is used for classification of tokens that represent primitive types. May be empty.
        /// </summary>
        public HashSet<string> DataTypes { get; set; }

        /// <summary>
        /// Gets or sets the string that begins a single-line comment (e.g. "//").
        /// Set to null when the language does not support single-line comments.
        /// </summary>
        public string LineCommentStart { get; set; }

        /// <summary>
        /// Gets or sets the string that begins a block comment (e.g. "/*").
        /// Set to null when the language does not support block comments.
        /// </summary>
        public string BlockCommentStart { get; set; }

        /// <summary>
        /// Gets or sets the string that ends a block comment (e.g. "*/").
        /// This must be paired with <see cref="BlockCommentStart"/>; may be null when block comments are unsupported.
        /// </summary>
        public string BlockCommentEnd { get; set; }

        /// <summary>
        /// Gets or sets the set of characters used to delimit string literals (e.g. '"', ''' or '`').
        /// The tokenizer should treat text enclosed by these characters as string literals, subject to escaping rules.
        /// </summary>
        public HashSet<char> StringDelimiters { get; set; }

        /// <summary>
        /// Gets or sets the character used to escape characters inside string literals (e.g. '\').
        /// Set to null if the language does not use an escape character for strings.
        /// </summary>
        public char? StringEscapeChar { get; set; }

        /// <summary>
        /// Gets or sets the set of operator tokens (strings) used by the language.
        /// Operators may consist of one or more characters (e.g. "==", "=>"). The tokenizer should match longest-first when parsing.
        /// </summary>
        public HashSet<string> Operators { get; set; }

        /// <summary>
        /// Gets or sets the set of opening bracket characters (e.g. '(', '{', '[').
        /// These are used to track nesting and grouping in tokenization/parsing.
        /// </summary>
        public HashSet<char> OpenBrackets { get; set; }

        /// <summary>
        /// Gets or sets the set of closing bracket characters (e.g. ')', '}', ']').
        /// These correspond to <see cref="OpenBrackets"/> and are used for matching bracket pairs.
        /// </summary>
        public HashSet<char> CloseBrackets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether underscores are allowed within numeric literals for this language.
        /// When true, numeric parsers/tokenizers should accept '_' as a digit separator (e.g. 1_000).
        /// </summary>
        public bool AllowUnderscoreInNumbers { get; set; }
        
        /// <summary>
        /// Returns a preconfigured <see cref="LanguageConfig"/> instance for the specified generic language.
        /// The returned configuration contains typical comment delimiters, operators, string delimiters,
        /// keywords and other lexical information suitable for tokenization and basic parsing tasks.
        /// </summary>
        /// <param name="language">The generic language to retrieve configuration for.</param>
        /// <returns>
        /// A <see cref="LanguageConfig"/> instance populated with language-specific sets and delimiters.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when an unsupported <paramref name="language"/> value is provided.</exception>
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
