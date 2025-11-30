# ZidGrid Header Context Menu

## Overview
The ZidGrid control now supports customizable context menus that appear when right-clicking on column headers. The menu system supports two types of menu items:

1. **Plugin-based menu items** - Dynamic menu options provided by plugins implementing `IZidGridPlugin`
2. **Designer-configurable menu items** - Static menu options configured in Visual Studio's designer

## Features

- Right-click on any column header to display the context menu
- Plugins appear at the top of the menu
- Custom menu items appear at the bottom
- Automatic separator between sections (only when both exist)
- Menu is hidden if no items are available
- Full access to DataGridView and DataSource from menu handlers

## Architecture

### Plugin System

#### IZidGridPlugin Interface
```csharp
public interface IZidGridPlugin
{
    string MenuText { get; }        // Display text for the menu item
    Image MenuImage { get; }        // Optional icon
    bool Enabled { get; }           // Enable/disable the menu item
    void Execute(ZidGridPluginContext context);  // Handler
}
```

#### ZidGridPluginContext
The context object passed to plugins contains:
- `DataGridView` - The DataGridView control within ZidGrid
- `DataSource` - The current data source
- `ColumnIndex` - Index of the column where menu was triggered
- `Column` - The DataGridViewColumn where menu was triggered

#### Creating a Plugin

1. Implement the `IZidGridPlugin` interface:
```csharp
public class MyCustomPlugin : IZidGridPlugin
{
    public string MenuText => "My Custom Action";
    public Image MenuImage => null; // or load an icon
    public bool Enabled => true;

    public void Execute(ZidGridPluginContext context)
    {
        // Access the grid
        var grid = context.DataGridView;
        var dataSource = context.DataSource;
        var column = context.Column;

        // Implement your custom functionality
        MessageBox.Show($"Column: {column.HeaderText}");
    }
}
```

2. Add the plugin to ZidGrid:
```csharp
zidGrid1.Plugins.Add(new MyCustomPlugin());
```

### Designer-Configurable Menu Items

#### Using the Designer

1. Select the ZidGrid control in the designer
2. Find the `CustomMenuItems` property in the Properties window
3. Click the ellipsis (...) button to open the collection editor
4. Add new menu items and configure:
   - **Name** - Unique identifier for the menu item
   - **Text** - Display text in the menu
   - **Image** - Optional icon
   - **Enabled** - Whether the item is enabled

5. Handle the `Click` event in code:
```csharp
private void InitializeComponent()
{
    // Designer generated code...

    // Add event handler
    this.customMenuItem1.Click += CustomMenuItem1_Click;
}

private void CustomMenuItem1_Click(object sender, ZidGridMenuItemClickEventArgs e)
{
    // Access event arguments
    var menuItem = e.MenuItem;
    var grid = e.DataGridView;
    var dataSource = e.DataSource;
    var column = e.Column;

    // Implement functionality
    e.DataGridView.Sort(e.Column, ListSortDirection.Ascending);
}
```

#### Programmatic Configuration

You can also add custom menu items programmatically:
```csharp
var sortMenuItem = new ZidGridMenuItem
{
    Name = "sortAsc",
    Text = "Sort Ascending",
    Enabled = true
};
sortMenuItem.Click += (sender, e) =>
{
    e.DataGridView.Sort(e.Column, ListSortDirection.Ascending);
};
zidGrid1.CustomMenuItems.Add(sortMenuItem);
```

## Menu Structure

When right-clicking on a column header, the menu is structured as:

```
┌─────────────────────────┐
│ Plugin Item 1           │  ← Plugin section (top)
│ Plugin Item 2           │
├─────────────────────────┤  ← Separator (only if both sections exist)
│ Custom Item 1           │  ← Custom section (bottom)
│ Custom Item 2           │
└─────────────────────────┘
```

- If only plugins exist: Only plugin items are shown
- If only custom items exist: Only custom items are shown
- If both exist: Both sections with separator
- If neither exist: Menu is not displayed

## Sample Plugin: ExportToCSVPlugin

A sample plugin is included that demonstrates exporting grid data to CSV:

```csharp
using ZidUtilities.CommonCode.Win.Controls.Grid.SamplePlugins;

// Add the plugin
zidGrid1.Plugins.Add(new ExportToCSVPlugin());
```

This plugin:
- Exports all visible columns
- Handles CSV escaping properly
- Shows a save file dialog
- Provides user feedback

## Test Application

A complete test application is available in:
- **File**: `TesterWin/FormZidGridMenuTest.cs`

To run the test:
1. Build the solution
2. Run TesterWin project
3. Open FormZidGridMenuTest
4. Right-click on any column header to see:
   - Export to CSV (plugin)
   - Sort Ascending (custom)
   - Sort Descending (custom)
   - Hide Column (custom)

## Best Practices

### For Plugins

1. **Keep Execute method fast** - Long operations should show progress indicators
2. **Handle exceptions** - Show user-friendly error messages
3. **Validate context** - Check that DataGridView and Column are not null
4. **Use descriptive MenuText** - Users should understand what the action does
5. **Consider Enabled state** - Disable if action is not currently valid

### For Custom Menu Items

1. **Use meaningful Names** - Makes event handlers easier to identify
2. **Provide event handlers** - Always handle the Click event
3. **Access context through event args** - Use ZidGridMenuItemClickEventArgs
4. **Consider user feedback** - Show messages or visual feedback for actions

## Common Use Cases

### Plugin Examples
- Export functionality (CSV, Excel, JSON)
- Data analysis tools
- Custom filtering options
- Column management utilities
- Integration with external systems

### Custom Menu Item Examples
- Sorting operations
- Column visibility toggles
- Format changes
- Custom filters
- Quick actions specific to your application

## API Reference

### ZidGrid Properties

| Property | Type | Description |
|----------|------|-------------|
| `CustomMenuItems` | `ZidGridMenuItemCollection` | Collection of custom menu items (designer-configurable) |
| `Plugins` | `ZidGridPluginCollection` | Collection of plugin instances |

### ZidGridMenuItem Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Unique identifier |
| `Text` | `string` | Display text |
| `Image` | `Image` | Optional icon |
| `Enabled` | `bool` | Enabled state |

### ZidGridMenuItem Events

| Event | Type | Description |
|-------|------|-------------|
| `Click` | `EventHandler<ZidGridMenuItemClickEventArgs>` | Fires when clicked |

### ZidGridMenuItemClickEventArgs Properties

| Property | Type | Description |
|----------|------|-------------|
| `MenuItem` | `ZidGridMenuItem` | The clicked menu item |
| `ColumnIndex` | `int` | Index of the column |
| `Column` | `DataGridViewColumn` | The column object |
| `DataGridView` | `DataGridView` | The grid control |
| `DataSource` | `object` | Current data source |
