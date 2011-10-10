using System;
using System.CodeDom;
using System.IO;
using System.Windows.Forms;
using System.Messaging;
using Microsoft.CSharp;
using Inmeta.Exception.Service.Common;
using Message = System.Messaging.Message;

namespace MsmqView
{
    public partial class Form1 : Form
    {
        private string _server;


        public Form1()
        {
            InitializeComponent();
            ConnectedToServer(false);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectToServer(txtServer.Text);
        }




        private void lblServer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnConnect_Click(sender, e);
        }

        private void lstQueues_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectQueue((MessageQueue)lstQueues.SelectedItem);
        }

        void ConnectToServer(string server)
        {
            _server = null;
            try
            {
                var queues = MessageQueue.GetPrivateQueuesByMachine(txtServer.Text);
                BindQueues(queues);
                _server = txtServer.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ConnectedToServer(bool connected)
        {
            lstQueues.Enabled = connected;
            btnAdd.Enabled = connected;
            btnDelete.Enabled = connected;
            txtQueueName.Enabled = connected;
            btnQueueRefresh.Enabled = connected;
            if (!connected)
                QueueSelected(false);
        }

        private void SelectQueue(MessageQueue queue)
        {
            try
            {
                Message[] messages = queue.GetAllMessages();
                BindMessages(messages);
                QueueSelected(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error");
            }
        }





        private void BindQueues(MessageQueue[] queues)
        {
            ConnectedToServer(queues.Length > 0);
            lstQueues.Items.Clear();
            lstQueues.Items.AddRange(queues);
        }

        private void QueueSelected(bool selected)
        {
            lstMessages.Enabled = selected;
            btnPush.Enabled = selected;
            btnPop.Enabled = selected;
            txtMessage.Enabled = selected;
            btnMessageRefresh.Enabled = selected;
        }

        private void BindMessages(Message[] messages)
        {
            QueueSelected(messages.Length > 0);
            lstMessages.Items.Clear();
            foreach(var message in messages)
            {
                //m.Formatter = new XmlMessageFormatter( new string[]{"System.String"});
                //lstMessages.Items.Add(m.Body.ToString());

                using (StreamReader reader = new StreamReader(message.BodyStream))
                {
                    string str = reader.ReadToEnd();
                    str = ToLiteral(str);
                    lstMessages.Items.Add(str);
                }
            }
        }

        //found on the net.
        static string ToLiteral(string input)
        {
            var writer = new StringWriter();
            CSharpCodeProvider provider = new CSharpCodeProvider();
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.GetStringBuilder().ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string queueName = _server + @"\Private$\" + txtQueueName.Text;

                MessageQueue.Create(queueName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            ConnectToServer(_server);
        }

        private void bntDelete_Click(object sender, EventArgs e)
        {
            try
            {
                MessageQueue.Delete(((MessageQueue)lstQueues.SelectedItem).Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            ConnectToServer(_server);
            
        }

        private void btnPush_Click(object sender, EventArgs e)
        {
            try
            {
                var q = (MessageQueue)lstQueues.SelectedItem;
                q.Send(txtMessage.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            SelectQueue((MessageQueue)lstQueues.SelectedItem);
        }

        private void btnPop_Click(object sender, EventArgs e)
        {
            try
            {
                var q = (MessageQueue)lstQueues.SelectedItem;
                q.Receive(new TimeSpan(0, 0, 0, 1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            SelectQueue((MessageQueue)lstQueues.SelectedItem);
        }

        private void btnQueueRefresh_Click(object sender, EventArgs e)
        {
            ConnectToServer(_server);
        }

        private void btnMessageRefresh_Click(object sender, EventArgs e)
        {
            SelectQueue((MessageQueue)lstQueues.SelectedItem);
        }

        private void lstMessages_DoubleClick(object sender, EventArgs e)
        {
            string selected = lstMessages.SelectedItem as string;
            if(selected != null)
            {
                MessageBox.Show(selected, "Full message");
            }
        }
    }
}
