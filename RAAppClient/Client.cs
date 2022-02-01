using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace RAApp
{
    public class Client
    {
        public Socket socket;
        public bool isRunning = false;

        public Thread packetListener;

        private byte[] bytes = new byte[655360];

        public Client()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string IP, int Port)
        {
            if(!isRunning)
            {
                isRunning = true;
                socket.Connect(IP, Port);
                packetListener = new Thread(PacketListener);
                packetListener.Start();
            }
        }

        public void Disconnect()
        {
            if(isRunning)
            {
                isRunning = false;
                socket.Shutdown(SocketShutdown.Both);
                packetListener.Abort();
                packetListener = null;
                socket.Disconnect(false);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
        }

        public void SendPacket(string packet)
        {
            socket.Send(Encoding.UTF8.GetBytes(packet));
        }

        void PacketListener()
        {
            while(isRunning)
            {
                try
                {
                    int count = socket.Receive(bytes, 0, bytes.Length, SocketFlags.None);
                    if (count < 1)
                    {
                        return;
                    }
                    else
                    {
                        byte[] buffer = bytes;
                        Array.Resize(ref buffer, count);
                        MainForm.instance.OnPacket(Encoding.UTF8.GetString(buffer));
                    }
                }
                catch(Exception) { }
            }
        }
    }
}
