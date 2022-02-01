using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAApp
{
    class Root : Server
    {
        static void Main(string[] args)
        {
            Console.Title = "RAApp Server";
            Root r = new Root();
            r.Start();
        }

        public override void OnClientConnected(ClientObject client)
        {
            Console.WriteLine($"> Client {client.ID} has connected from {client.IP}");
        }

        public override void OnClientDisconnected(ClientObject client)
        {
            Console.WriteLine($"> Client {client.ID} has disconnected");
        }

        public override void OnIncomingPacket(ClientObject client, string packet)
        {
            //Console.WriteLine($"[{client.ID}]> {packet}");
            switch(packet.Split(';')[0])
            {
                case "message": //message;target;text
                    {
                        string target = packet.Split(';')[1];
                        SendPacket(target, $"message;{packet.Split(new char[] { ';' }, 3)[2]}"); 
                    }
                    break;
                case "keyboard": //keyboard;target;press;vk_name
                    {
                        string target = packet.Split(';')[1];
                        SendPacket(target, $"keyboard;{packet.Split(';')[2]};{packet.Split(';')[3]}");
                    }
                    break;
            }
        }

        public override void OnOutgoingPacket(ClientObject client, string packet)
        {
            
        }

        public override void OnConsoleInput(string input)
        {
            switch(input)
            {
                case "start":
                    {
                        if(!Start())
                        {
                            Console.WriteLine("The server is already running");
                        }
                    }
                    break;
                case "stop":
                    {
                        if (!Stop())
                        {
                            Console.WriteLine("The server is already stopped");
                        }
                    }
                    break;
            }
        }
    }
}
