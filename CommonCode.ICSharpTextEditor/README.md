# CommonCode.ICSharpTextEditor

Extended text editor control based on ICSharpCode.TextEditor with advanced syntax highlighting and editing features.

## Features

By default this control provides syntax highlight for a good number of the languages, the syntax is based on the xshd files provided by 
this github repository:
https://github.com/xv/ICSharpCode.TextEditor-Lexers
Except for TransactSQL which has been custom made for this library.

To use the syntax highlighting features you need to set the `Syntax` property of the `ExtendedEditor` control to one of the predefined syntax types.
The control also provides code folding and bracket matching depending on the language selected.

Also there is a toolbar with expected functionality, comment, uncomment, toggle bookmark, etc. to access to this features search for the 
preperties type `ToolbarOption` in the `ExtendedEditor` control, you can work with them on the designer.
Buttons Run, Stop and Kill, do not provide functionality by default, you need to handle them using the control's events: `OnRun`, `OnStop` and `OnKill`.

The toolbar can be also customized with shortcuts, look at the class `ToolbarOption` and its properties ShortCut and ThenShortCut.
Finally the control also provide implicit shortcuts for common operations, selection to uppercase, lowercase, etc. Look at the class `ImplicitShortcut` for more information.

The original ICSharpCode.TextEditor.TextEditorControl can be accessed through the propety Editor of the `ExtendedEditor` control.

These functionlity satisfy most of MY needs, I might come back and change or add more features in the future, but for now this is it.

### Extended Editor Control
- **ExtendedEditor**: Enhanced text editor with toolbar and extended functionality
- Custom icon and designer support for Windows Forms integration

### Syntax Highlighting
- **SyntaxHighlighting**: Advanced syntax highlighting system
- **InlineSyntaxProvider**: Dynamic syntax provider for runtime syntax definitions
- **SyntaxFiles**: Embedded syntax definition resources
- Support for multiple programming languages

### Code Folding
- **FoldingStrategies**: Code folding strategies for different languages
- Collapsible code regions for improved readability

### Bracket Matching
- **BracketMatching**: Automatic bracket and parenthesis matching
- Visual highlighting of matching pairs

### Helper Forms
- Additional dialogs and forms for editor enhancement
- Context-sensitive helper interfaces

### Additional Features
- **ICSharpTextEditorExtensions**: Extension methods for the editor
- **ImplicitShortcut**: Keyboard shortcut management
- **ToolbarOption**: Customizable toolbar options
- **ToolbarTextBox**: Specialized toolbar text box control

## Dependencies

- ICSharpCode.TextEditor
- System.Windows.Forms

## Target Framework

.NET Framework 4.8

## Installation

Add a reference to `CommonCode.ICSharpTextEditor.dll` in your Windows Forms project. The ICSharpCode.TextEditor component is included.

## Usage Examples

### Basic Integration in Windows Forms

```csharp
using ZidUtilities.CommonCode.ICSharpTextEditor;

// Add ExtendedEditor to your form
var codeEditor = new ExtendedEditor
{
    Dock = DockStyle.Fill,
    ShowLineNumbers = true
};

this.Controls.Add(codeEditor);
```


### Syntax Highlighting

```csharp
using ICSharpCode.TextEditor.Document;

// Set syntax highlighting based on file extension
extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.CSharp;


// Set highlighting dynamically
string extension = Path.GetExtension(fileName).ToLower();
switch (extension)
{
    case ".cs":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.CSharp;
        break;
    case ".vb":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.VBNET;
        break;
    case ".xml":
    case ".config":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.XML;
        break;
    case ".html":
    case ".htm":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.HTML;
        break;
    case ".js":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.JavaScript;
        break;
    case ".java":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.Java;
        break;
    case ".cpp":
    case ".h":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.CPlusPlus;
        break;
    case ".sql":
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.TransactSQL;
        break;
    default:
        extendedEditor.Syntax = ZidUtilities.CommonCode.ICSharpTextEditor.SyntaxHighlighting.None;
        break;
}
```

### Text Selection and Manipulation

```csharp
// Get selected text
string selectedText = codeEditor.ActiveTextAreaControl.SelectionManager.SelectedText;

// Replace selected text
codeEditor.ActiveTextAreaControl.SelectionManager.SelectedText = "replacement";

// Select all
codeEditor.ActiveTextAreaControl.SelectAll();

// Get current line
int lineNumber = codeEditor.ActiveTextAreaControl.Caret.Line;
string lineText = codeEditor.Document.GetText(codeEditor.Document.GetLineSegment(lineNumber));

// Insert text at caret
int offset = codeEditor.ActiveTextAreaControl.Caret.Offset;
codeEditor.Document.Insert(offset, "inserted text");

// Clear all text
codeEditor.Text = string.Empty;
```

### Search and Replace

Use the provide search functionality in the toolbar or implement custom search using the editor's document methods.

### Complete Example: Simple Code Editor

```csharp
public class SimpleCodeEditor : Form
{
    private ExtendedEditor codeEditor;
    private string currentFile;

    public SimpleCodeEditor()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Simple Code Editor";
        this.Size = new Size(800, 600);

        // Create editor
        //Dont do this, use visual studio designer to add the control to the form, makes everythign easier
        //codeEditor = new ExtendedEditor
        //{
        //    Dock = DockStyle.Fill,
        //    ShowLineNumbers = true
        //};

        //By default open file functionality is included in the toolbar
        //var openItem = new ToolStripMenuItem("Open", null, OpenFile_Click);
        //By default save file functionality is included in the toolbar
        //var saveItem = new ToolStripMenuItem("Save", null, SaveFile_Click);
    }


}
```

## Common Use Cases

- **Code Editors** - Build custom IDEs for Windows Forms
- **Script Editors** - Edit SQL, PowerShell, Python scripts
- **Configuration Editors** - Edit XML, JSON configuration files
- **Log Viewers** - View code or log files with syntax highlighting
- **Source Code Viewers** - Display source code in applications
- **Learning Tools** - Create programming tutorial applications
- **Text Processing Tools** - Advanced text editing with code awareness

## Features Summary

- **Syntax Highlighting** - Support for 20+ programming languages
- **Code Folding** - Collapsible code regions
- **Bracket Matching** - Automatic matching bracket highlighting
- **Line Numbers** - Optional line number display
- **Undo/Redo** - Full undo stack support
- **Search and Replace** - Built-in text search functionality
- **Customizable Toolbar** - Extensible toolbar with common operations
- **Plugin Architecture** - Extend with custom plugins
- **Keyboard Shortcuts** - Customizable key bindings

## Related Projects

- **CommonCode.AvalonEdit**: WPF-based text editor (for WPF applications), which for now is work in progress, no code has been uploaded yet.

## Note

This library is specifically for Windows Forms applications. For WPF applications, use CommonCode.AvalonEdit instead.
