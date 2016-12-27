namespace PrintService
{
    partial class Configuration
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.acumaticaUrlTextBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.paperSourceCombo = new System.Windows.Forms.ComboBox();
            this.orientationGroupBox = new System.Windows.Forms.GroupBox();
            this.orientationLandscape = new System.Windows.Forms.RadioButton();
            this.orientationPortrait = new System.Windows.Forms.RadioButton();
            this.orientationDefault = new System.Windows.Forms.RadioButton();
            this.paperSizeCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.printerCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.queueName = new System.Windows.Forms.TextBox();
            this.removePrintQueue = new System.Windows.Forms.Button();
            this.addPrintQueue = new System.Windows.Forms.Button();
            this.queueList = new System.Windows.Forms.ListBox();
            this.mainTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.orientationGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(352, 297);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(433, 297);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.tabPage1);
            this.mainTab.Controls.Add(this.tabPage2);
            this.mainTab.Location = new System.Drawing.Point(1, 1);
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(507, 295);
            this.mainTab.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.passwordTextBox);
            this.tabPage1.Controls.Add(this.loginTextBox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.acumaticaUrlTextBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(499, 269);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Site";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Login:";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(68, 58);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(126, 20);
            this.passwordTextBox.TabIndex = 17;
            // 
            // loginTextBox
            // 
            this.loginTextBox.Location = new System.Drawing.Point(68, 32);
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(126, 20);
            this.loginTextBox.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "URL:";
            // 
            // acumaticaUrlTextBox
            // 
            this.acumaticaUrlTextBox.Location = new System.Drawing.Point(68, 6);
            this.acumaticaUrlTextBox.MaxLength = 3200;
            this.acumaticaUrlTextBox.Name = "acumaticaUrlTextBox";
            this.acumaticaUrlTextBox.Size = new System.Drawing.Size(309, 20);
            this.acumaticaUrlTextBox.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.paperSourceCombo);
            this.tabPage2.Controls.Add(this.orientationGroupBox);
            this.tabPage2.Controls.Add(this.paperSizeCombo);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.printerCombo);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.queueName);
            this.tabPage2.Controls.Add(this.removePrintQueue);
            this.tabPage2.Controls.Add(this.addPrintQueue);
            this.tabPage2.Controls.Add(this.queueList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(499, 269);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Queues";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(115, 93);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Paper Bin:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(115, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Paper Size:";
            // 
            // paperSourceCombo
            // 
            this.paperSourceCombo.DisplayMember = "SourceName";
            this.paperSourceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paperSourceCombo.FormattingEnabled = true;
            this.paperSourceCombo.Location = new System.Drawing.Point(182, 90);
            this.paperSourceCombo.Name = "paperSourceCombo";
            this.paperSourceCombo.Size = new System.Drawing.Size(156, 21);
            this.paperSourceCombo.TabIndex = 9;
            this.paperSourceCombo.ValueMember = "RawKind";
            this.paperSourceCombo.SelectedIndexChanged += new System.EventHandler(this.paperSourceCombo_SelectedIndexChanged);
            // 
            // orientationGroupBox
            // 
            this.orientationGroupBox.Controls.Add(this.orientationLandscape);
            this.orientationGroupBox.Controls.Add(this.orientationPortrait);
            this.orientationGroupBox.Controls.Add(this.orientationDefault);
            this.orientationGroupBox.Location = new System.Drawing.Point(118, 117);
            this.orientationGroupBox.Name = "orientationGroupBox";
            this.orientationGroupBox.Size = new System.Drawing.Size(220, 88);
            this.orientationGroupBox.TabIndex = 10;
            this.orientationGroupBox.TabStop = false;
            this.orientationGroupBox.Text = "Orientation";
            // 
            // orientationLandscape
            // 
            this.orientationLandscape.AutoSize = true;
            this.orientationLandscape.Location = new System.Drawing.Point(6, 65);
            this.orientationLandscape.Name = "orientationLandscape";
            this.orientationLandscape.Size = new System.Drawing.Size(78, 17);
            this.orientationLandscape.TabIndex = 2;
            this.orientationLandscape.TabStop = true;
            this.orientationLandscape.Tag = "2";
            this.orientationLandscape.Text = "&Landscape";
            this.orientationLandscape.UseVisualStyleBackColor = true;
            this.orientationLandscape.CheckedChanged += new System.EventHandler(this.orientationLandscape_CheckedChanged);
            // 
            // orientationPortrait
            // 
            this.orientationPortrait.AutoSize = true;
            this.orientationPortrait.Location = new System.Drawing.Point(6, 42);
            this.orientationPortrait.Name = "orientationPortrait";
            this.orientationPortrait.Size = new System.Drawing.Size(58, 17);
            this.orientationPortrait.TabIndex = 1;
            this.orientationPortrait.TabStop = true;
            this.orientationPortrait.Tag = "1";
            this.orientationPortrait.Text = "&Portrait";
            this.orientationPortrait.UseVisualStyleBackColor = true;
            this.orientationPortrait.CheckedChanged += new System.EventHandler(this.orientationPortrait_CheckedChanged);
            // 
            // orientationDefault
            // 
            this.orientationDefault.AutoSize = true;
            this.orientationDefault.Location = new System.Drawing.Point(6, 19);
            this.orientationDefault.Name = "orientationDefault";
            this.orientationDefault.Size = new System.Drawing.Size(92, 17);
            this.orientationDefault.TabIndex = 0;
            this.orientationDefault.TabStop = true;
            this.orientationDefault.Tag = "-1";
            this.orientationDefault.Text = "Printer &Default";
            this.orientationDefault.UseVisualStyleBackColor = true;
            this.orientationDefault.CheckedChanged += new System.EventHandler(this.orientationDefault_CheckedChanged);
            // 
            // paperSizeCombo
            // 
            this.paperSizeCombo.DisplayMember = "PaperName";
            this.paperSizeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.paperSizeCombo.FormattingEnabled = true;
            this.paperSizeCombo.Location = new System.Drawing.Point(182, 63);
            this.paperSizeCombo.Name = "paperSizeCombo";
            this.paperSizeCombo.Size = new System.Drawing.Size(156, 21);
            this.paperSizeCombo.TabIndex = 8;
            this.paperSizeCombo.ValueMember = "RawKind";
            this.paperSizeCombo.SelectedIndexChanged += new System.EventHandler(this.paperSizeCombo_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Printer:";
            // 
            // printerCombo
            // 
            this.printerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.printerCombo.FormattingEnabled = true;
            this.printerCombo.Location = new System.Drawing.Point(182, 36);
            this.printerCombo.Name = "printerCombo";
            this.printerCombo.Size = new System.Drawing.Size(309, 21);
            this.printerCombo.TabIndex = 7;
            this.printerCombo.SelectedIndexChanged += new System.EventHandler(this.printerCombo_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Name:";
            // 
            // queueName
            // 
            this.queueName.Location = new System.Drawing.Point(182, 10);
            this.queueName.MaxLength = 10;
            this.queueName.Name = "queueName";
            this.queueName.Size = new System.Drawing.Size(156, 20);
            this.queueName.TabIndex = 6;
            this.queueName.TextChanged += new System.EventHandler(this.queueName_TextChanged);
            this.queueName.Validated += new System.EventHandler(this.queueName_Validated);
            // 
            // removePrintQueue
            // 
            this.removePrintQueue.Location = new System.Drawing.Point(6, 239);
            this.removePrintQueue.Name = "removePrintQueue";
            this.removePrintQueue.Size = new System.Drawing.Size(98, 23);
            this.removePrintQueue.TabIndex = 5;
            this.removePrintQueue.Text = "Remove";
            this.removePrintQueue.UseVisualStyleBackColor = true;
            this.removePrintQueue.Click += new System.EventHandler(this.removePrintQueue_Click);
            // 
            // addPrintQueue
            // 
            this.addPrintQueue.Location = new System.Drawing.Point(6, 210);
            this.addPrintQueue.Name = "addPrintQueue";
            this.addPrintQueue.Size = new System.Drawing.Size(98, 23);
            this.addPrintQueue.TabIndex = 4;
            this.addPrintQueue.Text = "Add";
            this.addPrintQueue.UseVisualStyleBackColor = true;
            this.addPrintQueue.Click += new System.EventHandler(this.addPrintQueue_Click);
            // 
            // queueList
            // 
            this.queueList.DisplayMember = "QueueName";
            this.queueList.FormattingEnabled = true;
            this.queueList.Location = new System.Drawing.Point(6, 6);
            this.queueList.Name = "queueList";
            this.queueList.Size = new System.Drawing.Size(98, 199);
            this.queueList.TabIndex = 3;
            this.queueList.SelectedIndexChanged += new System.EventHandler(this.queueList_SelectedIndexChanged);
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(508, 325);
            this.Controls.Add(this.mainTab);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Configuration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.Configuration_Load);
            this.mainTab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.orientationGroupBox.ResumeLayout(false);
            this.orientationGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl mainTab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox acumaticaUrlTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox printerCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox queueName;
        private System.Windows.Forms.Button removePrintQueue;
        private System.Windows.Forms.Button addPrintQueue;
        private System.Windows.Forms.ListBox queueList;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox paperSourceCombo;
        private System.Windows.Forms.GroupBox orientationGroupBox;
        private System.Windows.Forms.ComboBox paperSizeCombo;
        private System.Windows.Forms.RadioButton orientationLandscape;
        private System.Windows.Forms.RadioButton orientationPortrait;
        private System.Windows.Forms.RadioButton orientationDefault;
    }
}