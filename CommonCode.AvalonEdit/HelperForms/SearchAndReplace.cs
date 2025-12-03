using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;

namespace ZidUtilities.CommonCode.AvalonEdit.HelperForms
{
    /// <summary>
    /// SearchAndReplace form allows users to search and replace text within a TextEditor (AvalonEdit).
    /// </summary>
    public partial class SearchAndReplace : Form
    {
        private TextEditorSearcher _search;
        private TextEditor _editor;
        private Dictionary<TextEditor, HighlightGroup> _highlightGroups = new Dictionary<TextEditor, HighlightGroup>();
        public bool _lastSearchWasBackward = false;
        public bool _lastSearchLoopedAround;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchAndReplace"/> form.
        /// </summary>
        public SearchAndReplace()
        {
            InitializeComponent();
            _search = new TextEditorSearcher();
        }

        /// <summary>
        /// Gets or sets the active <see cref="TextEditor"/> that this dialog operates on.
        /// </summary>
        private TextEditor Editor
        {
            get => _editor;
            set
            {
                _editor = value;
                _search.Document = _editor?.Document;
                UpdateTitleBar();
            }
        }

        /// <summary>
        /// Updates the form's title bar text to reflect current search/replace mode.
        /// </summary>
        private void UpdateTitleBar()
        {
            Text = "Text search or replace";
        }

        /// <summary>
        /// Properly hides the form and returns focus to the owner form.
        /// </summary>
        private void HideAndReturnFocus()
        {
            Hide();

            if (this.Owner != null && !this.Owner.IsDisposed)
            {
                this.Owner.Select();
                this.Owner.Activate();
            }
        }

        /// <summary>
        /// Shows the search/replace dialog for the specified editor and prepares search state.
        /// </summary>
        public void ShowFor(TextEditor editor, bool replaceMode)
        {
            Editor = editor;

            _search.ClearScanRegion();

            if (editor.SelectionLength > 0)
            {
                string selectedText = editor.SelectedText;
                if (!selectedText.Contains("\n") && !selectedText.Contains("\r"))
                {
                    TxtSearch.Text = selectedText;
                }
                else
                {
                    _search.SetScanRegion(editor.SelectionStart, editor.SelectionLength);
                }
            }
            else
            {
                // Get the current word that the caret is on
                if (editor.TryGetCurrentWord(out int offset, out int length))
                {
                    TxtSearch.Text = editor.Document.GetText(offset, length);
                }
            }

            this.Owner = Form.ActiveForm;
            this.Show();

            TxtSearch.SelectAll();
            TxtSearch.Focus();

            if (!_highlightGroups.ContainsKey(_editor))
                _highlightGroups[_editor] = new HighlightGroup(_editor);
            HighlightGroup group = _highlightGroups[_editor];
            group.ClearMarkers();
        }

        /// <summary>
        /// Gets or sets whether the dialog is in replace mode.
        /// </summary>
        public bool ReplaceMode
        {
            get => TxtReplace.Visible;
            set => UpdateTitleBar();
        }

        /// <summary>
        /// Event handler for the "Find Previous" button click.
        /// </summary>
        private void btnFindPrevious_Click(object sender, EventArgs e)
        {
            FindNext(false, true, "Text not found");
        }

        /// <summary>
        /// Event handler for the "Find Next" button click.
        /// </summary>
        private void btnFindNext_Click(object sender, EventArgs e)
        {
            FindNext(false, false, "Text not found");
        }

        /// <summary>
        /// Performs a search for the next occurrence of the text specified in the search box.
        /// </summary>
        public TextRange FindNext(bool viaF3, bool searchBackward, string messageIfNotFound)
        {
            if (string.IsNullOrEmpty(TxtSearch.Text))
            {
                MessageBox.Show("Please define text to find");
                return null;
            }

            _lastSearchWasBackward = searchBackward;
            _search.LookFor = TxtSearch.Text;
            _search.MatchCase = CaseSensitive.Checked;
            _search.MatchWholeWordOnly = WholeWords.Checked;

            int caretOffset = _editor.CaretOffset;
            if (viaF3 && _search.HasScanRegion &&
                (caretOffset < _search.BeginOffset || caretOffset > _search.EndOffset))
            {
                _search.ClearScanRegion();
                UpdateTitleBar();
            }

            int startFrom = caretOffset - (searchBackward ? 1 : 0);
            TextRange range = _search.FindNext(startFrom, searchBackward, out _lastSearchLoopedAround);

            if (range != null)
                SelectResult(range);
            else if (messageIfNotFound != null)
                MessageBox.Show(messageIfNotFound);

            return range;
        }

