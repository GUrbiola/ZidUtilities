using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ZidUtilities.CommonCode.ICSharpTextEditor
{
    /// <summary>
    /// Represents a pair of keyboard shortcuts (a primary and a following shortcut) with a name and enabled flag.
    /// </summary>
    [TypeConverter(typeof(ImplicitShortcutConverter))]
    public class ImplicitShortcut
    {
        [NotifyParentProperty(false)]
        [Browsable(true)]
        public Keys ShortCut { get; set; }
        [NotifyParentProperty(false)]
        [Browsable(true)]
        public Keys ThenShortCut { get; set; }
        
        [NotifyParentProperty(false)]
        [Browsable(true)]
        public string Name  { get; set; }

        [NotifyParentProperty(false)]
        [Browsable(true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitShortcut"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name or identifier for this implicit shortcut.</param>
        /// <remarks>
        /// After construction, <see cref="Enabled"/> is <c>true</c>, and both <see cref="ShortCut"/> and
        /// <see cref="ThenShortCut"/> are initialized to <see cref="Keys.None"/>.
        /// </remarks>
        public ImplicitShortcut(string name)
        {
            this.Enabled = true;
            this.Name = name;
            this.ShortCut = Keys.None;
            this.ThenShortCut = Keys.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitShortcut"/> class with default values.
        /// </summary>
        /// <remarks>
        /// Name is set to an empty string, shortcuts are set to <see cref="Keys.None"/>, and the shortcut is enabled.
        /// After construction, <see cref="Enabled"/> is <c>true</c>, <see cref="Name"/> is an empty string,
        /// and both <see cref="ShortCut"/> and <see cref="ThenShortCut"/> are initialized to <see cref="Keys.None"/>.
        /// </remarks>
        public ImplicitShortcut()
        {
            this.Enabled = true;
            this.Name = string.Empty;
            this.ShortCut = Keys.None;
            this.ThenShortCut = Keys.None;
        }
    }

    /// <summary>
    /// Provides a type converter that displays a friendly description for <see cref="ImplicitShortcut"/> instances
    /// in design-time property browsers.
    /// </summary>
    public class ImplicitShortcutConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Converts an <see cref="ImplicitShortcut"/> instance to a string representation for design-time display.
        /// When the destination type is <see cref="string"/> and the provided value is an <see cref="ImplicitShortcut"/>,
        /// this method returns a human-readable description ("Shortcut for: {Name}").
        /// </summary>
        /// <param name="context">
        /// An <see cref="ITypeDescriptorContext"/> that provides contextual information about the component; may be <c>null</c>.
        /// </param>
        /// <param name="culture">
        /// A <see cref="System.Globalization.CultureInfo"/> representing the culture to use in the conversion; may be <c>null</c>.
        /// </param>
        /// <param name="value">
        /// The object to convert; expected to be an <see cref="ImplicitShortcut"/> instance when converting to a string.
        /// </param>
        /// <param name="destinationType">
        /// The type to convert the value to. When this is <see cref="string"/> and <paramref name="value"/> is an
        /// <see cref="ImplicitShortcut"/>, a readable string is returned. For other destination types, the base
        /// converter behavior is used.
        /// </param>
        /// <returns>
        /// If <paramref name="destinationType"/> is <see cref="string"/> and <paramref name="value"/> is an <see cref="ImplicitShortcut"/>,
        /// returns a formatted string describing the shortcut (e.g., "Shortcut for: MyName"). Otherwise returns the result
        /// of <see cref="ExpandableObjectConverter.ConvertTo(ITypeDescriptorContext,System.Globalization.CultureInfo,object,Type)"/>.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(string) && value is ImplicitShortcut)
            {
                ImplicitShortcut Bt = (ImplicitShortcut)value;
                return String.Format("Shortcut for: {0}", Bt.Name);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
