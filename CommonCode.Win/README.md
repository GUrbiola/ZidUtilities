# CommonCode.Win

Comprehensive Windows Forms controls and utilities library featuring advanced data grids, dialogs, and theming.

## Features

### ZidGrid - Advanced Data Grid Control
- Feature-rich DataGridView-based control
- Plugin architecture (IZidGridPlugin)
- Custom context menus (ZidGridMenuItem, ZidGridMenuItemCollection)
- Theme support via GridThemeHelper
- Built-in filtering capabilities
- Plugin collection management

### Custom Controls
- **AnimatedWaitTextBox**: TextBox with animated wait indicator
- **DoubleBufferedPanel**: Flicker-free panel control
- **VerticalProgressBar**: Vertical progress bar control
- **ToastForm**: Non-intrusive toast notification control
- **ThemeManager**: Centralized theme management component
- **AddressBar**: Address/path navigation control

### Dialog Forms
- **ComplexObjectSelectionDialog**: Advanced object selection
- **MultiSelectionDialog**: Multiple item selection dialog
- **SingleSelectionDialog**: Single item selection dialog
- **ProcessingDialog**: Long-running operation dialog with progress
- **TextInputDialog**: Text input with validation
- **SqlConnectForm**: SQL connection configuration dialog

### Theming System
- **ZidThemes**: Unified theme enumeration
  - None, Default, Information, Success, Warning, Error
  - Professional, CodeProject, BlackAndWhite
- Consistent theming across all controls and forms
- Theme-aware component rendering

## Dependencies

- System.Windows.Forms
- System.Drawing
- CommonCode (base library)
- DocumentFormat.OpenXml (for export features)
- ClosedXML (for Excel operations)

## Target Framework

.NET Framework 4.8

## Installation

Add a reference to `CommonCode.Win.dll` in your Windows Forms project. Some features may require additional dependencies like ClosedXML for Excel export functionality.

## Usage Examples

### ZidGrid - Advanced Data Grid

#### Basic ZidGrid Setup

```csharp
using ZidUtilities.CommonCode.Win.Controls.Grid;
using System.Data;
using System.Windows.Forms;

// Create and configure ZidGrid
var grid = new ZidGrid
{
    Dock = DockStyle.Fill
};

this.Controls.Add(grid);

// Access the internal DataGridView for configuration
grid.GridView.AllowUserToAddRows = false;
grid.GridView.ReadOnly = true;
grid.GridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

// Bind data
DataTable data = GetYourData();
grid.DataSource = data;

// Apply a theme using the Theme property
grid.Theme = ZidThemes.Default;
```

#### Applying Themes to ZidGrid

```csharp
// Set theme using the Theme property
grid.Theme = ZidThemes.Default;         // Navy blue headers
grid.Theme = ZidThemes.Information;     // Information blue theme
grid.Theme = ZidThemes.Success;         // Green success theme
grid.Theme = ZidThemes.Warning;         // Yellow warning theme
grid.Theme = ZidThemes.Error;           // Red error theme
grid.Theme = ZidThemes.Professional;    // Professional gray theme
grid.Theme = ZidThemes.CodeProject;     // CodeProject orange theme
grid.Theme = ZidThemes.BlackAndWhite;   // Monochrome theme

// Or use GridThemeHelper directly for any DataGridView
GridThemeHelper.ApplyTheme(grid.GridView, ZidThemes.Professional);
```

#### Adding Plugins to ZidGrid

```csharp
using ZidUtilities.CommonCode.Win.Controls.Grid;
using ZidUtilities.CommonCode.Win.Controls.Grid.Plugins;

// Add built-in plugins
grid.Plugins.Add(new DataExportPlugin());
grid.Plugins.Add(new QuickFilterPlugin());
grid.Plugins.Add(new ColumnVisibilityPlugin());
grid.Plugins.Add(new FreezeColumnsPlugin());
grid.Plugins.Add(new CopySpecialPlugin());

// Plugins automatically add their context menu items and functionality
```

#### Custom Context Menu Items

