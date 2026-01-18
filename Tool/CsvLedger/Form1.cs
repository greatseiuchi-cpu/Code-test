using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.VisualBasic;

namespace CsvLedger
{
    public partial class Form1 : Form
    {
        private List<Transaction> _transactions = new List<Transaction>();
        private Dictionary<string, string> _categorizationRules = new Dictionary<string, string>();
        private readonly string _rulesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rules.json");

        public Form1()
        {
            InitializeComponent();
            LoadRulesFromFile();
            // Set up the DataGridView
            transactionsGridView.AutoGenerateColumns = false;
            transactionsGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var dateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Date",
                HeaderText = "日付",
                Name = "Date"
            };

            var descriptionColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Description",
                HeaderText = "ご利用先など",
                Name = "Description"
            };

            var amountColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Amount",
                HeaderText = "ご利用金額",
                Name = "Amount"
            };
            amountColumn.DefaultCellStyle.Format = "C"; // Currency format

            var categoryColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Category",
                HeaderText = "カテゴリ",
                Name = "Category"
            };

            transactionsGridView.Columns.AddRange(new DataGridViewColumn[] { dateColumn, descriptionColumn, amountColumn, categoryColumn });
        }

        private void LoadRulesFromFile()
        {
            try
            {
                if (File.Exists(_rulesFilePath))
                {
                    string json = File.ReadAllText(_rulesFilePath);
                    _categorizationRules = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                else
                {
                    InitializeDefaultRules();
                }
            }
            catch (Exception)
            {
                InitializeDefaultRules();
            }
        }

        private void SaveRulesToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(_categorizationRules, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_rulesFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ルールの保存中にエラーが発生しました。 " + ex.Message, "エラー");
            }
        }

        private void InitializeDefaultRules()
        {
            _categorizationRules = new Dictionary<string, string>
            {
                { "ＡＭＡＺＯＮ．ＣＯ．ＪＰ", "ネットショッピング" },
                { "Ａｍａｚｏｎ　Ｍａｒｋｅｔ　Ｐｌａｃｅ", "ネットショッピング" },
                { "ウーバートリップ", "食費" },
                { "ドミノ・ピザ", "食費" },
                { "ゆめタウン", "食費" },
                { "タイムズカー", "交通費" },
                { "ゆめＥＴＣカード", "交通費" }
            };
        }

        private void loadCsvButton_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    LoadTransactions(filePath);
                }
            }
        }

        private void LoadTransactions(string filePath)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("＜ご利用明細＞"))
                        {
                            reader.ReadLine(); 
                            break;
                        }
                    }

                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false,
                        ShouldSkipRecord = args => string.IsNullOrWhiteSpace(args.Row.GetField(0)) || !DateOnly.TryParse(args.Row.GetField(0), out _)
                    };

                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Context.RegisterClassMap<TransactionMap>();
                        _transactions = csv.GetRecords<Transaction>().ToList();
                    }
                }

                foreach(var transaction in _transactions)
                {
                    ApplyCategorizationRules(transaction);
                }

                var bindingSource = new BindingSource();
                bindingSource.DataSource = _transactions;
                transactionsGridView.DataSource = bindingSource;
                transactionsGridView.Refresh();

                MessageBox.Show("CSVファイルの読み込みが完了しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSVファイルの読み込み中にエラーが発生しました。 " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyCategorizationRules(Transaction transaction)
        {
            foreach (var rule in _categorizationRules)
            {
                if (transaction.Description.Contains(rule.Key, StringComparison.OrdinalIgnoreCase))
                {
                    transaction.Category = rule.Value;
                    return;
                }
            }
        }

        private void manageRulesButton_Click(object? sender, EventArgs e)
        {
            using (var ruleForm = new RuleForm(_categorizationRules))
            {
                if (ruleForm.ShowDialog() == DialogResult.OK)
                {
                    _categorizationRules = ruleForm.UpdatedRules;
                    SaveRulesToFile();
                    ReapplyRules();
                    MessageBox.Show("ルールが更新され、ファイルに保存されました。");
                }
            }
        }

        private void ReapplyRules()
        {
            foreach (var transaction in _transactions)
            {
                transaction.Category = "未分類";
                ApplyCategorizationRules(transaction);
            }
            transactionsGridView.Refresh();
        }

        private void saveButton_Click(object? sender, EventArgs e)
        {
            if (_transactions == null || !_transactions.Any())
            {
                MessageBox.Show("保存するデータがありません。");
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSVファイル (*.csv)|*.csv";
                saveFileDialog.Title = "名前を付けて保存";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var writer = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(_transactions);
                        }
                        MessageBox.Show("ファイルの保存が完了しました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ファイルの保存中にエラーが発生しました。 " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void summaryButton_Click(object? sender, EventArgs e)
        {
            if (_transactions == null || !_transactions.Any())
            {
                MessageBox.Show("集計するデータがありません。");
                return;
            }

            var summaryData = _transactions
                .GroupBy(t => t.Category)
                .Select(g => new CategorySummary
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToList();

            using (var summaryForm = new SummaryForm(summaryData))
            {
                summaryForm.ShowDialog();
            }
        }
    }

    public class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Map(m => m.Date).Index(0).TypeConverter<DateOnlyConverter>();
            Map(m => m.Description).Index(2);
            Map(m => m.Amount).Index(6).TypeConverter<YenCurrencyConverter>();
            Map(m => m.Category).Constant("未分類");
        }
    }

    public class YenCurrencyConverter : DefaultTypeConverter
    {
        private static readonly Regex _illegalCharsRegex = new Regex(@"[\u00A5\uFFE5,\\*]", RegexOptions.Compiled);

        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0m;
            }

            // 全角→半角変換
            string normalized = Strings.StrConv(text, VbStrConv.Narrow, 0);

            // 不要文字除去
            string cleanedText = _illegalCharsRegex.Replace(normalized, "");

            if (decimal.TryParse(cleanedText, out decimal result))
            {
                return result;
            }

            return 0m;
        }
    }

    public class DateOnlyConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateOnly.TryParse(text, out var date))
            {
                return date;
            }
            return DateOnly.MinValue;
        }
    }
}
