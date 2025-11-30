# ProcessingDialog - Quick Reference Guide

## 🚀 Quick Start

### ✅ CORRECT - Recommended Pattern

```csharp
// Pattern 1: Background thread with using block (BEST)
private void btnProcess_Click(object sender, EventArgs e)
{
    Task.Run(() =>
    {
        using (var dialog = ProcessingDialogManager.Show(
            "Processing",
            "Please wait...",
            DialogStyle.Information))
        {
            // Do your work here
            for (int i = 0; i <= 100; i += 10)
            {
                Thread.Sleep(500);
                dialog.Update($"Step {i}%", i);
            }
        } // Auto-cleanup
    });
}
```

### ❌ WRONG - Never Do This

```csharp
// WRONG: Blocking main thread
private void btnProcess_Click(object sender, EventArgs e)
{
    using (var dialog = ProcessingDialogManager.Show(...))
    {
        Thread.Sleep(5000); // FREEZES UI!
    }
}
```

---

## 📋 Common Patterns

### Pattern 1: Simple Progress (Recommended)

```csharp
Task.Run(() =>
{
    using (var dialog = ProcessingDialogManager.Show("Title", "Message"))
    {
        for (int i = 0; i <= 100; i++)
        {
            DoWork(i);
            dialog.UpdateProgress(i);
        }
    }
});
```

### Pattern 2: Async/Await (Modern)

```csharp
private async void btnClick(object sender, EventArgs e)
{
    await Task.Run(() =>
    {
        using (var dialog = ProcessingDialogManager.Show("Title", "Message"))
        {
            PerformWork(dialog);
        }
    });
    MessageBox.Show("Done!");
}
```

### Pattern 3: Cancellable Operation

```csharp
private CancellationTokenSource _cts;

private void btnStart_Click(object sender, EventArgs e)
{
    _cts = new CancellationTokenSource();

    Task.Run(() =>
    {
        using (var dialog = ProcessingDialogManager.Show(...))
        {
            try
            {
                for (int i = 0; i <= 100; i++)
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    DoWork(i);
                    dialog.UpdateProgress(i);
                }
            }
            catch (OperationCanceledException)
            {
                dialog.UpdateMessage("Cancelled!");
            }
        }
    }, _cts.Token);
}

private void btnCancel_Click(object sender, EventArgs e)
{
    _cts?.Cancel();
}
```

### Pattern 4: Multiple Phases

```csharp
Task.Run(() =>
{
    using (var dialog = ProcessingDialogManager.Show("Multi-Phase", "Starting..."))
    {
        dialog.Update("Phase 1: Loading...", 0);
        LoadData();

        dialog.Update("Phase 2: Processing...", 33);
        ProcessData();

        dialog.Update("Phase 3: Saving...", 66);
        SaveResults();

        dialog.Update("Complete!", 100);
    }
});
```

---

## 🎨 Dialog Styles

```csharp
DialogStyle.Default       // Blue
DialogStyle.Information   // Blue (brighter)
DialogStyle.Success       // Green
DialogStyle.Warning       // Orange/Yellow
DialogStyle.Error         // Red
DialogStyle.Professional  // Dark Gray
```

---

## 📝 API Methods

### Show Dialog

```csharp
ProcessingDialogManager.Show(
    string title,              // Window title
    string message,            // Message text
    DialogStyle style,         // Color scheme (optional)
    Image image,               // Icon (optional)
    bool isIndeterminate       // Marquee mode (optional)
)
```

### Update Methods

```csharp
// Update message only
dialog.UpdateMessage("New message");

// Update progress only (0-100)
dialog.UpdateProgress(50);

// Update both
dialog.Update("Processing 50%", 50);

// Close dialog
dialog.Close();
```

---

## ⚙️ Progress Modes

### Indeterminate (Marquee)

Use when you don't know how long it will take:

