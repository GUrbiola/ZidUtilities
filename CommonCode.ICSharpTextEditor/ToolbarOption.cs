using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonCode.ICSharpTextEditor
{
    public delegate void OnOptionChanged(ToolbarOption option);
    [TypeConverter(typeof(ToolbarOptionConverter))]
    public class ToolbarOption
    {
        public event OnOptionChanged OptionChanged;

        [NotifyParentProperty(false)]
        [Browsable(true)]
        public Keys ShortCut { get; set; }
        [NotifyParentProperty(false)]
        [Browsable(true)]
        public Keys ThenShortCut { get; set; }

        // Backing fields for properties that should notify parent when changed
        private string _name;
        private string _tooltip;
        private Image _icon;
        private bool _visible;
        private bool _enabled;

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

        [NotifyParentProperty(false)]
        [Browsable(false)]
        public Object Control { get; set; }

        public ToolbarOption()
        {
        }

        public ToolbarOption(string name, string tooltip, Image icon, bool visible)
        {
            this._name = name;
            this._tooltip = tooltip;
            this._icon = icon;
            this._visible = visible;
        }

        protected virtual void OnOptionChanged()
        {
            OptionChanged?.Invoke(this);
        }
    }
    public class ToolbarOptionConverter : ExpandableObjectConverter
    {
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
