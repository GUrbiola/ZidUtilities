# CommonCode.AvalonEdit

Utilities and extensions for AvalonEdit, the WPF-based text editor component.

## Description

This library provides utilities, extensions, and helpers for working with AvalonEdit, a popular WPF text editing component known for its syntax highlighting and code editing capabilities.

## Features

- AvalonEdit control extensions
- Custom syntax highlighting support
- Editor enhancement utilities
- Integration helpers for WPF applications

## Dependencies

- AvalonEdit
- PresentationCore (WPF)
- PresentationFramework (WPF)
- WindowsBase (WPF)
- CommonCode (base library)

## Target Framework

.NET Framework 4.8

## Installation

1. Add a reference to `CommonCode.AvalonEdit.dll` in your WPF project
2. Install AvalonEdit NuGet package:
   ```
   Install-Package AvalonEdit
   ```

## Usage Examples

### Basic AvalonEdit Integration

```xml
<!-- Add to your WPF Window XAML -->
<Window x:Class="YourNamespace.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:avalonedit="http://icsharpcode.net/sharpdevelop/avalonedit">
    <Grid>
        <avalonedit:TextEditor
            Name="textEditor"
            FontFamily="Consolas"
            FontSize="10pt"
            SyntaxHighlighting="C#"
            ShowLineNumbers="True"/>
    </Grid>
</Window>
```

### Loading and Saving Text

```csharp
using ICSharpCode.AvalonEdit;
using System.IO;

// Load text from file
textEditor.Load(fileName);

// Or load directly
string fileContent = File.ReadAllText(fileName);
textEditor.Text = fileContent;

// Save text to file
textEditor.Save(fileName);

// Or save directly
File.WriteAllText(fileName, textEditor.Text);
```

### Syntax Highlighting

```csharp
using ICSharpCode.AvalonEdit.Highlighting;

// Set syntax highlighting by language name
textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

// Available built-in syntaxes: C#, VB, C++, XML, HTML, ASP/XHTML, Boo, Coco,
// CSS, Java, JavaScript, Patch, PHP, TeX, VBNET, and more

// Change syntax highlighting dynamically based on file extension
string extension = Path.GetExtension(fileName);
switch (extension.ToLower())
{
    case ".cs":
        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
        break;
    case ".vb":
        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("VBNET");
        break;
    case ".xml":
    case ".xaml":
        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
        break;
    case ".html":
    case ".htm":
        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
        break;
    case ".js":
        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
        break;
    case ".sql":
        textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("SQL");
        break;
    default:
        textEditor.SyntaxHighlighting = null;
        break;
}
```

### Text Selection and Manipulation

```csharp
// Get selected text
string selectedText = textEditor.SelectedText;

// Replace selected text
textEditor.SelectedText = "Replacement text";

// Select text programmatically
textEditor.Select(startOffset, length);

// Select all text
textEditor.SelectAll();

// Get current line
var currentLine = textEditor.Document.GetLineByOffset(textEditor.CaretOffset);
string lineText = textEditor.Document.GetText(currentLine.Offset, currentLine.Length);

// Insert text at caret
textEditor.Document.Insert(textEditor.CaretOffset, "inserted text");

// Clear all text
textEditor.Clear();
```

### Search and Replace

```csharp
using ICSharpCode.AvalonEdit.Search;

// Show search panel
SearchPanel.Install(textEditor);

// Search programmatically
string searchText = "keyword";
int index = textEditor.Text.IndexOf(searchText);

if (index >= 0)
{
    textEditor.Select(index, searchText.Length);
    textEditor.ScrollTo(textEditor.Document.GetLocation(index).Line,
                       textEditor.Document.GetLocation(index).Column);
}

// Replace text
string oldText = "old";
string newText = "new";
textEditor.Text = textEditor.Text.Replace(oldText, newText);
```

### Code Folding

```csharp
using ICSharpCode.AvalonEdit.Folding;

// Enable folding for C# code
var foldingManager = FoldingManager.Install(textEditor.TextArea);
var foldingStrategy = new BraceFoldingStrategy();

// Update foldings
foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);

// Update foldings when text changes
textEditor.TextChanged += (sender, e) =>
{
    if (foldingStrategy != null && foldingManager != null)
    {
        foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
    }
};
```

### Line Numbers and Margins

```csharp
// Show/hide line numbers
textEditor.ShowLineNumbers = true;

// Customize line number margin
textEditor.TextArea.LeftMargins[0].MarginBrush = Brushes.LightGray;

// Add custom margin
var customMargin = new YourCustomMargin();
textEditor.TextArea.LeftMargins.Add(customMargin);
```

### Undo/Redo

