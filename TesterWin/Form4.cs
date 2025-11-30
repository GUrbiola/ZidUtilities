using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Controls.AddressBar;

namespace ZidUtilities.TesterWin
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            FileSystemNode rootNode = new FileSystemNode("C:\\", null);
            addressBar1.RootNode = rootNode;
        }
    }
}
