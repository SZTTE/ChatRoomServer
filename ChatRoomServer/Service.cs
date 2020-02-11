using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Linq;

namespace ChatRoomServer
{
    class Service
    {
        private int Port { get; } = 10086;
        private byte[] Data { get; set; }
        private Socket ServeSocket { get; set; }
        private List<Socket> Clients { get; set; } = new List<Socket>();
        private Log Log { get; set; }

        public Service(int maxDataLength = 1024)
        {
            ServeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Data = new byte[maxDataLength];

            Log = new Log();
        }

        public void StartService()
        {
            IPAddress ip = IPAddress.Any;

            try
            {
                ServeSocket.Bind(new IPEndPoint(ip, Port));
                ServeSocket.Listen(10);
            }
            catch (Exception e)
            {
                Log.Write(e.Message, Log.LogType.Error);

                throw new Exception();
            }

            Listen();
        }

        public void CloseService()
        {
            ServeSocket.Close();
        }

        private void Listen()
        {
            Socket client;
            while (true)
            {
                try
                {
                    client = ServeSocket.Accept();
                }
                catch (Exception e)
                {
                    Log.Write(e.Message, Log.LogType.Error);
                    throw new Exception();
                }

                Clients.Add(client);

                Thread thread = new Thread(Receive);
                thread.Start(client);
            }
        }
        
        private void Receive(object socket)
        {
            using Socket client = socket as Socket;
            int length;
            while (true)
            {
                if (!client.Connected)
                {
                    Clients.Remove(client);
                    break;
                }

                try
                {
                    length = client.Receive(Data);
                }
                catch (SocketException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    Log.Write(e.Message, Log.LogType.Error);
                    throw new Exception();
                }

                Send(length);
            }
        }

        private void Send(int length)
        {
            byte[] data = Data.Take(length).ToArray();

            for (int i = 0; i < Clients.Count; i++)
            {
                try
                {
                    if (Clients[i].Connected)
                        Clients[i].Send(data);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
