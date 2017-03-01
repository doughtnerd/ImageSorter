using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void sourceButton_Click(object sender, EventArgs e)
        {
            using(var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.SelectedPath;
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
                    this.targetTextBox.Text = path;
                    TargetChosenEvent?.Invoke(path);
                }
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            StartEvent?.Invoke();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CancelEvent?.Invoke();
        }

        private void numberOfThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThreadsChosenEvent?.Invoke(int.Parse(this.numberOfThreads.Text));
        }

        private void copyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CopyFilesEvent?.Invoke(this.copyCheckBox.Checked);
        }

        private void duplicatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AllowDuplicatesEvent?.Invoke(this.duplicatesCheckBox.Checked);
        }
    }
}