        /// <summary>
        /// Selects the specified search result range in the editor and moves the caret.
        /// </summary>
        private void SelectResult(TextRange range)
        {
            _editor.Select(range.Offset, range.Length);
            var location = _editor.Document.GetLocation(range.Offset);
            _editor.ScrollTo(location.Line, location.Column);
            _editor.CaretOffset = range.Offset + range.Length;
        }

        /// <summary>
        /// Event handler for the "Highlight All" button click.
        /// </summary>
        private void btnHighlightAll_Click(object sender, EventArgs e)
        {
            if (!_highlightGroups.ContainsKey(_editor))
                _highlightGroups[_editor] = new HighlightGroup(_editor);
            HighlightGroup group = _highlightGroups[_editor];

            if (string.IsNullOrEmpty(LookFor))
            {
                group.ClearMarkers();
            }
            else
            {
                _search.LookFor = TxtSearch.Text;
                _search.MatchCase = CaseSensitive.Checked;
                _search.MatchWholeWordOnly = WholeWords.Checked;

                bool looped = false;
                int offset = 0, count = 0;

                for (; ; )
                {
                    TextRange range = _search.FindNext(offset, false, out looped);
                    if (range == null || looped)
                        break;
                    offset = range.Offset + range.Length;
                    count++;

                    group.AddMarker(range.Offset, range.Length, Colors.Yellow);
                }

                if (count == 0)
                    MessageBox.Show("Specified text was not found.");
                else
                    HideAndReturnFocus();
            }
        }

        /// <summary>
        /// Handles the form closing event.
        /// </summary>
        private void FindAndReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                e.Cancel = true;

                if (_search != null)
                    _search.ClearScanRegion();

                HideAndReturnFocus();
            }
        }

        /// <summary>
        /// Event handler for the "Cancel" button click.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            HideAndReturnFocus();
        }

        /// <summary>
        /// Event handler for the "Replace" button click.
        /// </summary>
        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (string.Equals(_editor.SelectedText, TxtSearch.Text, StringComparison.OrdinalIgnoreCase))
                InsertText(TxtReplace.Text);
            FindNext(false, _lastSearchWasBackward, "Text not found.");
        }

        /// <summary>
        /// Event handler for the "Replace All" button click.
        /// </summary>
        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            int count = 0;
            _editor.CaretOffset = _search.BeginOffset;

            _editor.Document.BeginUpdate();
            try
            {
                while (FindNext(false, false, null) != null)
                {
                    if (_lastSearchLoopedAround)
                        break;

                    count++;
                    InsertText(TxtReplace.Text);
                }
            }
            finally
            {
                _editor.Document.EndUpdate();
            }

            if (count == 0)
            {
                MessageBox.Show("The specified text was not found.");
            }
            else
            {
                MessageBox.Show(string.Format("{0} replacements were made.", count));
                HideAndReturnFocus();
            }
        }

        /// <summary>
        /// Inserts the provided text at the current selection or caret position.
        /// </summary>
        private void InsertText(string text)
        {
            if (_editor.SelectionLength > 0)
            {
                int start = _editor.SelectionStart;
                _editor.Document.Remove(start, _editor.SelectionLength);
                _editor.Document.Insert(start, text);
                _editor.CaretOffset = start + text.Length;
            }
            else
            {
                _editor.Document.Insert(_editor.CaretOffset, text);
                _editor.CaretOffset += text.Length;
            }
        }

        /// <summary>
        /// Sets the search text shown in the dialog's search box.
        /// </summary>
        public void SetSearchString(string toSearch)
        {
            TxtSearch.Text = toSearch;
        }

        /// <summary>
        /// Gets the current search text from the dialog.
        /// </summary>
        public string LookFor => TxtSearch.Text;

        /// <summary>
        /// Event handler for the Close button.
        /// </summary>
        private void Close_Click(object sender, EventArgs e)
        {
            HideAndReturnFocus();
        }

        /// <summary>
        /// Gets whether the search is case sensitive.
        /// </summary>
        public bool MatchCase => CaseSensitive.Checked;

        /// <summary>
        /// Gets whether matching is restricted to whole words only.
        /// </summary>
        public bool MatchWholeWordOnly => WholeWords.Checked;
    }
}
