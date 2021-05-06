using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommunicationServer
{
    public partial class Form1 : Form
    {
        private Pipe pipeServer = new Pipe();
        private List<User> users = new List<User>();
        
        public Form1()
        {
            InitializeComponent();

            pipeServer.MessageReceived += pipeServer_MessageReceived;
            pipeServer.ClientDisconnected += pipeServer_ClientDisconnected;

            users.Add(new User("admin", "0000"));
            users.Add(new User("client1", "1111"));
            users.Add(new User("client2", "2222"));
        }
        void pipeServer_ClientDisconnected()
        {
            Invoke(new Pipe.ClientDisconnectedHandler(ClientDisconnected));
        }

        void ClientDisconnected()
        {
            statusStrip.Text = "Client Disconnected!";
        }
        void pipeServer_MessageReceived(byte[] message)
        {
            Invoke(new Pipe.MessageReceivedHandler(DisplayMessageReceived),
                new object[] { message });
        }

        void DisplayMessageReceived(byte[] message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            string str = encoder.GetString(message, 0, message.Length);

            textBoxReceived.Text += "Client: " + str + "\r\n";
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] msgBuffer = encoder.GetBytes(textBoxSend.Text);

            pipeServer.SendMessage(msgBuffer);
            textBoxSend.Clear();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            string id = textBoxId.Text;
            string pw = textBoxPw.Text;
            if (id == "admin" && pw == "0000")
            {
                if (!pipeServer.Running)
                {
                    pipeServer.Start(textBoxPipe.Text);
                    buttonStart.Enabled = false;
                }
                else
                {
                    statusStrip.Text = "Server already running.";
                }
            }
            else
            {
                statusStrip.Text = "Wrong ID or Password Input. Try again.";
            }
        }
    }
}
