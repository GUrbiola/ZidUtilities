# DataExporter Excel Styles Guide

## Overview
The `DataExporter` class now includes **14 professional Excel styles** for creating beautifully formatted spreadsheets. Each style features coordinated header, normal, and alternate row colors with matching borders.

## Available Styles

### 1. Default
**Classic professional look**
- Navy blue headers with white text
- White background for normal rows
- Light sky blue background for alternate rows
- Black borders on all cells

### 2. Simple
**Minimal and clean**
- White headers with black text and gray bottom border
- White background for normal rows
- White smoke background for alternate rows
- No borders except header bottom border

### 3. Ocean
**Professional and calming**
- Deep ocean blue headers (RGB: 0, 51, 102)
- White background for normal rows
- Light ocean blue for alternate rows (RGB: 204, 229, 255)
- Medium blue borders
- Best for: Financial reports, professional presentations

### 4. Forest
**Natural and balanced**
- Forest green headers (RGB: 34, 87, 44)
- White background for normal rows
- Light green for alternate rows (RGB: 220, 237, 200)
- Green borders
- Best for: Environmental data, sustainability reports

### 5. Sunset
**Energetic and modern**
- Sunset orange headers (RGB: 230, 92, 0)
- White background for normal rows
- Light peach for alternate rows (RGB: 255, 224, 178)
- Orange borders
- Best for: Marketing materials, creative projects

### 6. Monochrome
**Minimalist and elegant**
- Dark gray headers (RGB: 51, 51, 51)
- White background for normal rows
- Light gray for alternate rows (RGB: 245, 245, 245)
- Gray borders
- Best for: Print-ready documents, formal reports

### 7. Corporate
**Executive and professional**
- Corporate blue headers (RGB: 68, 114, 196)
- White background for normal rows
- Light blue-gray for alternate rows (RGB: 217, 225, 242)
- Blue borders
- Best for: Business reports, executive summaries

### 8. Mint
**Clean and modern**
- Mint green headers (RGB: 0, 176, 138)
- White background for normal rows
- Light mint for alternate rows (RGB: 198, 239, 206)
- Mint borders
- Best for: Health data, wellness reports

### 9. Lavender
**Sophisticated and refined**
- Lavender purple headers (RGB: 112, 48, 160)
- White background for normal rows
- Light lavender for alternate rows (RGB: 234, 221, 244)
- Purple borders
- Best for: Creative industries, design portfolios

### 10. Autumn
**Warm and earthy**
- Autumn brown headers (RGB: 140, 82, 37)
- White background for normal rows
- Light tan for alternate rows (RGB: 244, 224, 176)
- Brown/orange borders
- Best for: Seasonal reports, harvest data

### 11. Steel
**Modern and sleek**
- Steel gray headers (RGB: 96, 125, 139)
- White background for normal rows
- Light steel for alternate rows (RGB: 236, 239, 241)
- Dark gray borders
- Best for: Technical documentation, engineering reports

### 12. Cherry
**Bold and energetic**
- Cherry red headers (RGB: 192, 0, 0)
- White background for normal rows
- Light pink for alternate rows (RGB: 255, 205, 210)
- Dark red borders
- Best for: Urgent reports, attention-grabbing documents

### 13. Sky
**Airy and professional**
- Sky blue headers (RGB: 3, 155, 229)
- White background for normal rows
- Very light blue for alternate rows (RGB: 225, 245, 254)
- Blue borders
- Best for: Aviation data, weather reports

### 14. Charcoal
**Strong and professional**
- Charcoal headers (RGB: 38, 50, 56)
- White background for normal rows
- Light gray for alternate rows (RGB: 236, 239, 241)
- Black borders
- Best for: Dark theme documents, modern reports

## Usage Examples

### Basic Usage
```csharp
using CommonCode.Files;
using System.Data;

// Create sample data
var dt = new DataTable("Sales");
dt.Columns.Add("Product", typeof(string));
dt.Columns.Add("Revenue", typeof(decimal));
dt.Rows.Add("Widget A", 1500.00);
dt.Rows.Add("Widget B", 2300.50);

// Export with Ocean style
var exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    ExportExcelStyle = ExcelStyle.Ocean,
    ExportWithStyles = true,
    UseAlternateRowStyles = true
};
exporter.ExportToFile("sales_report.xlsx", dt);
```