```csharp
// Undo last action
if (textEditor.CanUndo)
{
    textEditor.Undo();
}

// Redo last undone action
if (textEditor.CanRedo)
{
    textEditor.Redo();
}

// Listen to undo/redo changes
textEditor.TextArea.Document.UndoStack.PropertyChanged += (sender, e) =>
{
    // Update UI buttons
    undoButton.IsEnabled = textEditor.CanUndo;
    redoButton.IsEnabled = textEditor.CanRedo;
};
```

### Custom Key Bindings

```csharp
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Input;

// Add custom command
textEditor.TextArea.KeyBindings.Add(new KeyBinding(
    new RelayCommand(SaveFile),
    new KeyGesture(Key.S, ModifierKeys.Control)
));

void SaveFile()
{
    textEditor.Save(currentFileName);
}
```

### Autocompletion

```csharp
using ICSharpCode.AvalonEdit.CodeCompletion;

CompletionWindow completionWindow;

textEditor.TextArea.TextEntering += TextArea_TextEntering;
textEditor.TextArea.TextEntered += TextArea_TextEntered;

void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
{
    if (e.Text == ".")
    {
        // Show completion window
        completionWindow = new CompletionWindow(textEditor.TextArea);

        var data = completionWindow.CompletionList.CompletionData;
        data.Add(new MyCompletionData("Item 1"));
        data.Add(new MyCompletionData("Item 2"));
        data.Add(new MyCompletionData("Item 3"));

        completionWindow.Show();
        completionWindow.Closed += (o, args) => completionWindow = null;
    }
}

void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
{
    if (e.Text.Length > 0 && completionWindow != null)
    {
        if (!char.IsLetterOrDigit(e.Text[0]))
        {
            completionWindow.CompletionList.RequestInsertion(e);
        }
    }
}

public class MyCompletionData : ICompletionData
{
    public MyCompletionData(string text)
    {
        this.Text = text;
    }

    public string Text { get; }
    public object Content => this.Text;
    public object Description => "Description for " + this.Text;

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        textArea.Document.Replace(completionSegment, this.Text);
    }
}
```

### Readonly Mode

```csharp
// Make editor read-only
textEditor.IsReadOnly = true;

// Allow specific operations while read-only
textEditor.IsReadOnly = false;
textEditor.IsModified = false;
```

### Modified State Tracking

```csharp
// Track if document has been modified
textEditor.IsModified = false; // Reset after save

// Listen to modifications
textEditor.TextChanged += (sender, e) =>
{
    bool isModified = textEditor.IsModified;
    // Update window title with * for modified documents
    this.Title = $"{fileName}{(isModified ? "*" : "")} - Editor";
};
```

### Custom Background Renderers

```csharp
using ICSharpCode.AvalonEdit.Rendering;

public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
{
    private TextEditor _editor;

    public HighlightCurrentLineBackgroundRenderer(TextEditor editor)
    {
        _editor = editor;
    }

    public KnownLayer Layer => KnownLayer.Background;

    public void Draw(TextView textView, DrawingContext drawingContext)
    {
        if (_editor.Document == null)
            return;

        var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
        var segment = new TextSegment { StartOffset = currentLine.Offset, EndOffset = currentLine.EndOffset };

        foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
        {
            drawingContext.DrawRectangle(
                new SolidColorBrush(Color.FromArgb(40, 0, 100, 200)),
                null,
                rect);
        }
    }
}

// Install the renderer
textEditor.TextArea.TextView.BackgroundRenderers.Add(
    new HighlightCurrentLineBackgroundRenderer(textEditor));
```

## Common Use Cases

- **Code Editors** - Build custom IDE or code editing tools
- **Configuration Editors** - Edit XML, JSON, YAML configuration files
- **Script Editors** - Edit PowerShell, Python, or other scripts
- **Log Viewers** - View and search through log files with syntax highlighting
- **Document Viewers** - Display formatted text documents
- **SQL Query Editors** - Write and edit SQL queries
- **Markdown Editors** - Edit Markdown with live preview

## Advantages of AvalonEdit

- **WPF Native** - Seamless integration with WPF applications
- **High Performance** - Handles large files efficiently
- **Extensible** - Easy to customize and extend
- **Syntax Highlighting** - Built-in support for many languages
- **Code Folding** - Collapsible code regions
- **Search and Replace** - Built-in search functionality
- **Undo/Redo** - Full undo stack support

## Related Projects

- **CommonCode.ICSharpTextEditor**: Windows Forms text editor (for non-WPF applications)

## Note

This library is specifically for WPF applications. For Windows Forms applications, use CommonCode.ICSharpTextEditor instead.
