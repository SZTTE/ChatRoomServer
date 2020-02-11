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
            Log.Write("创建Service类。");
        }

        public void StartService()
        {
            IPAddress ip = IPAddress.Any;

            Log.Write("尝试启动服务……");
            try
            {
                ServeSocket.Bind(new IPEndPoint(ip, Port));
                ServeSocket.Listen(10);
                Log.Write("服务启动成功！");
            }
            catch (Exception e)
            {
                Log.Write("服务启动失败！", Log.LogType.Error);
                Log.Write(e.Message, Log.LogType.Error);

                throw new Exception();
            }

            Listen();
        }

        public void CloseService()
        {
            ServeSocket.Close();
            Log.Write("关闭服务。");
        }

        private void Listen()
        {
            Log.Write("开始监听……");
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

                Log.Write($"{client.LocalEndPoint}连接成功！");
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
                    Log.Write($"{client.LocalEndPoint}断开连接。");
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

                Log.Write($"从{client.LocalEndPoint}接收到信息。");
                Send(length);
            }
        }

        private void Send(int length)
        {
            byte[] data = Data.Take(length).ToArray();

            foreach (var client in Clients)
            {
                try
                {
                    if (client.Connected)
                    {
                        Log.Write($"向{client.LocalEndPoint}发送信息。");
                        client.Send(data);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
