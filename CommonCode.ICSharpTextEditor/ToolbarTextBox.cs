namespace CommonCode.ICSharpTextEditor
{
    public class ToolbarTextBox
    {
        public string Text { get; set; }
        public bool Visible { get; set; }
        public ToolbarTextBox()
        {
        }
        public ToolbarTextBox(string text, bool visible)
        {
            this.Text = text;
            this.Visible = visible;
        }
    }
}
