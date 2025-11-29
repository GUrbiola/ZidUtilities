using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;

namespace ZidUtilities.CommonCode.ICSharpTextEditor.HelperForms  
{
    /// <summary>
    /// Bundles a group of markers together so that they can be cleared 
    /// together.
    /// </summary>
    public class HighlightGroup : IDisposable
    {
        List<TextMarker> _markers = new List<TextMarker>();
        TextEditorControl _editor;
        IDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightGroup"/> class
        /// associated with the given <see cref="TextEditorControl"/>.
        /// </summary>
        /// <param name="editor">
        /// The <see cref="TextEditorControl"/> whose document will host the markers.
        /// This must not be null.
        /// </param>
        public HighlightGroup(TextEditorControl editor)
        {
            _editor = editor;
            _document = editor.Document;
        }

        /// <summary>
        /// Adds a text marker to the group and registers it with the document's
        /// marker strategy so it becomes visible in the editor.
        /// </summary>
        /// <param name="marker">
        /// The <see cref="TextMarker"/> to add. This marker will be tracked by the group
        /// and will be removed when <see cref="ClearMarkers"/> or <see cref="Dispose"/>
        /// is called.
        /// </param>
        /// <remarks>
        /// This method updates both the group's internal list and the document's
        /// marker strategy. The caller remains responsible for creating a valid
        /// <see cref="TextMarker"/> instance.
        /// </remarks>
        public void AddMarker(TextMarker marker)
        {
            _markers.Add(marker);
            _document.MarkerStrategy.AddMarker(marker);
        }

        /// <summary>
        /// Removes all markers that are part of this group from the document and
        /// clears the group's internal list.
        /// </summary>
        /// <remarks>
        /// This method attempts to remove each tracked <see cref="TextMarker"/> from
        /// the underlying document's marker strategy and then refreshes the editor UI.
        /// If an exception occurs during removal, a message box is shown with the
        /// error message. After a successful run the group's marker list will be empty.
        /// </remarks>
        public void ClearMarkers()
        {
            try
            {
                foreach (TextMarker m in _markers)
                    _document.MarkerStrategy.RemoveMarker(m);
                _markers.Clear();
                _editor.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error ClearMarkers: SearchAndReplace.cs-501", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources. In this implementation it clears the group's
        /// markers and suppresses finalization.
        /// </summary>
        /// <remarks>
        /// Calling <see cref="Dispose"/> removes all markers tracked by this group
        /// from the document (via <see cref="ClearMarkers"/>) and calls
        /// <see cref="GC.SuppressFinalize(object)"/> to prevent the finalizer from
        /// running. This method is safe to call multiple times.
        /// </remarks>
        public void Dispose() { ClearMarkers(); GC.SuppressFinalize(this); }

        /// <summary>
        /// Finalizer that ensures markers are cleared if <see cref="Dispose"/> was not
        /// called. This is a fallback and should not be relied upon for deterministic
        /// cleanup.
        /// </summary>
        /// <remarks>
        /// Finalizers run on the GC thread and should avoid accessing managed objects
        /// that may have already been finalized; this finalizer simply calls
        /// <see cref="Dispose"/> to mirror the original behavior, but callers should
        /// prefer calling <see cref="Dispose"/> explicitly.
        /// </remarks>
        ~HighlightGroup() { Dispose(); }

        /// <summary>
        /// Gets a read-only snapshot of the markers currently tracked by this group.
        /// </summary>
        /// <returns>
        /// An <see cref="IList{TextMarker}"/> containing the markers tracked by this group.
        /// The returned list is read-only and reflects the group's current contents at the
        /// time of the call.
        /// </returns>
        public IList<TextMarker> Markers { get { return _markers.AsReadOnly(); } }
    }
}
