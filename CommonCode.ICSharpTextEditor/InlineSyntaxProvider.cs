using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ZidUtilities.CommonCode;

namespace CommonCode.ICSharpTextEditor
{
    /// <summary>
    /// Provides an in-memory syntax definition to the ICSharpCode text editor by implementing
    /// <see cref="ISyntaxModeFileProvider"/>.
    /// The syntax highlighting definitions all come from this URL: https://github.com/xv/ICSharpCode.TextEditor-Lexers
    /// Except for TSQL which was created by me.
    /// <see cref="ISyntaxModeFileProvider"/>.
    /// </summary>
    public class InlineSyntaxProvider : ISyntaxModeFileProvider
    {
        private List<SyntaxMode> syntaxModes;

        /// <summary>
        /// Gets the collection of available <see cref="SyntaxMode"/> objects exposed by this provider.
        /// </summary>
        /// <value>A collection containing the syntax modes this provider supplies.</value>
        public ICollection<SyntaxMode> SyntaxModes => syntaxModes;

        /// <summary>
        /// Gets or sets the language enum value that identifies which embedded syntax definition to use.
        /// </summary>
        /// <value>The <see cref="SyntaxLanguage"/> selected for this provider.</value>
        public SyntaxLanguage Language { get; set; }

        /// <summary>
        /// Gets or sets the raw XML string that represents the syntax highlighting definition.
        /// </summary>
        /// <value>A string containing the full XSHD (XML) syntax definition.</value>
        public string SyntaxString { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineSyntaxProvider"/> class for the specified language.
        /// </summary>
        /// <param name="language">The syntax language enumeration value to select an embedded syntax definition.</param>
        /// <remarks>
        /// The constructor loads the syntax definition XML for the given language using
        /// <see cref="SyntaxProvider.GetSyntaxFile(SyntaxLanguage)"/>, initializes the internal
        /// syntax mode list and registers a single unique <see cref="SyntaxMode"/> entry.
        /// </remarks>
        public InlineSyntaxProvider(SyntaxLanguage language)
        {
            this.syntaxModes = new List<SyntaxMode>();
            this.Language = language;
            SyntaxString = SyntaxProvider.GetSyntaxFile(language);
            UpdateSyntaxModeList();
        }

        /// <summary>
        /// Returns an <see cref="XmlTextReader"/> that reads the syntax mode definition for the provided <see cref="SyntaxMode"/>.
        /// </summary>
        /// <param name="syntaxMode">The <see cref="SyntaxMode"/> for which the syntax definition is requested.
        /// The implementation ignores properties of the provided object and always returns the same in-memory definition.</param>
        /// <returns>
        /// An <see cref="XmlTextReader"/> instance positioned at the start of the XML syntax definition stream.
        /// The caller is responsible for disposing the reader when finished.
        /// </returns>
        /// <remarks>
        /// This method takes the in-memory <see cref="SyntaxString"/>, replaces the root &lt;SyntaxDefinition ...&gt; element
        /// with a controlled element name ("InlineLanguage") and returns a reader over the transformed string.
        /// </remarks>
        public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
        {
            StringBuilder lines = new StringBuilder();
            string replacement = $"<SyntaxDefinition name=\"InlineLanguage\" extensions=\"DontCare\">";
            using (StringReader reader = new StringReader(SyntaxString))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("<SyntaxDefinition"))
                        lines.Append(replacement);
                    else
                        lines.Append(line);
                }
            }
            TextReader buff = new StringReader(lines.ToString());
            return new XmlTextReader(buff);
        }

        /// <summary>
        /// Ensures the provider's internal syntax mode list contains a single unique <see cref="SyntaxMode"/> entry
        /// that references the in-memory syntax definition.
        /// </summary>
        /// <remarks>
        /// This method creates a <see cref="SyntaxMode"/> named "InlineLanguage.xshd" with a single fake extension
        /// ".DontCare" and adds it to the internal list. It does not check for duplicates; calling it multiple times
        /// will append additional entries.
        /// </remarks>
        public void UpdateSyntaxModeList()
        {
            SyntaxMode uniqueSyntax = new SyntaxMode("InlineLanguage.xshd", "InlineLanguage", "")
            {
                Extensions = new string[] { ".DontCare" }
            };
            syntaxModes.Add(uniqueSyntax);
        }
    }
}
