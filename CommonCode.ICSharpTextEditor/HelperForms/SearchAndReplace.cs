using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ZidUtilities.CommonCode;

namespace ZidUtilities.CommonCode.ICSharpTextEditor.HelperForms  
{
    /// <summary>
    /// SearchAndReplace form allows users to search and replace text within a TextEditorControl.
    /// </summary>
    public partial class SearchAndReplace : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchAndReplace"/> form.
        /// Sets up components and creates a new <see cref="TextEditorSearcher"/>
        /// </summary>
        public SearchAndReplace()
		{
			InitializeComponent();
			_search = new TextEditorSearcher();
		}

		TextEditorSearcher _search;
		TextEditorControl _editor;

        /// <summary>
        /// Gets or sets the active <see cref="TextEditorControl"/> that this dialog operates on.
        /// Setting the editor also assigns its document to the internal <see cref="_search"/>
        /// and updates the form title via <see cref="UpdateTitleBar"/>.
        /// </summary>
		TextEditorControl Editor 
        { 
			get 
            { 
                return _editor; 
            } 
			set 
            { 
				_editor = value;
				_search.Document = _editor.Document;
				UpdateTitleBar();
			}
		}

		/// <summary>
		/// Updates the form's title bar text to reflect current search/replace mode.
		/// No parameters. This updates the UI only.
		/// </summary>
		private void UpdateTitleBar()
		{
            Text = "Text search or replace";

		}

		/// <summary>
		/// Properly hides the form and returns focus to the owner form.
		/// Hides this dialog and attempts to re-select and activate the Owner
		/// to avoid leaving the application in an unexpected minimized state.
		/// </summary>
		private void HideAndReturnFocus()
		{
			Hide();

			// Return focus to the owner form to prevent it from being minimized
			if (this.Owner != null && !this.Owner.IsDisposed)
			{
				this.Owner.Select();
				this.Owner.Activate();
			}
		}

        /// <summary>
        /// Shows the search/replace dialog for the specified editor and prepares search state.
        /// If there is a single-line selection in the editor, the selection text populates the search box.
        /// If a multi-line selection exists the search is restricted to that region.
        /// Otherwise the current word under the caret is used as the initial search text.
        /// The dialog becomes owned by the editor's top-level control and is shown.
        /// </summary>
        /// <param name="editor">The <see cref="TextEditorControl"/> to search within.</param>
        /// <param name="replaceMode">If true the dialog is expected to operate in replace mode (UI handled elsewhere).</param>
        public void ShowFor(TextEditorControl editor, bool replaceMode)
        {
            Editor = editor;

            _search.ClearScanRegion();
            var sm = editor.ActiveTextAreaControl.SelectionManager;
            if (sm.HasSomethingSelected && sm.SelectionCollection.Count == 1)
            {
                var sel = sm.SelectionCollection[0];
                if (sel.StartPosition.Line == sel.EndPosition.Line)
                    TxtSearch.Text = sm.SelectedText;
                else
                    _search.SetScanRegion(sel);
            }
            else
            {
                // Get the current word that the caret is on
                Caret caret = editor.ActiveTextAreaControl.Caret;
                int start = TextUtilities.FindWordStart(editor.Document, caret.Offset);
                int endAt = TextUtilities.FindWordEnd(editor.Document, caret.Offset);
                TxtSearch.Text = editor.Document.GetText(start, endAt - start);
            }

            this.Owner = (Form)editor.TopLevelControl;
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
		/// The getter returns true when the replace text box is visible.
		/// The setter currently refreshes the title bar (UI state) only.
		/// </summary>
		public bool ReplaceMode
		{
			get 
            { 
                return TxtReplace.Visible; 
            }
			set 
            {
				UpdateTitleBar();
			}
		}

		/// <summary>
		/// Event handler for the "Find Previous" button click.
		/// Starts a backward search and displays a 'not found' message if appropriate.
		/// </summary>
		private void btnFindPrevious_Click(object sender, EventArgs e)
		{
			FindNext(false, true, "name not found");
		}

		/// <summary>
		/// Event handler for the "Find Next" button click.
		/// Starts a forward search and displays a 'not found' message if appropriate.
		/// </summary>
		private void btnFindNext_Click(object sender, EventArgs e)
		{
			FindNext(false, false, "name not found");
		}

		public bool _lastSearchWasBackward = false;
		public bool _lastSearchLoopedAround;

        /// <summary>
        /// Performs a search for the next occurrence of the text specified in the search box.
        /// Configures the internal <see cref="_search"/> with match options and uses the current caret
        /// position to start the search. If a match is found the selection is updated via <see cref="SelectResult"/>.
        /// </summary>
        /// <param name="viaF3">True if this search was invoked via F3; used to clear scan regions if the caret left the region.</param>
        /// <param name="searchBackward">True to search backward, false to search forward.</param>
        /// <param name="messageIfNotFound">Message to display in a message box when no match is found; pass null to suppress messages.</param>
        /// <returns>The <see cref="TextRange"/> of the found match, or null if nothing was found.</returns>
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

			var caret = _editor.ActiveTextAreaControl.Caret;
			if (viaF3 && _search.HasScanRegion && !caret.Offset.IsInRange(_search.BeginOffset, _search.EndOffset)) 
            {
				// user moved outside of the originally selected region
				_search.ClearScanRegion();
				UpdateTitleBar();
			}

			int startFrom = caret.Offset - (searchBackward ? 1 : 0);
			TextRange range = _search.FindNext(startFrom, searchBackward, out _lastSearchLoopedAround);
			if (range != null)
				SelectResult(range);
			else if (messageIfNotFound != null)
				MessageBox.Show(messageIfNotFound);
			return range;
		}

