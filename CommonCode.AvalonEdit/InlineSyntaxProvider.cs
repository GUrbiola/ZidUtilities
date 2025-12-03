using System;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace ZidUtilities.CommonCode.AvalonEdit
{
    /// <summary>
    /// Provides an in-memory syntax definition for AvalonEdit.
    /// The syntax highlighting definitions all come from this URL: https://github.com/xv/ICSharpCode.TextEditor-Lexers
    /// Except for TSQL which was created by me.
    /// </summary>
    public class InlineSyntaxProvider
    {
        /// <summary>
        /// Gets or sets the language enum value that identifies which embedded syntax definition to use.
        /// </summary>
        public SyntaxLanguage Language { get; set; }

        /// <summary>
        /// Gets or sets the raw XML string that represents the syntax highlighting definition.
        /// </summary>
        public string SyntaxString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineSyntaxProvider"/> class for the specified language.
        /// </summary>
        /// <param name="language">The syntax language enumeration value to select an embedded syntax definition.</param>
        public InlineSyntaxProvider(SyntaxLanguage language)
        {
            this.Language = language;
            SyntaxString = SyntaxProvider.GetSyntaxFile(language);
        }

        /// <summary>
        /// Loads the syntax highlighting definition for AvalonEdit.
        /// </summary>
        /// <returns>An <see cref="IHighlightingDefinition"/> that can be applied to AvalonEdit's TextEditor.</returns>
        public IHighlightingDefinition GetHighlightingDefinition()
        {
            try
            {
                using (StringReader reader = new StringReader(SyntaxString))
                using (XmlReader xmlReader = XmlReader.Create(reader))
                {
                    return HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
