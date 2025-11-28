using CommonCode.ICSharpTextEditor.BracketMatching;
using CommonCode.ICSharpTextEditor.FoldingStrategies;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CommonCode.ICSharpTextEditor
{
    public delegate void OnToolbarButtonClick(string selectedText, ToolbarOption btnClicked);
    public partial class ExtendedEditor : UserControl
    {
        public event OnToolbarButtonClick OnRun, OnStop, OnKill;

        private SyntaxHighlighting _Syntax;
        [Category("Custom Properties")]
        [Description("Defines the syntax highlighting that the control will have. If you need syntax highlighting for other language, access directly the Editor control and load it manually")]
        public SyntaxHighlighting Syntax
        {
            get
            {
                return _Syntax;
            }
            set
            {

                if (!this.DesignMode)
                {
                    //HighlightingManager.Manager.HighlightingDefinitions.Clear();
                    switch (value)
                    {
                        default:
                        case SyntaxHighlighting.None:
                            Editor.Document.HighlightingStrategy = this.DefaultHighlightingStrategy;
                            Editor.Document.FormattingStrategy = this.DefaultFormattingStrategy;
                            Editor.Document.FoldingManager.FoldingStrategy = null;
                            break;
                        case SyntaxHighlighting.CSharp:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.CSharp));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.CSharp);

                            GenericFoldingStrategy csharpFolding = new GenericFoldingStrategy();
                            csharpFolding.StartFolding.Add("/*");
                            csharpFolding.EndFolding.Add("*/");
                            csharpFolding.StartFolding.Add("#region");
                            csharpFolding.EndFolding.Add("#endregion");

                            csharpFolding.SpamFolding.Add("///");

                            Editor.Document.FoldingManager.FoldingStrategy = csharpFolding;
                            //Editor.Document.FoldingManager.FoldingStrategy = new CSharpFoldingStrategy();
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.XML:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.XML));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.Xml);

                            GenericFoldingStrategy xmlFolding = new GenericFoldingStrategy();

                            Editor.Document.FoldingManager.FoldingStrategy = xmlFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.MySql:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.MySQL));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new SqlBracketMatcher();
                            Editor.Document.FoldingManager.FoldingStrategy = new SqlFoldingStrategy();
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.TransactSQL:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.TSQL));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new SqlBracketMatcher();
                            Editor.Document.FoldingManager.FoldingStrategy = new SqlFoldingStrategy();
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.JavaScript:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.JavaScript));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.JavaScript);

                            GenericFoldingStrategy jsFolding = new GenericFoldingStrategy();
                            jsFolding.StartFolding.Add("/*");
                            jsFolding.EndFolding.Add("*/");

                            Editor.Document.FoldingManager.FoldingStrategy = jsFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.HTML:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.HTML));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.Html);

                            GenericFoldingStrategy htmlFolding = new GenericFoldingStrategy();
                            htmlFolding.StartFolding.Add("<!--");
                            htmlFolding.EndFolding.Add("-->");
                            htmlFolding.StartFolding.Add("<style>");
                            htmlFolding.EndFolding.Add("</style>");
                            htmlFolding.StartFolding.Add("<script>");
                            htmlFolding.EndFolding.Add("</script>");
                            htmlFolding.StartFolding.Add("<body>");
                            htmlFolding.EndFolding.Add("</body>");
                            htmlFolding.StartFolding.Add("<head>");
                            htmlFolding.EndFolding.Add("</head>");



                            Editor.Document.FoldingManager.FoldingStrategy = htmlFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.CSS:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.CSS));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.Css);

                            GenericFoldingStrategy cssFolding = new GenericFoldingStrategy();
                            cssFolding.StartFolding.Add("/*");
                            cssFolding.EndFolding.Add("*/");

                            Editor.Document.FoldingManager.FoldingStrategy = cssFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.Json:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.JSON));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.Json);

                            GenericFoldingStrategy jsonFolding = new GenericFoldingStrategy();
                            jsonFolding.StartFolding.Add("{");
                            jsonFolding.EndFolding.Add("}");
                            jsonFolding.StartFolding.Add("[");
                            jsonFolding.EndFolding.Add("]");

                            Editor.Document.FoldingManager.FoldingStrategy = jsonFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                    }
                }
                _Syntax = value;
            }
        }
        [Category("Custom Properties")]
        [Description("Property that grants access to the text control itself: ICSharpCode.TextEditor.TextEditorControl")]
        public TextEditorControl Editor
        {
            get { return this.CtrlEditor; }
        }

        #region Toolbar Buttons
        private ToolbarOption _btnRun = new ToolbarOption("Run", "Executes selected/all code", Content.Play, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Button to execute code")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnRun
        {
            get
            {
                return _btnRun;
            }
            set
            {
                _btnRun = value;
            }
        }

        private ToolbarOption _btnStop = new ToolbarOption("Stop", "Stops code execution", Content.Stop, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Button to stop code execution")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnStop
        {
            get
            {
                return _btnStop;
            }
            set
            {
                _btnStop = value;
            }
        }

        private ToolbarOption _btnKill = new ToolbarOption("Kill", "Kills thread executing code", Content.RedAlert, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Button to kill thread executing code")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnKill
        {
            get
            {
                return _btnKill;
            }
            set
            {
                _btnKill = value;
            }
        }

        private ToolbarOption _btnComment = new ToolbarOption("Comment", "Comment selected code lines", Content.Comment, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Comment selected code lines")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnComment
        {
            get
            {
                return _btnComment;
            }
            set
            {
                _btnComment = value;
            }
        }

        private ToolbarOption _btnUncomment = new ToolbarOption("Uncomment", "Uncomment selected code lines", Content.UnComment, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Uncomment selected code lines")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnUncomment
        {
            get
            {
                return _btnUncomment;
            }
            set
            {
                _btnUncomment = value;
            }
        }

        private ToolbarOption _btnSearch = new ToolbarOption("Search", "Shows dialog to search/replace text from editor", Content.Search, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Shows dialog to search/replace text from editor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnSearch
        {
            get
            {
                return _btnSearch;
            }
            set
            {
                _btnSearch = value;
            }
        }

        private ToolbarOption _btnToggleBookmark = new ToolbarOption("ToggleBookmark", "Creates or remove a bookmark from the current line", Content.Bookmark, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Button that creates or remove a bookmark from the current line")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnToggleBookmark
        {
            get
            {
                return _btnToggleBookmark;
            }
            set
            {
                _btnToggleBookmark = value;
            }
        }

        private ToolbarOption _btnPreviousBookmark = new ToolbarOption("PreviousBookmark", "Moves cursor/position to the previous bookmark", Content.Previous, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Moves cursor/position to the previous bookmark")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnPreviousBookmark
        {
            get
            {
                return _btnPreviousBookmark;
            }
            set
            {
                _btnPreviousBookmark = value;
            }
        }

        private ToolbarOption _btnNextBookmark = new ToolbarOption("NextBookmark", "Moves cursor/position to the next bookmark", Content.Next, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Moves cursor/position to the next bookmark")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnNextBookmark
        {
            get
            {
                return _btnNextBookmark;
            }
            set
            {
                _btnNextBookmark = value;
            }
        }

        private ToolbarOption _btnClearBookmarks = new ToolbarOption("ClearBookmarks", "Clears all bookmarks", Content.DelBookmark, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Clears all bookmarks")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnClearBookmarks
        {
            get
            {
                return _btnClearBookmarks;
            }
            set
            {
                _btnClearBookmarks = value;
            }
        }

        private ToolbarOption _btnSaveToFile = new ToolbarOption("SaveToFile", "Save text on editor to a file", Content.Save, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Save text on editor to a file")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnSaveToFile
        {
            get
            {
                return _btnSaveToFile;
            }
            set
            {
                _btnSaveToFile = value;
            }
        }

        private ToolbarOption _btnLoadFromFile = new ToolbarOption("LoadFromFile", "Load text from file into the editor", Content.Open, true) { Enabled = true };
        [Category("Custom Properties")]
        [Description("Load text from file into the editor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnLoadFromFile
        {
            get
            {
                return _btnLoadFromFile;
            }
            set
            {
                _btnLoadFromFile = value;
            }
        }
        #endregion

        #region Toolbar TextBoxes
        private ToolbarTextBox _txt01Helper = new ToolbarTextBox("TextBoxHelper1", true);
        [Category("Custom Properties")]
        [Description("Input box that can be used to disply some text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarTextBox Txt01Helper
        {
            get { return _txt01Helper; }
            set { _txt01Helper = value; }
        }

        private ToolbarTextBox _txt02Helper = new ToolbarTextBox("TextBoxHelper2", true);
        [Category("Custom Properties")]
        [Description("Input box that can be used to disply some text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarTextBox Txt02Helper
        {
            get { return _txt02Helper; }
            set { _txt02Helper = value; }
        }
        #endregion

        public bool ShowToolbar
        {
            get
            {
                return this.CtrlToolbar.Visible;
            }
            set
            {
                this.CtrlToolbar.Visible = value;
            }
        }

        public IHighlightingStrategy DefaultHighlightingStrategy { get; private set; }
        public IFormattingStrategy DefaultFormattingStrategy { get; private set; }

        public ExtendedEditor()
        {
            InitializeComponent();

            this.DefaultHighlightingStrategy = Editor.Document.HighlightingStrategy;
            this.DefaultFormattingStrategy = Editor.Document.FormattingStrategy;

            _btnRun.Control = this.tsRun;
            _btnStop.Control = this.tsStop;
            _btnKill.Control = this.tsKill;
            _btnComment.Control = this.tsComment;
            _btnUncomment.Control = this.tsUncomment;
            _btnSearch.Control = this.tsSearch;
            _btnToggleBookmark.Control = this.tsToggleBookmark;
            _btnPreviousBookmark.Control = this.tsPreviousBookmark;
            _btnNextBookmark.Control = this.tsNextBookmark;
            _btnClearBookmarks.Control = this.tsClearBookmarks;
            _btnSaveToFile.Control = this.tsSaveToFile;
            _btnLoadFromFile.Control = this.tsLoadFromFile;

            List<ToolbarOption> toolbarOptions = new List<ToolbarOption>()
            {
                this.BtnRun,
                this.BtnStop,
                this.BtnKill,
                this.BtnComment,
                this.BtnUncomment,
                this.BtnSearch,
                this.BtnToggleBookmark,
                this.BtnPreviousBookmark,
                this.BtnNextBookmark,
                this.BtnClearBookmarks,
                this.BtnSaveToFile,
                this.BtnLoadFromFile
            };

            foreach (ToolbarOption option in toolbarOptions)
            {
                option.OptionChanged += (ToolbarOption changedOption) =>
                {
                    UpdateToolbarButton(changedOption);
                };

            }

            _txt01Helper.Control = this.tsText1;
            _txt02Helper.Control = this.tsText2;

            List<ToolbarTextBox> toolbarTextBoxes = new List<ToolbarTextBox>()
            {
                _txt01Helper,
                _txt02Helper
            };

            foreach (ToolbarTextBox textBox in toolbarTextBoxes)
            {
                textBox.TextOptionChanged += (ToolbarTextBox changedOption) =>
                {
                    UpdateToolbarTextBox(changedOption);
                };
            }

            CtrlEditor.Document.DocumentChanged += Query_DocumentChanged;

        }

        #region Code to refresh the folding, it will execute a second when the "last change" has been made 2 seconds ago
        private int ToRefresh = 10;
        void Query_DocumentChanged(object sender, DocumentEventArgs e)
        {
            ToRefresh = 10;
            if (!FoldingRefresher.Enabled && CtrlEditor.Document.FoldingManager.FoldingStrategy != null)
                FoldingRefresher.Enabled = true;
        }
        private void FoldingRefresher_Tick(object sender, EventArgs e)
        {
            ToRefresh--;
            if (ToRefresh <= 0)
            {
                if(CtrlEditor.Document.FoldingManager.FoldingStrategy != null)
                    CtrlEditor.Document.FoldingManager.UpdateFoldings(null, null);
                FoldingRefresher.Enabled = false;
                ToRefresh = 10;
            }
        }
        #endregion

        private void UpdateToolbarTextBox(ToolbarTextBox changedOption)
        {
            ToolStripTextBox toolbarTxtBox = changedOption.Control as ToolStripTextBox;
            if (toolbarTxtBox != null)
            {
                toolbarTxtBox.Visible = changedOption.Visible;
                toolbarTxtBox.Text = changedOption.Text;
                toolbarTxtBox.ToolTipText = changedOption.ToolTip;
            }
        }

        private void tsRun_Click(object sender, EventArgs e)
        {
            string CurrentScript = string.Empty;
            if (OnRun != null)
            {
                if (Editor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                    CurrentScript = Editor.ActiveTextAreaControl.SelectionManager.SelectedText;
                else
                    CurrentScript = Editor.Text;

                OnRun(CurrentScript, this.BtnRun);
            }
        }

        private void tsStop_Click(object sender, EventArgs e)
        {
            string CurrentScript = string.Empty;
            if (OnStop != null)
            {
                if (Editor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                    CurrentScript = Editor.ActiveTextAreaControl.SelectionManager.SelectedText;
                else
                    CurrentScript = Editor.Text;

                OnStop(CurrentScript, this.BtnStop);
            }
        }

        private void tsKill_Click(object sender, EventArgs e)
        {
            string CurrentScript = string.Empty;
            if (OnKill != null)
            {
                if (Editor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                    CurrentScript = Editor.ActiveTextAreaControl.SelectionManager.SelectedText;
                else
                    CurrentScript = Editor.Text;

                OnKill(CurrentScript, this.BtnKill);
            }
        }

        private void UpdateToolbarButton(ToolbarOption changedOption)
        {
            ToolStripButton toolbarBtn = changedOption.Control as ToolStripButton;
            if (toolbarBtn != null)
            {
                toolbarBtn.Visible = changedOption.Visible;
                toolbarBtn.Name = changedOption.Name;
                toolbarBtn.Enabled = changedOption.Enabled;
                toolbarBtn.ToolTipText = changedOption.Tooltip;
                toolbarBtn.Image = changedOption.Icon;
            }
        }

        private void UpdateToolbar()
        {
            List<ToolbarOption> toolbarOptions = new List<ToolbarOption>()
            {
                this.BtnRun,
                this.BtnStop,
                this.BtnKill,
                this.BtnComment,
                this.BtnUncomment,
                this.BtnSearch,
                this.BtnToggleBookmark,
                this.BtnPreviousBookmark,
                this.BtnNextBookmark,
                this.BtnClearBookmarks,
                this.BtnSaveToFile,
                this.BtnLoadFromFile
            };

            foreach (ToolbarOption option in toolbarOptions)
            {
                ToolStripButton toolbarBtn = option.Control as ToolStripButton;

                toolbarBtn.Visible = option.Visible;
                toolbarBtn.Name = option.Name;
                toolbarBtn.ToolTipText = option.Tooltip;
                toolbarBtn.Enabled = option.Enabled;
                toolbarBtn.Image = option.Icon;
            }

        }

    }


}
