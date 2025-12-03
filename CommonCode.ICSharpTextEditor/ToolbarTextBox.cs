using System;
using System.ComponentModel;

namespace ZidUtilities.CommonCode.ICSharpTextEditor
{
    /// <summary>
    /// Delegate invoked when a <see cref="ToolbarTextBox"/> option changes.
    /// </summary>
    /// <param name="option">The <see cref="ToolbarTextBox"/> instance that raised the change.</param>
    public delegate void OnTextOptionChanged(ToolbarTextBox option);

    /// <summary>
    /// Represents a text box option used on a toolbar, including its text, tooltip and visibility.
    /// Notifies listeners when relevant properties change.
    /// </summary>
    [TypeConverter(typeof(ToolbarTextConverter))]   
    public class ToolbarTextBox
    {
        // Backing fields
        private string _text;
        private string _toolTip;
        private bool _visible;
        private int _width = 200;

        /// <summary>
        /// Event fired whenever Text, Visible, or ToolTip changes.
        /// Subscribers will receive the <see cref="ToolbarTextBox"/> instance that changed.
        /// </summary>
        internal event OnTextOptionChanged TextOptionChanged;

        /// <summary>
        /// Gets or sets the text displayed by the toolbar text box.
        /// Setting this property raises <see cref="TextOptionChanged"/> if the value changes.
        /// </summary>
        /// <value>The text shown in the toolbar text box.</value>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnTextOptionChangedRaised();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar text box is visible.
        /// Setting this property raises <see cref="TextOptionChanged"/> if the value changes.
        /// </summary>
        /// <value><c>true</c> if the control is visible; otherwise, <c>false</c>.</value>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnTextOptionChangedRaised();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tooltip text associated with the toolbar text box.
        /// Setting this property raises <see cref="TextOptionChanged"/> if the value changes.
        /// </summary>
        /// <value>The tooltip text.</value>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public string ToolTip
        {
            get => _toolTip;
            set
            {
                if (_toolTip != value)
                {
                    _toolTip = value;
                    OnTextOptionChangedRaised();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the toolbar text box.
        /// Setting this property raises <see cref="TextOptionChanged"/> if the value changes.
        /// </summary>
        /// <value>The width of the toolbar text box.</value>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnTextOptionChangedRaised();
                }
            }
        }

        /// <summary>
        /// Gets or sets the underlying control object associated with this toolbar text box.
        /// This property is not browsable and is intended for runtime usage.
        /// </summary>
        /// <value>An <see cref="Object"/> representing the control; may be null.</value>
        [NotifyParentProperty(false)]
        [Browsable(false)]
        public Object Control { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarTextBox"/> class with default values.
        /// </summary>
        public ToolbarTextBox()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarTextBox"/> class with specified values.
        /// </summary>
        /// <param name="text">Initial text for the toolbar text box.</param>
        /// <param name="visible">Initial visibility state.</param>
        /// <param name="tooltip">Initial tooltip text. Optional; defaults to an empty string.</param>
        public ToolbarTextBox(string text, bool visible, string tooltip = "")
        {
            this._text = text;
            this._visible = visible;
            this._toolTip = tooltip;
        }

        /// <summary>
        /// Raises the <see cref="TextOptionChanged"/> event for this instance.
        /// This method is protected and virtual so derived types can override raising behavior.
        /// </summary>
        protected virtual void OnTextOptionChangedRaised()
        {
            TextOptionChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// Type converter for <see cref="ToolbarTextBox"/> to provide a string representation in property grids.
    /// </summary>
    public class ToolbarTextConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Converts the given <see cref="ToolbarTextBox"/> instance to the specified destination type.
        /// When converting to <see cref="string"/>, returns a concise representation containing the Text and Visible state.
        /// Otherwise, the base implementation is used.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context. May be null.</param>
        /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use for conversion. May be null.</param>
        /// <param name="value">The value to convert. Expected to be a <see cref="ToolbarTextBox"/> when converting to string.</param>
        /// <param name="destinationType">The target <see cref="Type"/> to convert to.</param>
        /// <returns>
        /// A converted object. If converting a <see cref="ToolbarTextBox"/> to <see cref="string"/>, returns a formatted string.
        /// Otherwise returns the result of <see cref="ExpandableObjectConverter.ConvertTo(ITypeDescriptorContext,System.Globalization.CultureInfo,object,Type)"/>.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(string) && value is ToolbarTextBox)
            {
                ToolbarTextBox Bt = (ToolbarTextBox)value;
                return String.Format("Text: {0}, Visible: {1}", Bt.Text, Bt.Visible);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