```csharp
using ZidUtilities.CommonCode.Win.Controls.Grid;

// Add custom context menu item
var customMenuItem = new ZidGridMenuItem
{
    Name = "CustomAction",
    Text = "Custom Action",
    Image = Properties.Resources.CustomIcon
};

customMenuItem.Click += (sender, e) =>
{
    // e is ZidGridMenuItemClickEventArgs with ColumnIndex and RowIndex
    var args = e as ZidGridMenuItemClickEventArgs;
    if (args != null)
    {
        MessageBox.Show($"Clicked on Row: {args.RowIndex}, Column: {args.ColumnIndex}");
    }
};

grid.CustomMenuItems.Add(customMenuItem);
```

#### Working with Grid Filtering

```csharp
// Enable filtering (filter controls appear above columns)
grid.FilterBoxPosition = FilterPosition.Top;

// Disable filtering
grid.FilterBoxPosition = FilterPosition.Off;

// Access the underlying DataGridView
DataGridView dgv = grid.GridView;

// Access filter panel (if needed)
var filterControl = grid.GridFilterHost;
```

### ThemeManager - Centralized Theme Component

ThemeManager is a Component (not a static class) that manages theming for an entire form:

```csharp
using ZidUtilities.CommonCode.Win.Controls;
using System.ComponentModel;
using System.Windows.Forms;

public class MyForm : Form
{
    private ThemeManager themeManager;

    public MyForm()
    {
        InitializeComponent();

        // Create ThemeManager component
        themeManager = new ThemeManager();
        themeManager.ParentForm = this;
        themeManager.Theme = ZidThemes.Professional;

        // Apply theme to all controls on the form
        themeManager.ApplyTheme();
    }
}
```

#### Using ThemeManager in Designer

You can also add ThemeManager from the toolbox in Visual Studio designer:

1. Add ThemeManager component to your form from the toolbox
2. Set the `Theme` property in the Properties window
3. The component will automatically apply the theme to all controls

```csharp
// In designer-generated code
this.themeManager1 = new ThemeManager();
this.themeManager1.ParentForm = this;
this.themeManager1.Theme = ZidThemes.Professional;

// Apply theme (call in Form_Load or after InitializeComponent)
this.themeManager1.ApplyTheme();
```

### ToastForm - Toast Notifications

```csharp
using ZidUtilities.CommonCode.Win.Controls;

// Show toast notification using static method
ToastForm.ShowToast(
    parent: this,                          // Parent form
    title: "Success",                      // Toast title
    message: "Operation completed!",       // Toast message
    toastType: ToastType.Success,          // Type (Success, Info, Warning, Danger, etc.)
    hP: HorizontalPosition.Right,          // Horizontal position
    vP: VerticalPosition.Bottom,           // Vertical position
    duration: 3000,                        // Duration in milliseconds (0 = modal)
    posReferenceIsParent: false,           // True = relative to parent, False = relative to screen
    animation: ToastAnimation.SlideUp      // Animation type
);

// Simple info toast (using defaults)
ToastForm.ShowToast(
    parent: this,
    title: "Information",
    message: "File uploaded successfully"
);

// Warning toast
ToastForm.ShowToast(
    parent: this,
    title: "Warning",
    message: "Disk space is running low",
    toastType: ToastType.Warning,
    duration: 5000
);

// Error toast
ToastForm.ShowToast(
    parent: this,
    title: "Error",
    message: "Failed to save file",
    toastType: ToastType.Danger,
    hP: HorizontalPosition.Center,
    vP: VerticalPosition.Center,
    duration: 4000
);
```

#### ToastType Options

- **Primary** - Primary informational style
- **Secondary** - Neutral secondary style
- **Success** - Green success indicator
- **Danger** - Red error/danger indicator
- **Warning** - Yellow/orange warning
- **Info** - Blue informational
- **Light** - Light/pale style
- **Dark** - Dark emphasis style
- **Custom** - Custom colors (set manually)

#### Manual Toast Creation

