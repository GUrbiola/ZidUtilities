using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.Win.Controls
{

    /// <summary>
    /// A vertical progress bar control composed of two panels: a top background and a bottom filled bar.
    /// Supports solid colors or linear gradients for both the filled portion and the background.
    /// </summary>
    public partial class VerticalProgressBar : UserControl
    {
        private int _memory;
        private bool _useGradient = false;
        private Color _gradientStartColor = Color.LightGreen;
        private Color _gradientEndColor = Color.DarkGreen;
        private LinearGradientMode _gradientMode = LinearGradientMode.Vertical;
        private Color _backgroundGradientStartColor = Color.LightGray;
        private Color _backgroundGradientEndColor = Color.DarkGray;
        private bool _useBackgroundGradient = false;

        /// <summary>
        /// Current Value of the progress bar, between 0 and 100.
        /// Setting this will clamp the value to [0,100], update panel heights, and invalidate panels when gradients are used.
        /// </summary>
        [Category("Behavior")]
        [Description("Current value of the progress bar (0-100)")]
        [DefaultValue(0)]
        public int Value
        {
            get
            {
                return _memory;
            }
            set
            {
                if(value < 0)
                    value = 0;

                if (value > 100)
                    value = 100;
                _memory = value;

                float fullSize = (float)this.Size.Height;

                bottomPanel.Height = (int)(((float)value/(float)100) * fullSize);
                topPanel.Height = (int)fullSize - bottomPanel.Height;

                // Force repaint when using gradient
                if (_useGradient || _useBackgroundGradient)
                {
                    bottomPanel.Invalidate();
                    topPanel.Invalidate();
                }
            }
        }

        /// <summary>
        /// Background color for the top (unfilled) portion when not using a gradient.
        /// Getter returns the current top panel BackColor. Setter updates the color and invalidates the top panel.
        /// </summary>
        [Category("Appearance")]
        [Description("Background color (when not using gradient)")]
        public Color BackgroundColor
        {
            get
            {
                return topPanel.BackColor;
            }
            set
            {
                topPanel.BackColor = value;
                topPanel.Invalidate();
            }
        }

        /// <summary>
        /// Bar color for the bottom (filled) portion when not using a gradient.
        /// Getter returns the current bottom panel BackColor. Setter updates the color and invalidates the bottom panel.
        /// </summary>
        [Category("Appearance")]
        [Description("Bar color (when not using gradient)")]
        public Color BarColor
        {
            get
            {
                return bottomPanel.BackColor;
            }
            set
            {
                bottomPanel.BackColor = value;
                bottomPanel.Invalidate();
            }
        }

        /// <summary>
        /// Enables or disables gradient coloring for the filled progress bar portion.
        /// Setter updates the backing field and invalidates the bottom panel so the gradient is redrawn.
        /// </summary>
        [Category("Appearance - Gradient")]
        [Description("Enable gradient coloring for the progress bar")]
        [DefaultValue(false)]
        public bool UseGradient
        {
            get { return _useGradient; }
            set
            {
                _useGradient = value;
                bottomPanel.Invalidate();
            }
        }

        /// <summary>
        /// The start color for the progress bar gradient.
        /// Getter returns the color used at the start of the gradient. Setter updates the color and invalidates the bottom panel when gradients are enabled.
        /// </summary>
        [Category("Appearance - Gradient")]
        [Description("Start color of the progress bar gradient")]
        public Color GradientStartColor
        {
            get { return _gradientStartColor; }
            set
            {
                _gradientStartColor = value;
                if (_useGradient)
                    bottomPanel.Invalidate();
            }
        }

        /// <summary>
        /// The end color for the progress bar gradient.
        /// Getter returns the color used at the end of the gradient. Setter updates the color and invalidates the bottomPanel when gradients are enabled.
        /// </summary>
        [Category("Appearance - Gradient")]
        [Description("End color of the progress bar gradient")]
        public Color GradientEndColor
        {
            get { return _gradientEndColor; }
            set
            {
                _gradientEndColor = value;
                if (_useGradient)
                    bottomPanel.Invalidate();
            }
        }

        /// <summary>
        /// The gradient direction mode for gradients applied to the filled portion.
        /// Setter updates the mode and invalidates the bottom panel when gradients are enabled.
        /// </summary>
        [Category("Appearance - Gradient")]
        [Description("Gradient direction mode")]
        [DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientMode
        {
            get { return _gradientMode; }
            set
            {
                _gradientMode = value;
                if (_useGradient)
                    bottomPanel.Invalidate();
            }
        }

        /// <summary>
        /// Enables or disables gradient coloring for the background (top) portion.
        /// Setter updates the backing field and invalidates the top panel so the gradient is redrawn.
        /// </summary>
        [Category("Appearance - Gradient")]
        [Description("Enable gradient coloring for the background")]
        [DefaultValue(false)]
        public bool UseBackgroundGradient
        {
            get { return _useBackgroundGradient; }
            set
            {
                _useBackgroundGradient = value;
                topPanel.Invalidate();
            }
        }

        /// <summary>
        /// The start color for the background gradient.
        /// Getter returns the start color for the top portion gradient. Setter updates the color and invalidates the top panel when background gradients are enabled.
        /// </summary>
        [Category("Appearance - Gradient")]
        [Description("Start color of the background gradient")]
        public Color BackgroundGradientStartColor
        {
            get { return _backgroundGradientStartColor; }
            set
            {
                _backgroundGradientStartColor = value;
                if (_useBackgroundGradient)
                    topPanel.Invalidate();
            }
        }

        /// <summary>
        /// The end color for the background gradient.
        /// Getter returns the end color for the top portion gradient. Setter updates the color and invalidates the topPanel when background gradients are enabled.
        /// </summary>
        [Category("Appearance - Gradient")]
        [Description("End color of the background gradient")]
        public Color BackgroundGradientEndColor
        {
            get { return _backgroundGradientEndColor; }
            set
            {
                _backgroundGradientEndColor = value;
                if (_useBackgroundGradient)
                    topPanel.Invalidate();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerticalProgressBar"/> class.
        /// Sets up double buffering and subscribes to paint events for custom drawing.
        /// </summary>
        public VerticalProgressBar()
        {
            InitializeComponent();

            // Enable double buffering to prevent flickering
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            // Subscribe to paint events
            bottomPanel.Paint += BottomPanel_Paint;
            topPanel.Paint += TopPanel_Paint;
        }

        /// <summary>
        /// Paint event handler for the bottom (filled) panel.
        /// Draws a linear gradient between <see cref="_gradientStartColor"/> and <see cref="_gradientEndColor"/> when gradients are enabled.
        /// If gradient drawing fails, falls back to filling with the panel's BackColor.
        /// </summary>
        /// <param name="sender">The source of the paint event (the bottom panel).</param>
        /// <param name="e">A <see cref="PaintEventArgs"/> that contains the event data and graphics context for drawing.</param>
        private void BottomPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_useGradient && bottomPanel.Height > 0 && bottomPanel.Width > 0)
            {
                try
                {
                    Rectangle rect = new Rectangle(0, 0, bottomPanel.Width, bottomPanel.Height);
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        rect, _gradientStartColor, _gradientEndColor, _gradientMode))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
                catch
                {
                    // Fallback to solid color if gradient fails
                    using (SolidBrush brush = new SolidBrush(bottomPanel.BackColor))
                    {
                        e.Graphics.FillRectangle(brush, bottomPanel.ClientRectangle);
                    }
                }
            }
        }

        /// <summary>
        /// Paint event handler for the top (background) panel.
        /// Draws a linear gradient between <see cref="_backgroundGradientStartColor"/> and <see cref="_backgroundGradientEndColor"/> when background gradients are enabled.
        /// If gradient drawing fails, falls back to filling with the panel's BackColor.
        /// </summary>
        /// <param name="sender">The source of the paint event (the top panel).</param>
        /// <param name="e">A <see cref="PaintEventArgs"/> that contains the event data and graphics context for drawing.</param>
        private void TopPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_useBackgroundGradient && topPanel.Height > 0 && topPanel.Width > 0)
            {
                try
                {
                    Rectangle rect = new Rectangle(0, 0, topPanel.Width, topPanel.Height);
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        rect, _backgroundGradientStartColor, _backgroundGradientEndColor, _gradientMode))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
                catch
                {
                    // Fallback to solid color if gradient fails
                    using (SolidBrush brush = new SolidBrush(topPanel.BackColor))
                    {
                        e.Graphics.FillRectangle(brush, topPanel.ClientRectangle);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the control is resized.
        /// Recalculates the heights of the top and bottom panels based on the current <see cref="Value"/>.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Recalculate panel heights when control is resized
            Value = _memory;
        }
    }
}

