using System;
using System.ComponentModel;

namespace CommonCode.ICSharpTextEditor
{
    public delegate void OnTextOptionChanged(ToolbarTextBox option);

    [TypeConverter(typeof(ToolbarTextConverter))]
    public class ToolbarTextBox
    {
        // Backing fields
        private string _text;
        private string _toolTip;
        private bool _visible;

        // Event fired whenever Text or Visible changes
        public event OnTextOptionChanged TextOptionChanged;

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


        [NotifyParentProperty(false)]
        [Browsable(false)]
        public Object Control { get; set; }

        public ToolbarTextBox()
        {
        }

        public ToolbarTextBox(string text, bool visible, string tooltip = "")
        {
            this._text = text;
            this._visible = visible;
            this._toolTip = tooltip;
        }

        protected virtual void OnTextOptionChangedRaised()
        {
            TextOptionChanged?.Invoke(this);
        }
    }

    public class ToolbarTextConverter : ExpandableObjectConverter
    {
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
