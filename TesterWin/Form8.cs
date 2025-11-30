using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Forms;

namespace ZidUtilities.TesterWin
{
    /// <summary>
    /// Comprehensive test form for ProcessingDialog with various usage scenarios.
    /// Demonstrates proper usage from main thread and background threads.
    /// </summary>
    public partial class Form8 : Form
    {
        private ProcessingDialogManager _currentDialog = null;

        public Form8()
        {
            InitializeComponent();
        }

        #region Scenario 1: Background Thread (Recommended)

        /// <summary>
        /// RECOMMENDED APPROACH: Processing dialog on separate thread while main UI remains responsive.
        /// This is the ideal way to use ProcessingDialog.
        /// </summary>
        private void btnBackgroundThread_Click(object sender, EventArgs e)
        {
            AppendLog("Starting background thread scenario...");

            // Run the actual work on a background thread
            Task.Run(() =>
            {
                // Show the processing dialog (it runs on its own UI thread)
                using (var dialog = ProcessingDialogManager.Show(
                    "Processing",
                    "Starting background process...",
                    DialogStyle.Information))
                {
                    try
                    {
                        // Simulate work with progress updates
                        for (int i = 0; i <= 100; i += 10)
                        {
                            Thread.Sleep(500); // Simulate work

                            // Update the dialog (thread-safe)
                            dialog.Update($"Processing step {i / 10 + 1} of 11...", i);
                        }

                        Thread.Sleep(500);
                        dialog.UpdateMessage("Finalizing...");
                        Thread.Sleep(500);

                        // Update UI on main thread when done
                        this.Invoke(new Action(() =>
                        {
                            AppendLog("Background thread scenario completed successfully!");
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() =>
                        {
                            AppendLog($"Error: {ex.Message}");
                        }));
                    }
                    // Dialog automatically closes when using block exits
                }
            });

            AppendLog("Main thread is free! You can click buttons during processing.");
        }

        #endregion

        #region Scenario 2: Background Thread with Manual Control

        /// <summary>
        /// Background thread with manual dialog management.
        /// Allows keeping dialog reference for later updates.
        /// </summary>
        private void btnBackgroundManual_Click(object sender, EventArgs e)
        {
            AppendLog("Starting background thread with manual control...");

            Task.Run(() =>
            {
                // Create dialog and keep reference
                _currentDialog = ProcessingDialogManager.Show(
                    "Long Running Process",
                    "Initializing...",
                    DialogStyle.Professional);

                try
                {
                    Thread.Sleep(1000);
                    _currentDialog.UpdateMessage("Loading data...");
                    Thread.Sleep(1000);

                    _currentDialog.UpdateMessage("Processing data...");
                    _currentDialog.UpdateProgress(0);

                    for (int i = 0; i <= 100; i += 5)
                    {
                        Thread.Sleep(200);
                        _currentDialog.UpdateProgress(i);
                    }

                    _currentDialog.UpdateMessage("Saving results...");
                    Thread.Sleep(1000);

                    this.Invoke(new Action(() =>
                    {
                        AppendLog("Background thread with manual control completed!");
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        AppendLog($"Error: {ex.Message}");
                    }));
                }
                finally
                {
                    // Clean up
                    _currentDialog?.Close();
                    _currentDialog = null;
                }
            });

            AppendLog("Dialog launched. Main thread is responsive!");
        }

        #endregion

        #region Scenario 3: Main Thread Blocked (ANTI-PATTERN - DO NOT USE)

        /// <summary>
        /// ANTI-PATTERN: Shows what happens when you block the main thread.
        /// This demonstrates why you should NOT do long-running work on the main thread.
        /// The dialog will appear frozen because it can't process messages.
        /// </summary>
        private void btnMainThreadBlocked_Click(object sender, EventArgs e)
        {
            AppendLog("WARNING: Starting main thread blocked scenario (ANTI-PATTERN)...");
            AppendLog("The form will freeze. This demonstrates what NOT to do!");

            // This is WRONG - never do this!
            using (var dialog = ProcessingDialogManager.Show(
                "Bad Example",
                "This dialog will be frozen...",
                DialogStyle.Warning))
            {
                // Blocking the main thread - BAD!
                for (int i = 0; i <= 100; i += 10)
                {
                    Thread.Sleep(500); // This blocks the main thread

                    // These updates won't be visible because main thread is blocked
                    dialog.Update($"Step {i / 10 + 1}...", i);
                }
            }

            AppendLog("Main thread blocked scenario finished (notice it was frozen).");
        }

        #endregion

        #region Scenario 4: Async/Await Pattern (Modern Approach)

        /// <summary>
        /// RECOMMENDED: Using async/await pattern for clean, modern code.
        /// This keeps the UI responsive while waiting for work to complete.
        /// </summary>
        private async void btnAsyncAwait_Click(object sender, EventArgs e)
        {
            AppendLog("Starting async/await scenario...");

            // Disable button to prevent multiple clicks
            btnAsyncAwait.Enabled = false;

            try
            {
                await Task.Run(() =>
                {
                    using (var dialog = ProcessingDialogManager.Show(
                        "Async Processing",
                        "Processing asynchronously...",
                        DialogStyle.Success))
                    {
                        // Simulate async work
                        for (int i = 0; i <= 100; i += 10)
                        {
                            Thread.Sleep(400);
                            dialog.Update($"Async step {i}%", i);
                        }
                    }
                });

                AppendLog("Async/await scenario completed!");
            }
            catch (Exception ex)
            {
                AppendLog($"Error: {ex.Message}");
            }
            finally
            {
                btnAsyncAwait.Enabled = true;
            }

            AppendLog("Notice the UI stayed responsive throughout!");
        }

        #endregion

        #region Scenario 5: Cancellable Operation

        /// <summary>
        /// Demonstrates a cancellable long-running operation.
        /// Shows how to integrate CancellationToken with ProcessingDialog.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        private void btnCancellable_Click(object sender, EventArgs e)
        {
            AppendLog("Starting cancellable operation...");
            AppendLog("Click 'Cancel Operation' button to cancel.");

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            btnCancelOperation.Enabled = true;

            Task.Run(() =>
            {
                using (var dialog = ProcessingDialogManager.Show(
                    "Cancellable Process",
                    "Processing... (can be cancelled)",
                    DialogStyle.Information))
                {
                    try
                    {
                        for (int i = 0; i <= 100; i += 2)
                        {
                            // Check for cancellation
                            token.ThrowIfCancellationRequested();

                            Thread.Sleep(100);
                            dialog.Update($"Processing step {i}%...", i);
                        }

                        this.Invoke(new Action(() =>
                        {
                            AppendLog("Cancellable operation completed successfully!");
                            btnCancelOperation.Enabled = false;
                        }));
                    }
                    catch (OperationCanceledException)
                    {
                        dialog.UpdateMessage("Operation cancelled by user.");
                        Thread.Sleep(1000);

                        this.Invoke(new Action(() =>
                        {
                            AppendLog("Operation was cancelled.");
                            btnCancelOperation.Enabled = false;
                        }));
                    }
                }
            }, token);
        }

        private void btnCancelOperation_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            AppendLog("Cancellation requested...");
            btnCancelOperation.Enabled = false;
        }