### Advanced Configuration
```csharp
var exporter = new DataExporter
{
    ExportType = ExportTo.XLSX,
    ExportExcelStyle = ExcelStyle.Corporate, // Choose your style
    ExportWithStyles = true,                 // Enable styling
    UseAlternateRowStyles = true,            // Enable alternating row colors
    WriteHeaders = true,                     // Include headers
    AutoCellAdjust = WidthAdjust.ByAllRows,  // Auto-adjust column widths

    // Optional Excel metadata
    Author = "Your Name",
    Company = "Your Company",
    Title = "Quarterly Report",
    Subject = "Q1 2024 Sales Data"
};

// Optionally ignore specific columns
exporter.IgnoredColumns.Add("InternalID");

exporter.ExportToFile("quarterly_report.xlsx", dataSet);
```

### Comparing Styles
To see all styles in action, run the Excel Styles Demo:

1. Build the solution
2. Run `Tester.exe`
3. Select option `4` - Excel Styles Demo
4. Open the generated files in `StyleDemos` folder

## Style Selection Guide

| Use Case | Recommended Styles |
|----------|-------------------|
| **Financial Reports** | Ocean, Corporate, Default |
| **Marketing Materials** | Sunset, Cherry, Lavender |
| **Environmental Data** | Forest, Mint |
| **Technical Documentation** | Steel, Charcoal, Monochrome |
| **Creative Projects** | Lavender, Sky, Sunset |
| **Executive Presentations** | Corporate, Default, Charcoal |
| **Print Documents** | Monochrome, Simple |
| **Healthcare** | Mint, Sky |
| **Seasonal Reports** | Autumn, Forest |

## Properties Reference

### ExcelStyle (Enum)
- `Default` - Classic navy blue theme
- `Simple` - Minimal styling
- `Ocean` - Deep blue ocean
- `Forest` - Forest green
- `Sunset` - Warm orange
- `Monochrome` - Black and white
- `Corporate` - Corporate blue
- `Mint` - Fresh mint green
- `Lavender` - Elegant purple
- `Autumn` - Autumn brown/orange
- `Steel` - Steel gray
- `Cherry` - Cherry red
- `Sky` - Light sky blue
- `Charcoal` - Dark charcoal

### DataExporter Properties
- `ExportType`: XLSX, CSV, or TXT
- `ExportExcelStyle`: The style to apply (XLSX only)
- `ExportWithStyles`: Enable/disable styling (default: true)
- `UseAlternateRowStyles`: Enable/disable alternating rows (default: true)
- `WriteHeaders`: Include header row (default: true)
- `AutoCellAdjust`: Column width adjustment strategy
- `Author`, `Company`, `Subject`, `Title`: Excel metadata

## Color Accessibility

All styles have been designed with readability in mind:
- Headers use sufficient contrast (white text on dark backgrounds)
- Alternate row colors are subtle but distinguishable
- Border colors complement the overall theme
- Print-friendly options available (Monochrome, Simple)

## Testing

Run the comprehensive test suite to verify all styles work correctly:

```bash
Tester.exe
# Select option 2: DataExporter/DataImporter Tests
# All 33 tests should pass including style tests
```

## Tips

1. **Match your brand**: Choose a style that matches your company colors
2. **Consider your audience**: Use professional styles (Corporate, Default) for executives
3. **Print considerations**: Use Monochrome or Simple for documents that will be printed
4. **Accessibility**: Ensure color choices work for colorblind users
5. **Consistency**: Use the same style across related documents

## Demo Files

The Excel Styles Demo generates 14 sample Excel files in `StyleDemos/` folder:
- Each file uses a different style
- Sample employee data with 8 records
- Shows headers, normal rows, and alternate rows
- Demonstrates auto-column width adjustment

Perfect for comparing styles side-by-side!

---

**Version**: 2.0
**Last Updated**: November 2024
**Maintainer**: ZidUtilities Team
