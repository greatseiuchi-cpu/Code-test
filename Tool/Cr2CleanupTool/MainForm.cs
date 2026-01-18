using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cr2CleanupTool
{
    public partial class MainForm : Form
    {
        // UI Controls
        private Label lblCr2Folder;
        private Button btnSelectCr2Folder;
        private TextBox txtCr2FolderPath;
        private Label lblJpegFolder;
        private Button btnSelectJpegFolder;
        private TextBox txtJpegFolderPath;
        private Button btnScan;
        private ListView lstUnmatchedFiles;
        private Button btnDelete;
        private Label lblStatus;

        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Initialize and configure UI controls
            this.lblCr2Folder = new Label();
            this.btnSelectCr2Folder = new Button();
            this.txtCr2FolderPath = new TextBox();
            this.lblJpegFolder = new Label();
            this.btnSelectJpegFolder = new Button();
            this.txtJpegFolderPath = new TextBox();
            this.btnScan = new Button();
            this.lstUnmatchedFiles = new ListView();
            this.btnDelete = new Button();
            this.lblStatus = new Label();

            // Form
            this.Text = "CR2 Cleanup Tool";
            this.Size = new Size(800, 600);

            // CR2 Folder Selection
            this.lblCr2Folder.Text = "CR2 フォルダ:";
            this.lblCr2Folder.Location = new Point(12, 15);
            this.lblCr2Folder.AutoSize = true;
            this.btnSelectCr2Folder.Text = "選択...";
            this.btnSelectCr2Folder.Location = new Point(120, 12);
            this.btnSelectCr2Folder.Click += new EventHandler(this.btnSelectCr2Folder_Click);
            this.txtCr2FolderPath.Location = new Point(200, 14);
            this.txtCr2FolderPath.Size = new Size(470, 20);
            this.txtCr2FolderPath.ReadOnly = true;

            // JPEG Folder Selection
            this.lblJpegFolder.Text = "JPEG フォルダ:";
            this.lblJpegFolder.Location = new Point(12, 45);
            this.lblJpegFolder.AutoSize = true;
            this.btnSelectJpegFolder.Text = "選択...";
            this.btnSelectJpegFolder.Location = new Point(120, 42);
            this.btnSelectJpegFolder.Click += new EventHandler(this.btnSelectJpegFolder_Click);
            this.txtJpegFolderPath.Location = new Point(200, 44);
            this.txtJpegFolderPath.Size = new Size(470, 20);
            this.txtJpegFolderPath.ReadOnly = true;

            // btnScan
            this.btnScan.Text = "スキャン開始";
            this.btnScan.Location = new Point(680, 12);
            this.btnScan.Size = new Size(92, 52); // Make button taller
            this.btnScan.Click += new EventHandler(this.btnScan_Click);

            // lstUnmatchedFiles
            this.lstUnmatchedFiles.Location = new Point(12, 80);
            this.lstUnmatchedFiles.Size = new Size(760, 420);
            this.lstUnmatchedFiles.View = View.Details;
            this.lstUnmatchedFiles.Columns.Add("不要なCR2ファイル", -2, HorizontalAlignment.Left);

            // btnDelete
            this.btnDelete.Text = "選択したファイルを削除";
            this.btnDelete.Location = new Point(600, 510);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);

            // lblStatus
            this.lblStatus.Text = "CR2フォルダとJPEGフォルダを選択してスキャンを開始してください。";
            this.lblStatus.Location = new Point(12, 515);
            this.lblStatus.AutoSize = true;

            // Add controls to the form
            this.Controls.Add(this.lblCr2Folder);
            this.Controls.Add(this.btnSelectCr2Folder);
            this.Controls.Add(this.txtCr2FolderPath);
            this.Controls.Add(this.lblJpegFolder);
            this.Controls.Add(this.btnSelectJpegFolder);
            this.Controls.Add(this.txtJpegFolderPath);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.lstUnmatchedFiles);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblStatus);
        }

        private void btnSelectCr2Folder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txtCr2FolderPath.Text = dialog.SelectedPath;
                    this.lblStatus.Text = "CR2フォルダが選択されました。";
                }
            }
        }

        private void btnSelectJpegFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txtJpegFolderPath.Text = dialog.SelectedPath;
                    this.lblStatus.Text = "JPEGフォルダが選択されました。";
                }
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            string cr2FolderPath = this.txtCr2FolderPath.Text;
            string jpegFolderPath = this.txtJpegFolderPath.Text;

            if (string.IsNullOrEmpty(cr2FolderPath) || !Directory.Exists(cr2FolderPath) ||
                string.IsNullOrEmpty(jpegFolderPath) || !Directory.Exists(jpegFolderPath))
            {
                MessageBox.Show("CR2フォルダとJPEGフォルダの両方を正しく選択してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                this.lblStatus.Text = "スキャン中...";
                this.Cursor = Cursors.WaitCursor;
                this.lstUnmatchedFiles.Items.Clear();

                var unmatchedFiles = ImageFileService.FindUnmatchedCr2Files(cr2FolderPath, jpegFolderPath);

                foreach (var file in unmatchedFiles)
                {
                    this.lstUnmatchedFiles.Items.Add(new ListViewItem(file));
                }

                this.lblStatus.Text = $"{unmatchedFiles.Count}個の不要なファイルが見つかりました。";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "スキャンエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.lblStatus.Text = "エラーが発生しました。";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.lstUnmatchedFiles.Items.Count == 0)
            {
                MessageBox.Show("削除するファイルがありません。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"{this.lstUnmatchedFiles.Items.Count}個のファイルを完全に削除します。よろしいですか？", "最終確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var filesToDelete = this.lstUnmatchedFiles.Items.Cast<ListViewItem>().Select(item => item.Text).ToList();
                    
                    ImageFileService.DeleteFiles(filesToDelete);

                    this.lstUnmatchedFiles.Items.Clear();
                    this.lblStatus.Text = $"{filesToDelete.Count}個のファイルを削除しました。";
                    MessageBox.Show("ファイルの削除が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"エラーが発生しました: {ex.Message}", "削除エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.lblStatus.Text = "エラーが発生しました。";
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
    }
}