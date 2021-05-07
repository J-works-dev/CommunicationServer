using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace CommunicationServer
{
    public partial class Form1 : Form
    {
        private Pipe pipeServer = new Pipe();
        private List<User> users = new List<User>();
        private bool login = false;
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
            byte[] msgBuffer;

            string str = encoder.GetString(message, 0, message.Length);
            if (!login)
            {
                string[] clientInfo = str.Split(',');
                string id = clientInfo[0];
                string pw = clientInfo[1];

                foreach (User user in users)
                {
                    if (user.Id.Equals(id))
                    {
                        if (user.Verify(pw))
                        {
                            msgBuffer = encoder.GetBytes("logged in");
                            pipeServer.SendMessage(msgBuffer);
                            login = true;
                        }
                        else
                        {
                            msgBuffer = encoder.GetBytes("Wrong Password");
                            pipeServer.SendMessage(msgBuffer);
                        }
                    }
                    else
                    {
                        if (user == users.Last()) // [users.Count - 1]
                        {
                            msgBuffer = encoder.GetBytes("No User found");
                            pipeServer.SendMessage(msgBuffer);
                        }
                    }
                }
            }
            else
            {
                textBoxReceived.Text += "Client: " + str + "\r\n";
            }
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
                    toolStripStatusLabel2.Text = "Admin Logged in";
                }
                else
                {
                    toolStripStatusLabel2.Text = "Server already running.";
                }
            }
            else
            {
                toolStripStatusLabel2.Text = "Wrong ID or Password Input. Try again.";
            }
        }
    }
}
