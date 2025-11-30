using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls
{
    /// <summary>
    /// Occurs when the wait interval has ended for the current text.
    /// </summary>
    /// <param name="Text">The current text of the control.</param>
    /// <param name="Decimals">The elapsed tick count when the wait ended.</param>
    public delegate void TextWaitEnded(string Text, int Decimals);

    /// <summary>
    /// Occurs when the text is explicitly secured (for example via Enter when the timer is not running).
    /// </summary>
    /// <param name="Text">The secured text.</param>
    public delegate void TextSecured(string Text);

    /// <summary>
    /// A composite WinForms UserControl that shows an animated image when the text is edited,
    /// waits for a configurable interval of timer ticks, and raises events when the wait ends
    /// or when the text is secured by the user.
    /// </summary>
    public partial class AnimatedWaitTextBox : UserControl
    {
        int CurPos, CurImage;
        private int _WaitInterval;

        /// <summary>
        /// Raised when the configured wait interval completes for the current text.
        /// </summary>
        public event TextWaitEnded OnTextWaitEnded;

        /// <summary>
        /// Raised when the text is secured (explicit confirmation by the user).
        /// </summary>
        public event TextSecured OnTextSecured;

        /// <summary>
        /// Exposes the child control's KeyPress events to subscribers.
        /// </summary>
        public event KeyPressEventHandler OnKeyPressed;

        /// <summary>
        /// Exposes the child control's KeyDown events to subscribers.
        /// </summary>
        public event KeyEventHandler OnKeyDowned;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedWaitTextBox"/> class.
        /// Sets up the component and initializes internal counters.
        /// </summary>
        public AnimatedWaitTextBox()
        {
            InitializeComponent();
            CurPos = CurImage = 0;
        }

        /// <summary>
        /// Handles font changes on the control and propagates them to the inner edit control.
        /// Also adjusts the control height to match the edit control.
        /// </summary>
        /// <param name="sender">The object that raised the font-changed event.</param>
        /// <param name="e">Event arguments (unused).</param>
        private void TOText_FontChanged(object sender, EventArgs e)
        {
            Edit.Font = Font;
            this.Height = Edit.Height;
        }

        /// <summary>
        /// Gets or sets the number of timer ticks to wait after the last text change before raising <see cref="TextWaitEnded"/>.
        /// </summary>
        /// <value>
        /// The wait interval measured in timer ticks. When the number of elapsed ticks reaches this value,
        /// the <see cref="TextWaitEnded"/> event is raised.
        /// </value>
        public int WaitInterval
        {
            get { return _WaitInterval; }
            set { _WaitInterval = value; }
        }

        private Image _defaultImage;

        /// <summary>
        /// Gets or sets the default image displayed by the control when no animation is active.
        /// Setting this property also updates the current displayed image to the provided value.
        /// </summary>
        /// <value>The default <see cref="Image"/> shown when the animation is not running.</value>
        public Image DefaultImage
        {
           get
            {
                return _defaultImage;
            }
           set
            {
                _defaultImage = value;
                Img.Image = value;
            }
        }

        /// <summary>
        /// Handles key press events from the inner edit control.
        /// - If Enter ('\r') is pressed: disables the running timer if enabled; otherwise raises <see cref="TextSecured"/>.
        ///   In both cases, raises <see cref="TextWaitEnded"/> and resets the animation image state.
        /// - For other keys: forwards the event to any <see cref="KeyPressed"/> subscribers.
        /// </summary>
        /// <param name="sender">The object that raised the key press event (the inner edit control).</param>
        /// <param name="e">The <see cref="KeyPressEventArgs"/> containing the key character.</param>
        private void Edit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (Step.Enabled)
                {
                    Step.Enabled = false;
                }
                else
                {
                    if (OnTextSecured != null)
                        OnTextSecured(Edit.Text);
                }
                if (OnTextWaitEnded != null)
                    OnTextWaitEnded(Edit.Text, CurPos);
                CurImage = 0;
                Img.Image = _defaultImage;
            }
            else
            {
                if (OnKeyPressed != null)
                    OnKeyPressed(sender, e);
            }
        }

        /// <summary>
        /// Timer tick handler that advances the animation and the wait counter.
        /// Updates the displayed image from <c>IList.Images</c>, increments the elapsed counter,
        /// and when the elapsed counter reaches <see cref="WaitInterval"/> it disables the timer,
        /// raises <see cref="TextWaitEnded"/>, and restores the default image.
        /// </summary>
        /// <param name="sender">The timer that raised the tick event.</param>
        /// <param name="e">Event arguments (unused).</param>
        private void Step_Tick(object sender, EventArgs e)
        {
            CurPos++;
            CurImage++;
            Img.Image = IList.Images[CurImage % IList.Images.Count];
            if (CurPos >= WaitInterval)
            {
                CurImage = 0;
                Step.Enabled = false;
                if (OnTextWaitEnded != null)
                    OnTextWaitEnded(Edit.Text, CurPos);
                Img.Image = _defaultImage;
            }
        }

        /// <summary>
        /// Gets or sets the current text of the inner edit control.
        /// </summary>
        /// <returns>The current text string from the inner edit control.</returns>
        public override string Text
        {
            get
            {
                return Edit.Text;
            }
            set
            {
                Edit.Text = value;
            }
        }

        /// <summary>
        /// Handles text changes from the inner edit control.
        /// Ensures the timer is enabled to start counting the wait interval, resets the elapsed counter,
        /// and starts the animation by setting the first image from <c>IList.Images</c>.
        /// </summary>
        /// <param name="sender">The object that raised the text changed event (the inner edit control).</param>
        /// <param name="e">Event arguments (unused).</param>
        private void Edit_TextChanged(object sender, EventArgs e)
        {
            if (!Step.Enabled)
                Step.Enabled = true;
            CurPos = 0;
            Img.Image = IList.Images[CurImage % IList.Images.Count];
        }

        /// <summary>
        /// Forwards KeyDown events from the inner edit control to any subscribers of <see cref="KeyDowned"/>.
        /// </summary>
        /// <param name="sender">The object that raised the key down event.</param>
        /// <param name="e">A <see cref="KeyEventArgs"/> that contains event data about the key press.</param>
        private void Edit_KeyDown(object sender, KeyEventArgs e)
        {
            if (OnKeyDowned != null)
                OnKeyDowned(sender, e);
        }

    }
}
