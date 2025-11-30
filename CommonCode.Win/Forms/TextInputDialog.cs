using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Forms
{
    /// <summary>
    /// A generic dialog form for requesting text input from the user with optional format validation.
    /// Supports various input formats including email, phone number, numeric values, and custom validation.
    /// </summary>
    public partial class TextInputDialog : Form
    {
        #region Fields

        private TextInputFormat _format = TextInputFormat.None;
        private bool _required = false;
        private string _customValidationPattern = null;
        private DialogStyle _style = DialogStyle.Default;

        #endregion

        #region Events

        /// <summary>
        /// Event fired when custom validation is needed.
        /// Set e.IsValid to true if the input is valid, false otherwise.
        /// Set e.ErrorMessage to provide feedback to the user.
        /// </summary>
        public event EventHandler<CustomValidationEventArgs> CustomValidation;

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
        /// Gets or sets the input format for validation.
        /// </summary>
        public TextInputFormat InputFormat
        {
            get { return _format; }
            set { _format = value; }
        }

        /// <summary>
        /// Gets or sets whether input is required.
        /// </summary>
        public bool Required
        {
            get { return _required; }
            set { _required = value; }
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
                ApplyStyle();
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
            }
        }

        /// <summary>
        /// Gets or sets the custom validation regex pattern (used when InputFormat is Custom).
        /// </summary>
        public string CustomValidationPattern
        {
            get { return _customValidationPattern; }
            set { _customValidationPattern = value; }
        }

        /// <summary>
        /// Gets the text entered by the user.
        /// </summary>
        public string InputText
        {
            get { return txtInput.Text; }
            set { txtInput.Text = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of TextInputDialog.
        /// </summary>
        public TextInputDialog()
        {
            InitializeComponent();
            ApplyStyle();
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
            btnOK.BackColor = accentColor;
            btnOK.ForeColor = Color.White;
        }

        /// <summary>
        /// Validates the input based on the specified format.
        /// </summary>
        private bool ValidateInput(out string errorMessage)
        {
            errorMessage = string.Empty;
            string input = txtInput.Text;

            // Check if required
            if (_required && string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "This field is required.";
                return false;
            }

            // If not required and empty, it's valid
            if (string.IsNullOrWhiteSpace(input))
                return true;

            // Format-specific validation
            switch (_format)
            {
                case TextInputFormat.Email:
                    if (!IsValidEmail(input))
                    {
                        errorMessage = "Please enter a valid email address.";
                        return false;
                    }
                    break;

                case TextInputFormat.PhoneNumber:
                    if (!IsValidPhoneNumber(input))
                    {
                        errorMessage = "Please enter a valid phone number.";
                        return false;
                    }
                    break;

                case TextInputFormat.Numeric:
                    if (!IsValidNumeric(input))
                    {
                        errorMessage = "Please enter a valid numeric value.";
                        return false;
                    }
                    break;

                case TextInputFormat.Integer:
                    if (!IsValidInteger(input))
                    {
                        errorMessage = "Please enter a valid integer value.";
                        return false;
                    }
                    break;

                case TextInputFormat.Custom:
                    // Use regex pattern if provided
                    if (!string.IsNullOrEmpty(_customValidationPattern))
                    {
                        if (!Regex.IsMatch(input, _customValidationPattern))
                        {
                            errorMessage = "Input does not match the required format.";
                            return false;
                        }
                    }

                    // Fire custom validation event
                    if (CustomValidation != null)
                    {
                        CustomValidationEventArgs args = new CustomValidationEventArgs(input);
                        CustomValidation(this, args);
                        if (!args.IsValid)
                        {
                            errorMessage = args.ErrorMessage ?? "Invalid input.";
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Validates email format.
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates phone number format.
        /// </summary>
        private bool IsValidPhoneNumber(string phone)
        {
            // Remove common phone number characters
            string cleaned = Regex.Replace(phone, @"[\s\-\(\)\.]", "");
            // Check if remaining characters are digits and length is reasonable
            return Regex.IsMatch(cleaned, @"^\+?\d{7,15}$");
        }

        /// <summary>
        /// Validates numeric format (decimal).
        /// </summary>
        private bool IsValidNumeric(string value)
        {
            decimal result;
            return decimal.TryParse(value, out result);
        }

        /// <summary>
        /// Validates integer format.
        /// </summary>
        private bool IsValidInteger(string value)
        {
            int result;
            return int.TryParse(value, out result);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles OK button click.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            string errorMessage;
            if (ValidateInput(out errorMessage))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInput.Focus();
            }
        }

        /// <summary>
        /// Handles Cancel button click.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Shows a text input dialog and returns the result.
        /// </summary>
        public static string ShowDialog(string title, string message, DialogStyle style = DialogStyle.Default,
            TextInputFormat format = TextInputFormat.None, bool required = false, string defaultValue = "",
            Image image = null, IWin32Window owner = null)
        {
            using (TextInputDialog dialog = new TextInputDialog())
            {
                dialog.DialogTitle = title;
                dialog.Message = message;
                dialog.Style = style;
                dialog.InputFormat = format;
                dialog.Required = required;
                dialog.InputText = defaultValue;
                dialog.DialogImage = image;

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                    return dialog.InputText;

                return null;
            }
        }

        #endregion
    }

    #region Event Arguments

    /// <summary>
    /// Event arguments for custom validation.
    /// </summary>
    public class CustomValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the input text being validated.
        /// </summary>
        public string InputText { get; private set; }

        /// <summary>
        /// Gets or sets whether the input is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the error message to display if validation fails.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of CustomValidationEventArgs.
        /// </summary>
        public CustomValidationEventArgs(string inputText)
        {
            this.InputText = inputText;
            this.IsValid = true;
            this.ErrorMessage = string.Empty;
        }
    }

    #endregion
}
