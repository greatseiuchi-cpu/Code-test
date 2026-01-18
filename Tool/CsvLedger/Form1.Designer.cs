namespace CsvLedger;

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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "CSV 家計簿";
        this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        this.loadCsvButton = new System.Windows.Forms.Button();
        this.manageRulesButton = new System.Windows.Forms.Button();
        this.saveButton = new System.Windows.Forms.Button();
        this.transactionsGridView = new System.Windows.Forms.DataGridView();
        this.tableLayoutPanel1.SuspendLayout();
        this.flowLayoutPanel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.transactionsGridView)).BeginInit();
        this.SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.ColumnCount = 1;
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
        this.tableLayoutPanel1.Controls.Add(this.transactionsGridView, 0, 1);
        this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 2;
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
        this.tableLayoutPanel1.TabIndex = 0;
        // 
        // flowLayoutPanel1
        // 
        this.flowLayoutPanel1.Controls.Add(this.loadCsvButton);
        this.flowLayoutPanel1.Controls.Add(this.manageRulesButton);
        this.flowLayoutPanel1.Controls.Add(this.saveButton);
        this.flowLayoutPanel1.Controls.Add(this.summaryButton);
        this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
        this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        this.flowLayoutPanel1.Size = new System.Drawing.Size(794, 34);
        this.flowLayoutPanel1.TabIndex = 0;
        // 
        // loadCsvButton
        // 
        this.loadCsvButton.Location = new System.Drawing.Point(3, 3);
        this.loadCsvButton.Name = "loadCsvButton";
        this.loadCsvButton.Size = new System.Drawing.Size(100, 23);
        this.loadCsvButton.TabIndex = 0;
        this.loadCsvButton.Text = "CSV読み込み";
        this.loadCsvButton.UseVisualStyleBackColor = true;
        this.loadCsvButton.Click += new System.EventHandler(this.loadCsvButton_Click);
        // 
        // manageRulesButton
        // 
        this.manageRulesButton.Location = new System.Drawing.Point(109, 3);
        this.manageRulesButton.Name = "manageRulesButton";
        this.manageRulesButton.Size = new System.Drawing.Size(100, 23);
        this.manageRulesButton.TabIndex = 1;
        this.manageRulesButton.Text = "ルール管理";
        this.manageRulesButton.UseVisualStyleBackColor = true;
        this.manageRulesButton.Click += new System.EventHandler(this.manageRulesButton_Click);
        // 
        // saveButton
        // 
        this.saveButton.Location = new System.Drawing.Point(215, 3);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new System.Drawing.Size(75, 23);
        this.saveButton.TabIndex = 2;
        this.saveButton.Text = "保存";
        this.saveButton.UseVisualStyleBackColor = true;
        this.saveButton.Click += new System.EventHandler(this.saveButton_Click);

        this.summaryButton = new System.Windows.Forms.Button();
        this.summaryButton.Name = "summaryButton";
        this.summaryButton.Text = "サマリー表示";
        this.summaryButton.UseVisualStyleBackColor = true;
        this.summaryButton.Click += new System.EventHandler(this.summaryButton_Click);
        // 
        // transactionsGridView
        // 
        this.transactionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.transactionsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
        this.transactionsGridView.Location = new System.Drawing.Point(3, 43);
        this.transactionsGridView.Name = "transactionsGridView";
        this.transactionsGridView.RowTemplate.Height = 25;
        this.transactionsGridView.Size = new System.Drawing.Size(794, 404);
        this.transactionsGridView.TabIndex = 1;
        // 
        // Form1
        // 
        this.Controls.Add(this.tableLayoutPanel1);
        this.Name = "Form1";
        this.tableLayoutPanel1.ResumeLayout(false);
        this.flowLayoutPanel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.transactionsGridView)).EndInit();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Button loadCsvButton;
    private System.Windows.Forms.Button manageRulesButton;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.Button summaryButton;
    private System.Windows.Forms.DataGridView transactionsGridView;
}
