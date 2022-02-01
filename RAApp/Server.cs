using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace RAApp
{
    public class Server
    {
        public List<ClientObject> clients = new List<ClientObject>();

        public bool isRunning { get; private set; } = false;

        public TcpListener tcpListener;

        public Thread serverLoopThread;
        public Thread consoleListenerThread;

        public int Port { get; private set; }

        public DateTime TimeStamp { get { return DateTime.Now; } }

        #region "Useless" crap
        Random rnd = new Random();
        #endregion

        public bool Start(int Port = 4040)
        {
            if(!isRunning)
            {
                this.Port = Port;
                isRunning = true;
                if(consoleListenerThread == null)
                {
                    consoleListenerThread = new Thread(ConsoleListener);
                    consoleListenerThread.Start();
                }
                serverLoopThread = new Thread(ServerLoop);
                serverLoopThread.Start();
                OnServerStart();
                return true;
            }
            return false;
        }

        public bool Stop()
        {
            if(isRunning)
            {
                isRunning = false;
                tcpListener.Stop();
                tcpListener = null;
                serverLoopThread.Abort();
                serverLoopThread = null;
                Disconnect();
                clients.Clear();
                OnServerStop();
                return true;
            }
            return false;
        }

        #region Disconnect Methods
        public void Disconnect(string ID)
        {
            ClientObject co = clients.FirstOrDefault(c => Equals(c.ID, ID));
            if (co != null)
            {
                OnClientDisconnected(co);
                co.Disconnect();
                clients.Remove(co);
            }
        }

        public void Disconnect()
        {
            foreach(ClientObject co in clients)
            {
                OnClientDisconnected(co);
                co.Disconnect();
                clients.Remove(co);
            }
        }
        #endregion

        #region SendPacket Methods
        public void SendPacket(string ID, string packet)
        {
            ClientObject co = clients.FirstOrDefault(c => Equals(c.ID, ID));
            if (co != null)
            {
                OnOutgoingPacket(co, packet);
                co.sendPacket(Encoding.UTF8.GetBytes(packet));
            }
        }

        public void SendPacket(string packet)
        {
            foreach (ClientObject co in clients)
            {
                if(co != null)
                {
                    OnOutgoingPacket(co, packet);
                    co.sendPacket(Encoding.UTF8.GetBytes(packet));
                }
            }
        }
        #endregion

        string GetNewID()
        {
            return rnd.Next(0, 999999).ToString("X2") + rnd.Next(0, 999999).ToString("X2");
        }

        void ServerLoop()
        {
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            while(isRunning)
            {
                if(tcpListener.Pending())
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    IPEndPoint iep = (IPEndPoint)client.Client.RemoteEndPoint;
                    ClientObject co = new ClientObject(client, GetNewID(), iep.Address.ToString(), this);
                    clients.Add(co);
                    OnClientConnected(co);
                    co.Start();
                }
            }
        }

        void ConsoleListener()
        {
            while(true)
            {
                OnConsoleInput(Console.ReadLine());
            }
        }

        public virtual void OnConsoleInput(string input) { }
        public virtual void OnServerStart() { }
        public virtual void OnServerStop() { }
        public virtual void OnClientConnected(ClientObject client) {  }
        public virtual void OnClientDisconnected(ClientObject client) { }
        public virtual void OnIncomingPacket(ClientObject client, string packet) { }
        public virtual void OnOutgoingPacket(ClientObject client, string packet) { }
    }
}
