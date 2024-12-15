using System.Runtime.InteropServices;

namespace Call.GUI;

public class BorderlessForm : Form
{
    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int LPAR);

    [DllImportAttribute("user32.dll")]
    private static extern bool ReleaseCapture();

    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HT_CAPTION = 0x2;

    private const int cGrip = 2;

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x84)
        {
            // Trap WM_NCHITTEST
            var pos = new Point(m.LParam.ToInt32());
            pos = PointToClient(pos);

            if (pos.X >= ClientSize.Width - cGrip && pos.Y >= ClientSize.Height - cGrip)
            {
                m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                return;
            }

            if (pos.X >= ClientSize.Width - cGrip && pos.Y <= cGrip)
            {
                m.Result = (IntPtr)14;
                return;
            }

            if (pos.X <= cGrip && pos.Y <= cGrip)
            {
                m.Result = (IntPtr)13;
                return;
            }

            if (pos.X <= cGrip && pos.Y >= ClientSize.Height - cGrip)
            {
                m.Result = (IntPtr)16; // HTBOTTOMRIGHT
                return;
            }

            if (pos.X >= ClientSize.Width - cGrip)
            {
                m.Result = (IntPtr)11; // HTBOTTOMRIGHT
                return;
            }

            if (pos.X <= cGrip)
            {
                m.Result = (IntPtr)10; // HTBOTTOMRIGHT
                return;
            }

            if (pos.Y >= ClientSize.Height - cGrip)
            {
                m.Result = (IntPtr)15;
                return;
            }

            if (pos.Y <= cGrip)
            {
                m.Result = (IntPtr)12; // HTBOTTOMRIGHT
                return;
            }
        }

        base.WndProc(ref m);
    }

    private void MaximizeChangeState(object sender, EventArgs e)
    {
        WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
    }

    protected void MoveForm(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            SuspendLayout();
            if (e.Clicks >= 2)
            {
                MaximizeChangeState(sender, e);
            }
            else
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

            ResumeLayout();
        }
    }


    protected static void SetupDoubleBuffer(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
            if (control is Panel panel)
            {
                SetupDoubleBuffer(panel.Controls);
                SetDoubleBuffered(control);
            }
    }

    private static void SetDoubleBuffered(Control c)
    {
        if (SystemInformation.TerminalServerSession)
            return;

        var aProp =
            typeof(Control).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

        aProp.SetValue(c, true);
    }

    public BorderlessForm()
    {
        SetStyle(ControlStyles.ResizeRedraw, true);
        Padding = new Padding(cGrip);
    }
}