        #endregion

        #region Scenario 6: Multiple Sequential Operations

        /// <summary>
        /// Shows how to handle multiple sequential operations with one dialog.
        /// </summary>
        private async void btnSequentialOps_Click(object sender, EventArgs e)
        {
            AppendLog("Starting sequential operations...");
            btnSequentialOps.Enabled = false;

            try
            {
                await Task.Run(() =>
                {
                    using (var dialog = ProcessingDialogManager.Show(
                        "Multi-Step Process",
                        "Starting operations...",
                        DialogStyle.Professional))
                    {
                        // Operation 1
                        dialog.Update("Operation 1: Connecting to database...", 0);
                        Thread.Sleep(1500);

                        // Operation 2
                        dialog.Update("Operation 2: Fetching records...", 25);
                        Thread.Sleep(1500);

                        // Operation 3
                        dialog.Update("Operation 3: Processing records...", 50);
                        Thread.Sleep(1500);

                        // Operation 4
                        dialog.Update("Operation 4: Generating report...", 75);
                        Thread.Sleep(1500);

                        // Operation 5
                        dialog.Update("Operation 5: Saving results...", 100);
                        Thread.Sleep(1000);
                    }
                });

                AppendLog("All sequential operations completed!");
            }
            catch (Exception ex)
            {
                AppendLog($"Error: {ex.Message}");
            }
            finally
            {
                btnSequentialOps.Enabled = true;
            }
        }

        #endregion

        #region Scenario 7: Error Handling

