using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Xml;
using ZidUtilities.CommonCode;
using ZidUtilities.CommonCode.AvalonEdit.FoldingStrategies;
using ZidUtilities.CommonCode.AvalonEdit.HelperForms;
using WinFormsKeys = System.Windows.Forms.Keys;

namespace ZidUtilities.CommonCode.AvalonEdit
{
    public delegate void OnToolbarButtonClick(string selectedText, ToolbarOption btnClicked);
    public delegate void OnKeyPressOnEditor(System.Windows.Forms.Keys keyData);

    [ToolboxBitmap(@"D:\Just For Fun\ZidUtilities\CommonCode.AvalonEdit\ExtendedEditor.ico")]
    public partial class ExtendedEditor : UserControl
    {
        [Category("Custom Event")]
        [Description("Event triggered on click/shortcut of toolbar button: Run")]
        public event OnToolbarButtonClick OnRun;

        [Category("Custom Event")]
        [Description("Event triggered on click/shortcut of toolbar button: Stop")]
        public event OnToolbarButtonClick OnStop;

        [Category("Custom Event")]
        [Description("Event triggered on click/shortcut of toolbar button: Kill")]
        public event OnToolbarButtonClick OnKill;

        [Category("Custom Event")]
        [Description("Event triggered on key press on editor")]
        public event OnKeyPressOnEditor OnEditorKeyPress;

        private SearchAndReplace searchForm = null;
        private bool _lastSearchLoopedAround = false, _lastSearchWasBackward = false;
        private System.Windows.Forms.Keys LastKeyPressed;
        private string curFileName = String.Empty;
        private SyntaxHighlighting _Syntax;
        private FoldingManager _foldingManager;
        private object _foldingStrategy;
        private System.Windows.Threading.DispatcherTimer _foldingUpdateTimer;

        // WPF ElementHost to host AvalonEdit
        private ElementHost elementHost;
        private TextEditor textEditor;

        [Category("Custom Properties")]
        [Description("Defines the syntax highlighting that the control will have")]
        public SyntaxHighlighting Syntax
        {
            get => _Syntax;
            set
            {
                if (!this.DesignMode)
                {
                    ApplySyntaxHighlighting(value);
                }
                _Syntax = value;
            }
        }

        [Category("Custom Properties")]
        [Description("Property that grants access to the text control itself: AvalonEdit TextEditor")]
        [Browsable(false)]
        public TextEditor Editor => this.textEditor;

        [Category("Custom Properties")]
        [Description("Exposes the TextEditor's property Text, for ease of use")]
        public string EditorText
        {
            get => textEditor?.Text ?? string.Empty;
            set { if (textEditor != null) textEditor.Text = value; }
        }

        [Category("Custom Properties")]
        [Description("Gets or sets the text content of the editor (overrides UserControl.Text)")]
        [Browsable(true)]
        public override string Text
        {
            get => EditorText;
            set => EditorText = value;
        }

        [Category("Custom Properties")]
        [Description("Shows or hides the toolbar")]
        public bool ShowToolbar
        {
            get => this.CtrlToolbar.Visible;
            set => this.CtrlToolbar.Visible = value;
        }

        [Category("Custom Properties")]
        [Description("Determines if the control should track key press for toolbar shortcuts and implicit shortcuts")]
        public bool TrackToolbarShortcuts { get; set; }

