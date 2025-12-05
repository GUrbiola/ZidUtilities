using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// A customizable dialog form that mimics MessageBox.Show functionality with additional theming support.
    /// Supports various button combinations and provides styled message display with optional icons.
    /// </summary>
    public partial class MessageBoxDialog : Form
    {
        #region Fields

        private ZidThemes _theme = ZidThemes.Default;
        private DialogResult _result = DialogResult.None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dialog title.
        /// </summary>
        public string DialogTitle
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        /// <summary>
        /// Gets or sets the message displayed to the user.
        /// </summary>
        public string Message
        {
            get { return lblMessage.Text; }
            set { lblMessage.Text = value; }
        }

        /// <summary>
        /// Gets or sets the ZidTheme (color scheme).
        /// </summary>
        public ZidThemes Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                ApplyTheme();
            }
        }

        /// <summary>
        /// Gets or sets the image displayed in the dialog.
        /// </summary>
        public Image DialogImage
        {
            get { return pictureBox.Image; }
            set
            {
                pictureBox.Image = value;
                pictureBox.Visible = (value != null);

                // Adjust message padding based on image visibility
                if (value != null)
                    lblMessage.Padding = new Padding(60, 0, 0, 0);
                else
                    lblMessage.Padding = new Padding(15, 0, 0, 0);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of MessageBoxDialog.
        /// </summary>
        public MessageBoxDialog()
        {
            InitializeComponent();
            ApplyTheme();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the selected theme to the dialog.
        /// </summary>
        private void ApplyTheme()
        {
            Color headerColor = DialogStyleHelper.GetHeaderColor(_theme);
            Color headerTextColor = DialogStyleHelper.GetHeaderTextColor(_theme);
            Color accentColor = DialogStyleHelper.GetAccentColor(_theme);

            pnlHeader.BackColor = headerColor;
            lblMessage.ForeColor = headerTextColor;
            btnButton1.BackColor = accentColor;
            btnButton1.ForeColor = Color.White;
        }

        /// <summary>
        /// Configures the buttons based on MessageBoxButtons enum.
        /// </summary>
        private void ConfigureButtons(MessageBoxButtons buttons)
        {
            // Hide all buttons initially
            btnButton1.Visible = false;
            btnButton2.Visible = false;
            btnButton3.Visible = false;

            Color accentColor = DialogStyleHelper.GetAccentColor(_theme);
            Color grayColor = Color.FromArgb(189, 195, 199);

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    btnButton1.Text = "OK";
                    btnButton1.Tag = DialogResult.OK;
                    btnButton1.BackColor = accentColor;
                    btnButton1.Visible = true;
                    btnButton1.Location = new Point(192, 10); // Center single button
                    this.AcceptButton = btnButton1;
                    break;

                case MessageBoxButtons.OKCancel:
                    btnButton1.Text = "OK";
                    btnButton1.Tag = DialogResult.OK;
                    btnButton1.BackColor = accentColor;
                    btnButton1.Visible = true;
                    btnButton1.Location = new Point(273, 10);

                    btnButton2.Text = "Cancel";
                    btnButton2.Tag = DialogResult.Cancel;
                    btnButton2.BackColor = grayColor;
                    btnButton2.Visible = true;
                    btnButton2.Location = new Point(374, 10);

                    this.AcceptButton = btnButton1;
                    this.CancelButton = btnButton2;
                    break;

                case MessageBoxButtons.YesNo:
                    btnButton1.Text = "Yes";
                    btnButton1.Tag = DialogResult.Yes;
                    btnButton1.BackColor = accentColor;
                    btnButton1.Visible = true;
                    btnButton1.Location = new Point(273, 10);

                    btnButton2.Text = "No";
                    btnButton2.Tag = DialogResult.No;
                    btnButton2.BackColor = grayColor;
                    btnButton2.Visible = true;
                    btnButton2.Location = new Point(374, 10);

                    this.AcceptButton = btnButton1;
                    this.CancelButton = btnButton2;
                    break;

                case MessageBoxButtons.YesNoCancel:
                    btnButton1.Text = "Yes";
                    btnButton1.Tag = DialogResult.Yes;
                    btnButton1.BackColor = accentColor;
                    btnButton1.Visible = true;
                    btnButton1.Location = new Point(172, 10);

                    btnButton2.Text = "No";
                    btnButton2.Tag = DialogResult.No;
                    btnButton2.BackColor = grayColor;
                    btnButton2.Visible = true;
                    btnButton2.Location = new Point(273, 10);

                    btnButton3.Text = "Cancel";
                    btnButton3.Tag = DialogResult.Cancel;
                    btnButton3.BackColor = grayColor;
                    btnButton3.Visible = true;
                    btnButton3.Location = new Point(374, 10);

                    this.AcceptButton = btnButton1;
                    this.CancelButton = btnButton3;
                    break;

                case MessageBoxButtons.RetryCancel:
                    btnButton1.Text = "Retry";
                    btnButton1.Tag = DialogResult.Retry;
                    btnButton1.BackColor = accentColor;
                    btnButton1.Visible = true;
                    btnButton1.Location = new Point(273, 10);

                    btnButton2.Text = "Cancel";
                    btnButton2.Tag = DialogResult.Cancel;
                    btnButton2.BackColor = grayColor;
                    btnButton2.Visible = true;
                    btnButton2.Location = new Point(374, 10);

                    this.AcceptButton = btnButton1;
                    this.CancelButton = btnButton2;
                    break;

                case MessageBoxButtons.AbortRetryIgnore:
                    btnButton1.Text = "Abort";
                    btnButton1.Tag = DialogResult.Abort;
                    btnButton1.BackColor = accentColor;
                    btnButton1.Visible = true;
                    btnButton1.Location = new Point(172, 10);

                    btnButton2.Text = "Retry";
                    btnButton2.Tag = DialogResult.Retry;
                    btnButton2.BackColor = grayColor;
                    btnButton2.Visible = true;
                    btnButton2.Location = new Point(273, 10);

                    btnButton3.Text = "Ignore";
                    btnButton3.Tag = DialogResult.Ignore;
                    btnButton3.BackColor = grayColor;
                    btnButton3.Visible = true;
                    btnButton3.Location = new Point(374, 10);

                    this.AcceptButton = btnButton2;
                    this.CancelButton = btnButton1;
                    break;
            }
        }

        /// <summary>
        /// Gets the appropriate system icon for the specified icon type.
        /// </summary>
        private Image GetSystemIcon(MessageBoxIcon icon)
        {
            Image back = null; ;

            switch (icon)
            {
                case MessageBoxIcon.Error:
                    back = Resources.Error64;
                    break;
                case MessageBoxIcon.Warning:
                    back = Resources.Warning64;
                    break;
                case MessageBoxIcon.Information:
                    back = Resources.Info264;
                    break;
                case MessageBoxIcon.Question:
                    back = Resources.Question64;
                    break;
                default:
                    return null;
            }

            return back;
        }

        /// <summary>
        /// Gets the appropriate theme based on MessageBoxIcon.
        /// </summary>
        private ZidThemes GetThemeFromIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    return ZidThemes.Error;
                case MessageBoxIcon.Warning:
                    return ZidThemes.Warning;
                case MessageBoxIcon.Information:
                    return ZidThemes.Information;
                case MessageBoxIcon.Question:
                    return ZidThemes.Default;
                default:
                    return ZidThemes.Default;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles Button1 click.
        /// </summary>
        private void btnButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = (DialogResult)(btnButton1.Tag ?? DialogResult.OK);
            this.Close();
        }

        /// <summary>
        /// Handles Button2 click.
        /// </summary>
        private void btnButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = (DialogResult)(btnButton2.Tag ?? DialogResult.Cancel);
            this.Close();
        }

        /// <summary>
        /// Handles Button3 click.
        /// </summary>
        private void btnButton3_Click(object sender, EventArgs e)
        {
            this.DialogResult = (DialogResult)(btnButton3.Tag ?? DialogResult.Cancel);
            this.Close();
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Displays a message box with specified text, caption, buttons, icon, and theme.
        /// </summary>
        public static DialogResult Show(string text, string caption = "Message",
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.None,
            ZidThemes theme = ZidThemes.Default,
            IWin32Window owner = null)
        {
            using (MessageBoxDialog dialog = new MessageBoxDialog())
            {
                dialog.Message = text;
                dialog.DialogTitle = caption;
                dialog.ConfigureButtons(buttons);

                // If theme is default and icon is specified, use theme based on icon
                if (theme == ZidThemes.Default && icon != MessageBoxIcon.None)
                {
                    dialog.Theme = dialog.GetThemeFromIcon(icon);
                }
                else
                {
                    dialog.Theme = theme;
                }

                // Set icon if specified
                if (icon != MessageBoxIcon.None)
                {
                    dialog.DialogImage = dialog.GetSystemIcon(icon);
                }

                return dialog.ShowDialog(owner);
            }
        }

        /// <summary>
        /// Displays a message box with specified text and caption.
        /// </summary>
        public static DialogResult Show(string text, string caption, ZidThemes theme)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, theme, null);
        }

        /// <summary>
        /// Displays a message box with specified text.
        /// </summary>
        public static DialogResult Show(string text, ZidThemes theme)
        {
            return Show(text, "Message", MessageBoxButtons.OK, MessageBoxIcon.None, theme, null);
        }

        /// <summary>
        /// Displays a message box with specified text, caption, and buttons.
        /// </summary>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, ZidThemes theme)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None, theme, null);
        }

        /// <summary>
        /// Displays a message box with specified text, caption, buttons, and icon.
        /// </summary>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, ZidThemes theme)
        {
            return Show(text, caption, buttons, icon, theme, null);
        }

        #endregion
    }
}
