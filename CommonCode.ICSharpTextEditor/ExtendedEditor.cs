using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode;
using ZidUtilities.CommonCode.ICSharpTextEditor.BracketMatching;
using ZidUtilities.CommonCode.ICSharpTextEditor.FoldingStrategies;
using ZidUtilities.CommonCode.ICSharpTextEditor.HelperForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ZidUtilities.CommonCode.ICSharpTextEditor
{
    public delegate void OnToolbarButtonClick(string selectedText, ToolbarOption btnClicked);
    public delegate bool OnKeyPressOnEditor(Keys keyData);
    
    [ToolboxBitmap(@"D:\Just For Fun\ZidUtilities\CommonCode.ICSharpTextEditor\ExtendedEditor.ico")]
    public partial class ExtendedEditor : UserControl
    {
        [Category("Custom Event")]
        [Description("Event triggered on click/shortcur of toolbar button: Run")]
        public event OnToolbarButtonClick OnRun;
        [Category("Custom Event")]
        [Description("Event triggered on click/shortcur of toolbar button: Stop")]
        public event OnToolbarButtonClick OnStop;
        [Category("Custom Event")]
        [Description("Event triggered on click/shortcur of toolbar button: Kill")]
        public event OnToolbarButtonClick OnKill;
        [Category("Custom Event")]
        [Description("Event triggered on key press on editor, it is processed after shortcuts.")]
        
        public event OnKeyPressOnEditor WhenKeyPress;

        private SearchAndReplace searchForm = null;
        private bool _lastSearchLoopedAround = false, _lastSearchWasBackward = false;

        Keys LastKeyPressed;

        public IHighlightingStrategy DefaultHighlightingStrategy { get; private set; }
        public IFormattingStrategy DefaultFormattingStrategy { get; private set; }

        private string curFileName = String.Empty;

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
                        case SyntaxHighlighting.VBNET:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.VBNET));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = this.DefaultFormattingStrategy;//no bracket matching strategy for vbnet

                            GenericFoldingStrategy vbnetFolding = new GenericFoldingStrategy();
                            vbnetFolding.StartFolding.Add("#Region");
                            vbnetFolding.EndFolding.Add("#End Region");

                            Editor.Document.FoldingManager.FoldingStrategy = vbnetFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.Java:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.Java));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.CSharp);

                            GenericFoldingStrategy javaFolding = new GenericFoldingStrategy();
                            javaFolding.StartFolding.Add("/*");
                            javaFolding.EndFolding.Add("*/");

                            Editor.Document.FoldingManager.FoldingStrategy = javaFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
                            break;
                        case SyntaxHighlighting.CPlusPlus:
                            HighlightingManager.Manager.AddSyntaxModeFileProvider(new InlineSyntaxProvider(SyntaxLanguage.CPlusPlus));
                            Editor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("InlineLanguage");
                            Editor.Document.FormattingStrategy = new GenericBracketMatcher(ZidUtilities.CommonCode.GenericLanguage.CSharp);

                            GenericFoldingStrategy cplusplusFolding = new GenericFoldingStrategy();
                            cplusplusFolding.StartFolding.Add("/*");
                            cplusplusFolding.EndFolding.Add("*/");

                            Editor.Document.FoldingManager.FoldingStrategy = cplusplusFolding;
                            Editor.Document.FoldingManager.UpdateFoldings(null, null);
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
        [Category("Custom Properties")]
        [Description("Exposes the TextEditor's property Text, for ease of use")]
        public string EditorText { get { return CtrlEditor.Text; } set { CtrlEditor.Text = value; } }

        [Category("Custom Properties")]
        [Description("Shows or hides the toolbar")]
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

        [Category("Custom Properties")]
        [Description("Determines if the control should track key press for toolbar shortcuts and implicit shortcuts.")]
        public bool TrackToolbarShortcuts { get; set; } = true;

        #region Properties that represent the toolbar buttons, class has an event to track changes so they can be customized at design time
        private ToolbarOption _btnRun = new ToolbarOption("Run", "Executes selected/all code (F5)", Content.Play, true) { Enabled = true, ShortCut = Keys.F5 };
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

        private ToolbarOption _btnStop = new ToolbarOption("Stop", "Stops code execution (Shift + F5)", Content.Stop, true) { Enabled = true, ShortCut = (Keys.Shift | Keys.F5) };
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

        private ToolbarOption _btnKill = new ToolbarOption("Kill", "Kills thread executing code (Ctr + F5)", Content.RedAlert, true) { Enabled = true, ShortCut = (Keys.Control | Keys.F5) };
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

        private ToolbarOption _btnComment = new ToolbarOption("Comment", "Comment selected code lines (Ctr + K, C)", Content.Comment, true) { Enabled = true, ShortCut = (Keys.Control | Keys.K), ThenShortCut = (Keys.Control | Keys.C) };
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

        private ToolbarOption _btnUncomment = new ToolbarOption("Uncomment", "Uncomment selected code lines (Ctr + K, U)", Content.UnComment, true) { Enabled = true, ShortCut = (Keys.Control | Keys.K), ThenShortCut = (Keys.Control | Keys.U) };
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

        private ToolbarOption _btnSearch = new ToolbarOption("Search", "Shows dialog to search/replace text from editor (Ctr + F)", Content.Search, true) { Enabled = true, ShortCut = (Keys.Control | Keys.F) };
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

        private ToolbarOption _btnToggleBookmark = new ToolbarOption("ToggleBookmark", "Creates or remove a bookmark from the current line (F2)", Content.Bookmark, true) { Enabled = true, ShortCut = (Keys.F2) };
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

        private ToolbarOption _btnPreviousBookmark = new ToolbarOption("PreviousBookmark", "Moves cursor/position to the previous bookmark (Shift + F2)", Content.Previous, true) { Enabled = true, ShortCut = (Keys.Shift | Keys.F2) };
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

        private ToolbarOption _btnNextBookmark = new ToolbarOption("NextBookmark", "Moves cursor/position to the next bookmark (Ctr + F2)", Content.Next, true) { Enabled = true, ShortCut = (Keys.Control | Keys.F2) };
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

        private ToolbarOption _btnClearBookmarks = new ToolbarOption("ClearBookmarks", "Clears all bookmarks (Ctr + + Shift + F2)", Content.DelBookmark, true) { Enabled = true, ShortCut = (Keys.Control | Keys.Shift | Keys.F2) };
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

        private ToolbarOption _btnSaveToFile = new ToolbarOption("SaveToFile", "Save text on editor to a file (Ctr + S)", Content.Save, true) { Enabled = true, ShortCut = (Keys.Control | Keys.S) };
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

        private ToolbarOption _btnLoadFromFile = new ToolbarOption("LoadFromFile", "Load text from file into the editor (Ctr + O, P)", Content.Open, true) { Enabled = true, ShortCut = (Keys.Control | Keys.O), ThenShortCut = (Keys.Control | Keys.P) };
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

        #region Properties for the implicit shortcuts
        [Category("Custom Properties")]
        [Description("Changes the selected text or the word at offset to upper case")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpToUpperCase { get; set; } = new ImplicitShortcut("ToUpper") { ShortCut = Keys.Control | Keys.Shift | Keys.U };
        [Category("Custom Properties")]
        [Description("Changes the selected text or the word at offset to lower case")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpToLowerCase { get; set; } = new ImplicitShortcut("ToLower") { ShortCut = Keys.Control | Keys.Shift | Keys.L };
        [Category("Custom Properties")]
        [Description("Searchs for the selected text or the word at offset without showing the search form")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpSilentSearch { get; set; } = new ImplicitShortcut("SilentSearch") { ShortCut = Keys.Control | Keys.F3 };
        [Category("Custom Properties")]
        [Description("Searchs forward, you need to have a search already done")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpSearchForward { get; set; } = new ImplicitShortcut("SearchForward") { ShortCut = Keys.F3 };
        [Category("Custom Properties")]
        [Description("Searchs backwards, you need to have a search already done")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpSearchBackward { get; set; } = new ImplicitShortcut("SearchBackward") { ShortCut = Keys.Shift | Keys.F3 };
        [Category("Custom Properties")]
        [Description("Expands all outlining")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpExpandOutlining { get; set; } = new ImplicitShortcut("ExpandOutlining") { ShortCut = Keys.Control | Keys.O, ThenShortCut = Keys.Control | Keys.E };
        [Category("Custom Properties")]
        [Description("Collapses all outlining")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpCollapseOutlining { get; set; } = new ImplicitShortcut("CollapseOutlining") { ShortCut = Keys.Control | Keys.O, ThenShortCut = Keys.Control | Keys.C };
        [Category("Custom Properties")]
        [Description("Toggles all outlining")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpToggleOutlining { get; set; } = new ImplicitShortcut("ToggleOutlining") { ShortCut = Keys.Control | Keys.O, ThenShortCut = Keys.Control | Keys.T };
        #endregion

        #region Properties that represent toolbar text boxes, class has an eveant to track changes so they can be customized at design time
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
        
        [Category("Custom Properties")]
        [Description("Sets the control to read only mode.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool IsReadOnly { get { return Editor.IsReadOnly; } set { Editor.IsReadOnly = value; } }

        public ExtendedEditor()
        {
            InitializeComponent();

            this.DefaultHighlightingStrategy = Editor.Document.HighlightingStrategy;
            this.DefaultFormattingStrategy = Editor.Document.FormattingStrategy;

            #region Iniatilize and link the properties with the toolbar buttons and text boxes
            //Link toolbar buttons with their properties
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

            //Create a list of all toolbar options to easily link the event that notifies changes
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

            //Link the event that notifies changes in the toolbar options
            foreach (ToolbarOption option in toolbarOptions)
            {
                option.OptionChanged += (ToolbarOption changedOption) =>
                {
                    UpdateToolbarButton(changedOption);
                };

            }

            //Link toolbar text boxes with their properties
            _txt01Helper.Control = this.tsText1;
            _txt02Helper.Control = this.tsText2;

            //Create a list of all toolbar text boxes to easily link the event that notifies changes
            List<ToolbarTextBox> toolbarTextBoxes = new List<ToolbarTextBox>()
            {
                _txt01Helper,
                _txt02Helper
            };

            //Link the event that notifies changes in the toolbar text boxes
            foreach (ToolbarTextBox textBox in toolbarTextBoxes)
            {
                textBox.TextOptionChanged += (ToolbarTextBox changedOption) =>
                {
                    UpdateToolbarTextBox(changedOption);
                };
            }
            #endregion

            //Subscribe to document changed event to refresh folding
            CtrlEditor.Document.DocumentChanged += Query_DocumentChanged;

            //Method that handles autocomplete and key impShortcuts
            CtrlEditor.ActiveTextAreaControl.TextArea.DoProcessDialogKey += CtrlEditor_DoProcessDialogKey;


        }

        bool CtrlEditor_DoProcessDialogKey(Keys keyData)//Process hot keys
        {
            bool NoEcho = true, Echo = false;
            List<ImplicitShortcut> impShortcuts = new List<ImplicitShortcut>();

            if(ImpToUpperCase.Enabled)
                impShortcuts.Add(ImpToUpperCase);
            if(ImpToLowerCase.Enabled)
                impShortcuts.Add(ImpToLowerCase);
            if (ImpSilentSearch.Enabled)
                impShortcuts.Add(ImpSilentSearch);
            if (ImpSearchForward.Enabled)
                impShortcuts.Add(ImpSearchForward);
            if (ImpSearchBackward.Enabled)
                impShortcuts.Add(ImpSearchBackward);
            if (ImpExpandOutlining.Enabled)
                impShortcuts.Add(ImpExpandOutlining);
            if (ImpCollapseOutlining.Enabled)
                impShortcuts.Add(ImpCollapseOutlining);
            if (ImpToggleOutlining.Enabled)
                impShortcuts.Add(ImpToggleOutlining);

            if (!TrackToolbarShortcuts)
            {
                if(WhenKeyPress != null)
                    return WhenKeyPress(keyData);
                
                return Echo;
            }

            // Echo == true, then NoEcho == false
            #region Key shortcut processing for toolbar shortcuts
            //Group buttons by single key impShortcuts and two key impShortcuts
            List<ToolbarOption> allBtns = new List<ToolbarOption>()
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
            List<ToolbarOption> singleKeyBtns = new List<ToolbarOption>();
            foreach (ToolbarOption btn in allBtns)
            {
                if (btn.Enabled && btn.Visible)
                {
                    if (btn.ShortCut != Keys.None && btn.ThenShortCut == Keys.None)
                        singleKeyBtns.Add(btn);
                }
            }
            List<ToolbarOption> twoKeyBtns = new List<ToolbarOption>();
            foreach (ToolbarOption btn in allBtns)
            {
                if (btn.ShortCut != Keys.None && btn.ThenShortCut != Keys.None)
                {
                    twoKeyBtns.Add(btn);
                }
            }

            //then first lets try to match single key impShortcuts
            foreach (ToolbarOption btn in singleKeyBtns)
            {
                if (btn.ShortCut == keyData)
                {
                    ExecuteButton(btn);
                    return NoEcho;
                }
            }

            //now try to match two key impShortcuts
            foreach (ToolbarOption btn in twoKeyBtns)
            {
                if (btn.ThenShortCut == keyData && btn.ShortCut == LastKeyPressed)
                {
                    LastKeyPressed = Keys.Space;//clean the last key pressed aux var, to avoid wrong behavior
                    ExecuteButton(btn);
                    return NoEcho;
                }
            }

            #endregion

            //same logic for implicit impShortcuts
            #region Implicit shortcuts processing
            List<ImplicitShortcut> singleImplicit = new List<ImplicitShortcut>();
            foreach (ImplicitShortcut imp in impShortcuts)
            {
                if (imp.ShortCut != Keys.None && imp.ThenShortCut == Keys.None)
                    singleImplicit.Add(imp);
            }
            List<ImplicitShortcut> twoKeyImplicit = new List<ImplicitShortcut>();
            foreach (ImplicitShortcut imp in impShortcuts)
            {
                if (imp.ShortCut != Keys.None && imp.ThenShortCut != Keys.None)
                {
                    twoKeyImplicit.Add(imp);
                }
            }

            //then first lets try to match single key impShortcuts
            foreach (ImplicitShortcut imp in singleImplicit)
            {
                if (imp.ShortCut == keyData)
                {
                    ExecuteImplicitShortcut(imp);
                    return NoEcho;
                }
            }

            //now try to match two key impShortcuts
            foreach (ImplicitShortcut imp in twoKeyImplicit)
            {
                if (imp.ThenShortCut == keyData && imp.ShortCut == LastKeyPressed)
                {
                    LastKeyPressed = Keys.Space;//clean the last key pressed aux var, to avoid wrong behavior
                    ExecuteImplicitShortcut(imp);
                    return NoEcho;
                }
            }
            #endregion

            LastKeyPressed = keyData;//store last key pressed for two key impShortcuts

            if (WhenKeyPress != null)
                return WhenKeyPress(keyData);

            return Echo;
        }

        private void ExecuteImplicitShortcut(ImplicitShortcut imp)
        {
            int offset, length;
            if (imp == null)
                return;

            if (imp == ImpToUpperCase)
            {
                if (CtrlEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                {
                    foreach (ISelection selection in CtrlEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection)
                    {
                        string selectedText = selection.SelectedText;
                        CtrlEditor.Document.Replace(selection.Offset, selection.Length, selectedText.ToUpper());
                    }
                }
                else
                {
                    if (CtrlEditor.TryGetCurrentWord(out offset, out length))
                    {
                        string selectedText = CtrlEditor.Document.GetText(offset, length);
                        CtrlEditor.Document.Replace(offset, length, selectedText.ToUpper());
                    }
                }
            }
            else if (imp == ImpToLowerCase)
            {
                if (CtrlEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                {
                    foreach (ISelection selection in CtrlEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection)
                    {
                        string selectedText = selection.SelectedText;
                        CtrlEditor.Document.Replace(selection.Offset, selection.Length, selectedText.ToLower());
                    }
                }
                else
                {
                    if (CtrlEditor.TryGetCurrentWord(out offset, out length))
                    {
                        string selectedText = CtrlEditor.Document.GetText(offset, length);
                        CtrlEditor.Document.Replace(offset, length, selectedText.ToLower());
                    }
                }
            }
            else if (imp == ImpSilentSearch)
            {
                if (CtrlEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                {
                    string lookFor = CtrlEditor.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;

                    if (searchForm == null)
                        searchForm = new SearchAndReplace();
                    if (!String.IsNullOrEmpty(lookFor) && lookFor.Trim(' ', '\t', '\r', '\n').Length > 0)
                    {
                        searchForm.SetSearchString(lookFor);
                        FindNext(true, false, String.Format("Specified text: {0}, was not found.", searchForm.LookFor));
                    }
                }
                else
                {
                    if (CtrlEditor.TryGetCurrentWord(out offset, out length))
                    {
                        string lookFor = CtrlEditor.Document.GetText(offset, length);
                        if (searchForm == null)
                            searchForm = new SearchAndReplace();
                        if (!String.IsNullOrEmpty(lookFor) && lookFor.Trim(' ', '\t', '\r', '\n').Length > 0)
                        {
                            searchForm.SetSearchString(lookFor);
                            FindNext(true, false, String.Format("Specified text: {0}, was not found.", searchForm.LookFor));
                        }
                    }
                }
            }
            else if (imp == ImpSearchForward)
            {
                if (searchForm != null && !String.IsNullOrEmpty(searchForm.LookFor) && searchForm.LookFor.Trim(' ', '\t', '\r', '\n').Length > 0)
                {
                    FindNext(true, false, String.Format("Specified text: {0}, was not found.", searchForm.LookFor));
                }
            }
            else if (imp == ImpSearchBackward)
            {
                if (searchForm != null && !String.IsNullOrEmpty(searchForm.LookFor) && searchForm.LookFor.Trim(' ', '\t', '\r', '\n').Length > 0)
                {
                    FindNext(true, true, String.Format("Specified text: {0}, was not found.", searchForm.LookFor));
                }
            }
            else if (imp == ImpExpandOutlining)
            {
                if (Editor.Document.FoldingManager.FoldingStrategy != null)
                {
                    foreach (var fm in CtrlEditor.Document.FoldingManager.FoldMarker)
                    {
                        fm.IsFolded = false;
                    }
                    CtrlEditor.Document.FoldingManager.UpdateFoldings(null, null);
                    CtrlEditor.Refresh();
                }
            }
            else if (imp == ImpCollapseOutlining)
            {
                if (Editor.Document.FoldingManager.FoldingStrategy != null)
                {

                    foreach (var fm in CtrlEditor.Document.FoldingManager.FoldMarker)
                    {
                        fm.IsFolded = true;
                    }
                    CtrlEditor.Document.FoldingManager.UpdateFoldings(null, null);
                    CtrlEditor.Refresh();
                }
            }
            else if (imp == ImpToggleOutlining)
            {
                if (Editor.Document.FoldingManager.FoldingStrategy != null)
                {
                    foreach (var fm in CtrlEditor.Document.FoldingManager.FoldMarker)
                    {
                        fm.IsFolded = !fm.IsFolded;
                    }
                    CtrlEditor.Document.FoldingManager.UpdateFoldings(null, null);
                    CtrlEditor.Refresh();
                }
            }

        }

        private void ExecuteButton(ToolbarOption btn)
        {
            if (btn == BtnKill)
                tsKill_Click(this, EventArgs.Empty);
            else if (btn == BtnRun)
                tsRun_Click(this, EventArgs.Empty);
            else if (btn == BtnStop)
                tsStop_Click(this, EventArgs.Empty);
            else if (btn == BtnComment)
                BtnComment_Click(this, EventArgs.Empty);
            else if (btn == BtnUncomment)
                BtnUncomment_Click(this, EventArgs.Empty);
            else if (btn == BtnSearch)
                BtnSearch_Click(this, EventArgs.Empty);
            else if (btn == BtnToggleBookmark)
                BtnBookmark_Click(this, EventArgs.Empty);
            else if (btn == BtnPreviousBookmark)
                BtnPrevious_Click(this, EventArgs.Empty);
            else if (btn == BtnNextBookmark)
                BtnNext_Click(this, EventArgs.Empty);
            else if (btn == BtnClearBookmarks)
                BtnClearBookmarks_Click(this, EventArgs.Empty);
            else if (btn == BtnSaveToFile)
                BtnSave_Click(this, EventArgs.Empty);
            else if (btn == BtnLoadFromFile)
                BtnLoad_Click(this, EventArgs.Empty);
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
                if (CtrlEditor.Document.FoldingManager.FoldingStrategy != null)
                    CtrlEditor.Document.FoldingManager.UpdateFoldings(null, null);
                FoldingRefresher.Enabled = false;
                ToRefresh = 10;
            }
        }
        #endregion

        #region Methods to update the control's toolbar when the properties change, this will update visibility, text, tooltip, etc. even on seginer time
        private void UpdateToolbarTextBox(ToolbarTextBox changedOption)
        {
            ToolStripTextBox toolbarTxtBox = changedOption.Control as ToolStripTextBox;
            if (toolbarTxtBox != null)
            {
                toolbarTxtBox.Visible = changedOption.Visible;
                toolbarTxtBox.Text = changedOption.Text;
                toolbarTxtBox.ToolTipText = changedOption.ToolTip;
                toolbarTxtBox.Size = new Size(changedOption.Width, toolbarTxtBox.Height);
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
        #endregion

        #region Code for the actions in the toolbar
        #region Methods that can be called externally to simulate button clicks
        public void RunClick(object sender, EventArgs e)
        {
            tsRun_Click(sender, e);
        }

        public void StopClick(object sender, EventArgs e)
        {
            tsStop_Click(sender, e);
        }

        public void KillClick(object sender, EventArgs e)
        {
            tsKill_Click(sender, e);
        }

        public void CommentClick(object sender, EventArgs e)
        {
            BtnComment_Click(this, EventArgs.Empty);
        }

        public void UncommentClick(object sender, EventArgs e)
        {
            BtnUncomment_Click(sender, e);
        }

        public void BookmarkClick(object sender, EventArgs e)
        {
            BtnBookmark_Click(sender, e);
        }

        public void PreviousBookmarkClick(object sender, EventArgs e)
        {
            BtnPrevious_Click(sender, e);
        }

        public void NextBookmarkClick(object sender, EventArgs e)
        {
            BtnNext_Click(sender, e);
        }

        public void ClearBookmarksClick(object sender, EventArgs e)
        {
            BtnClearBookmarks_Click(sender, e);
        }

        public void SaveClick(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        public void LoadClick(object sender, EventArgs e)
        {
            BtnLoad_Click(sender, e);
        }

        public void SearchClick(object sender, EventArgs e)
        {
            BtnSearch_Click(sender, e);
        }
        #endregion

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
        private void BtnComment_Click(object sender, EventArgs e)
        {
            int startLine, endLine, i;
            string comment = string.Empty;
            switch (_Syntax)
            {
                case SyntaxHighlighting.CSharp:
                case SyntaxHighlighting.JavaScript:
                    comment = "//";
                    break;
                case SyntaxHighlighting.TransactSQL:
                case SyntaxHighlighting.MySql:
                    comment = "--";
                    break;
                case SyntaxHighlighting.XML:
                case SyntaxHighlighting.HTML:
                case SyntaxHighlighting.CSS:
                case SyntaxHighlighting.Json:
                case SyntaxHighlighting.None:
                default:
                    return;
            }


            if (!comment.IsEmpty())
            {
                if (CtrlEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                {//if there is a selection, comment each line within the selection range
                    foreach (ISelection selection in CtrlEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection)
                    {
                        startLine = selection.StartPosition.Y;
                        endLine = selection.EndPosition.Y;

                        for (i = endLine; i >= startLine; --i)
                        {
                            LineSegment line = CtrlEditor.Document.GetLineSegment(i);
                            if (selection != null && i == endLine && line.Offset == selection.Offset + selection.Length)
                            {
                                --endLine;
                                continue;
                            }

                            string lineText = CtrlEditor.Document.GetText(line.Offset, line.Length);
                            CtrlEditor.Document.Insert(line.Offset, comment);
                        }
                    }
                }
                else
                {//If there is no selection comment the current line
                    startLine = CtrlEditor.ActiveTextAreaControl.TextArea.Caret.Line;
                    endLine = CtrlEditor.ActiveTextAreaControl.TextArea.Caret.Line;

                    for (i = endLine; i >= startLine; --i)
                    {
                        LineSegment line = CtrlEditor.Document.GetLineSegment(i);
                        if (line.ToString().Trim().Length == 0)
                        {
                            --endLine;
                            continue;
                        }

                        string lineText = CtrlEditor.Document.GetText(line.Offset, line.Length);
                        CtrlEditor.Document.Insert(line.Offset, comment);
                    }
                }
            }
        }
        private void BtnUncomment_Click(object sender, EventArgs e)
        {
            int startLine, endLine, i;
            string comment = string.Empty;
            switch (_Syntax)
            {
                case SyntaxHighlighting.CSharp:
                case SyntaxHighlighting.JavaScript:
                    comment = "//";
                    break;
                case SyntaxHighlighting.TransactSQL:
                case SyntaxHighlighting.MySql:
                    comment = "--";
                    break;
                case SyntaxHighlighting.XML:
                case SyntaxHighlighting.HTML:
                case SyntaxHighlighting.CSS:
                case SyntaxHighlighting.Json:
                case SyntaxHighlighting.None:
                default:
                    return;
            }

            if (!comment.IsEmpty())
            {
                if (CtrlEditor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected)
                {//if there is a selection, uncomment each line within the selection range
                    foreach (ISelection selection in CtrlEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection)
                    {
                        startLine = selection.StartPosition.Y;
                        endLine = selection.EndPosition.Y;

                        for (i = endLine; i >= startLine; --i)
                        {
                            LineSegment line = CtrlEditor.Document.GetLineSegment(i);
                            if (selection != null && i == endLine && line.Offset == selection.Offset + selection.Length)
                            {
                                --endLine;
                                continue;
                            }

                            string lineText = CtrlEditor.Document.GetText(line.Offset, line.Length);
                            if (lineText.Trim().StartsWith(comment))
                                CtrlEditor.Document.Remove(line.Offset + lineText.IndexOf(comment), comment.Length);
                        }
                    }
                }
                else
                {//If there is no selection uncomment the current line
                    startLine = CtrlEditor.ActiveTextAreaControl.TextArea.Caret.Line;
                    endLine = CtrlEditor.ActiveTextAreaControl.TextArea.Caret.Line;

                    for (i = endLine; i >= startLine; --i)
                    {
                        LineSegment line = CtrlEditor.Document.GetLineSegment(i);
                        if (line.ToString().Trim().Length == 0)
                        {
                            --endLine;
                            continue;
                        }

                        string lineText = CtrlEditor.Document.GetText(line.Offset, line.Length);
                        if (lineText.Trim().StartsWith(comment))
                            CtrlEditor.Document.Remove(line.Offset + lineText.IndexOf(comment), comment.Length);
                    }
                }
            }
        }
        private void BtnBookmark_Click(object sender, EventArgs e)
        {
            DoEditAction(CtrlEditor, new ICSharpCode.TextEditor.Actions.ToggleBookmark());
        }
        private void DoEditAction(ICSharpCode.TextEditor.TextEditorControl editor, ICSharpCode.TextEditor.Actions.IEditAction action)
        {
            if (editor != null && action != null)
            {
                var area = editor.ActiveTextAreaControl.TextArea;
                editor.BeginUpdate();
                try
                {
                    lock (editor.Document)
                    {
                        action.Execute(area);
                        if (area.SelectionManager.HasSomethingSelected && area.AutoClearSelection /*&& caretchanged*/)
                        {
                            if (area.Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal)
                            {
                                area.SelectionManager.ClearSelection();
                            }
                        }
                    }
                }
                finally
                {
                    editor.EndUpdate();
                    area.Caret.UpdateCaretPosition();
                }
            }
        }
        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            DoEditAction(CtrlEditor, new ICSharpCode.TextEditor.Actions.GotoPrevBookmark(bookmark => true));
        }
        private void BtnNext_Click(object sender, EventArgs e)
        {
            DoEditAction(CtrlEditor, new ICSharpCode.TextEditor.Actions.GotoNextBookmark(bookmark => true));
        }
        private void BtnClearBookmarks_Click(object sender, EventArgs e)
        {
            DoEditAction(CtrlEditor, new ICSharpCode.TextEditor.Actions.ClearAllBookmarks(bookmark => true));
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            bool save = false, export = false;
            if (curFileName.IsEmpty())
            {
                export = true;
            }
            else
            {
                FileInfo fi = new FileInfo(curFileName);
                DialogResult result = MessageBox.Show($"Do you want to save the current file {fi.Name}?(Y) or Export current text to a file ?(N)\n\n", "Save to file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    save = true;
                }
                else if (result == DialogResult.No)
                {
                    export = true;
                }
                else
                {
                    return;
                }

            }

            if (export)
            {
                SaveFileDialog saver = new SaveFileDialog();
                switch (_Syntax)
                {
                    case SyntaxHighlighting.CSharp:
                        saver.Title = "Save C# Code to file";
                        saver.Filter = "C# file|*.cs|Any file|*.*";
                        break;
                    case SyntaxHighlighting.XML:
                        saver.Title = "Save XML document";
                        saver.Filter = "XML file|*.xml|Any file|*.*";
                        break;
                    case SyntaxHighlighting.TransactSQL:
                    case SyntaxHighlighting.MySql:
                        saver.Title = "Save SQL script";
                        saver.Filter = "Text file|*.txt|SQL file|*.sql|Any file|*.*";
                        break;
                    case SyntaxHighlighting.JavaScript:
                        saver.Title = "Save JavaScript file";
                        saver.Filter = "JavaScript file|*.js|Any file|*.*";
                        break;
                    case SyntaxHighlighting.HTML:
                        saver.Title = "Save HTML document";
                        saver.Filter = "HTML file|*.html;*.htm|Any file|*.*";
                        break;
                    case SyntaxHighlighting.CSS:
                        saver.Title = "Save CSS file";
                        saver.Filter = "CSS file|*.css|Any file|*.*";
                        break;
                    case SyntaxHighlighting.Json:
                        saver.Title = "Save JSON file";
                        saver.Filter = "JSON file|*.json|Any file|*.*";
                        break;
                    case SyntaxHighlighting.None:
                    default:
                        saver.Title = "Text to file";
                        saver.Filter = "Text file|*.txt|Any file|*.*";
                        break;
                }

                saver.AddExtension = true;
                saver.AutoUpgradeEnabled = true;

                if (saver.ShowDialog() == DialogResult.OK)
                {
                    curFileName = saver.FileName;
                    using (StreamWriter Wr = new StreamWriter(saver.FileName))
                    {
                        Wr.Write(CtrlEditor.Text);
                        Wr.Close();
                    }
                }
            }
            else
            {
                using (StreamWriter Wr = new StreamWriter(curFileName))
                {
                    Wr.Write(CtrlEditor.Text);
                    Wr.Close();
                }
            }
        }
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog opener = new OpenFileDialog();
            opener.Title = "Load from file";
            switch (_Syntax)
            {
                case SyntaxHighlighting.CSharp:
                    opener.Filter = "C# file|*.cs|Any file|*.*";
                    break;
                case SyntaxHighlighting.XML:
                    opener.Filter = "XML file|*.xml|Any file|*.*";
                    break;
                case SyntaxHighlighting.TransactSQL:
                case SyntaxHighlighting.MySql:
                    opener.Filter = "Text file|*.txt|SQL file|*.sql|Any file|*.*";
                    break;
                case SyntaxHighlighting.JavaScript:
                    opener.Filter = "JavaScript file|*.js|Any file|*.*";
                    break;
                case SyntaxHighlighting.HTML:
                    opener.Filter = "HTML file|*.html;*.htm|Any file|*.*";
                    break;
                case SyntaxHighlighting.CSS:
                    opener.Filter = "CSS file|*.css|Any file|*.*";
                    break;
                case SyntaxHighlighting.Json:
                    opener.Filter = "JSON file|*.json|Any file|*.*";
                    break;
                case SyntaxHighlighting.None:
                default:
                    opener.Filter = "Text file|*.txt|Any file|*.*";
                    break;
            }

            if (opener.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                using (StreamReader Rdr = new StreamReader(opener.FileName))
                {
                    string line;
                    while ((line = Rdr.ReadLine()) != null)
                        sb.AppendLine(line);
                    Rdr.Close();
                }
                curFileName = opener.FileName;
                CtrlEditor.Text = sb.ToString();
                CtrlEditor.Refresh();
            }

        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (searchForm == null || searchForm.IsDisposed)
                searchForm = new SearchAndReplace();
            searchForm.ShowFor(CtrlEditor, false);
        }
        public TextRange FindNext(bool viaF3, bool searchBackward, string messageIfNotFound)
        {
            if (searchForm == null || searchForm.IsDisposed)
                searchForm = new SearchAndReplace();

            TextEditorSearcher _search = new TextEditorSearcher();
            _lastSearchWasBackward = searchBackward;
            _search.Document = CtrlEditor.Document;
            _search.LookFor = searchForm.LookFor;
            _search.MatchCase = searchForm.MatchCase;
            _search.MatchWholeWordOnly = searchForm.MatchWholeWordOnly;

            var caret = CtrlEditor.ActiveTextAreaControl.Caret;
            if (viaF3 && _search.HasScanRegion && !caret.Offset.IsInRange(_search.BeginOffset, _search.EndOffset))
            {
                // user moved outside of the originally selected region
                _search.ClearScanRegion();
            }

            int startFrom = caret.Offset - (searchBackward ? 1 : 0);
            TextRange range = _search.FindNext(startFrom, searchBackward, out _lastSearchLoopedAround);
            if (range != null)
                SelectResult(range);
            else if (messageIfNotFound != null)
                MessageBox.Show(messageIfNotFound);
            return range;
        }
        private void SelectResult(TextRange range)
        {
            TextLocation p1 = CtrlEditor.Document.OffsetToPosition(range.Offset);
            TextLocation p2 = CtrlEditor.Document.OffsetToPosition(range.Offset + range.Length);
            CtrlEditor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
            CtrlEditor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
            CtrlEditor.ActiveTextAreaControl.Caret.Position = CtrlEditor.Document.OffsetToPosition(range.Offset + range.Length);
        }
        #endregion

    }


}
