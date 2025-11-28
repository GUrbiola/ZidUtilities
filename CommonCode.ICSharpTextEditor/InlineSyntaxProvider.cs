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
    public class InlineSyntaxProvider : ISyntaxModeFileProvider
    {
        private List<SyntaxMode> syntaxModes;

        public ICollection<SyntaxMode> SyntaxModes => syntaxModes;
        public SyntaxLanguage Language { get; set; }

        public string SyntaxString { get; set; }

        public InlineSyntaxProvider(SyntaxLanguage language)
        {
            this.syntaxModes = new List<SyntaxMode>();
            this.Language = language;   
            SyntaxString = SyntaxProvider.GetSyntaxFile(language);
            UpdateSyntaxModeList();
        }

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
