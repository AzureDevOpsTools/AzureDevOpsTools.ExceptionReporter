namespace ExceptionReporterTestApp
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
            this.btnFail = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblServiceUrl = new System.Windows.Forms.Label();
            this.chkNewWi = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFail
            // 
            this.btnFail.Location = new System.Drawing.Point(174, 42);
            this.btnFail.Name = "btnFail";
            this.btnFail.Size = new System.Drawing.Size(91, 23);
            this.btnFail.TabIndex = 0;
            this.btnFail.Text = "Post Exception";
            this.btnFail.UseVisualStyleBackColor = true;
            this.btnFail.Click += new System.EventHandler(this.btnFail_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Service URL:";
            // 
            // lblServiceUrl
            // 
            this.lblServiceUrl.AutoSize = true;
            this.lblServiceUrl.Location = new System.Drawing.Point(89, 9);
            this.lblServiceUrl.Name = "lblServiceUrl";
            this.lblServiceUrl.Size = new System.Drawing.Size(63, 13);
            this.lblServiceUrl.TabIndex = 2;
            this.lblServiceUrl.Text = "<unknown>";
            // 
            // chkNewWi
            // 
            this.chkNewWi.AutoSize = true;
            this.chkNewWi.Location = new System.Drawing.Point(12, 42);
            this.chkNewWi.Name = "chkNewWi";
            this.chkNewWi.Size = new System.Drawing.Size(121, 17);
            this.chkNewWi.TabIndex = 4;
            this.chkNewWi.Text = "Force new workitem";
            this.chkNewWi.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(174, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Post Thread Exception";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnThreadFail_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 149);
            this.Controls.Add(this.chkNewWi);
            this.Controls.Add(this.lblServiceUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnFail);
            this.Name = "Form1";
            this.Text = "Exception Reporter Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFail;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblServiceUrl;
        private System.Windows.Forms.CheckBox chkNewWi;
        private System.Windows.Forms.Button button1;
    }
}