```csharp
// Create toast manually for more control
ToastForm toast = new ToastForm();
toast.Title = "Processing";
toast.Description = "Please wait...";
toast.Kind = ToastType.Info;
toast.PosH = HorizontalPosition.Right;
toast.PosV = VerticalPosition.Top;
toast.Duration = 0; // 0 = modal dialog
toast.ReferenceParentForPosition = true; // Position relative to parent

// Show the toast
toast.LoadToast(this, ToastAnimation.SlideDown);
```

### ProcessingDialog - Long Running Operations

```csharp
using ZidUtilities.CommonCode.Win.Forms;
using System.ComponentModel;

// Create processing dialog
ProcessingDialog dialog = new ProcessingDialog();
dialog.DialogTitle = "Processing Data";
dialog.Message = "Please wait while we process your request...";
dialog.IsIndeterminate = true; // Marquee style progress bar

// Set theme (optional)
dialog.Theme = ZidThemes.Information;

// Or use Style property
// dialog.Style = DialogStyle.Information;

// Perform work (dialog runs on UI thread, you provide the background work)
dialog.Show();

try
{
    // Do your work here
    PerformLongRunningOperation();
}
finally
{
    dialog.Close();
}
```

#### ProcessingDialog with Progress Updates

```csharp
ProcessingDialog dialog = new ProcessingDialog();
dialog.DialogTitle = "Processing Items";
dialog.Message = "Processing...";
dialog.IsIndeterminate = false; // Show actual progress

// Update progress from your code
for (int i = 0; i < 100; i++)
{
    // Update message (thread-safe)
    dialog.Message = $"Processing item {i + 1} of 100";

    // Update progress if you have ProgressValue property
    // dialog.ProgressValue = i;

    // Do work
    System.Threading.Thread.Sleep(50);

    Application.DoEvents(); // Keep UI responsive
}

dialog.Close();
```

### MultiSelectionDialog

```csharp
using ZidUtilities.CommonCode.Win.Forms;
using System.Collections.Generic;

// Simple string list
List<string> items = new List<string>
{
    "Option 1",
    "Option 2",
    "Option 3",
    "Option 4"
};

MultiSelectionDialog dialog = new MultiSelectionDialog();
dialog.DialogTitle = "Select Items";
dialog.Message = "Please select one or more items:";
dialog.DataSource = items; // Set data source
dialog.Required = true; // Require at least one selection
dialog.Theme = ZidThemes.Professional;

if (dialog.ShowDialog() == DialogResult.OK)
{
    // Get selected texts
    List<string> selectedTexts = dialog.SelectedTexts;
    foreach (string text in selectedTexts)
    {
        Console.WriteLine($"Selected: {text}");
    }
}
```

#### MultiSelectionDialog with DataTable

```csharp
// Using DataTable
DataTable dt = new DataTable();
dt.Columns.Add("ID", typeof(int));
dt.Columns.Add("Name", typeof(string));
dt.Rows.Add(1, "Option A");
dt.Rows.Add(2, "Option B");
dt.Rows.Add(3, "Option C");

MultiSelectionDialog dialog = new MultiSelectionDialog();
dialog.DialogTitle = "Select Options";
dialog.Message = "Choose one or more options:";
dialog.DataSource = dt;
dialog.DisplayMember = "Name";  // Column to display
dialog.ValueMember = "ID";      // Column for value
dialog.Style = DialogStyle.Information;

if (dialog.ShowDialog() == DialogResult.OK)
{
    // Get selected values (IDs)
    List<object> selectedValues = dialog.SelectedValues;

    // Get selected display texts
    List<string> selectedTexts = dialog.SelectedTexts;
}
```

### SingleSelectionDialog

```csharp
using ZidUtilities.CommonCode.Win.Forms;
using System.Collections.Generic;

List<string> options = new List<string> { "Option A", "Option B", "Option C" };

SingleSelectionDialog dialog = new SingleSelectionDialog();
dialog.DialogTitle = "Choose Option";
dialog.Message = "Please select one option:";
dialog.DataSource = options;
dialog.Required = true;
dialog.Theme = ZidThemes.Success;

if (dialog.ShowDialog() == DialogResult.OK)
{
    // Get selected text
    string selectedText = dialog.SelectedText;

    // Get selected value
    object selectedValue = dialog.SelectedValue;

    MessageBox.Show($"You selected: {selectedText}");
}
```

