using ICSharpCode.TextEditor;
using System;
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
    public partial class ExtendedEditor : UserControl
    {
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
                _Syntax = value; 
            }
        }
        [Category("Custom Properties")]
        [Description("Property that grants access to the text control itself: ICSharpCode.TextEditor.TextEditorControl")]
        public TextEditorControl Editor
        {
            get { return this.CtrlEditor; }
        }

        private ToolbarOption _btnRun = new ToolbarOption("Run", "Executes selected/all code", Content.Run, true) { Enabled = true };
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

        private ToolbarOption _btnKill = new ToolbarOption("Kill", "Kills thread executing code", Content.RedAlert, true) { Enabled = false };
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

        private ToolbarOption _btnComment = new ToolbarOption("Comment", "Comment selected code lines", Content.Comment, true) { Enabled = false };
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
        
        private ToolbarOption _btnNextBookmark = new ToolbarOption("NextBookmark", "Moves cursor/position to the next bookmark", Content.Next, true) { Enabled = true }  ;
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

        [Category("Custom Properties")]
        [Description("Input box that can be used to disply some text.")]
        public ToolbarTextBox Txt01Helper { get; set; }
        [Category("Custom Properties")]
        [Description("Input box that can be used to disply some text.")]
        public ToolbarTextBox Txt02Helper { get; set; }

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



        public ExtendedEditor()
        {
            InitializeComponent();

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
