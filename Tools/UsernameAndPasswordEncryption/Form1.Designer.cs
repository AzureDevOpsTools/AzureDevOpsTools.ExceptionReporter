namespace UsernameAndPasswordEncryption
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.usernameTxtBox = new System.Windows.Forms.TextBox();
            this.PasswordTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.userNameEncrypt = new System.Windows.Forms.TextBox();
            this.passwordEncrypt = new System.Windows.Forms.TextBox();
            this.genAppSettingsBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.domainEncrypt = new System.Windows.Forms.TextBox();
            this.domainTxtBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Password";
            // 
            // usernameTxtBox
            // 
            this.usernameTxtBox.Location = new System.Drawing.Point(96, 15);
            this.usernameTxtBox.Name = "usernameTxtBox";
            this.usernameTxtBox.Size = new System.Drawing.Size(227, 20);
            this.usernameTxtBox.TabIndex = 1;
            this.usernameTxtBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.usernameTxtBox_KeyUp);
            // 
            // PasswordTxtBox
            // 
            this.PasswordTxtBox.Location = new System.Drawing.Point(96, 45);
            this.PasswordTxtBox.Name = "PasswordTxtBox";
            this.PasswordTxtBox.PasswordChar = '*';
            this.PasswordTxtBox.Size = new System.Drawing.Size(227, 20);
            this.PasswordTxtBox.TabIndex = 2;
            this.PasswordTxtBox.UseSystemPasswordChar = true;
            this.PasswordTxtBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PasswordTxtBox_KeyUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(329, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "-->";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(329, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "-->";
            // 
            // userNameEncrypt
            // 
            this.userNameEncrypt.Enabled = false;
            this.userNameEncrypt.Location = new System.Drawing.Point(354, 15);
            this.userNameEncrypt.Name = "userNameEncrypt";
            this.userNameEncrypt.Size = new System.Drawing.Size(227, 20);
            this.userNameEncrypt.TabIndex = 2;
            this.userNameEncrypt.TabStop = false;
            // 
            // passwordEncrypt
            // 
            this.passwordEncrypt.Enabled = false;
            this.passwordEncrypt.Location = new System.Drawing.Point(354, 45);
            this.passwordEncrypt.Name = "passwordEncrypt";
            this.passwordEncrypt.Size = new System.Drawing.Size(227, 20);
            this.passwordEncrypt.TabIndex = 4;
            this.passwordEncrypt.TabStop = false;
            // 
            // genAppSettingsBtn
            // 
            this.genAppSettingsBtn.Location = new System.Drawing.Point(436, 100);
            this.genAppSettingsBtn.Name = "genAppSettingsBtn";
            this.genAppSettingsBtn.Size = new System.Drawing.Size(145, 23);
            this.genAppSettingsBtn.TabIndex = 4;
            this.genAppSettingsBtn.Text = "Generate AppSettings";
            this.genAppSettingsBtn.UseVisualStyleBackColor = true;
            this.genAppSettingsBtn.Click += new System.EventHandler(this.genAppSettingsBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(329, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "-->";
            // 
            // domainEncrypt
            // 
            this.domainEncrypt.Enabled = false;
            this.domainEncrypt.Location = new System.Drawing.Point(354, 71);
            this.domainEncrypt.Name = "domainEncrypt";
            this.domainEncrypt.Size = new System.Drawing.Size(227, 20);
            this.domainEncrypt.TabIndex = 8;
            this.domainEncrypt.TabStop = false;
            // 
            // domainTxtBox
            // 
            this.domainTxtBox.Location = new System.Drawing.Point(96, 71);
            this.domainTxtBox.Name = "domainTxtBox";
            this.domainTxtBox.Size = new System.Drawing.Size(227, 20);
            this.domainTxtBox.TabIndex = 6;
            this.domainTxtBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.domainEncrypt_KeyUp);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(35, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Domain";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 142);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.domainEncrypt);
            this.Controls.Add(this.domainTxtBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.genAppSettingsBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.passwordEncrypt);
            this.Controls.Add(this.userNameEncrypt);
            this.Controls.Add(this.PasswordTxtBox);
            this.Controls.Add(this.usernameTxtBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PasswordTxtBox;
        private System.Windows.Forms.TextBox usernameTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox userNameEncrypt;
        private System.Windows.Forms.TextBox passwordEncrypt;
        private System.Windows.Forms.Button genAppSettingsBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox domainEncrypt;
        private System.Windows.Forms.TextBox domainTxtBox;
        private System.Windows.Forms.Label label6;
    }
}