#### SingleSelectionDialog with DataTable

```csharp
DataTable dt = new DataTable();
dt.Columns.Add("Code", typeof(string));
dt.Columns.Add("Description", typeof(string));
dt.Rows.Add("OPT_A", "Option A Description");
dt.Rows.Add("OPT_B", "Option B Description");
dt.Rows.Add("OPT_C", "Option C Description");

SingleSelectionDialog dialog = new SingleSelectionDialog();
dialog.DialogTitle = "Select Option";
dialog.Message = "Choose one option:";
dialog.DataSource = dt;
dialog.DisplayMember = "Description";
dialog.ValueMember = "Code";
dialog.Style = DialogStyle.Default;

if (dialog.ShowDialog() == DialogResult.OK)
{
    string code = dialog.SelectedValue.ToString();
    string description = dialog.SelectedText;

    MessageBox.Show($"Selected: {code} - {description}");
}
```

### TextInputDialog

```csharp
using ZidUtilities.CommonCode.Win.Forms;

// Simple text input
TextInputDialog dialog = new TextInputDialog();
dialog.DialogTitle = "Enter Name";
dialog.Message = "Please enter your name:";
dialog.InputLabel = "Name:";
dialog.DefaultValue = "John Doe";
dialog.ValidationMode = TextInputFormat.Text;
dialog.Theme = ZidThemes.Default;

if (dialog.ShowDialog() == DialogResult.OK)
{
    string name = dialog.InputValue;
    MessageBox.Show($"Hello, {name}!");
}
```

#### TextInputDialog with Validation

```csharp
// Numeric input
TextInputDialog numDialog = new TextInputDialog();
numDialog.DialogTitle = "Enter Age";
numDialog.Message = "Please enter your age:";
numDialog.InputLabel = "Age:";
numDialog.ValidationMode = TextInputFormat.Numeric;
numDialog.Required = true;

if (numDialog.ShowDialog() == DialogResult.OK)
{
    int age = int.Parse(numDialog.InputValue);
    MessageBox.Show($"You are {age} years old");
}

// Email validation
TextInputDialog emailDialog = new TextInputDialog();
emailDialog.DialogTitle = "Enter Email";
emailDialog.Message = "Please enter your email address:";
emailDialog.InputLabel = "Email:";
emailDialog.ValidationMode = TextInputFormat.Email;
emailDialog.Style = DialogStyle.Information;

if (emailDialog.ShowDialog() == DialogResult.OK)
{
    string email = emailDialog.InputValue;
    MessageBox.Show($"Email: {email}");
}
```

#### TextInputFormat Options

- **Text** - Any text
- **Numeric** - Numbers only
- **Email** - Email validation
- **Phone** - Phone number format
- **Url** - URL format

### ComplexObjectSelectionDialog

```csharp
using ZidUtilities.CommonCode.Win.Forms;

// Define data class
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public string Email { get; set; }

    // Override ToString for display
    public override string ToString()
    {
        return $"{Name} ({Department})";
    }
}

// Create list of employees
List<Employee> employees = new List<Employee>
{
    new Employee { Id = 1, Name = "John Doe", Department = "IT", Email = "john@example.com" },
    new Employee { Id = 2, Name = "Jane Smith", Department = "HR", Email = "jane@example.com" },
    new Employee { Id = 3, Name = "Bob Johnson", Department = "Sales", Email = "bob@example.com" }
};

ComplexObjectSelectionDialog<Employee> dialog = new ComplexObjectSelectionDialog<Employee>();
dialog.DialogTitle = "Select Employee";
dialog.Message = "Choose an employee:";
dialog.DataSource = employees;
dialog.DisplayMember = "Name";  // Property to display (if not using ToString)
dialog.Theme = ZidThemes.Professional;

if (dialog.ShowDialog() == DialogResult.OK)
{
    Employee selected = dialog.SelectedItem;
    MessageBox.Show($"Selected: {selected.Name} from {selected.Department}");
}
```

