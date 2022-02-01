using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABSoftware;

namespace RAApp
{
    public partial class MainForm : Form
    {
        public static MainForm instance;

        public MainForm()
        {
            instance = this;
            InitializeComponent();
        }

        Client client = new Client();

        private void button1_Click(object sender, EventArgs e)
        {
            client.Connect(IP.Text, 4040);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.SendPacket(Packet.Text);
        }

        public void OnPacket(string packet)
        {
            switch(packet.Split(';')[0])
            {
                case "message":
                    {
                        MessageBox.Show(packet.Split(new char[] { ';' }, 2)[1]);
                    }
                    break;
                case "keyboard":
                    {
                        switch(packet.Split(';')[1])
                        {
                            case "press":
                                {
                                    Keyboard.PressKey(Keyboard.GetVK(packet.Split(';')[2]));
                                }
                                break;
                            case "release":
                                {
                                    Keyboard.ReleaseKey(Keyboard.GetVK(packet.Split(';')[2]));
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Disconnect();
        }
    }
}
