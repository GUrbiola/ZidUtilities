using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.ICSharpTextEditor;
using ZidUtilities.CommonCode.Win.Controls.Diff;

namespace ZidUtilities.TesterWin
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void loadSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sampleSQL1, sampleSQL2;

            diffInspector1.Highlighting = SyntaxHighlighting.TransactSQL;

            sampleSQL1 = System.IO.File.ReadAllText(@"C:\Users\Gonzalo\Desktop\Extensions\sample sql1.sql");
            sampleSQL2 = System.IO.File.ReadAllText(@"C:\Users\Gonzalo\Desktop\Extensions\sample sql2.sql");

            diffInspector1.LoadTexts(sampleSQL1, sampleSQL2);
        }

        private void loadCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sampleCsharp1, sampleCsharp2;

            diffInspector1.Highlighting = SyntaxHighlighting.CSharp;

            sampleCsharp1 = System.IO.File.ReadAllText(@"C:\Users\Gonzalo\Desktop\Extensions\Extensions11.cs");
            sampleCsharp2 = System.IO.File.ReadAllText(@"C:\Users\Gonzalo\Desktop\Extensions\Extensions12.cs");

            diffInspector1.LoadTexts(sampleCsharp1, sampleCsharp2);
        }

        private void loadJavaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string js1, js2;

            diffInspector1.Highlighting = SyntaxHighlighting.JavaScript;

            js1 = System.IO.File.ReadAllText(@"D:\Restart\PlusSalud\PlusSalud\Website\Scripts\Common.js");
            js2 = System.IO.File.ReadAllText(@"D:\FF Again\FFsSupplierPortal\SupplierPortal\WebApp\Scripts\Common.js");

            diffInspector1.LoadTexts(js1, js2);
        }
    }
}
