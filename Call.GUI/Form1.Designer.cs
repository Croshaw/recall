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
        ReservedTable = new System.Windows.Forms.DataGridView();
        SepTable = new System.Windows.Forms.DataGridView();
        NumberTable = new System.Windows.Forms.DataGridView();
        IdTable = new System.Windows.Forms.DataGridView();
        tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        CodeRTB = new System.Windows.Forms.RichTextBox();
        tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
        TokensRTB = new System.Windows.Forms.RichTextBox();
        TokensTable = new System.Windows.Forms.DataGridView();
        richTextBox1 = new System.Windows.Forms.RichTextBox();
        menuStrip1 = new System.Windows.Forms.MenuStrip();
        ((System.ComponentModel.ISupportInitialize)ReservedTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)SepTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)NumberTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)IdTable).BeginInit();
        tableLayoutPanel1.SuspendLayout();
        tableLayoutPanel2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)TokensTable).BeginInit();
        SuspendLayout();
        // 
        // ReservedTable
        // 
        ReservedTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        ReservedTable.Dock = System.Windows.Forms.DockStyle.Fill;
        ReservedTable.Location = new System.Drawing.Point(3, 3);
        ReservedTable.Name = "ReservedTable";
        ReservedTable.Size = new System.Drawing.Size(125, 223);
        ReservedTable.TabIndex = 0;
        ReservedTable.Text = "dataGridView1";
        // 
        // SepTable
        // 
        SepTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        SepTable.Dock = System.Windows.Forms.DockStyle.Fill;
        SepTable.Location = new System.Drawing.Point(134, 3);
        SepTable.Name = "SepTable";
        SepTable.Size = new System.Drawing.Size(126, 223);
        SepTable.TabIndex = 1;
        SepTable.Text = "dataGridView2";
        // 
        // NumberTable
        // 
        NumberTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        NumberTable.Dock = System.Windows.Forms.DockStyle.Fill;
        NumberTable.Location = new System.Drawing.Point(3, 232);
        NumberTable.Name = "NumberTable";
        NumberTable.Size = new System.Drawing.Size(125, 223);
        NumberTable.TabIndex = 2;
        NumberTable.Text = "dataGridView3";
        // 
        // IdTable
        // 
        IdTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        IdTable.Dock = System.Windows.Forms.DockStyle.Fill;
        IdTable.Location = new System.Drawing.Point(134, 232);
        IdTable.Name = "IdTable";
        IdTable.Size = new System.Drawing.Size(126, 223);
        IdTable.TabIndex = 3;
        IdTable.Text = "dataGridView4";
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 2;
        tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tableLayoutPanel1.Controls.Add(IdTable, 1, 1);
        tableLayoutPanel1.Controls.Add(NumberTable, 0, 1);
        tableLayoutPanel1.Controls.Add(SepTable, 1, 0);
        tableLayoutPanel1.Controls.Add(ReservedTable, 0, 0);
        tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
        tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 2;
        tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tableLayoutPanel1.Size = new System.Drawing.Size(263, 458);
        tableLayoutPanel1.TabIndex = 1;
        // 
        // CodeRTB
        // 
        CodeRTB.Dock = System.Windows.Forms.DockStyle.Fill;
        CodeRTB.Location = new System.Drawing.Point(263, 24);
        CodeRTB.Name = "CodeRTB";
        CodeRTB.Size = new System.Drawing.Size(430, 316);
        CodeRTB.TabIndex = 0;
        CodeRTB.Text = "";
        // 
        // tableLayoutPanel2
        // 
        tableLayoutPanel2.ColumnCount = 1;
        tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        tableLayoutPanel2.Controls.Add(TokensRTB, 0, 1);
        tableLayoutPanel2.Controls.Add(TokensTable, 0, 0);
        tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
        tableLayoutPanel2.Location = new System.Drawing.Point(693, 24);
        tableLayoutPanel2.Name = "tableLayoutPanel2";
        tableLayoutPanel2.RowCount = 2;
        tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        tableLayoutPanel2.Size = new System.Drawing.Size(185, 458);
        tableLayoutPanel2.TabIndex = 2;
        // 
        // TokensRTB
        // 
        TokensRTB.Dock = System.Windows.Forms.DockStyle.Fill;
        TokensRTB.Location = new System.Drawing.Point(3, 232);
        TokensRTB.Name = "TokensRTB";
        TokensRTB.Size = new System.Drawing.Size(179, 223);
        TokensRTB.TabIndex = 3;
        TokensRTB.Text = "";
        // 
        // TokensTable
        // 
        TokensTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        TokensTable.Dock = System.Windows.Forms.DockStyle.Fill;
        TokensTable.Location = new System.Drawing.Point(3, 3);
        TokensTable.Name = "TokensTable";
        TokensTable.Size = new System.Drawing.Size(179, 223);
        TokensTable.TabIndex = 2;
        TokensTable.Text = "dataGridView5";
        // 
        // richTextBox1
        // 
        richTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
        richTextBox1.Location = new System.Drawing.Point(263, 340);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.Size = new System.Drawing.Size(430, 142);
        richTextBox1.TabIndex = 3;
        richTextBox1.Text = "";
        // 
        // menuStrip1
        // 
        menuStrip1.Location = new System.Drawing.Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new System.Drawing.Size(878, 24);
        menuStrip1.TabIndex = 4;
        menuStrip1.Text = "menuStrip1";
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(878, 482);
        Controls.Add(CodeRTB);
        Controls.Add(richTextBox1);
        Controls.Add(tableLayoutPanel2);
        Controls.Add(tableLayoutPanel1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)ReservedTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)SepTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)NumberTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)IdTable).EndInit();
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)TokensTable).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.MenuStrip menuStrip1;

    private System.Windows.Forms.RichTextBox richTextBox1;

    private System.Windows.Forms.RichTextBox TokensRTB;

    private System.Windows.Forms.DataGridView dataGridView6;

    private System.Windows.Forms.DataGridView TokensTable;

    private System.Windows.Forms.DataGridView SepTable;
    private System.Windows.Forms.DataGridView NumberTable;
    private System.Windows.Forms.DataGridView IdTable;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

    private System.Windows.Forms.DataGridView ReservedTable;

    private System.Windows.Forms.RichTextBox CodeRTB;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    #endregion
}