### AnimatedWaitTextBox

```csharp
using ZidUtilities.CommonCode.Win.Controls;

// Add to form
AnimatedWaitTextBox waitTextBox = new AnimatedWaitTextBox
{
    Location = new Point(50, 50),
    Size = new Size(200, 25),
    Text = "Loading..."
};

this.Controls.Add(waitTextBox);

// Start animation
waitTextBox.StartAnimation();

// Perform async operation
await Task.Run(() =>
{
    // Long running operation
    System.Threading.Thread.Sleep(3000);
});

// Stop animation
waitTextBox.StopAnimation();
waitTextBox.Text = "Complete!";
```

### VerticalProgressBar

```csharp
using ZidUtilities.CommonCode.Win.Controls;

// Add vertical progress bar
VerticalProgressBar vProgressBar = new VerticalProgressBar
{
    Location = new Point(100, 50),
    Size = new Size(30, 200),
    Minimum = 0,
    Maximum = 100,
    Value = 0
};

this.Controls.Add(vProgressBar);

// Update progress
for (int i = 0; i <= 100; i += 10)
{
    vProgressBar.Value = i;
    await Task.Delay(100);
}
```

### DoubleBufferedPanel

```csharp
using ZidUtilities.CommonCode.Win.Controls;

// Use instead of regular Panel for custom drawing to prevent flicker
DoubleBufferedPanel panel = new DoubleBufferedPanel
{
    Dock = DockStyle.Fill
};

panel.Paint += (sender, e) =>
{
    // Custom drawing code
    e.Graphics.FillRectangle(Brushes.Blue, 10, 10, 100, 100);
    e.Graphics.DrawString("Hello", this.Font, Brushes.White, 20, 20);
};

this.Controls.Add(panel);
```

### Complete Form Example

