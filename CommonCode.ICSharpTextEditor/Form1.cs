using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.ICSharpTextEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ExtendedEditor textEditor = new ExtendedEditor();
            textEditor.OnEditorKeyPress += TextEditor_OnEditorKeyPress;
        }

        private bool TextEditor_OnEditorKeyPress(Keys keyData)
        {
            throw new NotImplementedException();
        }
    }
}