```csharp
using (var dialog = ProcessingDialogManager.Show(
    "Please Wait",
    "Loading...",
    DialogStyle.Information,
    null,
    true))  // isIndeterminate = true
{
    PerformUnknownDurationWork();
}
```

### Determinate (Percentage)

Use when you know total work:

```csharp
using (var dialog = ProcessingDialogManager.Show(
    "Processing",
    "Working...",
    DialogStyle.Information,
    null,
    false)) // isIndeterminate = false (or call UpdateProgress)
{
    for (int i = 0; i <= 100; i++)
    {
        ProcessItem(i);
        dialog.UpdateProgress(i);
    }
}
```

---

## 💡 Best Practices

### ✅ DO

1. **Always use background threads**
   ```csharp
   Task.Run(() => { /* work with dialog */ });
   ```

2. **Use `using` for automatic cleanup**
   ```csharp
   using (var dialog = ...) { /* work */ }
   ```

3. **Update progress regularly**
   ```csharp
   for (int i = 0; i < count; i++)
   {
       if (i % 10 == 0) dialog.UpdateProgress(i * 100 / count);
   }
   ```

4. **Use meaningful messages**
   ```csharp
   dialog.UpdateMessage("Processing record 50 of 100...");
   ```

### ❌ DON'T

1. **Don't block main thread**
   ```csharp
   // ❌ WRONG
   using (var dialog = ...) { Thread.Sleep(5000); }
   ```

2. **Don't forget to dispose**
   ```csharp
   // ❌ WRONG - Memory leak
   var dialog = ProcessingDialogManager.Show(...);
   // Never closed!
   ```

3. **Don't update too frequently**
   ```csharp
   // ❌ WRONG - Too many updates
   for (int i = 0; i < 1000000; i++)
       dialog.UpdateProgress(i);

   // ✅ CORRECT - Batch updates
   for (int i = 0; i < 1000000; i++)
       if (i % 10000 == 0) dialog.UpdateProgress(i);
   ```

---

## 🧪 Testing

A comprehensive test form is available: **Form8.cs** in TesterWin project.

It includes 9 test scenarios:
1. ✅ Background Thread (Using Block) - **Recommended**
2. ✅ Background Thread (Manual Control)
3. ✅ Async/Await Pattern - **Modern Approach**
4. ✅ Cancellable Operation
5. ✅ Sequential Operations
6. ✅ Error Handling
7. ✅ Indeterminate Progress
8. ✅ Timer-Based Updates
9. ❌ Main Thread Blocked - **Anti-Pattern Demo**

Run Form8 to see all patterns in action!

---

## 🔍 Troubleshooting

### Problem: Dialog Freezes

**Cause**: Running work on main thread
**Solution**: Move work to background thread

```csharp
// ✅ CORRECT
Task.Run(() => {
    using (var dialog = ...) { /* work */ }
});
```

### Problem: Dialog Won't Close

**Cause**: Not disposing properly
**Solution**: Use `using` or explicit `Close()`

```csharp
// ✅ CORRECT
using (var dialog = ...) { /* work */ }
// or
dialog.Close();
```

### Problem: Progress Not Updating

**Cause**: Main thread blocked
**Solution**: Ensure work is on background thread

---

## 📚 Full Documentation

For complete documentation, see: `ProcessingDialog.Documentation.md`

---

## 🎯 Decision Tree

```
Is this a long operation (> 1 second)?
├─ Yes
│  ├─ Do you know total work?
│  │  ├─ Yes → Use determinate progress (UpdateProgress)
│  │  └─ No → Use indeterminate progress (marquee)
│  └─ Is it on background thread?
│     ├─ Yes → ✅ You're good!
│     └─ No → Move to Task.Run()
└─ No (< 1 second)
   └─ Skip dialog, too quick for user to see
```

---

## 📞 Quick Help

- **Test Examples**: Run Form8.cs in TesterWin
- **Full Docs**: ProcessingDialog.Documentation.md
- **Code Location**: CommonCode.Win\Forms\ProcessingDialog.cs