```csharp
using System;
using System.Data;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win;
using ZidUtilities.CommonCode.Win.Controls;
using ZidUtilities.CommonCode.Win.Controls.Grid;
using ZidUtilities.CommonCode.Win.Controls.Grid.Plugins;
using ZidUtilities.CommonCode.Win.Forms;

public class MainForm : Form
{
    private ZidGrid dataGrid;
    private Button btnLoad;
    private Button btnProcess;
    private ComboBox cboTheme;
    private ThemeManager themeManager;

    public MainForm()
    {
        InitializeComponents();
        LoadData();
    }

    private void InitializeComponents()
    {
        this.Text = "ZidUtilities Demo";
        this.Size = new Size(1000, 600);

        // Create ThemeManager
        themeManager = new ThemeManager();
        themeManager.ParentForm = this;
        themeManager.Theme = ZidThemes.Default;

        // Theme selector
        cboTheme = new ComboBox
        {
            Location = new Point(10, 10),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboTheme.Items.AddRange(Enum.GetNames(typeof(ZidThemes)));
        cboTheme.SelectedIndex = 1; // Default
        cboTheme.SelectedIndexChanged += (s, e) => ApplySelectedTheme();
        this.Controls.Add(cboTheme);

        // Buttons
        btnLoad = new Button
        {
            Text = "Load Data",
            Location = new Point(170, 10),
            Size = new Size(100, 25)
        };
        btnLoad.Click += BtnLoad_Click;
        this.Controls.Add(btnLoad);

        btnProcess = new Button
        {
            Text = "Process",
            Location = new Point(280, 10),
            Size = new Size(100, 25)
        };
        btnProcess.Click += BtnProcess_Click;
        this.Controls.Add(btnProcess);

        // ZidGrid
        dataGrid = new ZidGrid
        {
            Location = new Point(10, 45),
            Size = new Size(960, 500)
        };

        dataGrid.GridView.AllowUserToAddRows = false;
        dataGrid.GridView.ReadOnly = true;
        dataGrid.GridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        // Add plugins
        dataGrid.Plugins.Add(new DataExportPlugin());
        dataGrid.Plugins.Add(new ColumnVisibilityPlugin());
        dataGrid.Plugins.Add(new QuickFilterPlugin());

        this.Controls.Add(dataGrid);

        // Apply initial theme
        ApplySelectedTheme();
    }

    private void ApplySelectedTheme()
    {
        if (Enum.TryParse<ZidThemes>(cboTheme.SelectedItem.ToString(), out var theme))
        {
            themeManager.Theme = theme;
            themeManager.ApplyTheme();
            dataGrid.Theme = theme;
        }
    }

    private void LoadData()
    {
        DataTable data = new DataTable();
        data.Columns.Add("ID", typeof(int));
        data.Columns.Add("Name", typeof(string));
        data.Columns.Add("Department", typeof(string));
        data.Columns.Add("Status", typeof(string));

        for (int i = 1; i <= 50; i++)
        {
            data.Rows.Add(
                i,
                $"Employee {i}",
                i % 3 == 0 ? "IT" : i % 3 == 1 ? "Sales" : "HR",
                i % 2 == 0 ? "Active" : "Inactive"
            );
        }

        dataGrid.DataSource = data;
    }

    private void BtnLoad_Click(object sender, EventArgs e)
    {
        LoadData();
        ToastForm.ShowToast(
            this,
            "Success",
            "Data loaded successfully!",
            ToastType.Success,
            HorizontalPosition.Right,
            VerticalPosition.Bottom,
            2000
        );
    }

    private void BtnProcess_Click(object sender, EventArgs e)
    {
        ProcessingDialog dialog = new ProcessingDialog();
        dialog.DialogTitle = "Processing Data";
        dialog.Message = "Please wait...";
        dialog.IsIndeterminate = true;
        dialog.Theme = ZidThemes.Information;

        dialog.Show();

        try
        {
            // Simulate work
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(30);
                Application.DoEvents();
            }
        }
        finally
        {
            dialog.Close();
        }

        ToastForm.ShowToast(
            this,
            "Complete",
            "Processing complete!",
            ToastType.Success,
            duration: 2000
        );
    }
}
```

## DialogStyle Enumeration

The `DialogStyle` enum provides predefined color schemes for dialogs:

- **Default** - Default blue theme
- **Information** - Information style
- **Success** - Success green theme
- **Warning** - Warning yellow theme
- **Error** - Error red theme
- **Question** - Question style
- **None** - No special styling

## ZidThemes Enumeration

- **None** - No theme applied
- **Default** - Navy blue theme
- **Information** - Blue informational theme
- **Success** - Green success theme
- **Warning** - Yellow/orange warning theme
- **Error** - Red error theme
- **Professional** - Professional gray theme
- **CodeProject** - CodeProject orange theme
- **BlackAndWhite** - Monochrome theme

## Best Practices

1. **Use ThemeManager as a component** - Add it to your form and set ParentForm and Theme properties
2. **Apply themes consistently** - Use the same theme across your application
3. **Leverage ZidGrid plugins** - Extend functionality without modifying core code
4. **Use ToastForm for notifications** - Non-intrusive user feedback
5. **ProcessingDialog for long operations** - Any operation taking more than 1 second
6. **Validate user input** - Use TextInputDialog validation modes
7. **Access GridView for configuration** - Use `grid.GridView` to access the underlying DataGridView
8. **Set DataSource correctly** - Use the DataSource property on dialogs, not Items

## Common Use Cases

- **Data Management Applications** - Display and manipulate datasets with ZidGrid
- **Business Applications** - Professional forms with consistent theming
- **Admin Tools** - User management and configuration interfaces
- **Reporting Tools** - Export data to Excel, CSV, or HTML
- **System Utilities** - Progress dialogs for long-running operations

## Related Projects

- **CommonCode.Files**: Data export functionality used by ZidGrid export features
- **CommonCode**: Core utilities and extensions
