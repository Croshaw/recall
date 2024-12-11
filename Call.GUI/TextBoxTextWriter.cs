using System.Text;

namespace Call.GUI;

public class TextBoxTextWriter : TextWriter
{
    private readonly RichTextBox _textBox;
    private readonly Color _textColor;

    public TextBoxTextWriter(RichTextBox textBox, Color textColor)
    {
        _textBox = textBox;
        _textColor = textColor;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        AppendText(value.ToString());
    }

    public override void Write(string? value)
    {
        AppendText(value);
    }

    private void AppendText(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        // Потокобезопасное добавление текста с учетом цвета
        _textBox.Invoke((MethodInvoker)(() =>
        {
            var start = _textBox.TextLength;
            _textBox.AppendText(text);
            var end = _textBox.TextLength;

            // Применяем цвет к добавленному тексту
            _textBox.Select(start, end - start);
            _textBox.SelectionColor = _textColor;
            _textBox.SelectionLength = 0;
        }));
    }
}