using System.ComponentModel;
using Accessibility;
using Call.GUI.Common;
using Call.GUI.Controls;
using Call.GUI.Controls.Styled;

namespace Call.GUI;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }
    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        DoubleBuffered = true;
        MainPanel = new System.Windows.Forms.Panel();
        BottomPanel = new PagesPanel();
        RightPanel = new PagesPanel();
        RightBar = new System.Windows.Forms.Panel();
        LeftPanel = new PagesPanel();
        LeftBar = new System.Windows.Forms.Panel();
        StatusBar = new System.Windows.Forms.Panel();
        TitlePanel = new System.Windows.Forms.Panel();
        MenuPanel = new System.Windows.Forms.Panel();
        MainPanel.SuspendLayout();
        
        SuspendLayout();
        //
        // TitlePanel
        //
        TitlePanel.Dock = DockStyle.Top;
        TitlePanel.Height = 32;
        TitlePanel.Name = "TitlePanel";
        var closeButton = CreateUtils.CreateButton("✖");
        var maximizeButton = CreateUtils.CreateButton("□");
        var minimizeButton = CreateUtils.CreateButton("─");
        closeButton.Dock = maximizeButton.Dock = minimizeButton.Dock = DockStyle.Right;
        closeButton.Width = minimizeButton.Width = maximizeButton.Width = 32;
        
        closeButton.Click += (_, _) => { this.Close(); };
        minimizeButton.Click += (_, _) => { this.WindowState = FormWindowState.Minimized; };
        maximizeButton.Click += (_, _) =>
        {
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
                maximizeButton.Text = "□";
            }
            else
            {
                WindowState = FormWindowState.Maximized;
                maximizeButton.Text = "⧉";
            }
        };
        TitlePanel.DoubleClick += (_, _) =>
        {
            maximizeButton.PerformClick();
        };
        TitlePanel.MouseMove += MoveForm;
        //
        // MenuPanel
        //
        MenuPanel.Dock = DockStyle.Fill;
        MenuPanel.Name = "MenuPanel";
        MenuPanel.MouseMove += MoveForm;
        TitlePanel.Controls.Add(MenuPanel);
        TitlePanel.Controls.AddRange(minimizeButton, maximizeButton, closeButton);

        // 
        // StatusBar
        // 
        StatusBar.BackColor = System.Drawing.SystemColors.ButtonShadow;
        StatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
        StatusBar.Location = new System.Drawing.Point(0, 494);
        StatusBar.MaximumSize = new System.Drawing.Size(0, 18);
        StatusBar.MinimumSize = new System.Drawing.Size(0, 18);
        StatusBar.Name = "StatusBar";
        StatusBar.Size = new System.Drawing.Size(878, 18);
        StatusBar.TabIndex = 5;
        StatusBar.Controls.Add(new Separator(){Thickness = 1, Direction = Direction.Horizontal, Dock = DockStyle.Top, BackColor = Color.Black});
        // 
        // LeftBar
        // 
        LeftBar.BackColor = System.Drawing.SystemColors.ButtonShadow;
        LeftBar.Dock = System.Windows.Forms.DockStyle.Left;
        LeftBar.Location = new System.Drawing.Point(0, 24);
        LeftBar.MaximumSize = new System.Drawing.Size(28, 0);
        LeftBar.MinimumSize = new System.Drawing.Size(28, 0);
        LeftBar.Name = "LeftBar";
        LeftBar.Size = new System.Drawing.Size(28, 469);
        LeftBar.TabIndex = 6;
        LeftBar.Controls.Add(new Separator(){Thickness = 1, Direction = Direction.Vertical, Dock = DockStyle.Right, BackColor = Color.Black});
        // 
        // RightBar
        // 
        RightBar.BackColor = System.Drawing.SystemColors.ButtonShadow;
        RightBar.Dock = System.Windows.Forms.DockStyle.Right;
        RightBar.Location = new System.Drawing.Point(854, 24);
        RightBar.MaximumSize = new System.Drawing.Size(28, 0);
        RightBar.MinimumSize = new System.Drawing.Size(28, 0);
        RightBar.Name = "RightBar";
        RightBar.Size = new System.Drawing.Size(28, 469);
        RightBar.TabIndex = 7;
        RightBar.Controls.Add(new Separator(){Thickness = 1, Direction = Direction.Vertical, Dock = DockStyle.Left, BackColor = Color.Black});
        // 
        // RightPanel
        // 
        RightPanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
        RightPanel.Dock = System.Windows.Forms.DockStyle.Right;
        RightPanel.Location = new System.Drawing.Point(854, 24);
        RightPanel.Name = "RightPanel";
        RightPanel.Size = new System.Drawing.Size(50, 469);
        RightPanel.TabIndex = 7;
        RightPanel.Visible = false;
        var rightPanelSeparator = new Separator()
        {
            Thickness = 3, Direction = Direction.Vertical, Dock = DockStyle.Left, BackColor = Color.Black
        };
        RightPanel.Controls.Add(rightPanelSeparator);
        ModifyUtils.LinkSeparatorToControl(rightPanelSeparator, RightPanel);
        // 
        // LeftPanel
        // 
        LeftPanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
        LeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
        LeftPanel.Location = new System.Drawing.Point(854, 24);
        LeftPanel.Name = "LeftPanel";
        LeftPanel.Size = new System.Drawing.Size(50, 469);
        LeftPanel.TabIndex = 7;
        LeftPanel.Visible = false;
        var leftPanelSeparator = new Separator()
        {
            Thickness = 3, Direction = Direction.Vertical, Dock = DockStyle.Right, BackColor = Color.Black
        };
        LeftPanel.Controls.Add(leftPanelSeparator);
        ModifyUtils.LinkSeparatorToControl(leftPanelSeparator, LeftPanel);
        //
        // BottomPanel
        //
        BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        BottomPanel.Name = "BottomPanel";
        BottomPanel.Visible = false;
        var bottomPanelSeparator = new Separator()
        {
            Thickness = 3, Direction = Direction.Horizontal, Dock = DockStyle.Top, BackColor = Color.Black
        };
        BottomPanel.Controls.Add(bottomPanelSeparator);
        ModifyUtils.LinkSeparatorToControl(bottomPanelSeparator, BottomPanel);
        //
        // MainPanel
        //
        MainPanel.Dock = DockStyle.Fill;
        MainPanel.Name = "MainPanel";
        MainPanel.MinimumSize = new System.Drawing.Size(100, 0);
        
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(878, 512);
        this.Text = "MainForm";
        this.FormBorderStyle = FormBorderStyle.None;
        
        Controls.Add(MainPanel);
        
        Controls.Add(RightPanel);
        Controls.Add(LeftPanel);
        
        Controls.Add(BottomPanel);
        
        
        Controls.Add(RightBar);
        Controls.Add(LeftBar);
        
        Controls.Add(TitlePanel);
        Controls.Add(StatusBar);
        MainPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
    private System.Windows.Forms.Panel MainPanel;
    private PagesPanel BottomPanel;
    private PagesPanel RightPanel;
    private System.Windows.Forms.Panel RightBar;
    private PagesPanel LeftPanel;
    private System.Windows.Forms.Panel LeftBar;
    private System.Windows.Forms.Panel StatusBar;
    private System.Windows.Forms.Panel TitlePanel;
    private Panel MenuPanel;

    #endregion
}