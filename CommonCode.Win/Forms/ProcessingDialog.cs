using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// A dialog form that displays a processing animation with an updatable message.
    /// Designed for background processes with thread-safe update methods.
    /// </summary>
    public partial class ProcessingDialog : Form
    {
        #region Fields

        private DialogStyle _style = DialogStyle.Default;
        private ZidThemes? _theme = null;
        private object _lockObject = new object();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        public string DialogTitle
        {
            get { return this.Text; }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => this.Text = value));
                }
                else
                {
                    this.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the message displayed to the user.
        /// </summary>
        public string Message
        {
            get
            {
                if (this.InvokeRequired)
                {
                    return (string)this.Invoke(new Func<string>(() => lblMessage.Text));
                }
                return lblMessage.Text;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => lblMessage.Text = value));
                }
                else
                {
                    lblMessage.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dialog style (color scheme).
        /// </summary>
        public DialogStyle Style
        {
            get { return _style; }
            set
            {
                _style = value;
                _theme = null; // Clear theme when style is set
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => ApplyStyle()));
                }
                else
                {
                    ApplyStyle();
                }
            }
        }

        /// <summary>
        /// Gets or sets the ZidTheme (color scheme).
        /// </summary>
        public ZidThemes Theme
        {
            get { return _theme ?? ZidThemes.Default; }
            set
            {
                _theme = value;
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => ApplyTheme()));
                }
                else
                {
                    ApplyTheme();
                }
            }
        }

        /// <summary>
        /// Gets or sets the image displayed in the dialog.
        /// </summary>
        public Image DialogImage
        {
            get
            {
                if (this.InvokeRequired)
                {
                    return (Image)this.Invoke(new Func<Image>(() => pictureBox.Image));
                }
                return pictureBox.Image;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        pictureBox.Image = value;
                        pictureBox.Visible = (value != null);
                    }));
                }
                else
                {
                    pictureBox.Image = value;
                    pictureBox.Visible = (value != null);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the progress bar is in marquee (indeterminate) mode.
        /// </summary>
        public bool IsIndeterminate
        {
            get
            {
                if (this.InvokeRequired)
                {
                    return (bool)this.Invoke(new Func<bool>(() =>
                        progressBar.Style == ProgressBarStyle.Marquee));
                }
                return progressBar.Style == ProgressBarStyle.Marquee;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        progressBar.Style = value ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
                        if (!value)
                            progressBar.Value = 0;
                    }));
                }
                else
                {
                    progressBar.Style = value ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
                    if (!value)
                        progressBar.Value = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the progress percentage (0-100). Only applicable when IsIndeterminate is false.
        /// </summary>
        public int Progress
        {
            get
            {
                if (this.InvokeRequired)
                {
                    return (int)this.Invoke(new Func<int>(() => progressBar.Value));
                }
                return progressBar.Value;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => SetProgress(value)));
                }
                else
                {
                    SetProgress(value);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of ProcessingDialog.
        /// </summary>
        public ProcessingDialog()
        {
            InitializeComponent();
            ApplyStyle();
            IsIndeterminate = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the message displayed in the dialog (thread-safe).
        /// </summary>
        public void UpdateMessage(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Updates the progress percentage (thread-safe). Sets mode to determinate.
        /// </summary>
        public void UpdateProgress(int percentage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    if (IsIndeterminate)
                        IsIndeterminate = false;
                    SetProgress(percentage);
                }));
            }
            else
            {
                if (IsIndeterminate)
                    IsIndeterminate = false;
                SetProgress(percentage);
            }
        }

        /// <summary>
        /// Updates both message and progress (thread-safe).
        /// </summary>
        public void Update(string message, int percentage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    lblMessage.Text = message;
                    if (IsIndeterminate)
                        IsIndeterminate = false;
                    SetProgress(percentage);
                }));
            }
            else
            {
                lblMessage.Text = message;
                if (IsIndeterminate)
                    IsIndeterminate = false;
                SetProgress(percentage);
            }
        }

        /// <summary>
        /// Closes the dialog (thread-safe).
        /// </summary>
        public void CloseDialog()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.Close()));
            }
            else
            {
                this.Close();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the selected style to the dialog.
        /// </summary>
        private void ApplyStyle()
        {
            Color headerColor = DialogStyleHelper.GetHeaderColor(_style);
            Color headerTextColor = DialogStyleHelper.GetHeaderTextColor(_style);
            Color accentColor = DialogStyleHelper.GetAccentColor(_style);

            pnlHeader.BackColor = headerColor;
            lblMessage.ForeColor = headerTextColor;
            progressBar.ForeColor = accentColor;
        }

        /// <summary>
        /// Applies the selected theme to the dialog.
        /// </summary>
        private void ApplyTheme()
        {
            if (_theme.HasValue)
            {
                Color headerColor = DialogStyleHelper.GetHeaderColor(_theme.Value);
                Color headerTextColor = DialogStyleHelper.GetHeaderTextColor(_theme.Value);
                Color accentColor = DialogStyleHelper.GetAccentColor(_theme.Value);

                pnlHeader.BackColor = headerColor;
                lblMessage.ForeColor = headerTextColor;
                progressBar.ForeColor = accentColor;
            }
        }

        /// <summary>
        /// Sets the progress value (must be called on UI thread).
        /// </summary>
        private void SetProgress(int value)
        {
            if (value < 0) value = 0;
            if (value > 100) value = 100;

            progressBar.Value = value;
            lblProgress.Text = string.Format("{0}%", value);
        }

        /// <summary>
        /// Prevents the user from closing the dialog with the X button.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Only allow programmatic close or Alt+F4
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        #endregion
    }

    #region Processing Dialog Manager

    /// <summary>
    /// Helper class to easily show and manage a ProcessingDialog on a separate UI thread.
    /// Provides thread-safe methods to update the dialog from background threads.
    /// </summary>
    public class ProcessingDialogManager : IDisposable
    {
        private ProcessingDialog _dialog;
        private Thread _dialogThread;
        private ManualResetEvent _dialogReady = new ManualResetEvent(false);
        private bool _disposed = false;

        /// <summary>
        /// Shows a processing dialog on a separate UI thread.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Initial message.</param>
        /// <param name="style">Dialog style.</param>
        /// <param name="image">Optional image.</param>
        /// <param name="isIndeterminate">Whether to show indeterminate progress.</param>
        public ProcessingDialogManager(string title, string message,
            DialogStyle style, Image image = null, bool isIndeterminate = true)
        {
            _dialogThread = new Thread(() =>
            {
                _dialog = new ProcessingDialog();
                _dialog.DialogTitle = title;
                _dialog.Message = message;
                _dialog.Style = style;
                _dialog.DialogImage = image;
                _dialog.IsIndeterminate = isIndeterminate;

                _dialogReady.Set();
                Application.Run(_dialog);
            });

            _dialogThread.SetApartmentState(ApartmentState.STA);
            _dialogThread.IsBackground = true;
            _dialogThread.Start();

            // Wait for dialog to be created
            _dialogReady.WaitOne();
        }

        /// <summary>
        /// Shows a processing dialog on a separate UI thread.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Initial message.</param>
        /// <param name="style">Dialog style.</param>
        /// <param name="isIndeterminate">Whether to show indeterminate progress.</param>
        public ProcessingDialogManager(string title, string message,
            DialogStyle style, bool isIndeterminate = true)
        {
            _dialogThread = new Thread(() =>
            {
                _dialog = new ProcessingDialog();
                _dialog.DialogTitle = title;
                _dialog.Message = message;
                _dialog.Style = style;
                _dialog.DialogImage = Resources.InProgress;
                _dialog.IsIndeterminate = isIndeterminate;

                _dialogReady.Set();
                Application.Run(_dialog);
            });

            _dialogThread.SetApartmentState(ApartmentState.STA);
            _dialogThread.IsBackground = true;
            _dialogThread.Start();

            // Wait for dialog to be created
            _dialogReady.WaitOne();
        }

        /// <summary>
        /// Shows a processing dialog on a separate UI thread.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Initial message.</param>
        /// <param name="theme">ZidTheme to apply.</param>
        /// <param name="image">Optional image.</param>
        /// <param name="isIndeterminate">Whether to show indeterminate progress.</param>
        public ProcessingDialogManager(string title, string message,
            ZidThemes theme, Image image = null, bool isIndeterminate = true)
        {
            _dialogThread = new Thread(() =>
            {
                _dialog = new ProcessingDialog();
                _dialog.DialogTitle = title;
                _dialog.Message = message;
                _dialog.Theme = theme;
                _dialog.DialogImage = image;
                _dialog.IsIndeterminate = isIndeterminate;

                _dialogReady.Set();
                Application.Run(_dialog);
            });

            _dialogThread.SetApartmentState(ApartmentState.STA);
            _dialogThread.IsBackground = true;
            _dialogThread.Start();

            // Wait for dialog to be created
            _dialogReady.WaitOne();
        }

        /// <summary>
        /// Shows a processing dialog on a separate UI thread.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Initial message.</param>
        /// <param name="theme">ZidTheme to apply.</param>
        /// <param name="isIndeterminate">Whether to show indeterminate progress.</param>
        public ProcessingDialogManager(string title, string message,
            ZidThemes theme, bool isIndeterminate = true)
        {
            _dialogThread = new Thread(() =>
            {
                _dialog = new ProcessingDialog();
                _dialog.DialogTitle = title;
                _dialog.Message = message;
                _dialog.Theme = theme;
                _dialog.DialogImage = Resources.InProgress;
                _dialog.IsIndeterminate = isIndeterminate;

                _dialogReady.Set();
                Application.Run(_dialog);
            });

            _dialogThread.SetApartmentState(ApartmentState.STA);
            _dialogThread.IsBackground = true;
            _dialogThread.Start();

            // Wait for dialog to be created
            _dialogReady.WaitOne();
        }

        /// <summary>
        /// Updates the message (thread-safe).
        /// </summary>
        public void UpdateMessage(string message)
        {
            if (_dialog != null && !_dialog.IsDisposed)
            {
                _dialog.UpdateMessage(message);
            }
        }

        /// <summary>
        /// Updates the progress percentage (thread-safe).
        /// </summary>
        public void UpdateProgress(int percentage)
        {
            if (_dialog != null && !_dialog.IsDisposed)
            {
                _dialog.UpdateProgress(percentage);
            }
        }

        /// <summary>
        /// Updates both message and progress (thread-safe).
        /// </summary>
        public void Update(string message, int percentage)
        {
            if (_dialog != null && !_dialog.IsDisposed)
            {
                _dialog.Update(message, percentage);
            }
        }

        /// <summary>
        /// Closes and disposes the dialog.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes the dialog manager and closes the dialog.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_dialog != null && !_dialog.IsDisposed)
                {
                    _dialog.Invoke(new Action(() =>
                    {
                        _dialog.Close();
                        _dialog.Dispose();
                    }));
                }

                _dialogReady.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Static helper method to show a processing dialog.
        /// Returns a ProcessingDialogManager that can be used to update or close the dialog.
        /// </summary>
        public static ProcessingDialogManager Show(string title, string message,
            DialogStyle style = DialogStyle.Default, Image image = null, bool isIndeterminate = true)
        {
            return new ProcessingDialogManager(title, message, style, image, isIndeterminate);
        }
    }

    #endregion
}
