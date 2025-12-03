using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ZidUtilities.CommonCode.AvalonEdit.HelperForms
{
    /// <summary>
    /// Bundles a group of highlight renderers together so that they can be cleared together.
    /// </summary>
    public class HighlightGroup : IDisposable
    {
        private readonly List<ColorizeAvalonEdit> _markers = new List<ColorizeAvalonEdit>();
        private readonly TextEditor _editor;

        public HighlightGroup(TextEditor editor)
        {
            _editor = editor;
        }

        public void AddMarker(int offset, int length, Color color)
        {
            var marker = new ColorizeAvalonEdit(offset, length, color);
            _markers.Add(marker);
            _editor.TextArea.TextView.LineTransformers.Add(marker);
        }

        public void ClearMarkers()
        {
            try
            {
                foreach (var m in _markers)
                    _editor.TextArea.TextView.LineTransformers.Remove(m);
                _markers.Clear();
            }
            catch (Exception)
            {
                // Silently handle errors
            }
        }

        public void Dispose()
        {
            ClearMarkers();
            GC.SuppressFinalize(this);
        }

        ~HighlightGroup() { Dispose(); }
    }

    /// <summary>
    /// Document colorizer for highlighting specific text ranges
    /// </summary>
    public class ColorizeAvalonEdit : DocumentColorizingTransformer
    {
        private readonly int _offset;
        private readonly int _length;
        private readonly Color _color;

        public ColorizeAvalonEdit(int offset, int length, Color color)
        {
            _offset = offset;
            _length = length;
            _color = color;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            int lineEndOffset = line.EndOffset;

            // Check if this line intersects with our highlight range
            if (lineEndOffset < _offset || lineStartOffset > _offset + _length)
                return;

            int start = Math.Max(_offset, lineStartOffset);
            int end = Math.Min(_offset + _length, lineEndOffset);

            if (end > start)
            {
                ChangeLinePart(start, end, element =>
                {
                    element.TextRunProperties.SetBackgroundBrush(new SolidColorBrush(_color));
                    element.TextRunProperties.SetForegroundBrush(Brushes.Black);
                });
            }
        }
    }
}
