using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
