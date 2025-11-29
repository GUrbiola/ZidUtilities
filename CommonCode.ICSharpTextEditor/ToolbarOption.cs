using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonCode.ICSharpTextEditor
{
    /// <summary>
    /// Delegate invoked when a <see cref="ToolbarOption"/> value changes.
    /// </summary>
    /// <param name="option">The <see cref="ToolbarOption"/> that changed.</param>
    public delegate void OnOptionChanged(ToolbarOption option);

    [TypeConverter(typeof(ToolbarOptionConverter))]
    /// <summary>
    /// Represents a single option displayed on a toolbar, including display metadata (name, tooltip, icon),
    /// keyboard shortcuts and runtime state (visibility and enabled). Instances can notify a parent or controller
    /// when one of the monitored properties changes via the <see cref="OptionChanged"/> event.
    /// </summary>
    /// <remarks>
    /// This class is designed to be used with designers and property grids (see <see cref="ToolbarOptionConverter"/>).
    /// Some properties are marked with <see cref="NotifyParentPropertyAttribute"/> to indicate the designer should
    /// refresh parent state when they change. The <see cref="OptionChanged"/> event is raised when properties that
    /// should notify are modified.
    /// </remarks>
    public class ToolbarOption
    {
        /// <summary>
        /// Event raised when this option has changed. Subscribers receive the modified <see cref="ToolbarOption"/> instance.
        /// Only properties that intentionally notify their parent (for example: <see cref="Name"/>, <see cref="Tooltip"/>, 
        /// <see cref="Icon"/>, <see cref="Visible"/>, <see cref="Enabled"/>) invoke this event when changed.
        /// </summary>
        internal event OnOptionChanged OptionChanged;

        /// <summary>
        /// Gets or sets a shortcut key that can activate this option.
        /// This property is browsable in property grids and does not notify parent when changed.
        /// </summary>
        [NotifyParentProperty(false)]
        [Browsable(true)]
        public Keys ShortCut { get; set; }

        /// <summary>
        /// Gets or sets an additional shortcut key that may follow <see cref="ShortCut"/>.
        /// This property is browsable in property grids and does not notify parent when changed.
        /// </summary>
        [NotifyParentProperty(false)]
        [Browsable(true)]
        public Keys ThenShortCut { get; set; }

        // Backing fields for properties that should notify parent when changed
        private string _name;
        private string _tooltip;
        private Image _icon;
        private bool _visible;
        private bool _enabled;

        /// <summary>
        /// Gets or sets the display name of this option. Changing this property will raise <see cref="OptionChanged"/>.
        /// May be null or empty if the option has no explicit name.
        /// </summary>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public string Name
        {
            get => _name;
            set
            {
                if (!Equals(_name, value))
                {
                    _name = value;
                    OnOptionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tooltip text for the option. Changing this property will raise <see cref="OptionChanged"/>.
        /// May be null when no tooltip is desired.
        /// </summary>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public string Tooltip
        {
            get => _tooltip;
            set
            {
                if (!Equals(_tooltip, value))
                {
                    _tooltip = value;
                    OnOptionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon displayed for the option. Changing this property will raise <see cref="OptionChanged"/>.
        /// May be null when no icon is provided.
        /// </summary>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public Image Icon
        {
            get => _icon;
            set
            {
                if (!Equals(_icon, value))
                {
                    _icon = value;
                    OnOptionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the option is visible. Changing this property will raise <see cref="OptionChanged"/>.
        /// </summary>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public bool Visible
        {
            get => _visible;
            set
            {
                if (!Equals(_visible, value))
                {
                    _visible = value;
                    OnOptionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the option is enabled. Changing this property will raise <see cref="OptionChanged"/>.
        /// </summary>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (!Equals(_enabled, value))
                {
                    _enabled = value;
                    OnOptionChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets an associated control object for this option. This property is non-browsable and intended
        /// for runtime association (for example, mapping an option to the UI control that implements it).
        /// </summary>
        [NotifyParentProperty(false)]
        [Browsable(false)]
        public Object Control { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarOption"/> class with default values.
        /// Use this constructor when you plan to set properties individually after construction.
        /// This constructor does not raise the <see cref="OptionChanged"/> event.
        /// </summary>
        public ToolbarOption()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarOption"/> class with the specified values.
        /// This constructor sets backing fields directly and does not invoke <see cref="OptionChanged"/>.
        /// </summary>
        /// <param name="name">The display name of the option. May be null or empty if unset.</param>
        /// <param name="tooltip">The tooltip text describing the option. May be null.</param>
        /// <param name="icon">The icon associated with the option; may be null when no icon is desired.</param>
        /// <param name="visible">Whether the option is initially visible.</param>
        public ToolbarOption(string name, string tooltip, Image icon, bool visible)
        {
            this._name = name;
            this._tooltip = tooltip;
            this._icon = icon;
            this._visible = visible;
        }

        /// <summary>
        /// Raises the <see cref="OptionChanged"/> event if there are any subscribers.
        /// This method is called internally whenever a property that should notify its parent changes.
        /// </summary>
        protected virtual void OnOptionChanged()
        {
            OptionChanged?.Invoke(this);
        }
    }

    public class ToolbarOptionConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Converts a <see cref="ToolbarOption"/> instance to a specified destination type.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides contextual information about the component. May be null.</param>
        /// <param name="culture">A <see cref="System.Globalization.CultureInfo"/> representing the culture to use in the conversion. May be null.</param>
        /// <param name="value">The object to convert; expected to be a <see cref="ToolbarOption"/> instance.</param>
        /// <param name="destinationType">The type to convert the value to; when equal to <see cref="string"/> a readable string is returned.</param>
        /// <returns>
        /// If <paramref name="destinationType"/> is <see cref="string"/> and <paramref name="value"/> is a <see cref="ToolbarOption"/>,
        /// returns a human-readable description combining the option's name and tooltip. Otherwise returns the base conversion result.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(string) && value is ToolbarOption)
            {
                ToolbarOption Bt = (ToolbarOption)value;
                return String.Format("Option: {0}, Tooltip: {1}", Bt.Name, Bt.Tooltip);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
