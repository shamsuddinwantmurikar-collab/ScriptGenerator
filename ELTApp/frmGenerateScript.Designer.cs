namespace ELTApp
{
    partial class frmGenerateScript
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerateScript));
            buttonGenerate = new Button();
            txtInsert = new RichTextBox();
            checkedListBoxTables = new CheckedListBox();
            txtFilter = new TextBox();
            btnLoad = new Button();
            txtServerName = new TextBox();
            txtDbName = new TextBox();
            txtUserName = new TextBox();
            txtPassword = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            chkTrusted = new CheckBox();
            btnClear = new Button();
            lblMessage = new Label();
            txtSiteFilter = new TextBox();
            label5 = new Label();
            label6 = new Label();
            chkSelectAll = new CheckBox();
            SuspendLayout();
            // 
            // buttonGenerate
            // 
            buttonGenerate.BackColor = SystemColors.ActiveCaption;
            buttonGenerate.Font = new Font("Tahoma", 11.25F);
            buttonGenerate.ForeColor = SystemColors.ButtonHighlight;
            buttonGenerate.Location = new Point(562, 183);
            buttonGenerate.Name = "buttonGenerate";
            buttonGenerate.Size = new Size(132, 31);
            buttonGenerate.TabIndex = 11;
            buttonGenerate.Text = "Generate";
            buttonGenerate.UseVisualStyleBackColor = false;
            buttonGenerate.Click += buttonGenerate_Click;
            // 
            // txtInsert
            // 
            txtInsert.BorderStyle = BorderStyle.FixedSingle;
            txtInsert.Font = new Font("Tahoma", 11.25F);
            txtInsert.ForeColor = SystemColors.Highlight;
            txtInsert.Location = new Point(356, 220);
            txtInsert.Name = "txtInsert";
            txtInsert.ReadOnly = true;
            txtInsert.Size = new Size(998, 613);
            txtInsert.TabIndex = 14;
            txtInsert.Text = "";
            // 
            // checkedListBoxTables
            // 
            checkedListBoxTables.BorderStyle = BorderStyle.None;
            checkedListBoxTables.Font = new Font("Tahoma", 11.25F);
            checkedListBoxTables.ForeColor = SystemColors.Highlight;
            checkedListBoxTables.FormattingEnabled = true;
            checkedListBoxTables.Location = new Point(3, 245);
            checkedListBoxTables.Name = "checkedListBoxTables";
            checkedListBoxTables.Size = new Size(338, 588);
            checkedListBoxTables.Sorted = true;
            checkedListBoxTables.TabIndex = 13;
            checkedListBoxTables.ItemCheck += checkedListBoxTables_ItemCheck;
            // 
            // txtFilter
            // 
            txtFilter.CharacterCasing = CharacterCasing.Upper;
            txtFilter.Font = new Font("Tahoma", 11.25F);
            txtFilter.ForeColor = SystemColors.Highlight;
            txtFilter.Location = new Point(3, 188);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(200, 26);
            txtFilter.TabIndex = 9;
            // 
            // btnLoad
            // 
            btnLoad.BackColor = SystemColors.ActiveCaption;
            btnLoad.Font = new Font("Tahoma", 11.25F);
            btnLoad.ForeColor = SystemColors.ButtonHighlight;
            btnLoad.Location = new Point(209, 183);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(132, 31);
            btnLoad.TabIndex = 10;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += btnLoad_Click;
            // 
            // txtServerName
            // 
            txtServerName.CharacterCasing = CharacterCasing.Upper;
            txtServerName.Font = new Font("Tahoma", 11.25F);
            txtServerName.ForeColor = SystemColors.Highlight;
            txtServerName.Location = new Point(129, 23);
            txtServerName.Name = "txtServerName";
            txtServerName.Size = new Size(250, 26);
            txtServerName.TabIndex = 1;
            // 
            // txtDbName
            // 
            txtDbName.CharacterCasing = CharacterCasing.Upper;
            txtDbName.Font = new Font("Tahoma", 11.25F);
            txtDbName.ForeColor = SystemColors.Highlight;
            txtDbName.Location = new Point(129, 55);
            txtDbName.Name = "txtDbName";
            txtDbName.Size = new Size(250, 26);
            txtDbName.TabIndex = 3;
            // 
            // txtUserName
            // 
            txtUserName.Font = new Font("Tahoma", 11.25F);
            txtUserName.ForeColor = SystemColors.Highlight;
            txtUserName.Location = new Point(129, 87);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(250, 26);
            txtUserName.TabIndex = 5;
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Tahoma", 11.25F);
            txtPassword.ForeColor = SystemColors.Highlight;
            txtPassword.Location = new Point(129, 119);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(250, 26);
            txtPassword.TabIndex = 7;
            // 
            // label1
            // 
            label1.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            label1.ForeColor = SystemColors.ButtonHighlight;
            label1.Location = new Point(7, 29);
            label1.Name = "label1";
            label1.Size = new Size(120, 15);
            label1.TabIndex = 0;
            label1.Text = "DB Server Name:";
            label1.TextAlign = ContentAlignment.TopRight;
            // 
            // label2
            // 
            label2.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            label2.ForeColor = SystemColors.ButtonHighlight;
            label2.Location = new Point(7, 61);
            label2.Name = "label2";
            label2.Size = new Size(120, 15);
            label2.TabIndex = 2;
            label2.Text = "DB  Name:";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // label3
            // 
            label3.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            label3.ForeColor = SystemColors.ButtonHighlight;
            label3.Location = new Point(7, 93);
            label3.Name = "label3";
            label3.Size = new Size(120, 15);
            label3.TabIndex = 4;
            label3.Text = "User Name:";
            label3.TextAlign = ContentAlignment.TopRight;
            // 
            // label4
            // 
            label4.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            label4.ForeColor = SystemColors.ButtonHighlight;
            label4.Location = new Point(7, 125);
            label4.Name = "label4";
            label4.Size = new Size(120, 15);
            label4.TabIndex = 6;
            label4.Text = "Password:";
            label4.TextAlign = ContentAlignment.TopRight;
            // 
            // chkTrusted
            // 
            chkTrusted.AutoSize = true;
            chkTrusted.CheckAlign = ContentAlignment.MiddleRight;
            chkTrusted.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            chkTrusted.ForeColor = SystemColors.ButtonHighlight;
            chkTrusted.Location = new Point(129, 151);
            chkTrusted.Name = "chkTrusted";
            chkTrusted.Size = new Size(157, 20);
            chkTrusted.TabIndex = 8;
            chkTrusted.Text = "Trusted Connection:";
            chkTrusted.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.BackColor = SystemColors.ActiveCaption;
            btnClear.Font = new Font("Tahoma", 11.25F);
            btnClear.ForeColor = SystemColors.ButtonHighlight;
            btnClear.Location = new Point(1222, 185);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(132, 31);
            btnClear.TabIndex = 12;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // lblMessage
            // 
            lblMessage.Font = new Font("Tahoma", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblMessage.ForeColor = SystemColors.ButtonHighlight;
            lblMessage.Location = new Point(700, 153);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(516, 63);
            lblMessage.TabIndex = 15;
            lblMessage.Text = "Status";
            // 
            // txtSiteFilter
            // 
            txtSiteFilter.CharacterCasing = CharacterCasing.Upper;
            txtSiteFilter.Font = new Font("Tahoma", 11.25F);
            txtSiteFilter.ForeColor = SystemColors.Highlight;
            txtSiteFilter.Location = new Point(356, 188);
            txtSiteFilter.Name = "txtSiteFilter";
            txtSiteFilter.Size = new Size(200, 26);
            txtSiteFilter.TabIndex = 9;
            // 
            // label5
            // 
            label5.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            label5.ForeColor = SystemColors.ButtonHighlight;
            label5.Location = new Point(356, 170);
            label5.Name = "label5";
            label5.Size = new Size(120, 15);
            label5.TabIndex = 4;
            label5.Text = "Site Filter:";
            // 
            // label6
            // 
            label6.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            label6.ForeColor = SystemColors.ButtonHighlight;
            label6.Location = new Point(3, 170);
            label6.Name = "label6";
            label6.Size = new Size(120, 15);
            label6.TabIndex = 4;
            label6.Text = "Table Filter:";
            // 
            // chkSelectAll
            // 
            chkSelectAll.AutoSize = true;
            chkSelectAll.CheckAlign = ContentAlignment.MiddleRight;
            chkSelectAll.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            chkSelectAll.ForeColor = SystemColors.ButtonHighlight;
            chkSelectAll.Location = new Point(3, 220);
            chkSelectAll.Name = "chkSelectAll";
            chkSelectAll.Size = new Size(91, 20);
            chkSelectAll.TabIndex = 8;
            chkSelectAll.Text = "Select All:";
            chkSelectAll.UseVisualStyleBackColor = true;
            chkSelectAll.CheckedChanged += chkSelectAll_CheckedChanged;
            // 
            // frmGenerateScript
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1368, 847);
            Controls.Add(chkSelectAll);
            Controls.Add(chkTrusted);
            Controls.Add(lblMessage);
            Controls.Add(label4);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPassword);
            Controls.Add(txtUserName);
            Controls.Add(txtDbName);
            Controls.Add(txtServerName);
            Controls.Add(btnLoad);
            Controls.Add(txtSiteFilter);
            Controls.Add(txtFilter);
            Controls.Add(checkedListBoxTables);
            Controls.Add(txtInsert);
            Controls.Add(btnClear);
            Controls.Add(buttonGenerate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmGenerateScript";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Generate Scripts";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonGenerate;
        private RichTextBox txtInsert;
        private CheckedListBox checkedListBoxTables;
        private TextBox txtFilter;
        private Button btnLoad;
        private TextBox txtServerName;
        private TextBox txtDbName;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private CheckBox chkTrusted;
        private Button btnClear;
        private Label lblMessage;
        private TextBox txtSiteFilter;
        private Label label5;
        private Label label6;
        private CheckBox chkSelectAll;
    }
}
