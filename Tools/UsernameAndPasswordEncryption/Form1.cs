using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Inmeta.Exception.Service.Common.Sec;

namespace UsernameAndPasswordEncryption
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void genAppSettingsBtn_Click(object sender, EventArgs e)
        {
            var form = new Form();
            form.Text = @"Copy this text to clipboard and paste into <appsettings> section in the [ApplicationName].exe.config file.";
            var textbox = new TextBox();
            form.Size = new Size(400, 100);
            form.Controls.Add(textbox);
            textbox.Parent = form;
            textbox.Multiline = true;
            textbox.Dock = DockStyle.Fill;
            textbox.Text = 
                @"<add key=""username"" value=""" + userNameEncrypt.Text + @"""/>" +
                System.Environment.NewLine + 
                @"<add key=""password"" value=""" + passwordEncrypt.Text + @"""/>" +
                System.Environment.NewLine +
                @"<add key=""domain"" value=""" + domainEncrypt.Text + @"""/>";
            form.ShowDialog(this);
        }

        private void usernameTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            userNameEncrypt.Text = new Encryption().Encrypt(usernameTxtBox.Text);
        }

        private void PasswordTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            passwordEncrypt.Text = new Encryption().Encrypt(PasswordTxtBox.Text);
        }

        private void domainEncrypt_KeyUp(object sender, KeyEventArgs e)
        {
            domainEncrypt.Text = new Encryption().Encrypt(domainTxtBox.Text);
        }
    }
}
