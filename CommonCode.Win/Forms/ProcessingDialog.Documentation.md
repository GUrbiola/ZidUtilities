# ProcessingDialog - Comprehensive Documentation

## Table of Contents
1. [Overview](#overview)
2. [Key Concepts](#key-concepts)
3. [Architecture](#architecture)
4. [Usage Patterns](#usage-patterns)
5. [Best Practices](#best-practices)
6. [Common Pitfalls](#common-pitfalls)
7. [API Reference](#api-reference)
8. [Examples](#examples)

---

## Overview

**ProcessingDialog** is a thread-safe dialog form designed to display background processing operations with progress indicators. It runs on a separate UI thread, allowing your main application thread to remain responsive while showing progress to the user.

### Key Features

- ✅ **Thread-Safe**: Can be safely updated from any thread
- ✅ **Responsive**: Runs on its own UI thread, never blocks your main thread
- ✅ **Flexible**: Supports both indeterminate (marquee) and determinate (percentage) progress modes
- ✅ **Easy to Use**: Simple API with helper class `ProcessingDialogManager`
- ✅ **Customizable**: Supports multiple dialog styles and custom images
- ✅ **Safe**: Prevents user from closing dialog during critical operations

---

## Key Concepts

### 1. **Separate UI Thread**

The ProcessingDialog runs on **its own dedicated UI thread**, separate from your main application thread. This is critical because:

- Your main thread can continue processing without blocking
- The dialog remains responsive even when your main thread is busy
- Updates to the dialog are thread-safe and non-blocking

### 2. **Thread-Safe Updates**

All update methods (`UpdateMessage()`, `UpdateProgress()`, `Update()`) are **thread-safe**. You can call them from:
- Background threads (`Task.Run`, `ThreadPool`, manual threads)
- Main UI thread (Timer callbacks, event handlers)
- Any other thread in your application

The dialog automatically handles the thread marshaling internally using `InvokeRequired` checks.

### 3. **ProcessingDialogManager**

The **ProcessingDialogManager** is a helper class that simplifies the lifecycle management of ProcessingDialog:

- Creates and shows the dialog on a background UI thread
- Provides easy-to-use update methods
- Handles cleanup automatically when disposed
- Implements `IDisposable` for use with `using` statements

---

## Architecture

```
┌─────────────────────────────────────────┐
│        Your Application Thread         │
│  (Main Thread or Background Thread)    │
└─────────────────┬───────────────────────┘
                  │
                  │ ProcessingDialogManager.Show()
                  │
                  ▼
┌─────────────────────────────────────────┐
│    ProcessingDialogManager              │
│  ┌───────────────────────────────────┐  │
│  │  Creates Dedicated UI Thread      │  │
│  └───────────────┬───────────────────┘  │
└──────────────────┼──────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────┐
│      Dedicated Dialog UI Thread         │
│  ┌───────────────────────────────────┐  │
│  │     ProcessingDialog Form         │  │
│  │  - Progress Bar                   │  │
│  │  - Message Label                  │  │
│  │  - Style/Image                    │  │
│  └───────────────────────────────────┘  │
│         Own Message Pump                │
└─────────────────────────────────────────┘
                   ▲
                   │
                   │ Thread-Safe Updates
                   │ (UpdateMessage, UpdateProgress)
                   │
┌─────────────────────────────────────────┐
│     Your Work Thread(s)                 │
│  - Processing data                      │
│  - Calling dialog.Update()              │
│  - Completing tasks                     │
└─────────────────────────────────────────┘
```

---

## Usage Patterns

### Pattern 1: Background Thread with Using Block (RECOMMENDED)

**Best for**: Most scenarios where you have a discrete task to perform.

```csharp
// Start work on background thread
Task.Run(() =>
{
    // Show dialog (runs on its own UI thread)
    using (var dialog = ProcessingDialogManager.Show(
        "Processing",
        "Please wait...",
        DialogStyle.Information))
    {
        // Do your work here
        for (int i = 0; i <= 100; i += 10)
        {
            Thread.Sleep(500); // Simulate work

            // Update progress (thread-safe)
            dialog.Update($"Processing step {i}%", i);
        }

        // Dialog automatically closes when using block exits
    }

    // Update main UI when done
    this.Invoke(() => MessageBox.Show("Done!"));
});
```

**Advantages**:
- ✅ Clean, simple code
- ✅ Automatic cleanup
- ✅ Main thread stays responsive
- ✅ Dialog is responsive

---

### Pattern 2: Async/Await (MODERN)

**Best for**: Modern codebases using async/await patterns.

```csharp
private async void btnProcess_Click(object sender, EventArgs e)
{
    btnProcess.Enabled = false;

    try
    {
        await Task.Run(() =>
        {
            using (var dialog = ProcessingDialogManager.Show(
                "Async Processing",
                "Working asynchronously...",
                DialogStyle.Success))
            {
                // Your async work here
                PerformWork(dialog);
            }
        });

        MessageBox.Show("Completed!");
    }
    finally
    {
        btnProcess.Enabled = true;
    }
}

private void PerformWork(ProcessingDialogManager dialog)
{
    for (int i = 0; i <= 100; i += 10)
    {
        Thread.Sleep(500);
        dialog.Update($"Step {i}%", i);
    }
}
```

**Advantages**:
- ✅ Modern C# patterns
- ✅ Clean error handling
- ✅ Easy to understand control flow
- ✅ Proper button state management

---

### Pattern 3: Manual Management

**Best for**: Long-running operations where you need to keep a reference to the dialog.

```csharp
private ProcessingDialogManager _dialog;

private void btnStart_Click(object sender, EventArgs e)
{
    Task.Run(() =>
    {
        _dialog = ProcessingDialogManager.Show(
            "Long Process",
            "Starting...",
            DialogStyle.Professional);

        try
        {
            PerformLongRunningWork();
        }
        finally
        {
            _dialog?.Close();
            _dialog = null;
        }
    });
}

private void PerformLongRunningWork()
{
    _dialog.UpdateMessage("Phase 1...");
    Thread.Sleep(2000);

    _dialog.UpdateMessage("Phase 2...");
    _dialog.UpdateProgress(50);
    Thread.Sleep(2000);

    _dialog.Update("Finalizing...", 100);
    Thread.Sleep(1000);
}
```

**Advantages**:
- ✅ Full control over dialog lifetime
- ✅ Can update from multiple methods
- ✅ Can be cancelled externally

---

### Pattern 4: Cancellable Operations

**Best for**: Operations that user should be able to cancel.

```csharp
private CancellationTokenSource _cts;

private void btnStartCancellable_Click(object sender, EventArgs e)
{
    _cts = new CancellationTokenSource();
    btnCancel.Enabled = true;

    Task.Run(() =>
    {
        using (var dialog = ProcessingDialogManager.Show(
            "Cancellable",
            "Press Cancel to stop...",
            DialogStyle.Warning))
        {
            try
            {
                for (int i = 0; i <= 100; i++)
                {
                    _cts.Token.ThrowIfCancellationRequested();

                    Thread.Sleep(100);
                    dialog.UpdateProgress(i);
                }
            }
            catch (OperationCanceledException)
            {
                dialog.UpdateMessage("Cancelled!");
                Thread.Sleep(1000);
            }
        }

        this.Invoke(() => btnCancel.Enabled = false);
    }, _cts.Token);
}

private void btnCancel_Click(object sender, EventArgs e)
{
    _cts?.Cancel();
}
```

---

### Pattern 5: Timer-Based Updates (Main Thread)

**Best for**: When you're using a Timer and want to show progress.

```csharp
private ProcessingDialogManager _dialog;
private Timer _timer;
private int _progress;

private void btnTimerBased_Click(object sender, EventArgs e)
{
    _progress = 0;

    // Show dialog on background thread
    Task.Run(() =>
    {
        _dialog = ProcessingDialogManager.Show(
            "Timer Progress",
            "Processing...",
            DialogStyle.Information);
    });

    // Update from main thread using Timer
    _timer = new Timer();
    _timer.Interval = 200;
    _timer.Tick += (s, e) =>
    {
        _progress += 5;

        if (_progress <= 100)
        {
            _dialog?.UpdateProgress(_progress);
        }
        else
        {
            _timer.Stop();
            _dialog?.Close();
            _dialog = null;
        }
    };
    _timer.Start();
}
```

---

## Best Practices

### ✅ DO

1. **Always run long operations on background threads**
   ```csharp
   Task.Run(() => {
       using (var dialog = ProcessingDialogManager.Show(...)) {
           // Work here
       }
   });
   ```

2. **Use `using` statements for automatic cleanup**
   ```csharp
   using (var dialog = ProcessingDialogManager.Show(...)) {
       // Work
   } // Automatically disposed
   ```

3. **Update progress regularly to show user feedback**
   ```csharp
   for (int i = 0; i <= 100; i++) {
       ProcessItem(i);
       dialog.UpdateProgress(i);
   }
   ```

4. **Use meaningful messages**
   ```csharp
   dialog.UpdateMessage("Loading records from database...");
   dialog.UpdateMessage("Processing 1,234 records...");
   dialog.UpdateMessage("Saving results...");
   ```

5. **Choose appropriate progress mode**
   - Use **determinate** (percentage) when you know total work
   - Use **indeterminate** (marquee) when duration is unknown

6. **Handle errors gracefully**
   ```csharp
   try {
       using (var dialog = ProcessingDialogManager.Show(...)) {
           PerformWork();
       }
   } catch (Exception ex) {
       MessageBox.Show($"Error: {ex.Message}");
   }
   ```

---

### ❌ DON'T

1. **DON'T block the main thread**
   ```csharp
   // ❌ WRONG - This freezes the UI
   using (var dialog = ProcessingDialogManager.Show(...)) {
       Thread.Sleep(5000); // Blocks main thread!
   }
   ```

2. **DON'T forget to dispose**
   ```csharp
   // ❌ WRONG - Memory leak
   var dialog = ProcessingDialogManager.Show(...);
   PerformWork();
   // Forgot to close/dispose!
   ```

3. **DON'T update too frequently**
   ```csharp
   // ❌ WRONG - Unnecessary overhead
   for (int i = 0; i < 1000000; i++) {
       ProcessItem(i);
       dialog.UpdateProgress(i / 10000.0); // Updates 1M times!
   }

   // ✅ CORRECT - Batch updates
   for (int i = 0; i < 1000000; i++) {
       ProcessItem(i);
       if (i % 10000 == 0) {
           dialog.UpdateProgress(i / 10000.0);
       }
   }
   ```

4. **DON'T ignore thread safety**
   ```csharp
   // ✅ CORRECT - ProcessingDialog handles this automatically
   Task.Run(() => dialog.UpdateMessage("From background"));
   ```

---

## Common Pitfalls

### Pitfall 1: Blocking Main Thread

**Problem**: Running work on the main thread freezes both your app and the dialog.

```csharp
// ❌ WRONG
private void btnProcess_Click(object sender, EventArgs e)
{
    using (var dialog = ProcessingDialogManager.Show(...)) {
        Thread.Sleep(5000); // FREEZES EVERYTHING!
    }
}
```

**Solution**: Always use background threads.

```csharp
// ✅ CORRECT
private void btnProcess_Click(object sender, EventArgs e)
{
    Task.Run(() => {
        using (var dialog = ProcessingDialogManager.Show(...)) {
            Thread.Sleep(5000); // Only blocks this thread
        }
    });
}
```

---

### Pitfall 2: Forgetting to Clean Up

**Problem**: Not disposing the dialog causes it to stay open or leak resources.

```csharp
// ❌ WRONG
var dialog = ProcessingDialogManager.Show(...);
PerformWork();
// Dialog never closes!
```

**Solution**: Use `using` or explicit `Close()`.

```csharp
// ✅ CORRECT
using (var dialog = ProcessingDialogManager.Show(...)) {
    PerformWork();
} // Automatically disposed
```

---

### Pitfall 3: Updating UI Without Invoke

**Problem**: Trying to update main UI from background thread without proper marshaling.

```csharp
// ❌ WRONG
Task.Run(() => {
    using (var dialog = ProcessingDialogManager.Show(...)) {
        PerformWork();
        lblStatus.Text = "Done!"; // CRASH! Wrong thread!
    }
});
```

**Solution**: Use `Invoke` for main UI updates.

```csharp
// ✅ CORRECT
Task.Run(() => {
    using (var dialog = ProcessingDialogManager.Show(...)) {
        PerformWork();
    }

    this.Invoke(() => lblStatus.Text = "Done!");
});
```

---

## API Reference

### ProcessingDialogManager Class

#### Constructor

```csharp
ProcessingDialogManager(
    string title,
    string message,
    DialogStyle style = DialogStyle.Default,
    Image image = null,
    bool isIndeterminate = true
)
```

**Parameters**:
- `title`: Dialog window title
- `message`: Initial message to display
- `style`: Color scheme (Default, Information, Success, Warning, Error, Professional)
- `image`: Optional image to display
- `isIndeterminate`: True for marquee progress, false for percentage

---

#### Methods

##### `void UpdateMessage(string message)`
Updates the message text. Thread-safe.

```csharp
dialog.UpdateMessage("Loading data...");
```

---

##### `void UpdateProgress(int percentage)`
Updates progress bar to specific percentage (0-100). Thread-safe. Automatically switches to determinate mode.

```csharp
dialog.UpdateProgress(75); // Shows 75%
```

---

##### `void Update(string message, int percentage)`
Updates both message and progress. Thread-safe.

```csharp
dialog.Update("Processing item 50 of 100...", 50);
```

---

##### `void Close()`
Closes and disposes the dialog. Thread-safe.

```csharp
dialog.Close();
```

---

##### `void Dispose()`
Implements IDisposable. Closes dialog and releases resources.

```csharp
using (var dialog = ProcessingDialogManager.Show(...)) {
    // Work
} // Dispose called automatically
```

---

#### Static Methods

##### `ProcessingDialogManager Show(...)`
Factory method to create and show a new dialog.

```csharp
var dialog = ProcessingDialogManager.Show(
    "Processing",
    "Please wait...",
    DialogStyle.Information
);
```

---

### ProcessingDialog Class

Direct usage is not recommended. Use `ProcessingDialogManager` instead.

#### Properties

- `string DialogTitle { get; set; }` - Thread-safe
- `string Message { get; set; }` - Thread-safe
- `DialogStyle Style { get; set; }` - Thread-safe
- `Image DialogImage { get; set; }` - Thread-safe
- `bool IsIndeterminate { get; set; }` - Thread-safe
- `int Progress { get; set; }` - Thread-safe (0-100)

---

## Examples

### Example 1: Simple File Processing

```csharp
private async void btnProcessFiles_Click(object sender, EventArgs e)
{
    var files = Directory.GetFiles(@"C:\Data", "*.txt");
    btnProcessFiles.Enabled = false;

    try
    {
        await Task.Run(() =>
        {
            using (var dialog = ProcessingDialogManager.Show(
                "Processing Files",
                "Starting...",
                DialogStyle.Information))
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileName(files[i]);
                    dialog.Update(
                        $"Processing {fileName} ({i + 1} of {files.Length})",
                        (int)((i + 1) / (double)files.Length * 100)
                    );

                    ProcessFile(files[i]);
                }
            }
        });

        MessageBox.Show($"Processed {files.Length} files successfully!");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    finally
    {
        btnProcessFiles.Enabled = true;
    }
}
```

---

### Example 2: Database Operation with Stages

```csharp
private async void btnDatabaseSync_Click(object sender, EventArgs e)
{
    await Task.Run(() =>
    {
        using (var dialog = ProcessingDialogManager.Show(
            "Database Synchronization",
            "Connecting to database...",
            DialogStyle.Professional))
        {
            // Stage 1: Connect
            ConnectToDatabase();
            dialog.Update("Fetching records...", 25);

            // Stage 2: Fetch
            var records = FetchRecords();
            dialog.Update($"Processing {records.Count} records...", 50);

            // Stage 3: Process
            ProcessRecords(records, dialog);
            dialog.Update("Saving changes...", 90);

            // Stage 4: Save
            SaveChanges();
            dialog.Update("Complete!", 100);

            Thread.Sleep(500); // Brief pause to show 100%
        }
    });
}

private void ProcessRecords(List<Record> records, ProcessingDialogManager dialog)
{
    for (int i = 0; i < records.Count; i++)
    {
        ProcessRecord(records[i]);

        // Update every 10 records to avoid too many updates
        if (i % 10 == 0 || i == records.Count - 1)
        {
            int progress = 50 + (int)((i + 1) / (double)records.Count * 40);
            dialog.UpdateProgress(progress);
        }
    }
}
```

---

### Example 3: Web Download with Cancellation

```csharp
private CancellationTokenSource _downloadCts;

private async void btnDownload_Click(object sender, EventArgs e)
{
    _downloadCts = new CancellationTokenSource();
    btnCancelDownload.Enabled = true;
    btnDownload.Enabled = false;

    try
    {
        await DownloadWithProgressAsync(_downloadCts.Token);
        MessageBox.Show("Download completed!");
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("Download cancelled.");
    }
    finally
    {
        btnCancelDownload.Enabled = false;
        btnDownload.Enabled = true;
    }
}

private async Task DownloadWithProgressAsync(CancellationToken token)
{
    await Task.Run(() =>
    {
        using (var dialog = ProcessingDialogManager.Show(
            "Downloading",
            "Starting download...",
            DialogStyle.Information))
        {
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) =>
                {
                    token.ThrowIfCancellationRequested();
                    dialog.Update(
                        $"Downloading... {e.BytesReceived / 1024}KB of {e.TotalBytesToReceive / 1024}KB",
                        e.ProgressPercentage
                    );
                };

                client.DownloadFileTaskAsync(
                    "https://example.com/file.zip",
                    "file.zip"
                ).Wait(token);
            }
        }
    }, token);
}

private void btnCancelDownload_Click(object sender, EventArgs e)
{
    _downloadCts?.Cancel();
}
```

---

## Summary

### When to Use ProcessingDialog

✅ **Use ProcessingDialog when**:
- You have a long-running operation (> 1 second)
- You want to keep the UI responsive
- You want to show progress to the user
- You need thread-safe progress updates
- You want to prevent user interaction during critical operations

❌ **Don't use ProcessingDialog when**:
- Operation is very quick (< 1 second) - users won't even see it
- You need user input during the process - use a custom dialog instead
- You want user to be able to interact with main UI - use a modeless approach

### Quick Decision Tree

```
Need to show progress?
├─ Yes
│  ├─ Operation on background thread?
│  │  ├─ Yes → Use ProcessingDialogManager ✅
│  │  └─ No → Move to background thread, then use ProcessingDialogManager
│  └─ Operation very quick (< 1 second)?
│     └─ Yes → Skip dialog, show status in UI instead
└─ No
   └─ Consider Progress Bar in status bar or other non-modal indicator
```

---

## Support and Issues

For questions, issues, or feature requests related to ProcessingDialog, please refer to the test form `Form8.cs` which demonstrates all usage scenarios.

**Test Form Location**: `ZidUtilities.TesterWin.Form8`

The test form includes 9 different scenarios demonstrating proper and improper usage patterns.
