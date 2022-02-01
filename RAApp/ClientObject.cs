using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace RAApp
{
    public class ClientObject
    {
        public Server serverInstance;

        public string ID;
        public string IP;

        public TcpClient client;
        public bool isRunning = false;

        public Thread packetListener;

        public byte[] buffer = new byte[655360];

        public void sendPacket(byte[] data) { client.GetStream().Write(data, 0, data.Length); }

        public ClientObject(TcpClient client, string ID, string IP, Server serverInstance)
        {
            this.serverInstance = serverInstance;
            this.client = client;
            this.ID = ID;
            this.IP = IP;
        }

        public void Start()
        {
            isRunning = true;
            packetListener = new Thread(PacketListener);
            packetListener.Start();
        }

        public void PacketListener()
        {
            while(isRunning)
            {
                try
                {
                    int count = client.GetStream().Read(buffer, 0, buffer.Length);
                    if (count > 0)
                    {
                        byte[] bytes = buffer;
                        Array.Resize(ref bytes, count);
                        serverInstance.OnIncomingPacket(this, Encoding.UTF8.GetString(bytes));
                    }
                    else
                    {
                        serverInstance.Disconnect(ID);
                    }
                }
                catch(Exception) { }
            }
        }

        public void Disconnect()
        {
            isRunning = false;
            packetListener.Abort();
            packetListener = null;
            client.Close();
            client = null;
        }
    }
}