        #region Toolbar Button Properties
        private ToolbarOption _btnRun = new ToolbarOption("Run", "Executes selected/all code (F5)", Content.Play, true) { Enabled = true, ShortCut = System.Windows.Forms.Keys.F5 };
        [Category("Custom Properties")]
        [Description("Button to execute code")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnRun { get => _btnRun; set => _btnRun = value; }

        private ToolbarOption _btnStop = new ToolbarOption("Stop", "Stops code execution (Shift + F5)", Content.Stop, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5) };
        [Category("Custom Properties")]
        [Description("Button to stop code execution")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnStop { get => _btnStop; set => _btnStop = value; }

        private ToolbarOption _btnKill = new ToolbarOption("Kill", "Kills thread executing code (Ctrl + F5)", Content.RedAlert, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5) };
        [Category("Custom Properties")]
        [Description("Button to kill thread executing code")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnKill { get => _btnKill; set => _btnKill = value; }

        private ToolbarOption _btnComment = new ToolbarOption("Comment", "Comment selected code lines (Ctrl + K, C)", Content.Comment, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K), ThenShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C) };
        [Category("Custom Properties")]
        [Description("Comment selected code lines")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnComment { get => _btnComment; set => _btnComment = value; }

        private ToolbarOption _btnUncomment = new ToolbarOption("Uncomment", "Uncomment selected code lines (Ctrl + K, U)", Content.UnComment, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K), ThenShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U) };
        [Category("Custom Properties")]
        [Description("Uncomment selected code lines")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnUncomment { get => _btnUncomment; set => _btnUncomment = value; }

        private ToolbarOption _btnSearch = new ToolbarOption("Search", "Shows dialog to search/replace text (Ctrl + F)", Content.Search, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F) };
        [Category("Custom Properties")]
        [Description("Shows dialog to search/replace text from editor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnSearch { get => _btnSearch; set => _btnSearch = value; }

        private ToolbarOption _btnToggleBookmark = new ToolbarOption("ToggleBookmark", "Creates or removes a bookmark (F2)", Content.Bookmark, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.F2) };
        [Category("Custom Properties")]
        [Description("Button that creates or removes a bookmark from the current line")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnToggleBookmark { get => _btnToggleBookmark; set => _btnToggleBookmark = value; }

        private ToolbarOption _btnPreviousBookmark = new ToolbarOption("PreviousBookmark", "Moves to previous bookmark (Shift + F2)", Content.Previous, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2) };
        [Category("Custom Properties")]
        [Description("Moves cursor to the previous bookmark")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnPreviousBookmark { get => _btnPreviousBookmark; set => _btnPreviousBookmark = value; }

        private ToolbarOption _btnNextBookmark = new ToolbarOption("NextBookmark", "Moves to next bookmark (Ctrl + F2)", Content.Next, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2) };
        [Category("Custom Properties")]
        [Description("Moves cursor to the next bookmark")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnNextBookmark { get => _btnNextBookmark; set => _btnNextBookmark = value; }

        private ToolbarOption _btnClearBookmarks = new ToolbarOption("ClearBookmarks", "Clears all bookmarks (Ctrl + Shift + F2)", Content.DelBookmark, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2) };
        [Category("Custom Properties")]
        [Description("Clears all bookmarks")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnClearBookmarks { get => _btnClearBookmarks; set => _btnClearBookmarks = value; }

        private ToolbarOption _btnSaveToFile = new ToolbarOption("SaveToFile", "Save text to file (Ctrl + S)", Content.Save, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S) };
        [Category("Custom Properties")]
        [Description("Save text on editor to a file")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnSaveToFile { get => _btnSaveToFile; set => _btnSaveToFile = value; }

        private ToolbarOption _btnLoadFromFile = new ToolbarOption("LoadFromFile", "Load text from file (Ctrl + O, P)", Content.Open, true) { Enabled = true, ShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O), ThenShortCut = (System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P) };
        [Category("Custom Properties")]
        [Description("Load text from file into the editor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarOption BtnLoadFromFile { get => _btnLoadFromFile; set => _btnLoadFromFile = value; }
        #endregion

        #region Implicit Shortcut Properties
        [Category("Custom Properties")]
        [Description("Changes the selected text to upper case")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpToUpperCase { get; set; } = new ImplicitShortcut("ToUpper") { ShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.U };

        [Category("Custom Properties")]
        [Description("Changes the selected text to lower case")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpToLowerCase { get; set; } = new ImplicitShortcut("ToLower") { ShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.L };

        [Category("Custom Properties")]
        [Description("Searches for the selected text without showing the search form")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpSilentSearch { get; set; } = new ImplicitShortcut("SilentSearch") { ShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3 };

        [Category("Custom Properties")]
        [Description("Searches forward")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpSearchForward { get; set; } = new ImplicitShortcut("SearchForward") { ShortCut = System.Windows.Forms.Keys.F3 };

        [Category("Custom Properties")]
        [Description("Searches backwards")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpSearchBackward { get; set; } = new ImplicitShortcut("SearchBackward") { ShortCut = System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3 };

        [Category("Custom Properties")]
        [Description("Expands all outlining")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpExpandOutlining { get; set; } = new ImplicitShortcut("ExpandOutlining") { ShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O, ThenShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E };

        [Category("Custom Properties")]
        [Description("Collapses all outlining")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpCollapseOutlining { get; set; } = new ImplicitShortcut("CollapseOutlining") { ShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O, ThenShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C };

        [Category("Custom Properties")]
        [Description("Toggles all outlining")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImplicitShortcut ImpToggleOutlining { get; set; } = new ImplicitShortcut("ToggleOutlining") { ShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O, ThenShortCut = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T };
        #endregion

        #region Toolbar TextBox Properties
        private ToolbarTextBox _txt01Helper = new ToolbarTextBox("TextBoxHelper1", true);
        [Category("Custom Properties")]
        [Description("Input box that can be used to display some text")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarTextBox Txt01Helper { get => _txt01Helper; set => _txt01Helper = value; }

        private ToolbarTextBox _txt02Helper = new ToolbarTextBox("TextBoxHelper2", true);
        [Category("Custom Properties")]
        [Description("Input box that can be used to display some text")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolbarTextBox Txt02Helper { get => _txt02Helper; set => _txt02Helper = value; }
        #endregion

        [Category("Custom Properties")]
        [Description("Sets the control to read only mode")]
        public bool IsReadOnly
        {
            get => textEditor?.IsReadOnly ?? false;
            set { if (textEditor != null) textEditor.IsReadOnly = value; }
        }

        public ExtendedEditor()
        {
            InitializeComponent();
            InitializeAvalonEdit();
            InitializeToolbar();
            InitializeFolding();
        }

        private void InitializeAvalonEdit()
        {
            // Create AvalonEdit TextEditor
            textEditor = new TextEditor
            {
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12,
                ShowLineNumbers = true,
                Options = { EnableHyperlinks = false, EnableEmailHyperlinks = false }
            };

            // Create ElementHost to host WPF control
            elementHost = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = textEditor
            };

            this.Controls.Add(elementHost);
            elementHost.BringToFront();

            // Subscribe to key events
            textEditor.PreviewKeyDown += TextEditor_PreviewKeyDown;
            textEditor.TextChanged += TextEditor_TextChanged;
        }

        private void InitializeToolbar()
        {
            // Link toolbar buttons with their properties
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
                BtnRun, BtnStop, BtnKill, BtnComment, BtnUncomment, BtnSearch,
                BtnToggleBookmark, BtnPreviousBookmark, BtnNextBookmark,
                BtnClearBookmarks, BtnSaveToFile, BtnLoadFromFile
            };

            foreach (ToolbarOption option in toolbarOptions)
            {
                option.OptionChanged += (changedOption) => UpdateToolbarButton(changedOption);
            }

            // Link toolbar text boxes
            _txt01Helper.Control = this.tsText1;
            _txt02Helper.Control = this.tsText2;

            List<ToolbarTextBox> toolbarTextBoxes = new List<ToolbarTextBox>() { _txt01Helper, _txt02Helper };
            foreach (ToolbarTextBox textBox in toolbarTextBoxes)
            {
                textBox.TextOptionChanged += (changedOption) => UpdateToolbarTextBox(changedOption);
            }
        }

        private void InitializeFolding()
        {
            if (textEditor != null)
            {
                _foldingManager = FoldingManager.Install(textEditor.TextArea);

                // Setup timer for folding updates
                _foldingUpdateTimer = new System.Windows.Threading.DispatcherTimer();
                _foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
                _foldingUpdateTimer.Tick += (s, e) => UpdateFoldings();
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            if (_foldingUpdateTimer != null && !_foldingUpdateTimer.IsEnabled)
            {
                _foldingUpdateTimer.Start();
            }
        }

        private void UpdateFoldings()
        {
            _foldingUpdateTimer?.Stop();

            if (_foldingManager != null && _foldingStrategy != null)
            {
                IEnumerable<NewFolding> foldings = null;

                if (_foldingStrategy is GenericFoldingStrategy genericStrategy)
                {
                    foldings = genericStrategy.GenerateFoldMarkers(textEditor.Document);
                }
                else if (_foldingStrategy is SqlFoldingStrategy sqlStrategy)
                {
                    foldings = sqlStrategy.GenerateFoldMarkers(textEditor.Document);
                }

                if (foldings != null)
                {
                    _foldingManager.UpdateFoldings(foldings, -1);
                }
            }
        }

        private void ApplySyntaxHighlighting(SyntaxHighlighting syntax)
        {
            if (textEditor == null) return;

            _foldingStrategy = null;

            switch (syntax)
            {
                case SyntaxHighlighting.None:
                    textEditor.SyntaxHighlighting = null;
                    break;

                case SyntaxHighlighting.CSharp:
                    LoadCustomHighlighting(SyntaxLanguage.CSharp);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.CSharp);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.AddRange(new[] { "/*", "#region" });
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.AddRange(new[] { "*/", "#endregion" });
                    ((GenericFoldingStrategy)_foldingStrategy).SpamFolding.Add("///");
                    break;

                case SyntaxHighlighting.VBNET:
                    LoadCustomHighlighting(SyntaxLanguage.VBNET);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.VBNET);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.Add("#Region");
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.Add("#End Region");
                    break;

                case SyntaxHighlighting.Java:
                    LoadCustomHighlighting(SyntaxLanguage.Java);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.Java);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.Add("/*");
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.Add("*/");
                    break;

                case SyntaxHighlighting.CPlusPlus:
                    LoadCustomHighlighting(SyntaxLanguage.CPlusPlus);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.CPlusPlus);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.Add("/*");
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.Add("*/");
                    break;

                case SyntaxHighlighting.XML:
                    LoadCustomHighlighting(SyntaxLanguage.XML);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.XML);
                    break;

                case SyntaxHighlighting.TransactSQL:
                    LoadCustomHighlighting(SyntaxLanguage.TSQL);
                    _foldingStrategy = new SqlFoldingStrategy();
                    break;

                case SyntaxHighlighting.MySql:
                    LoadCustomHighlighting(SyntaxLanguage.MySQL);
                    _foldingStrategy = new SqlFoldingStrategy();
                    break;

                case SyntaxHighlighting.JavaScript:
                    LoadCustomHighlighting(SyntaxLanguage.JavaScript);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.JavaScript);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.Add("/*");
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.Add("*/");
                    break;

                case SyntaxHighlighting.HTML:
                    LoadCustomHighlighting(SyntaxLanguage.HTML);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.HTML);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.AddRange(new[] { "<!--", "<style>", "<script>", "<body>", "<head>" });
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.AddRange(new[] { "-->", "</style>", "</script>", "</body>", "</head>" });
                    break;

                case SyntaxHighlighting.CSS:
                    LoadCustomHighlighting(SyntaxLanguage.CSS);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.CSS);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.Add("/*");
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.Add("*/");
                    break;

                case SyntaxHighlighting.Json:
                    LoadCustomHighlighting(SyntaxLanguage.JSON);
                    _foldingStrategy = new GenericFoldingStrategy(SyntaxLanguage.JSON);
                    ((GenericFoldingStrategy)_foldingStrategy).StartFolding.AddRange(new[] { "{", "[" });
                    ((GenericFoldingStrategy)_foldingStrategy).EndFolding.AddRange(new[] { "}", "]" });
                    break;
            }

            UpdateFoldings();
        }

        private void LoadCustomHighlighting(SyntaxLanguage language)
        {
            try
            {
                string xshdContent = SyntaxProvider.GetSyntaxFile(language);
                using (StringReader reader = new StringReader(xshdContent))
                using (XmlReader xmlReader = XmlReader.Create(reader))
                {
                    var highlighting = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);
                    textEditor.SyntaxHighlighting = highlighting;
                }
            }
            catch
            {
                textEditor.SyntaxHighlighting = null;
            }
        }

        private void TextEditor_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!TrackToolbarShortcuts)
            {
                OnEditorKeyPress?.Invoke(ConvertToWinFormsKeys(e.Key, Keyboard.Modifiers));
                return;
            }

            var key = ConvertToWinFormsKeys(e.Key, Keyboard.Modifiers);
            bool handled = ProcessShortcut(key);

            if (handled)
            {
                e.Handled = true;
            }
            else
            {
                OnEditorKeyPress?.Invoke(key);
            }
        }

        private WinFormsKeys ConvertToWinFormsKeys(Key wpfKey, ModifierKeys modifiers)
        {
            int keyValue = KeyInterop.VirtualKeyFromKey(wpfKey);

            if ((modifiers & System.Windows.Input.ModifierKeys.Control) != 0)
                keyValue |= (int)WinFormsKeys.Control;
            if ((modifiers & System.Windows.Input.ModifierKeys.Shift) != 0)
                keyValue |= (int)WinFormsKeys.Shift;
            if ((modifiers & System.Windows.Input.ModifierKeys.Alt) != 0)
                keyValue |= (int)WinFormsKeys.Alt;

            return (WinFormsKeys)keyValue;
        }

        private bool ProcessShortcut(System.Windows.Forms.Keys keyData)
        {
            // Check toolbar button shortcuts
            var allButtons = new List<ToolbarOption>
            {
                BtnRun, BtnStop, BtnKill, BtnComment, BtnUncomment, BtnSearch,
                BtnToggleBookmark, BtnPreviousBookmark, BtnNextBookmark,
                BtnClearBookmarks, BtnSaveToFile, BtnLoadFromFile
            };

            // Single key shortcuts
            foreach (var btn in allButtons.Where(b => b.Enabled && b.Visible && b.ShortCut != System.Windows.Forms.Keys.None && b.ThenShortCut == System.Windows.Forms.Keys.None))
            {
                if (btn.ShortCut == keyData)
                {
                    ExecuteButton(btn);
                    return true;
                }
            }

            // Two key shortcuts
            foreach (var btn in allButtons.Where(b => b.ShortCut != System.Windows.Forms.Keys.None && b.ThenShortCut != System.Windows.Forms.Keys.None))
            {
                if (btn.ThenShortCut == keyData && btn.ShortCut == LastKeyPressed)
                {
                    LastKeyPressed = System.Windows.Forms.Keys.Space;
                    ExecuteButton(btn);
                    return true;
                }
            }

            // Check implicit shortcuts
            var implicitShortcuts = new List<ImplicitShortcut>
            {
                ImpToUpperCase, ImpToLowerCase, ImpSilentSearch, ImpSearchForward,
                ImpSearchBackward, ImpExpandOutlining, ImpCollapseOutlining, ImpToggleOutlining
            }.Where(i => i.Enabled).ToList();

            foreach (var imp in implicitShortcuts.Where(i => i.ShortCut != System.Windows.Forms.Keys.None && i.ThenShortCut == System.Windows.Forms.Keys.None))
            {
                if (imp.ShortCut == keyData)
                {
                    ExecuteImplicitShortcut(imp);
                    return true;
                }
            }

            foreach (var imp in implicitShortcuts.Where(i => i.ShortCut != System.Windows.Forms.Keys.None && i.ThenShortCut != System.Windows.Forms.Keys.None))
            {
                if (imp.ThenShortCut == keyData && imp.ShortCut == LastKeyPressed)
                {
                    LastKeyPressed = System.Windows.Forms.Keys.Space;
                    ExecuteImplicitShortcut(imp);
                    return true;
                }
            }

            LastKeyPressed = keyData;
            return false;
        }

        private void ExecuteImplicitShortcut(ImplicitShortcut imp)
        {
            if (imp == ImpToUpperCase)
            {
                if (textEditor.SelectionLength > 0)
                {
                    string selected = textEditor.SelectedText;
                    textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, selected.ToUpper());
                }
                else if (textEditor.TryGetCurrentWord(out int offset, out int length))
                {
                    string word = textEditor.Document.GetText(offset, length);
                    textEditor.Document.Replace(offset, length, word.ToUpper());
                }
            }
            else if (imp == ImpToLowerCase)
            {
                if (textEditor.SelectionLength > 0)
                {
                    string selected = textEditor.SelectedText;
                    textEditor.Document.Replace(textEditor.SelectionStart, textEditor.SelectionLength, selected.ToLower());
                }
                else if (textEditor.TryGetCurrentWord(out int offset, out int length))
                {
                    string word = textEditor.Document.GetText(offset, length);
                    textEditor.Document.Replace(offset, length, word.ToLower());
                }
            }
            else if (imp == ImpSilentSearch)
            {
                string lookFor = textEditor.SelectionLength > 0
                    ? textEditor.SelectedText
                    : (textEditor.TryGetCurrentWord(out int offset, out int length) ? textEditor.Document.GetText(offset, length) : "");

                if (!string.IsNullOrEmpty(lookFor?.Trim()))
                {
                    if (searchForm == null) searchForm = new SearchAndReplace();
                    searchForm.SetSearchString(lookFor);
                    FindNext(true, false, $"Text '{lookFor}' was not found.");
                }
            }
            else if (imp == ImpSearchForward)
            {
                if (searchForm != null && !string.IsNullOrEmpty(searchForm.LookFor))
                {
                    FindNext(true, false, $"Text '{searchForm.LookFor}' was not found.");
                }
            }
            else if (imp == ImpSearchBackward)
            {
                if (searchForm != null && !string.IsNullOrEmpty(searchForm.LookFor))
                {
                    FindNext(true, true, $"Text '{searchForm.LookFor}' was not found.");
                }
            }
            else if (imp == ImpExpandOutlining)
            {
                if (_foldingManager != null)
                {
                    foreach (var folding in _foldingManager.AllFoldings)
                    {
                        folding.IsFolded = false;
                    }
                }
            }
            else if (imp == ImpCollapseOutlining)
            {
                if (_foldingManager != null)
                {
                    foreach (var folding in _foldingManager.AllFoldings)
                    {
                        folding.IsFolded = true;
                    }
                }
            }
            else if (imp == ImpToggleOutlining)
            {
                if (_foldingManager != null)
                {
                    foreach (var folding in _foldingManager.AllFoldings)
                    {
                        folding.IsFolded = !folding.IsFolded;
                    }
                }
            }
        }

        private void ExecuteButton(ToolbarOption btn)
        {
            if (btn == BtnRun) tsRun_Click(this, EventArgs.Empty);
            else if (btn == BtnStop) tsStop_Click(this, EventArgs.Empty);
            else if (btn == BtnKill) tsKill_Click(this, EventArgs.Empty);
            else if (btn == BtnComment) BtnComment_Click(this, EventArgs.Empty);
            else if (btn == BtnUncomment) BtnUncomment_Click(this, EventArgs.Empty);
            else if (btn == BtnSearch) BtnSearch_Click(this, EventArgs.Empty);
            else if (btn == BtnToggleBookmark) BtnBookmark_Click(this, EventArgs.Empty);
            else if (btn == BtnPreviousBookmark) BtnPrevious_Click(this, EventArgs.Empty);
            else if (btn == BtnNextBookmark) BtnNext_Click(this, EventArgs.Empty);
            else if (btn == BtnClearBookmarks) BtnClearBookmarks_Click(this, EventArgs.Empty);
            else if (btn == BtnSaveToFile) BtnSave_Click(this, EventArgs.Empty);
            else if (btn == BtnLoadFromFile) BtnLoad_Click(this, EventArgs.Empty);
        }

        private void UpdateToolbarTextBox(ToolbarTextBox changedOption)
        {
            if (changedOption.Control is ToolStripTextBox toolbarTxtBox)
            {
                toolbarTxtBox.Visible = changedOption.Visible;
                toolbarTxtBox.Text = changedOption.Text;
                toolbarTxtBox.ToolTipText = changedOption.ToolTip;
            }
        }

        private void UpdateToolbarButton(ToolbarOption changedOption)
        {
            if (changedOption.Control is ToolStripButton toolbarBtn)
            {
                toolbarBtn.Visible = changedOption.Visible;
                toolbarBtn.Name = changedOption.Name;
                toolbarBtn.Enabled = changedOption.Enabled;
                toolbarBtn.ToolTipText = changedOption.Tooltip;
                toolbarBtn.Image = changedOption.Icon;
            }
        }

        #region Toolbar Button Click Handlers
        public void RunClick(object sender, EventArgs e) => tsRun_Click(sender, e);
        public void StopClick(object sender, EventArgs e) => tsStop_Click(sender, e);
        public void KillClick(object sender, EventArgs e) => tsKill_Click(sender, e);
        public void CommentClick(object sender, EventArgs e) => BtnComment_Click(this, EventArgs.Empty);
        public void UncommentClick(object sender, EventArgs e) => BtnUncomment_Click(sender, e);
        public void BookmarkClick(object sender, EventArgs e) => BtnBookmark_Click(sender, e);
        public void PreviousBookmarkClick(object sender, EventArgs e) => BtnPrevious_Click(sender, e);
        public void NextBookmarkClick(object sender, EventArgs e) => BtnNext_Click(sender, e);
        public void ClearBookmarksClick(object sender, EventArgs e) => BtnClearBookmarks_Click(sender, e);
        public void SaveClick(object sender, EventArgs e) => BtnSave_Click(sender, e);
        public void LoadClick(object sender, EventArgs e) => BtnLoad_Click(sender, e);
        public void SearchClick(object sender, EventArgs e) => BtnSearch_Click(sender, e);

        private void tsRun_Click(object sender, EventArgs e)
        {
            string currentScript = textEditor.SelectionLength > 0 ? textEditor.SelectedText : textEditor.Text;
            OnRun?.Invoke(currentScript, BtnRun);
        }

        private void tsStop_Click(object sender, EventArgs e)
        {
            string currentScript = textEditor.SelectionLength > 0 ? textEditor.SelectedText : textEditor.Text;
            OnStop?.Invoke(currentScript, BtnStop);
        }

        private void tsKill_Click(object sender, EventArgs e)
        {
            string currentScript = textEditor.SelectionLength > 0 ? textEditor.SelectedText : textEditor.Text;
            OnKill?.Invoke(currentScript, BtnKill);
        }

        private void BtnComment_Click(object sender, EventArgs e)
        {
            string comment = GetCommentString();
            if (string.IsNullOrEmpty(comment)) return;

            var doc = textEditor.Document;
            int startLine, endLine;

            if (textEditor.SelectionLength > 0)
            {
                startLine = doc.GetLocation(textEditor.SelectionStart).Line;
                endLine = doc.GetLocation(textEditor.SelectionStart + textEditor.SelectionLength).Line;
            }
            else
            {
                startLine = endLine = textEditor.TextArea.Caret.Line;
            }

            doc.BeginUpdate();
            try
            {
                for (int i = startLine; i <= endLine; i++)
                {
                    var line = doc.GetLineByNumber(i);
                    if (line.Length > 0)
                    {
                        doc.Insert(line.Offset, comment);
                    }
                }
            }
            finally
            {
                doc.EndUpdate();
            }
        }

        private void BtnUncomment_Click(object sender, EventArgs e)
        {
            string comment = GetCommentString();
            if (string.IsNullOrEmpty(comment)) return;

            var doc = textEditor.Document;
            int startLine, endLine;

            if (textEditor.SelectionLength > 0)
            {
                startLine = doc.GetLocation(textEditor.SelectionStart).Line;
                endLine = doc.GetLocation(textEditor.SelectionStart + textEditor.SelectionLength).Line;
            }
            else
            {
                startLine = endLine = textEditor.TextArea.Caret.Line;
            }

            doc.BeginUpdate();
            try
            {
                for (int i = startLine; i <= endLine; i++)
                {
                    var line = doc.GetLineByNumber(i);
                    string lineText = doc.GetText(line.Offset, line.Length);
                    if (lineText.TrimStart().StartsWith(comment))
                    {
                        int commentIndex = lineText.IndexOf(comment);
                        doc.Remove(line.Offset + commentIndex, comment.Length);
                    }
                }
            }
            finally
            {
                doc.EndUpdate();
            }
        }

        private string GetCommentString()
        {
            switch (_Syntax)
            {
                case SyntaxHighlighting.CSharp:
                case SyntaxHighlighting.JavaScript:
                case SyntaxHighlighting.Java:
                case SyntaxHighlighting.CPlusPlus:
                    return "//";
                case SyntaxHighlighting.TransactSQL:
                case SyntaxHighlighting.MySql:
                    return "--";
                default:
                    return null;
            }
        }

        private void BtnBookmark_Click(object sender, EventArgs e)
        {
            // AvalonEdit doesn't have built-in bookmarks, would need custom implementation
            MessageBox.Show("Bookmark functionality not yet implemented for AvalonEdit", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            // Would need custom bookmark implementation
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            // Would need custom bookmark implementation
        }

        private void BtnClearBookmarks_Click(object sender, EventArgs e)
        {
            // Would need custom bookmark implementation
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
                DialogResult result = MessageBox.Show($"Do you want to save the current file {fi.Name}? (Y) or Export to a new file? (N)", "Save to file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    save = true;
                else if (result == DialogResult.No)
                    export = true;
                else
                    return;
            }

            if (export)
            {
                SaveFileDialog saver = new SaveFileDialog { AddExtension = true, AutoUpgradeEnabled = true };
                SetSaveDialogFilter(saver);

                if (saver.ShowDialog() == DialogResult.OK)
                {
                    curFileName = saver.FileName;
                    File.WriteAllText(saver.FileName, textEditor.Text);
                }
            }
            else
            {
                File.WriteAllText(curFileName, textEditor.Text);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog opener = new OpenFileDialog { Title = "Load from file" };
            SetOpenDialogFilter(opener);

            if (opener.ShowDialog() == DialogResult.OK)
            {
                curFileName = opener.FileName;
                textEditor.Text = File.ReadAllText(opener.FileName);
            }
        }

        private void SetSaveDialogFilter(SaveFileDialog dialog)
        {
            switch (_Syntax)
            {
                case SyntaxHighlighting.CSharp:
                    dialog.Title = "Save C# Code to file";
                    dialog.Filter = "C# file|*.cs|Any file|*.*";
                    break;
                case SyntaxHighlighting.XML:
                    dialog.Title = "Save XML document";
                    dialog.Filter = "XML file|*.xml|Any file|*.*";
                    break;
                case SyntaxHighlighting.TransactSQL:
                case SyntaxHighlighting.MySql:
                    dialog.Title = "Save SQL script";
                    dialog.Filter = "Text file|*.txt|SQL file|*.sql|Any file|*.*";
                    break;
                case SyntaxHighlighting.JavaScript:
                    dialog.Title = "Save JavaScript file";
                    dialog.Filter = "JavaScript file|*.js|Any file|*.*";
                    break;
                case SyntaxHighlighting.HTML:
                    dialog.Title = "Save HTML document";
                    dialog.Filter = "HTML file|*.html;*.htm|Any file|*.*";
                    break;
                case SyntaxHighlighting.CSS:
                    dialog.Title = "Save CSS file";
                    dialog.Filter = "CSS file|*.css|Any file|*.*";
                    break;
                case SyntaxHighlighting.Json:
                    dialog.Title = "Save JSON file";
                    dialog.Filter = "JSON file|*.json|Any file|*.*";
                    break;
                default:
                    dialog.Title = "Text to file";
                    dialog.Filter = "Text file|*.txt|Any file|*.*";
                    break;
            }
        }

        private void SetOpenDialogFilter(OpenFileDialog dialog)
        {
            switch (_Syntax)
            {
                case SyntaxHighlighting.CSharp:
                    dialog.Filter = "C# file|*.cs|Any file|*.*";
                    break;
                case SyntaxHighlighting.XML:
                    dialog.Filter = "XML file|*.xml|Any file|*.*";
                    break;
                case SyntaxHighlighting.TransactSQL:
                case SyntaxHighlighting.MySql:
                    dialog.Filter = "Text file|*.txt|SQL file|*.sql|Any file|*.*";
                    break;
                case SyntaxHighlighting.JavaScript:
                    dialog.Filter = "JavaScript file|*.js|Any file|*.*";
                    break;
                case SyntaxHighlighting.HTML:
                    dialog.Filter = "HTML file|*.html;*.htm|Any file|*.*";
                    break;
                case SyntaxHighlighting.CSS:
                    dialog.Filter = "CSS file|*.css|Any file|*.*";
                    break;
                case SyntaxHighlighting.Json:
                    dialog.Filter = "JSON file|*.json|Any file|*.*";
                    break;
                default:
                    dialog.Filter = "Text file|*.txt|Any file|*.*";
                    break;
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (searchForm == null || searchForm.IsDisposed)
                searchForm = new SearchAndReplace();
            searchForm.ShowFor(textEditor, false);
        }

        public TextRange FindNext(bool viaF3, bool searchBackward, string messageIfNotFound)
        {
            if (searchForm == null || searchForm.IsDisposed)
                searchForm = new SearchAndReplace();

            return searchForm.FindNext(viaF3, searchBackward, messageIfNotFound);
        }
        #endregion
    }
}
