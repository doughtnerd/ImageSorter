namespace ImageSorter
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            this.sourceTextBox = new System.Windows.Forms.TextBox();
            this.targetTextBox = new System.Windows.Forms.TextBox();
            this.sourceFolderLabel = new System.Windows.Forms.Label();
            this.targetFolderLabel = new System.Windows.Forms.Label();
            this.sourceButton = new System.Windows.Forms.Button();
            this.targetButton = new System.Windows.Forms.Button();
            this.numberOfThreads = new System.Windows.Forms.ComboBox();
            this.threadCountLabel = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.mainTab = new System.Windows.Forms.TabPage();
            this.optionsTab = new System.Windows.Forms.TabPage();
            this.additionalLabel = new System.Windows.Forms.Label();
            this.duplicatesCheckBox = new System.Windows.Forms.CheckBox();
            this.copyCheckBox = new System.Windows.Forms.CheckBox();
            this.tabs.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.optionsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceTextBox.Location = new System.Drawing.Point(6, 34);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.ReadOnly = true;
            this.sourceTextBox.Size = new System.Drawing.Size(313, 20);
            this.sourceTextBox.TabIndex = 0;
            // 
            // targetTextBox
            // 
            this.targetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetTextBox.Location = new System.Drawing.Point(7, 113);
            this.targetTextBox.Name = "targetTextBox";
            this.targetTextBox.ReadOnly = true;
            this.targetTextBox.Size = new System.Drawing.Size(312, 20);
            this.targetTextBox.TabIndex = 1;
            // 
            // sourceFolderLabel
            // 
            this.sourceFolderLabel.AutoSize = true;
            this.sourceFolderLabel.Location = new System.Drawing.Point(7, 15);
            this.sourceFolderLabel.Name = "sourceFolderLabel";
            this.sourceFolderLabel.Size = new System.Drawing.Size(117, 13);
            this.sourceFolderLabel.TabIndex = 2;
            this.sourceFolderLabel.Text = "Source Folder Location";
            // 
            // targetFolderLabel
            // 
            this.targetFolderLabel.AutoSize = true;
            this.targetFolderLabel.Location = new System.Drawing.Point(10, 94);
            this.targetFolderLabel.Name = "targetFolderLabel";
            this.targetFolderLabel.Size = new System.Drawing.Size(114, 13);
            this.targetFolderLabel.TabIndex = 3;
            this.targetFolderLabel.Text = "Target Folder Location";
            // 
            // sourceButton
            // 
            this.sourceButton.Location = new System.Drawing.Point(6, 60);
            this.sourceButton.Name = "sourceButton";
            this.sourceButton.Size = new System.Drawing.Size(75, 20);
            this.sourceButton.TabIndex = 4;
            this.sourceButton.Text = "Browse";
            this.sourceButton.UseVisualStyleBackColor = true;
            this.sourceButton.Click += new System.EventHandler(this.sourceButton_Click);
            // 
            // targetButton
            // 
            this.targetButton.Location = new System.Drawing.Point(7, 139);
            this.targetButton.Name = "targetButton";
            this.targetButton.Size = new System.Drawing.Size(75, 20);
            this.targetButton.TabIndex = 5;
            this.targetButton.Text = "Browse";
            this.targetButton.UseVisualStyleBackColor = true;
            this.targetButton.Click += new System.EventHandler(this.targetButton_Click);
            // 
            // numberOfThreads
            // 
            this.numberOfThreads.FormattingEnabled = true;
            this.numberOfThreads.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.numberOfThreads.Location = new System.Drawing.Point(7, 193);
            this.numberOfThreads.Name = "numberOfThreads";
            this.numberOfThreads.Size = new System.Drawing.Size(98, 21);
            this.numberOfThreads.TabIndex = 6;
            this.numberOfThreads.SelectedIndexChanged += new System.EventHandler(this.numberOfThreads_SelectedIndexChanged);
            // 
            // threadCountLabel
            // 
            this.threadCountLabel.AutoSize = true;
            this.threadCountLabel.Location = new System.Drawing.Point(7, 174);
            this.threadCountLabel.Name = "threadCountLabel";
            this.threadCountLabel.Size = new System.Drawing.Size(98, 13);
            this.threadCountLabel.TabIndex = 7;
            this.threadCountLabel.Text = "Number of Threads";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(155, 191);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 8;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(236, 191);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.mainTab);
            this.tabs.Controls.Add(this.optionsTab);
            this.tabs.Location = new System.Drawing.Point(3, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(335, 260);
            this.tabs.TabIndex = 10;
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.sourceFolderLabel);
            this.mainTab.Controls.Add(this.cancelButton);
            this.mainTab.Controls.Add(this.sourceTextBox);
            this.mainTab.Controls.Add(this.startButton);
            this.mainTab.Controls.Add(this.targetTextBox);
            this.mainTab.Controls.Add(this.threadCountLabel);
            this.mainTab.Controls.Add(this.targetFolderLabel);
            this.mainTab.Controls.Add(this.numberOfThreads);
            this.mainTab.Controls.Add(this.sourceButton);
            this.mainTab.Controls.Add(this.targetButton);
            this.mainTab.Location = new System.Drawing.Point(4, 22);
            this.mainTab.Name = "mainTab";
            this.mainTab.Padding = new System.Windows.Forms.Padding(3);
            this.mainTab.Size = new System.Drawing.Size(327, 234);
            this.mainTab.TabIndex = 0;
            this.mainTab.Text = "Main";
            this.mainTab.UseVisualStyleBackColor = true;
            // 
            // optionsTab
            // 
            this.optionsTab.Controls.Add(this.additionalLabel);
            this.optionsTab.Controls.Add(this.duplicatesCheckBox);
            this.optionsTab.Controls.Add(this.copyCheckBox);
            this.optionsTab.Location = new System.Drawing.Point(4, 22);
            this.optionsTab.Name = "optionsTab";
            this.optionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.optionsTab.Size = new System.Drawing.Size(327, 234);
            this.optionsTab.TabIndex = 1;
            this.optionsTab.Text = "Options";
            this.optionsTab.UseVisualStyleBackColor = true;
            // 
            // additionalLabel
            // 
            this.additionalLabel.AutoSize = true;
            this.additionalLabel.Location = new System.Drawing.Point(6, 3);
            this.additionalLabel.Name = "additionalLabel";
            this.additionalLabel.Size = new System.Drawing.Size(94, 13);
            this.additionalLabel.TabIndex = 2;
            this.additionalLabel.Text = "Additional Settings";
            // 
            // duplicatesCheckBox
            // 
            this.duplicatesCheckBox.AutoSize = true;
            this.duplicatesCheckBox.Location = new System.Drawing.Point(7, 42);
            this.duplicatesCheckBox.Name = "duplicatesCheckBox";
            this.duplicatesCheckBox.Size = new System.Drawing.Size(104, 17);
            this.duplicatesCheckBox.TabIndex = 1;
            this.duplicatesCheckBox.Text = "Allow Duplicates";
            this.duplicatesCheckBox.UseVisualStyleBackColor = true;
            this.duplicatesCheckBox.CheckedChanged += new System.EventHandler(this.duplicatesCheckBox_CheckedChanged);
            // 
            // copyCheckBox
            // 
            this.copyCheckBox.AutoSize = true;
            this.copyCheckBox.Location = new System.Drawing.Point(7, 19);
            this.copyCheckBox.Name = "copyCheckBox";
            this.copyCheckBox.Size = new System.Drawing.Size(74, 17);
            this.copyCheckBox.TabIndex = 0;
            this.copyCheckBox.Text = "Copy Files";
            this.copyCheckBox.UseVisualStyleBackColor = true;
            this.copyCheckBox.CheckedChanged += new System.EventHandler(this.copyCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(339, 261);
            this.Controls.Add(this.tabs);
            this.MaximumSize = new System.Drawing.Size(355, 300);
            this.MinimumSize = new System.Drawing.Size(355, 300);
            this.Name = "MainForm";
            this.Text = "Image Sorter";
            this.tabs.ResumeLayout(false);
            this.mainTab.ResumeLayout(false);
            this.mainTab.PerformLayout();
            this.optionsTab.ResumeLayout(false);
            this.optionsTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox sourceTextBox;
        private System.Windows.Forms.TextBox targetTextBox;
        private System.Windows.Forms.Label sourceFolderLabel;
        private System.Windows.Forms.Label targetFolderLabel;
        private System.Windows.Forms.Button sourceButton;
        private System.Windows.Forms.Button targetButton;
        private System.Windows.Forms.ComboBox numberOfThreads;
        private System.Windows.Forms.Label threadCountLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage mainTab;
        private System.Windows.Forms.TabPage optionsTab;
        private System.Windows.Forms.CheckBox duplicatesCheckBox;
        private System.Windows.Forms.CheckBox copyCheckBox;
        private System.Windows.Forms.Label additionalLabel;
    }
}

