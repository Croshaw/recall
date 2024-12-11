using Call.GUI.Controls;
using System.ComponentModel;

namespace Call.GUI;

public class CodeTextBox : UserControl
{
    private readonly RichTextBox _richTextBox;
    private readonly LineNumbersForRTB _lineNumbersForRtb;

    [Category("CatBehavior")]
    [Localizable(true)]
    [DefaultValue(true)]
    public bool WordWrap
    {
        get => _richTextBox.WordWrap;
        set => _richTextBox.WordWrap = value;
    }

    [Category("CatBehavior")]
    [Localizable(true)]
    [DefaultValue(true)]
    public bool ShowLineNumbers
    {
        get => _lineNumbersForRtb.ShowLineNumbers;
        set => _lineNumbersForRtb.ShowLineNumbers = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color BackColor
    {
        get => _richTextBox.BackColor;
        set => _richTextBox.BackColor = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
        get => _richTextBox.ForeColor;
        set => _richTextBox.ForeColor = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color SecondaryBackColor
    {
        get => _lineNumbersForRtb.BackColor;
        set => _lineNumbersForRtb.BackColor = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BorderColor
    {
        get => _lineNumbersForRtb.BorderLinesColor;
        set => _lineNumbersForRtb.BorderLinesColor = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color SecondaryForeColor
    {
        get => _lineNumbersForRtb.ForeColor;
        set => _lineNumbersForRtb.ForeColor = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Font Font
    {
        get => _richTextBox.Font;
        set => _richTextBox.Font = _lineNumbersForRtb.Font = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RichTextBoxScrollBars ScrollBars
    {
        get => _richTextBox.ScrollBars;
        set => _richTextBox.ScrollBars = value;
    }

    public RichTextBox RichTextBox => _richTextBox;

    public CodeTextBox()
    {
        //ScrollBars
        _richTextBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            BorderStyle = BorderStyle.None
        };
        _lineNumbersForRtb = new LineNumbersForRTB()
        {
            Dock = DockStyle.Left,
            AutoSizing = true,
            DockSide = LineNumbersForRTB.LineNumberDockSide.Left,
            LineNumbersAlignment = ContentAlignment.TopRight,
            LineNumbersAntiAlias = true,
            LineNumbersAsHexadecimal = false,
            LineNumbersClippedByItemRectangle = true,
            LineNumbersLeadingZeroes = false,
            ParentRichTextBox = _richTextBox,
            ShowLineNumbers = true,
            ShowGridLines = false,
            ShowBorderLines = false,
            ShowMarginLines = true,
            ShowBackgroundGradient = false,
            Font = _richTextBox.Font,
            Size = new Size(1, 210)
        };

        Controls.Add(_richTextBox);
        Controls.Add(_lineNumbersForRtb);
    }
}