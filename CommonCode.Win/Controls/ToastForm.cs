using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls
{
    /// <summary>
    /// Specifies the visual styles for a toast notification.
    /// </summary>
    public enum ToastType
    {
        /// <summary>Primary informational style.</summary>
        Primary,
        /// <summary>Neutral secondary style.</summary>
        Secondary,
        /// <summary>Indicates a successful operation.</summary>
        Success,
        /// <summary>Indicates an error or critical condition.</summary>
        Danger,
        /// <summary>Indicates a warning or caution.</summary>
        Warning,
        /// <summary>Informational style.</summary>
        Info,
        /// <summary>Light / pale style for subtle messages.</summary>
        Light,
        /// <summary>Dark / emphasis style.</summary>
        Dark,
        /// <summary>Custom style; developer can configure colors/icons manually.</summary>
        Custom
    }

    /// <summary>
    /// Vertical alignment options for toast placement.
    /// </summary>
    public enum VerticalPosition
    {
        /// <summary>Place the toast at the top edge (or relative top of the parent).</summary>
        Top,
        /// <summary>Place the toast in the vertical center.</summary>
        Center,
        /// <summary>Place the toast at the bottom edge (or relative bottom of the parent).</summary>
        Bottom
    }

    /// <summary>
    /// Horizontal alignment options for toast placement.
    /// </summary>
    public enum HorizontalPosition
    {
        /// <summary>Place the toast at the left edge (or relative left of the parent).</summary>
        Left,
        /// <summary>Place the toast in the horizontal center.</summary>
        Center,
        /// <summary>Place the toast at the right edge (or relative right of the parent).</summary>
        Right
    }

    /// <summary>
    /// Animation types used when showing a toast.
    /// </summary>
    public enum ToastAnimation
    {
        /// <summary>Fade in from transparent to opaque.</summary>
        Fade,
        /// <summary>Slide the toast downwards into view.</summary>
        SlideDown,
        /// <summary>Slide the toast upwards into view.</summary>
        SlideUp,
        /// <summary>Slide the toast leftwards into view.</summary>
        SlideLeft,
        /// <summary>Slide the toast rightwards into view.</summary>
        SlideRight,
        /// <summary>Let the control choose a default animation based on position/context.</summary>
        Default
    }

    /// <summary>
    /// A lightweight notification window (toast) that can be shown relative to a parent form or the screen.
    /// Supports different visual types, stacking of multiple toasts, and simple show animations.
    /// </summary>
    public partial class ToastForm : Form
    {
        /// <summary>
        /// Creates a rounded rectangle region.
        /// </summary>
        /// <param name="nLeftRect">X-coordinate of upper-left corner.</param>
        /// <param name="nTopRect">Y-coordinate of upper-left corner.</param>
        /// <param name="nRightRect">X-coordinate of lower-right corner.</param>
        /// <param name="nBottomRect">Y-coordinate of lower-right corner.</param>
        /// <param name="nWidthEllipse">Width of the ellipse used to create rounded corners.</param>
        /// <param name="nHeightEllipse">Height of the ellipse used to create rounded corners.</param>
        /// <returns>
        /// A handle to the created region (HRGN). Returns IntPtr.Zero on failure.
        /// Caller is responsible for releasing/using the region appropriately (for example using System.Drawing.Region.FromHrgn).
        /// </returns>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        // Static collection to track all active toasts
        /// <summary>
        /// Holds references to currently displayed toast instances so they can be stacked and managed.
        /// Toasts remove themselves from this list when closed.
        /// </summary>
        private static readonly List<ToastForm> activeToasts = new List<ToastForm>();

        /// <summary>
        /// Lock object used to synchronize access to <see cref="activeToasts"/>.
        /// </summary>
        private static readonly object lockObject = new object();

        /// <summary>
        /// Space in pixels between stacked toasts.
        /// </summary>
        private const int ToastSpacing = 10; // Space between stacked toasts

        /// <summary>
        /// Top margin offset (pixels) from the top of the reference area when using <see cref="VerticalPosition.Top"/>.
        /// </summary>
        public int MarginTop { get; set; }

        /// <summary>
        /// Bottom margin offset (pixels) from the bottom of the reference area when using <see cref="VerticalPosition.Bottom"/>.
        /// </summary>
        public int MarginBottom { get; set; }

        /// <summary>
        /// Title text displayed on the toast.
        /// Backed by the designer label control `labTitle`.
        /// </summary>
        public string Title
        {
            get
            {
                return labTitle.Text;
            }
            set
            {
                labTitle.Text = value;
            }
        }

        /// <summary>
        /// Description / body text displayed on the toast.
        /// Backed by the designer label control `labText`.
        /// </summary>
        public string Description
        {
            get
            {
                return labText.Text;
            }
            set
            {
                labText.Text = value;
            }
        }

        private ToastType _Kind;

        /// <summary>
        /// Gets or sets the visual <see cref="ToastType"/> for the toast.
        /// Setting this property updates the toast's colors, icon, and progress bar appearance.
        /// </summary>
        public ToastType Kind
        {
            get
            {
                return _Kind;
            }
            set
            {
                _Kind = value;
                switch (_Kind)
                {
                    case ToastType.Primary:
                        this.BackColor = Color.FromArgb(204, 229, 255);
                        this.labText.ForeColor = Color.FromArgb(0, 39, 82);
                        this.labTitle.ForeColor = Color.FromArgb(0, 39, 82);
                        this.pBar.BarColor = Color.FromArgb(0, 39, 82);
                        this.pBar.BackgroundColor = Color.FromArgb(204, 229, 255);
                        this.pictureBox2.BackgroundImage = Resources.message;
                        break;
                    case ToastType.Secondary:
                        this.BackColor = Color.FromArgb(226, 227, 229);
                        this.labText.ForeColor = Color.FromArgb(32, 35, 38);
                        this.labTitle.ForeColor = Color.FromArgb(32, 35, 38);
                        this.pBar.BarColor = Color.FromArgb(32, 35, 38);
                        this.pBar.BackgroundColor = Color.FromArgb(226, 227, 229);
                        this.pictureBox2.BackgroundImage = Resources.message;
                        break;
                    case ToastType.Success:
                        this.BackColor = Color.FromArgb(212, 237, 218);
                        this.labText.ForeColor = Color.FromArgb(11, 46, 19);
                        this.labTitle.ForeColor = Color.FromArgb(11, 46, 19);
                        this.pBar.BarColor = Color.FromArgb(11, 46, 19);
                        this.pBar.BackgroundColor = Color.FromArgb(212, 237, 218);
                        this.pictureBox2.BackgroundImage = Resources.checkmark;
                        break;
                    case ToastType.Danger:
                        this.BackColor = Color.FromArgb(248, 215, 218);
                        this.labText.ForeColor = Color.FromArgb(73, 18, 23);
                        this.labTitle.ForeColor = Color.FromArgb(73, 18, 23);
                        this.pBar.BarColor = Color.FromArgb(73, 18, 23);
                        this.pBar.BackgroundColor = Color.FromArgb(248, 215, 218);
                        this.pictureBox2.BackgroundImage = Resources.warning;
                        break;
                    case ToastType.Warning:
                        this.BackColor = Color.FromArgb(255, 243, 205);
                        this.labText.ForeColor = Color.FromArgb(83, 63, 3);
                        this.labTitle.ForeColor = Color.FromArgb(83, 63, 3);
                        this.pBar.BarColor = Color.FromArgb(83, 63, 3);
                        this.pBar.BackgroundColor = Color.FromArgb(255, 243, 205);
                        this.pictureBox2.BackgroundImage = Resources.warning_sign;
                        break;
                    case ToastType.Info:
                        this.BackColor = Color.FromArgb(209, 236, 241);
                        this.labText.ForeColor = Color.FromArgb(6, 44, 51);
                        this.labTitle.ForeColor = Color.FromArgb(6, 44, 51);
                        this.pBar.BarColor = Color.FromArgb(6, 44, 51);
                        this.pBar.BackgroundColor = Color.FromArgb(209, 236, 241);
                        this.pictureBox2.BackgroundImage = Resources.Info64;
                        break;
                    case ToastType.Light:
                        this.BackColor = Color.FromArgb(254, 254, 254);
                        this.labText.ForeColor = Color.FromArgb(104, 104, 104);
                        this.labTitle.ForeColor = Color.FromArgb(104, 104, 104);
                        this.pBar.BarColor = Color.FromArgb(104, 104, 104);
                        this.pBar.BackgroundColor = Color.FromArgb(254, 254, 254);
                        this.pictureBox2.BackgroundImage = Resources.Info64;
                        break;
                    case ToastType.Dark:
                        this.BackColor = Color.FromArgb(214, 216, 217);
                        this.labText.ForeColor = Color.FromArgb(4, 5, 5);
                        this.labTitle.ForeColor = Color.FromArgb(4, 5, 5);
                        this.pBar.BarColor = Color.FromArgb(4, 5, 5);
                        this.pBar.BackgroundColor = Color.FromArgb(214, 216, 217);
                        this.pictureBox2.BackgroundImage = Resources.Info64;
                        break;
                    case ToastType.Custom:
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Vertical position used when computing the toast's placement.
        /// </summary>
        public VerticalPosition PosV { get; set; }

        /// <summary>
        /// Horizontal position used when computing the toast's placement.
        /// </summary>
        public HorizontalPosition PosH { get; set; }

        /// <summary>
        /// Duration (in milliseconds) that the toast remains visible before auto-closing.
        /// A value of 0 causes the toast to be shown modally via ShowDialog.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// When true the toast's position is calculated relative to the supplied parent form instead of the screen working area.
        /// </summary>
        public bool ReferenceParentForPosition { get; set; }

        /// <summary>
        /// When true, prevents the mouse leave handler from restarting the countdown (used when the user closes the toast manually).
        /// </summary>
        private bool preventMouseLeave = false;

        /// <summary>
        /// Calculated screen location where the toast should finish its show animation.
        /// </summary>
        private Point ShowAt;

        /// <summary>
        /// The animation type used when showing the toast.
        /// </summary>
        private ToastAnimation Animation { get; set; }

        /// <summary>
        /// Total number of pixels used as the initial offset when sliding the toast into view.
        /// </summary>
        private int TotalMovement = 150;

        /// <summary>
        /// Cumulative time (in milliseconds) that the toast has already been shown (used by the countdown).
        /// </summary>
        private int ShownTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastForm"/> class.
        /// Sets up double buffering to reduce flicker, initializes default margins and duration,
        /// and applies a rounded region to the form.
        /// </summary>
        /// <remarks>
        /// This constructor calls InitializeComponent and attempts to enable control-level double buffering
        /// via reflection. Any failure to set the DoubleBuffered property will show a message box.
        /// </remarks>
        public ToastForm()
        {
            InitializeComponent();

            // Enable double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            MarginTop = 40;
            MarginBottom = 25;

            try
            {
                typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this, new object[] { true });
            }
            catch (Exception)
            {
                MessageBox.Show("Error setting double buffer");
            }

            Duration = 0;

            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    // Do not call base.OnPaintBackground to reduce flickering
        //}


        /// <summary>
        /// Handles the close button click event.
        /// Stops the countdown timer (if running), prevents mouse leave handling,
        /// and closes the toast immediately.
        /// </summary>
        /// <param name="sender">Event source (close button).</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>None. This method closes the current toast form.</returns>
        private void btnClose_Click(object sender, EventArgs e)
        {
            ShownTime = 0;
            preventMouseLeave = true;
            if (countDown.Enabled)
                countDown.Stop();
            this.Close();
            
        }

        /// <summary>
        /// Configures and displays the toast relative to the given parent control or screen.
        /// Determines animation type, computes position (including stacking with other toasts),
        /// registers the toast in the active list and starts the show animation or displays the dialog.
        /// </summary>
        /// <param name="Parent">Parent form used for reference positioning (if <see cref="ReferenceParentForPosition"/> is true).</param>
        /// <param name="animation">Animation to use for showing the toast. If <see cref="ToastAnimation.Default"/>, a default is chosen based on position.</param>
        /// <returns>None. Shows the toast either modelessly or modally depending on <see cref="Duration"/>.</returns>
        public void LoadToast(Form Parent, ToastAnimation animation)
        {
            // Set the position of the form based on PosV and PosH
            if(Parent == null)
            {
                return;
            }
            this.Animation = animation;
            Screen curScreen = Screen.FromControl(Parent);
            Point StartPos = curScreen.WorkingArea.Location;
            int height, width;
            this.Opacity = 0.0;

            if (ReferenceParentForPosition)
            {
                StartPos = Parent.Location;
                height = Parent.Height;
                width = Parent.Width;
                if(animation == ToastAnimation.Default)
                    this.Animation = ToastAnimation.Fade;
            }
            else
            {
                height = curScreen.WorkingArea.Height;
                width = curScreen.WorkingArea.Width;

                if (animation == ToastAnimation.Default)
                {
                    if (PosV == VerticalPosition.Top)
                        this.Animation = ToastAnimation.SlideDown;
                    else if (PosH == HorizontalPosition.Left)
                        this.Animation = ToastAnimation.SlideRight;
                    else if (PosH == HorizontalPosition.Right)
                        this.Animation = ToastAnimation.SlideLeft;
                    else if (PosH == HorizontalPosition.Center && PosV != VerticalPosition.Center)
                        this.Animation = ToastAnimation.SlideUp;
                    else
                        this.Animation = ToastAnimation.Fade;
                }
            }



            switch (PosV)
            {
                case VerticalPosition.Top:
                    StartPos.Y += 0 + MarginTop;
                    break;
                case VerticalPosition.Center:
                    StartPos.Y += (height - this.Height) / 2;
                    break;
                case VerticalPosition.Bottom:
                    StartPos.Y += height - this.Height - MarginBottom ;
                    break;
            }

            switch (PosH)
            {
                case HorizontalPosition.Left:
                    StartPos.X += 0;
                    break;
                case HorizontalPosition.Center:
                    StartPos.X += (width - this.Width) / 2;
                    break;
                case HorizontalPosition.Right:
                    StartPos.X += width - this.Width;
                    break;
            }

            ShowAt = StartPos;

            // Adjust position based on existing toasts at the same location
            ShowAt = CalculateStackedPosition(ShowAt, PosV, PosH, ReferenceParentForPosition);

            if(Duration > 0)
            {
                // Initialize animation properties
                switch (Animation)
                {
                    case ToastAnimation.SlideDown:
                        this.Location = new Point(ShowAt.X, ShowAt.Y - TotalMovement);
                        break;
                    case ToastAnimation.SlideLeft:
                        this.Location = new Point(ShowAt.X + TotalMovement, ShowAt.Y);
                        break;
                    case ToastAnimation.SlideRight:
                        this.Location = new Point(ShowAt.X - TotalMovement, ShowAt.Y);
                        break;
                    case ToastAnimation.SlideUp:
                        this.Location = new Point(ShowAt.X, ShowAt.Y + TotalMovement);
                        break;
                    case ToastAnimation.Fade:
                    case ToastAnimation.Default:
                        this.Location = ShowAt;
                        this.Opacity = 0;
                        break;
                }

                // Show the form
                this.Show();

                // Register this toast as active
                lock (lockObject)
                {
                    activeToasts.Add(this);
                }

                animator.Start();
            }
            else
            {
                this.Location = ShowAt;

                // Register this toast as active
                lock (lockObject)
                {
                    activeToasts.Add(this);
                }

                this.ShowDialog();
            }
        }

        /// <summary>
        /// Calculates an adjusted screen position for the toast so multiple toasts at the same
        /// configuration (vertical/horizontal/useParentRef) stack without overlapping.
        /// </summary>
        /// <param name="basePosition">The initial computed position for this toast.</param>
        /// <param name="vPos">Vertical position configuration (<see cref="VerticalPosition"/>).</param>
        /// <param name="hPos">Horizontal position configuration (<see cref="HorizontalPosition"/>).</param>
        /// <param name="useParentRef">
        /// True if the position is relative to the parent form; false if relative to the screen working area.
        /// </param>
        /// <returns>
        /// A <see cref="Point"/> representing the adjusted position for this toast after accounting for stacking.
        /// If no other toasts are present at the same position configuration, returns <paramref name="basePosition"/>.
        /// </returns>
        private Point CalculateStackedPosition(Point basePosition, VerticalPosition vPos, HorizontalPosition hPos, bool useParentRef)
        {
            lock (lockObject)
            {
                // Clean up any closed toasts
                activeToasts.RemoveAll(t => t.IsDisposed || !t.Visible);

                // Find toasts at the same position configuration
                List<ToastForm> toastsAtSamePosition = new List<ToastForm>();
                foreach (var toast in activeToasts)
                {
                    if (toast.PosV == vPos && toast.PosH == hPos && toast.ReferenceParentForPosition == useParentRef)
                    {
                        toastsAtSamePosition.Add(toast);
                    }
                }

                if (toastsAtSamePosition.Count == 0)
                    return basePosition;

                Point adjustedPosition = basePosition;

                // Calculate total offset based on position
                switch (vPos)
                {
                    case VerticalPosition.Top:
                        // Stack downward (below existing toasts)
                        foreach (var toast in toastsAtSamePosition)
                        {
                            adjustedPosition.Y += toast.Height + ToastSpacing;
                        }
                        break;

                    case VerticalPosition.Bottom:
                        // Stack upward (above existing toasts)
                        foreach (var toast in toastsAtSamePosition)
                        {
                            adjustedPosition.Y -= toast.Height + ToastSpacing;
                        }
                        break;

                    case VerticalPosition.Center:
                        // For center, stack based on horizontal position
                        if (hPos == HorizontalPosition.Left)
                        {
                            // Stack to the right
                            foreach (var toast in toastsAtSamePosition)
                            {
                                adjustedPosition.X += toast.Width + ToastSpacing;
                            }
                        }
                        else if (hPos == HorizontalPosition.Right)
                        {
                            // Stack to the left
                            foreach (var toast in toastsAtSamePosition)
                            {
                                adjustedPosition.X -= toast.Width + ToastSpacing;
                            }
                        }
                        else // Center-Center
                        {
                            // Stack downward
                            foreach (var toast in toastsAtSamePosition)
                            {
                                adjustedPosition.Y += toast.Height + ToastSpacing;
                            }
                        }
                        break;
                }

                return adjustedPosition;
            }
        }

        /// <summary>
        /// Overrides the form closed handler to remove this toast from the shared active list.
        /// Ensures thread-safety by locking while modifying <see cref="activeToasts"/>.
        /// </summary>
        /// <param name="e">Form closed event arguments.</param>
        /// <returns>None. Removes the current toast instance from the internal active list and calls base.</returns>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            lock (lockObject)
            {
                activeToasts.Remove(this);
            }
            base.OnFormClosed(e);
        }

        /// <summary>
        /// Handles the countdown timer tick.
        /// Updates the progress bar and opacity based on elapsed time and closes the toast when duration is reached.
        /// </summary>
        /// <param name="sender">Event source (the countdown timer).</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>None. May close the toast when elapsed time &gt;= <see cref="Duration"/>.</returns>
        private void countDown_Tick(object sender, EventArgs e)
        {
            ShownTime += countDown.Interval;
            this.pBar.Value = 100 - (int)(ShownTime * 100 / Duration);
            this.Opacity = pBar.Value / 100.0;
            if (ShownTime >= Duration)
            {
                countDown.Stop();
                this.Close();
            }

        }

        /// <summary>
        /// Mouse enter handler for the toast.
        /// Pauses the countdown timer (if a duration is set) and makes the toast fully opaque to improve readability while hovered.
        /// </summary>
        /// <param name="sender">Event source (toast form).</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>None. Pauses timer and sets opacity to fully opaque if <see cref="Duration"/> &gt; 0.</returns>
        private void ToastForm_MouseEnter(object sender, EventArgs e)
        {
            if (Duration > 0)
            {
                countDown.Stop();
                Opacity = 1;
            }
        }

        /// <summary>
        /// Mouse leave handler for the toast.
        /// Resumes the countdown timer (if a duration is set and not prevented) and restores opacity to reflect progress.
        /// </summary>
        /// <param name="sender">Event source (toast form).</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>None. Resumes timer and restores opacity when appropriate.</returns>
        private void ToastForm_MouseLeave(object sender, EventArgs e)
        {
            if (Duration > 0 && !preventMouseLeave)
            {
                countDown.Start();
                Opacity = pBar.Value / 100.0;
            }
        }

        /// <summary>
        /// Animator timer tick handler.
        /// Advances the show animation based on the selected <see cref="ToastAnimation"/>.
        /// Updates form location and opacity in steps, stops the animator when the final position/opacity is reached,
        /// and starts the countdown timer if applicable.
        /// </summary>
        /// <param name="sender">Event source (animator timer).</param>
        /// <param name="e">Event arguments.</param>
        /// <returns>None. Moves form and adjusts opacity while animating; may start countdown when animation completes.</returns>
        private void animator_Tick(object sender, EventArgs e)
        {
            int step  = TotalMovement / 10;

            switch (Animation)
            {
                case ToastAnimation.Fade:
                    this.Opacity += 0.10;
                    if (this.Opacity >= 1)
                    {
                        this.Opacity = 1;
                        animator.Stop();
                        if (Duration > 0)
                            countDown.Start();
                    }
                    break;
                case ToastAnimation.SlideDown:
                    this.Opacity += 0.10;
                    this.Location = new Point(this.Location.X, this.Location.Y + step);
                    if (this.Location.Y >= ShowAt.Y)
                    {
                        this.Location = ShowAt;
                        this.Opacity = 1.0;
                        animator.Stop();
                        if (Duration > 0)
                            countDown.Start();
                    }
                    break;
                case ToastAnimation.SlideRight:
                    this.Opacity += 0.10;
                    this.Location = new Point(this.Location.X + step, this.Location.Y);
                    if (this.Location.X >= ShowAt.X)
                    {
                        this.Location = ShowAt;
                        this.Opacity = 1.0;
                        animator.Stop();
                        if (Duration > 0)
                            countDown.Start();
                    }
                    break;
                case ToastAnimation.SlideLeft:
                    this.Opacity += 0.10;
                    this.Location = new Point(this.Location.X - step, this.Location.Y);
                    if (this.Location.X <= ShowAt.X)
                    {
                        this.Location = ShowAt;
                        this.Opacity = 1.0;
                        animator.Stop();
                        if (Duration > 0)
                            countDown.Start();
                    }
                    break;
                case ToastAnimation.SlideUp:
                    this.Opacity += 0.10;
                    this.Location = new Point(this.Location.X, this.Location.Y - step);
                    if (this.Location.Y <= ShowAt.Y)
                    {
                        this.Location = ShowAt;
                        this.Opacity = 1.0;
                        animator.Stop();
                        if (Duration > 0)
                            countDown.Start();
                    }
                    break;
                case ToastAnimation.Default:
                    break;
            }
            this.Invalidate();
        }
    }

    /// <summary>
    /// Static helper to create and show toast notifications.
    /// </summary>
    public static class Toaster
    {

        /// <summary>
        /// Creates and displays a toast message.
        /// Configures the toast instance, sets the title, message, type, position, duration and reference behavior,
        /// then calls <see cref="ToastForm.LoadToast(Form, ToastAnimation)"/> to show it.
        /// </summary>
        /// <param name="parent">Parent form used to determine screen context and optional relative positioning. If null, the method returns immediately.</param>
        /// <param name="title">Title text to display in the toast.</param>
        /// <param name="message">Description or message text to display in the toast.</param>
        /// <param name="toastType">Visual style/type of the toast (default: <see cref="ToastType.Info"/>).</param>
        /// <param name="hP">Horizontal position of the toast on the screen or relative to parent (default: <see cref="HorizontalPosition.Left"/>).</param>
        /// <param name="vP">Vertical position of the toast on the screen or relative to parent (default: <see cref="VerticalPosition.Top"/>).</param>
        /// <param name="duration">
        /// Duration in milliseconds for which the toast should remain visible before auto-closing.
        /// A value of 0 indicates modal behavior (shown with <see cref="Form.ShowDialog"/>).
        /// </param>
        /// <param name="posReferenceIsParent">When true, toast position is relative to the provided parent form instead of the screen working area.</param>
        /// <param name="animation">Animation to use when showing the toast (default: <see cref="ToastAnimation.Fade"/>).</param>
        /// <returns>None. Instantiates and shows a toast using the provided parameters.</returns>
        public static void ShowToast(Form parent, string title, string message, ToastType toastType = ToastType.Info, HorizontalPosition hP = HorizontalPosition.Left,
            VerticalPosition vP = VerticalPosition.Top, int duration = 0, bool posReferenceIsParent = false, ToastAnimation animation = ToastAnimation.Fade)
        {
            ToastForm tf = new ToastForm();
            tf.Title = title;
            tf.Description = message;
            tf.Kind = toastType;
            tf.PosH = hP;
            tf.PosV = vP;
            tf.Duration = duration;
            tf.ReferenceParentForPosition = posReferenceIsParent;

            tf.LoadToast(parent, animation);
        }
    }
}