        /// <summary>
        /// Demonstrates proper error handling with ProcessingDialog.
        /// </summary>
        private async void btnErrorHandling_Click(object sender, EventArgs e)
        {
            AppendLog("Starting error handling scenario...");
            btnErrorHandling.Enabled = false;

            try
            {
                await Task.Run(() =>
                {
                    using (var dialog = ProcessingDialogManager.Show(
                        "Error Handling Demo",
                        "Processing...",
                        DialogStyle.Error))
                    {
                        try
                        {
                            for (int i = 0; i <= 100; i += 10)
                            {
                                Thread.Sleep(300);

                                // Simulate error at 50%
                                if (i == 50)
                                {
                                    throw new InvalidOperationException("Simulated error at 50%");
                                }

                                dialog.Update($"Processing {i}%...", i);
                            }
                        }
                        catch (Exception ex)
                        {
                            dialog.UpdateMessage($"Error occurred: {ex.Message}");
                            Thread.Sleep(2000); // Show error message

                            this.Invoke(new Action(() =>
                            {
                                AppendLog($"Error handled gracefully: {ex.Message}");
                            }));
                        }
                    }
                });
            }
            finally
            {
                btnErrorHandling.Enabled = true;
            }
        }

        #endregion

        #region Scenario 8: Indeterminate Progress

        /// <summary>
        /// Shows indeterminate progress (when you don't know how long it will take).
        /// </summary>
        private async void btnIndeterminate_Click(object sender, EventArgs e)
        {
            AppendLog("Starting indeterminate progress scenario...");
            btnIndeterminate.Enabled = false;

            try
            {
                await Task.Run(() =>
                {
                    using (var dialog = ProcessingDialogManager.Show(
                        "Unknown Duration",
                        "Please wait...",
                        DialogStyle.Information,
                        null,
                        true)) // Indeterminate = true
                    {
                        Thread.Sleep(2000);
                        dialog.UpdateMessage("Still working...");
                        Thread.Sleep(2000);
                        dialog.UpdateMessage("Almost there...");
                        Thread.Sleep(2000);
                        dialog.UpdateMessage("Finishing up...");
                        Thread.Sleep(1000);
                    }
                });

                AppendLog("Indeterminate progress completed!");
            }
            finally
            {
                btnIndeterminate.Enabled = true;
            }
        }

        #endregion

        #region Scenario 9: Updating from Timer (Main Thread)

        /// <summary>
        /// Shows how to use ProcessingDialog with a Timer (main thread updates).
        /// This demonstrates that updates work from the main thread too.
        /// </summary>
        private System.Windows.Forms.Timer _progressTimer;
        private int _timerProgress = 0;

        private void btnTimerBased_Click(object sender, EventArgs e)
        {
            AppendLog("Starting timer-based scenario...");
            btnTimerBased.Enabled = false;
            _timerProgress = 0;

            // Show dialog on background thread
            Task.Run(() =>
            {
                _currentDialog = ProcessingDialogManager.Show(
                    "Timer-Based Progress",
                    "Processing with timer updates...",
                    DialogStyle.Success);
            });

            // Wait a bit for dialog to initialize
            Thread.Sleep(100);

            // Update from main thread using a timer
            _progressTimer = new System.Windows.Forms.Timer();
            _progressTimer.Interval = 200;
            _progressTimer.Tick += (s, args) =>
            {
                _timerProgress += 5;

                if (_timerProgress <= 100)
                {
                    // Update from main thread (thread-safe)
                    _currentDialog?.Update($"Progress: {_timerProgress}%", _timerProgress);
                }
                else
                {
                    _progressTimer.Stop();
                    _progressTimer.Dispose();
                    _currentDialog?.Close();
                    _currentDialog = null;

                    AppendLog("Timer-based scenario completed!");
                    btnTimerBased.Enabled = true;
                }
            };
            _progressTimer.Start();

            AppendLog("Timer started. Main thread remains responsive!");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Thread-safe log appending.
        /// </summary>
        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AppendLog(message)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            txtLog.AppendText($"[{timestamp}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
            AppendLog("Log cleared. Ready for testing.");
        }

        private void btnTestResponsiveness_Click(object sender, EventArgs e)
        {
            AppendLog("UI is responsive! You can click this button anytime.");
        }

        #endregion

        #region Cleanup

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up any running dialogs
            _cancellationTokenSource?.Cancel();
            _currentDialog?.Close();
            _currentDialog = null;
            _progressTimer?.Dispose();

            base.OnFormClosing(e);
        }

        #endregion
    }
}
