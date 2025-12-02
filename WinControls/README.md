# WinControls

Additional Windows Forms controls library - part of the ZidUtilities solution.

## Description

This is a supplementary library for Windows Forms controls that extends the functionality provided by CommonCode.Win. It serves as a container for additional custom controls and components that complement the main control library.

## Dependencies

- System.Windows.Forms
- System.Drawing
- CommonCode (base library)
- CommonCode.Win (main controls library)

## Target Framework

.NET Framework 4.8

## Purpose

WinControls provides:

- **Extension Point** - Additional custom controls not included in CommonCode.Win
- **Experimental Controls** - New control prototypes before moving to CommonCode.Win
- **Specialized Controls** - Domain-specific controls for particular use cases
- **Third-party Integration** - Wrappers and adapters for third-party controls
- **Custom Components** - Application-specific reusable components

## Usage

### Basic Integration

```csharp
using WinControls;

// Reference WinControls in your Windows Forms project
// Use controls from the toolbox or create them programmatically
```

### Creating Custom Controls

This library follows the same patterns as CommonCode.Win:

```csharp
using System;
using System.Windows.Forms;
using System.Drawing;

namespace WinControls.CustomControls
{
    public class MyCustomControl : UserControl
    {
        public MyCustomControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Control initialization
            this.BackColor = Color.White;
            this.Size = new Size(200, 100);
        }

        // Custom properties
        public string CustomProperty { get; set; }

        // Custom events
        public event EventHandler CustomEvent;

        // Custom methods
        public void CustomMethod()
        {
            CustomEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
```

### Extending CommonCode.Win Controls

```csharp
using ZidUtilities.CommonCode.Win.Controls;

namespace WinControls.Extensions
{
    // Extend existing controls with additional functionality
    public class EnhancedZidGrid : ZidGrid
    {
        public EnhancedZidGrid()
        {
            // Additional initialization
        }

        // Add new features
        public void ExportToCustomFormat()
        {
            // Custom export logic
        }

        // Override existing behavior
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            // Custom cell formatting
            base.OnCellFormatting(e);
        }
    }
}
```

### Integrating with Existing Applications

```csharp
using WinControls;
using ZidUtilities.CommonCode.Win;

public class MainForm : Form
{
    public MainForm()
    {
        // Use controls from both libraries
        var grid = new ZidGrid();  // From CommonCode.Win
        var custom = new MyCustomControl();  // From WinControls

        // Both work together seamlessly
        this.Controls.Add(grid);
        this.Controls.Add(custom);

        // Apply consistent theming
        ThemeManager.ApplyTheme(this, ZidThemes.Professional);
    }
}
```

## Development Guidelines

### Adding New Controls

1. **Create Control Class**
   ```csharp
   public class NewControl : UserControl
   {
       // Implementation
   }
   ```

2. **Add Designer Support**
   ```csharp
   [Designer(typeof(NewControlDesigner))]
   [ToolboxItem(true)]
   [ToolboxBitmap(typeof(NewControl))]
   public class NewControl : UserControl
   {
       // Implementation
   }
   ```

3. **Implement Theme Support**
   ```csharp
   public void ApplyTheme(ZidThemes theme)
   {
       // Theme application logic
   }
   ```

4. **Add to Toolbox** - Ensure controls appear in Visual Studio toolbox

### Control Naming Conventions

- Use descriptive names: `CustomerSelector`, `DataFilter`, `ReportViewer`
- Prefix with purpose: `Enhanced`, `Custom`, `Advanced`
- Follow .NET naming conventions: PascalCase for classes

### Documentation

- Add XML documentation comments
- Provide usage examples
- Document properties, methods, and events
- Include code samples in comments

```csharp
/// <summary>
/// Represents a custom control for selecting customers.
/// </summary>
/// <example>
/// <code>
/// var selector = new CustomerSelector();
/// selector.DataSource = customers;
/// selector.CustomerSelected += (s, e) => {
///     MessageBox.Show($"Selected: {e.Customer.Name}");
/// };
/// </code>
/// </example>
public class CustomerSelector : UserControl
{
    // Implementation
}
```

## Architecture

### Separation of Concerns

- **WinControls**: Application-specific or experimental controls
- **CommonCode.Win**: Stable, general-purpose controls
- Both share: CommonCode utilities and themes

### Migration Path

Controls that prove useful and stable can be moved from WinControls to CommonCode.Win:

1. Control is tested and stable
2. Control has general applicability
3. Control is documented
4. Control follows CommonCode.Win patterns
5. Move to CommonCode.Win and update references

## Common Use Cases

- **Prototyping New Controls** - Test ideas before committing to CommonCode.Win
- **Application-Specific Controls** - Controls tied to specific business logic
- **Wrapper Controls** - Simplify complex control hierarchies
- **Composite Controls** - Combine multiple controls into reusable components
- **Domain-Specific UI** - Industry or domain-specific interface elements

## Best Practices

1. **Keep It Simple** - Don't over-engineer controls
2. **Follow Patterns** - Use CommonCode.Win as a reference
3. **Theme Support** - Implement theme support for consistency
4. **Document Everything** - Clear documentation helps adoption
5. **Test Thoroughly** - Ensure controls work in various scenarios
6. **Performance** - Profile custom drawing and event handling
7. **Accessibility** - Support keyboard navigation and screen readers

## Examples

### Simple Custom Control

```csharp
public class StatusIndicator : UserControl
{
    private Label lblStatus;
    private PictureBox picIcon;

    public StatusIndicator()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Size = new Size(200, 30);

        picIcon = new PictureBox
        {
            Location = new Point(5, 5),
            Size = new Size(20, 20),
            SizeMode = PictureBoxSizeMode.Zoom
        };

        lblStatus = new Label
        {
            Location = new Point(30, 7),
            AutoSize = true
        };

        this.Controls.Add(picIcon);
        this.Controls.Add(lblStatus);
    }

    public StatusType Status
    {
        get => _status;
        set
        {
            _status = value;
            UpdateDisplay();
        }
    }
    private StatusType _status;

    private void UpdateDisplay()
    {
        switch (Status)
        {
            case StatusType.Online:
                lblStatus.Text = "Online";
                picIcon.Image = Properties.Resources.GreenDot;
                break;
            case StatusType.Offline:
                lblStatus.Text = "Offline";
                picIcon.Image = Properties.Resources.RedDot;
                break;
            case StatusType.Away:
                lblStatus.Text = "Away";
                picIcon.Image = Properties.Resources.YellowDot;
                break;
        }
    }

    public enum StatusType
    {
        Online,
        Offline,
        Away
    }
}
```

### Usage

```csharp
var indicator = new StatusIndicator
{
    Location = new Point(10, 10),
    Status = StatusIndicator.StatusType.Online
};

this.Controls.Add(indicator);
```

## Related Projects

- **CommonCode.Win**: Main Windows Forms utilities and controls library
- **CommonCode**: Core utilities library
- **ZidUtilities**: Main application demonstrating all libraries

## Future Development

This library provides a space for:

- New control experiments
- Application-specific customizations
- Third-party control wrappers
- Advanced specialized controls
- Community contributions

## Note

Most of the Windows Forms control functionality is provided by CommonCode.Win. This library serves as an extension point for additional controls that may be developed in the future. When adding new controls, consider whether they should be in WinControls (specific/experimental) or CommonCode.Win (general/stable).
