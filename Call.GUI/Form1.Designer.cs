using Call.GUI.Controls;

namespace Call.GUI;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        MainPanel = new System.Windows.Forms.Panel();
        BottomPanel = new System.Windows.Forms.Panel();
        BottomPanelSeparator = new System.Windows.Forms.Panel();
        
        RightPanelSeparator = new System.Windows.Forms.Panel();
        RightPanel = new System.Windows.Forms.Panel();
        RightBarSeparator = new System.Windows.Forms.Panel();
        RightBar = new System.Windows.Forms.Panel();

        LeftPanelSeparator = new System.Windows.Forms.Panel();
        LeftPanel = new System.Windows.Forms.Panel();
        LeftBarSeparator = new System.Windows.Forms.Panel();
        LeftBar = new System.Windows.Forms.Panel();
        
        StatusBar = new System.Windows.Forms.Panel();
        StatusBarSeparator = new System.Windows.Forms.Panel();
        
        Menu = new System.Windows.Forms.MenuStrip();
        
        MainPanel.SuspendLayout();
        SuspendLayout();
        // 
        // Menu
        // 
        Menu.Location = new System.Drawing.Point(0, 0);
        Menu.Name = "Menu";
        Menu.Size = new System.Drawing.Size(878, 24);
        Menu.TabIndex = 4;
        Menu.Text = "menuStrip1";
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
        // 
        // LeftBarSeparator
        // 
        LeftBarSeparator.BackColor = System.Drawing.Color.Black;
        LeftBarSeparator.Dock = System.Windows.Forms.DockStyle.Left;
        LeftBarSeparator.Location = new System.Drawing.Point(24, 24);
        LeftBarSeparator.MaximumSize = new System.Drawing.Size(1, 0);
        LeftBarSeparator.MinimumSize = new System.Drawing.Size(1, 0);
        LeftBarSeparator.Name = "LeftBarSeparator";
        LeftBarSeparator.Size = new System.Drawing.Size(1, 469);
        LeftBarSeparator.TabIndex = 8;
        // 
        // StatusBarSeparator
        // 
        StatusBarSeparator.BackColor = System.Drawing.SystemColors.Desktop;
        StatusBarSeparator.Dock = System.Windows.Forms.DockStyle.Bottom;
        StatusBarSeparator.Location = new System.Drawing.Point(0, 493);
        StatusBarSeparator.MaximumSize = new System.Drawing.Size(0, 1);
        StatusBarSeparator.MinimumSize = new System.Drawing.Size(0, 1);
        StatusBarSeparator.Name = "StatusBarSeparator";
        StatusBarSeparator.Size = new System.Drawing.Size(878, 1);
        StatusBarSeparator.TabIndex = 9;
        // 
        // RightMenuSeparator
        // 
        RightBarSeparator.BackColor = System.Drawing.Color.Black;
        RightBarSeparator.Dock = System.Windows.Forms.DockStyle.Right;
        RightBarSeparator.Location = new System.Drawing.Point(853, 24);
        RightBarSeparator.MaximumSize = new System.Drawing.Size(1, 0);
        RightBarSeparator.MinimumSize = new System.Drawing.Size(1, 0);
        RightBarSeparator.Name = "RightBarSeparator";
        RightBarSeparator.Size = new System.Drawing.Size(1, 469);
        RightBarSeparator.TabIndex = 10;
        // 
        // MainPanel
        // 
        MainPanel.Controls.Add(BottomPanel);
        MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        MainPanel.Location = new System.Drawing.Point(25, 24);
        MainPanel.Name = "MainPanel";
        MainPanel.Size = new System.Drawing.Size(828, 469);
        MainPanel.MinimumSize = new System.Drawing.Size(50, 50);
        MainPanel.TabIndex = 11;
        // 
        // panel1
        // 
        BottomPanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
        BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        BottomPanel.Location = new System.Drawing.Point(0, 400);
        BottomPanel.Name = "BottomPanel";
        BottomPanel.Size = new System.Drawing.Size(828, 69);
        BottomPanel.TabIndex = 6;
        // 
        // BottomPanelSeparator
        // 
        BottomPanelSeparator.BackColor = System.Drawing.SystemColors.Desktop;
        BottomPanelSeparator.Dock = System.Windows.Forms.DockStyle.Bottom;
        BottomPanelSeparator.Location = new System.Drawing.Point(0, 493);
        BottomPanelSeparator.MaximumSize = new System.Drawing.Size(0, 1);
        BottomPanelSeparator.MinimumSize = new System.Drawing.Size(0, 1);
        BottomPanelSeparator.Name = "StatusBarSeparator";
        BottomPanelSeparator.Size = new System.Drawing.Size(878, 1);
        BottomPanelSeparator.TabIndex = 9;
        // 
        // LeftPanelSeparator
        // 
        LeftPanelSeparator.BackColor = System.Drawing.Color.Black;
        LeftPanelSeparator.Dock = System.Windows.Forms.DockStyle.Left;
        LeftPanelSeparator.Location = new System.Drawing.Point(24, 24);
        LeftPanelSeparator.MaximumSize = new System.Drawing.Size(1, 0);
        LeftPanelSeparator.MinimumSize = new System.Drawing.Size(1, 0);
        LeftPanelSeparator.Name = "LeftPanelSeparator";
        LeftPanelSeparator.Size = new System.Drawing.Size(1, 469);
        LeftPanelSeparator.TabIndex = 8;
        // 
        // RightPanelSeparator
        // 
        RightPanelSeparator.BackColor = System.Drawing.Color.Black;
        RightPanelSeparator.Dock = System.Windows.Forms.DockStyle.Right;
        RightPanelSeparator.Location = new System.Drawing.Point(24, 24);
        RightPanelSeparator.MaximumSize = new System.Drawing.Size(1, 0);
        RightPanelSeparator.MinimumSize = new System.Drawing.Size(1, 0);
        RightPanelSeparator.Name = "RightPanelSeparator";
        RightPanelSeparator.Size = new System.Drawing.Size(1, 469);
        RightPanelSeparator.TabIndex = 8;
        // 
        // RightPanel
        // 
        RightPanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
        RightPanel.Dock = System.Windows.Forms.DockStyle.Right;
        RightPanel.Location = new System.Drawing.Point(854, 24);
        RightPanel.Name = "RightPanel";
        RightPanel.Size = new System.Drawing.Size(50, 469);
        RightPanel.TabIndex = 7;
        // 
        // LeftPanel
        // 
        LeftPanel.BackColor = System.Drawing.SystemColors.ButtonShadow;
        LeftPanel.Dock = System.Windows.Forms.DockStyle.Left;
        LeftPanel.Location = new System.Drawing.Point(854, 24);
        LeftPanel.Name = "LeftPanel";
        LeftPanel.Size = new System.Drawing.Size(50, 469);
        LeftPanel.TabIndex = 7;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(878, 512);
        Controls.Add(MainPanel);
        
        
        Controls.Add(RightPanelSeparator);
        Controls.Add(LeftPanelSeparator);
        
        Controls.Add(RightPanel);
        Controls.Add(LeftPanel);
        
        Controls.Add(BottomPanelSeparator);
        Controls.Add(BottomPanel);
        
        Controls.Add(RightBarSeparator);
        Controls.Add(LeftBarSeparator);
        
        Controls.Add(RightBar);
        Controls.Add(LeftBar);
        Controls.Add(Menu);
        Controls.Add(StatusBarSeparator);
        Controls.Add(StatusBar);
        MainMenuStrip = Menu;
        Text = "Form1";
        MainPanel.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
    private System.Windows.Forms.Panel MainPanel;
    
    private System.Windows.Forms.Panel BottomPanelSeparator;
    private System.Windows.Forms.Panel BottomPanel;
    
    
    private System.Windows.Forms.Panel RightPanelSeparator;
    private System.Windows.Forms.Panel RightPanel;
    private System.Windows.Forms.Panel RightBarSeparator;
    private System.Windows.Forms.Panel RightBar;
    
    private System.Windows.Forms.Panel LeftPanelSeparator;
    private System.Windows.Forms.Panel LeftPanel;
    private System.Windows.Forms.Panel LeftBarSeparator;
    private System.Windows.Forms.Panel LeftBar;
    
    private System.Windows.Forms.Panel StatusBarSeparator;
    private System.Windows.Forms.Panel StatusBar;
    
    private System.Windows.Forms.MenuStrip Menu;

    #endregion
}