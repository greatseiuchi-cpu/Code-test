
namespace CsvLedger
{
    partial class SummaryForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 500);
            this.Text = "カテゴリ別サマリー";

            this.summaryGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.summaryGridView)).BeginInit();
            this.SuspendLayout();

            // summaryGridView
            this.summaryGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.summaryGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.summaryGridView.Name = "summaryGridView";
            this.summaryGridView.ReadOnly = true;

            this.Controls.Add(this.summaryGridView);
            ((System.ComponentModel.ISupportInitialize)(this.summaryGridView)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView summaryGridView;
    }
}
