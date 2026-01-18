
using System.ComponentModel;

namespace CsvLedger
{
    public partial class RuleForm : Form
    {
        private readonly Dictionary<string, string> _initialRules;
        public Dictionary<string, string> UpdatedRules { get; private set; }

        private BindingList<Rule>? _bindingList;

        public RuleForm(Dictionary<string, string> rules)
        {
            InitializeComponent();
            _initialRules = rules;
            UpdatedRules = new Dictionary<string, string>(rules); // Initialize with original rules

            // Set up DataGridView
            rulesGridView.AutoGenerateColumns = false;
            rulesGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            var keywordColumn = new DataGridViewTextBoxColumn { DataPropertyName = "Keyword", HeaderText = "キーワード" };
            var categoryColumn = new DataGridViewTextBoxColumn { DataPropertyName = "Category", HeaderText = "カテゴリ" };
            rulesGridView.Columns.AddRange(keywordColumn, categoryColumn);

            // Set up event handlers
            this.Load += RuleForm_Load;
            this.addButton.Click += AddButton_Click;
            this.editButton.Click += EditButton_Click;
            this.deleteButton.Click += DeleteButton_Click;
            this.saveButton.Click += SaveButton_Click;
            this.closeButton.Click += CloseButton_Click;
        }

        private void RuleForm_Load(object? sender, EventArgs e)
        {
            var ruleList = _initialRules.Select(kvp => new Rule { Keyword = kvp.Key, Category = kvp.Value }).ToList();
            _bindingList = new BindingList<Rule>(ruleList);
            rulesGridView.DataSource = _bindingList;
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            _bindingList?.Add(new Rule());
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("この機能は、行を直接編集することで利用できます。");
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (rulesGridView.CurrentRow != null && rulesGridView.CurrentRow.DataBoundItem is Rule rule)
            {
                _bindingList?.Remove(rule);
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (_bindingList == null) return;

            // Validate for duplicate keywords before saving
            var duplicates = _bindingList.GroupBy(r => r.Keyword)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key).ToList();

            if (duplicates.Any())
            {
                MessageBox.Show("キーワードが重複しています: " + string.Join(", ", duplicates), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convert the BindingList back to a Dictionary
            UpdatedRules = _bindingList.ToDictionary(r => r.Keyword, r => r.Category);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CloseButton_Click(object? sender, EventArgs e)
        { 
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
