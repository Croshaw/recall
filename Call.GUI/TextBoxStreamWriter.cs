using System.Text;

namespace Call.GUI;

public class RichTextBoxTextReader : TextReader
{
    private readonly RichTextBox _textBox;
    private int _lastReadPosition;

    public RichTextBoxTextReader(RichTextBox textBox)
    {
        _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        _lastReadPosition = textBox.TextLength;

        // Подписываемся на событие KeyDown
        _textBox.KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            // Удаляем перенос строки, если он только что добавлен
            var textLength = _textBox.TextLength;
            if (textLength > 0 && _textBox.Text[textLength - 1] == '\n')
            {
                _textBox.Text = _textBox.Text.Remove(textLength - 1);
                _textBox.SelectionStart = textLength - 1; // Возвращаем каретку в конец
            }

            // Сбрасываем _lastReadPosition, чтобы учитывать новый текст
            _lastReadPosition = _textBox.TextLength;
            e.SuppressKeyPress = true; // Предотвращаем добавление новой строки в RichTextBox
        }
    }

    public override string ReadLine()
    {
        // Читаем текст, начиная с позиции _lastReadPosition
        if (_lastReadPosition >= _textBox.TextLength)
            return null;

        var newlinePos = _textBox.Text.IndexOf('\n', _lastReadPosition);
        if (newlinePos == -1)
        {
            var line = _textBox.Text.Substring(_lastReadPosition);
            _lastReadPosition = _textBox.TextLength;
            return line.TrimEnd('\r', '\n');
        }

        var result = _textBox.Text.Substring(_lastReadPosition, newlinePos - _lastReadPosition);
        _lastReadPosition = newlinePos + 1;
        return result.TrimEnd('\r', '\n');
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _textBox.KeyDown -= OnKeyDown;
        base.Dispose(disposing);
    }
}

public class TextBoxStreamWriter : TextWriter
{
    private readonly RichTextBox _textBox;
    private readonly Color _textColor;

    public TextBoxStreamWriter(RichTextBox textBox, Color textColor)
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