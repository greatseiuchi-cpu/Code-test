using System.Collections.Generic;
using System.Windows.Forms;

namespace CsvLedger
{
    public partial class SummaryForm : Form
    {
        public SummaryForm(List<CategorySummary> summaryData)
        {
            InitializeComponent();

            summaryGridView.AutoGenerateColumns = false;
            summaryGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var categoryColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Category",
                HeaderText = "カテゴリ",
                Name = "Category"
            };

            var amountColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalAmount",
                HeaderText = "合計金額",
                Name = "TotalAmount"
            };
            amountColumn.DefaultCellStyle.Format = "C";

            summaryGridView.Columns.AddRange(categoryColumn, amountColumn);

            summaryGridView.DataSource = summaryData;
        }
    }
}