        /// <summary>
        /// Selects the specified search result range in the editor and moves the caret.
        /// The selection is applied and the editor is scrolled to make the selection visible.
        /// </summary>
        /// <param name="range">The <see cref="TextRange"/> to select. Must not be null.</param>
		private void SelectResult(TextRange range)
		{
			TextLocation p1 = _editor.Document.OffsetToPosition(range.Offset);
			TextLocation p2 = _editor.Document.OffsetToPosition(range.Offset + range.Length);
			_editor.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
			_editor.ActiveTextAreaControl.ScrollTo(p1.Line, p1.Column);
			// Also move the caret to the end of the selection, because when the user 
			// presses F3, the caret is where we start searching next time.
			_editor.ActiveTextAreaControl.Caret.Position = 
				_editor.Document.OffsetToPosition(range.Offset + range.Length);
		}
		Dictionary<TextEditorControl, HighlightGroup> _highlightGroups = new Dictionary<TextEditorControl, HighlightGroup>();

        /// <summary>
        /// Event handler for the "Highlight All" button click.
        /// Highlights all matches in the current document (or within the scan region).
        /// If the search text is empty, existing highlights are cleared.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
		private void btnHighlightAll_Click(object sender, EventArgs e)
		{
			if (!_highlightGroups.ContainsKey(_editor))
				_highlightGroups[_editor] = new HighlightGroup(_editor);
			HighlightGroup group = _highlightGroups[_editor];

            if (string.IsNullOrEmpty(LookFor))
            {
                // Clear highlights
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

                    var m = new TextMarker(range.Offset, range.Length,
                            TextMarkerType.SolidBlock, Color.Yellow, Color.Black);
                    group.AddMarker(m);
                }
                if (count == 0)
                    MessageBox.Show("Specified text was not found.");
                else
                    HideAndReturnFocus();
            }
		}

        /// <summary>
        /// Handles the form closing event to prevent disposal when the owner is not closing.
        /// If the owner is not closing the close is canceled, search region is cleared and
        /// the editor is refreshed, then the dialog is hidden and focus returned to the owner.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Form closing event arguments.</param>
		private void FindAndReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
		{	// Prevent dispose, as this form can be re-used
			// Only cancel closing if the owner form is not closing
			// If owner is closing, allow this form to be disposed too
			if (e.CloseReason != CloseReason.FormOwnerClosing)
			{
				e.Cancel = true;

				// Discard search region
				if (_search != null)
					_search.ClearScanRegion();
				if (_editor != null && !_editor.IsDisposed)
					_editor.Refresh(); // must repaint manually

				HideAndReturnFocus();
			}
		}

        /// <summary>
        /// Event handler for the "Cancel" button click.
        /// Hides the dialog and returns focus to the owner.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
		private void btnCancel_Click(object sender, EventArgs e)
		{
            HideAndReturnFocus();
		}

        /// <summary>
        /// Event handler for the "Replace" button click.
        /// If the currently selected text matches the search text (case-insensitive),
        /// replaces it with the content from the replace text box and finds the next match.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
		private void btnReplace_Click(object sender, EventArgs e)
		{
			var sm = _editor.ActiveTextAreaControl.SelectionManager;
            if (string.Equals(sm.SelectedText, TxtSearch.Text, StringComparison.OrdinalIgnoreCase))
                InsertText(TxtReplace.Text);
			FindNext(false, _lastSearchWasBackward, "name not found.");
		}

        /// <summary>
        /// Event handler for the "Replace All" button click.
        /// Replaces all occurrences starting from the search region's begin offset (to avoid infinite loops
        /// when replacement contains the search text). Uses the document undo stack to group the operation.
        /// Shows a message with the number of replacements performed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
		private void btnReplaceAll_Click(object sender, EventArgs e)
		{
			int count = 0;
			// BUG FIX: if the replacement string contains the original search string
			// (e.g. replace "red" with "very red") we must avoid looping around and
			// replacing forever! To fix, start replacing at beginning of region (by 
			// moving the caret) and stop as soon as we loop around.
			_editor.ActiveTextAreaControl.Caret.Position = 
				_editor.Document.OffsetToPosition(_search.BeginOffset);

			_editor.Document.UndoStack.StartUndoGroup();
			try 
            {
				while (FindNext(false, false, null) != null)
				{
					if (_lastSearchLoopedAround)
						break;

					// Replace
					count++;
					InsertText(TxtReplace.Text);
				}
			} 
            finally 
            {
				_editor.Document.UndoStack.EndUndoGroup();
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
        /// If there is a selection it is replaced. The operation is wrapped in an undo group.
        /// </summary>
        /// <param name="text">The text to insert into the document.</param>
		private void InsertText(string text)
		{
			var textArea = _editor.ActiveTextAreaControl.TextArea;
			textArea.Document.UndoStack.StartUndoGroup();
			try 
            {
				if (textArea.SelectionManager.HasSomethingSelected) 
                {
					textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
					textArea.SelectionManager.RemoveSelectedText();
				}
				textArea.InsertString(text);
			} 
            finally 
            {
				textArea.Document.UndoStack.EndUndoGroup();
			}
		}

        /// <summary>
        /// Sets the search text shown in the dialog's search box.
        /// </summary>
        /// <param name="ToSearch">The string to place into the search text box.</param>
        public void SetSearchString(string ToSearch)
        {
            TxtSearch.Text = ToSearch;
        }

        /// <summary>
        /// Gets the current search text from the dialog.
        /// </summary>
        public string LookFor { get { return TxtSearch.Text; } }

        /// <summary>
        /// Event handler for the Close button; hides the dialog and returns focus to the owner.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void Close_Click(object sender, EventArgs e)
        {
            HideAndReturnFocus();
        }

        /// <summary>
        /// Gets whether the search is case sensitive based on the CaseSensitive checkbox.
        /// </summary>
        public bool MatchCase { get { return CaseSensitive.Checked; } }

        /// <summary>
        /// Gets whether matching is restricted to whole words only based on the WholeWords checkbox.
        /// </summary>
        public bool MatchWholeWordOnly { get { return WholeWords.Checked; } }
    }
}
