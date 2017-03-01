using System;
using System.IO;
using System.Windows.Forms;

namespace ImageSorter
{
    public partial class MainForm : Form, ISorterView
    {
        public event Action<bool> AllowDuplicatesEvent;
        public event Action CancelEvent;
        public event Action<bool> CopyFilesEvent;
        public event Action<string> SourceChosenEvent;
        public event Action StartEvent;
        public event Action<string> TargetChosenEvent;
        public event Action<int> ThreadsChosenEvent;

        private string sourcePath;
        private string targetPath;
        private int numThreads;

        public MainForm()
        {
            InitializeComponent();
        }

        private void sourceButton_Click(object sender, EventArgs e)
        {
            using(var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.SelectedPath;
                    this.sourcePath = path;
                    this.sourceTextBox.Text = path;
                    SourceChosenEvent?.Invoke(path);
                }
            }
        }

        private void targetButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.SelectedPath;
                    this.targetPath = path;
                    this.targetTextBox.Text = path;
                    TargetChosenEvent?.Invoke(path);
                }
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(targetPath) && Directory.Exists(sourcePath))
            {
                DisableControls();
                StartEvent?.Invoke();
            }

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CancelEvent?.Invoke();
            ResetControls();
        }

        private void numberOfThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            int numThreads = int.Parse(this.numberOfThreads.Text);
            this.numThreads = numThreads;
            ThreadsChosenEvent?.Invoke(numThreads);
        }

        private void copyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CopyFilesEvent?.Invoke(this.copyCheckBox.Checked);
        }

        private void duplicatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AllowDuplicatesEvent?.Invoke(this.duplicatesCheckBox.Checked);
        }

        public void ResetControls()
        {
            this.sourceButton.Enabled = true;
            this.sourceTextBox.Enabled = true;
            this.targetButton.Enabled = true;
            this.targetTextBox.Enabled = true;
            this.startButton.Enabled = true;
            this.numberOfThreads.Enabled = true;
            this.copyCheckBox.Enabled = true;
            this.duplicatesCheckBox.Enabled = true;
        }

        public void DisableControls()
        {
            this.sourceButton.Enabled = false;
            this.sourceTextBox.Enabled = false;
            this.targetButton.Enabled = false;
            this.targetTextBox.Enabled = false;
            this.startButton.Enabled = false;
            this.numberOfThreads.Enabled = false;
            this.copyCheckBox.Enabled = false;
            this.duplicatesCheckBox.Enabled = false;
        }

        public void Invoke(Action action)
        {
            this.cancelButton.Invoke(action);
        }
    }
}
