using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace WPF_ChatClient.Core
{
    class MessageManager
    {
        // 连接到服务器的 IP 地址和端口
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        // 创建 TCP Socket
        private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        #region 单例实现
        private static readonly Lazy<MessageManager> _instance =
            new Lazy<MessageManager>(() => new MessageManager());
        public static MessageManager Instance => _instance.Value;

        private MessageManager()
        {
            // 连接服务器
            _clientSocket.Connect(endPoint);
            try
            {
                Task.Run(() => ReceiveMsg());
                Task.Run(() => DealMsg());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion

        private int Id = 114514;

        private ConcurrentQueue<byte[]> _message = new ConcurrentQueue<byte[]>();

        public event Action<string> OnMessageReceived;

        enum MessageType : byte
        {
            Text = 1, //用户信息
            Join = 2, //用户加入
            Leave = 3, //用户离开
            System = 4, //系统消息
            Heartbeat = 5 //心跳信息
        }

        public void SendMsg(string mes, int receiver)
        {
            _clientSocket.Send(PackMsg((byte)MessageType.Text, Id, receiver, mes));
        }
        public async Task ReceiveMsg()
        {
            while (true)
            {

                try
                {

                    byte[] buffer = new byte[1024];
                    int bytesReceive = _clientSocket.Receive(buffer);
                    byte[] input = new byte[bytesReceive];//规范长度
                    Array.Copy(buffer, 0, input, 0, bytesReceive);
                    string mes = Encoding.UTF8.GetString(buffer, 0, bytesReceive);
                    if (bytesReceive != 0)
                    {
                        Debug.WriteLine(mes);
                        _message.Enqueue(input);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }
        }

        public async void DealMsg()
        {
            while (true)
            {
                byte[] mes;
                if (_message.TryDequeue(out mes))
                {

                    #region 解析信息
                    byte type = mes[0];
                    int sender = BitConverter.ToInt32(mes, 1);
                    int receiver = BitConverter.ToInt32(mes, 5);
                    Console.WriteLine(type + " " + sender + " " + receiver);
                    byte[] body = new byte[mes.Length - 9];
                    Array.Copy(mes, 9, body, 0, body.Length);
                    string mesbody = Encoding.UTF8.GetString(body, 0, body.Length);
                    #endregion

                    switch (type)
                    {
                        case (byte)MessageType.Text:
                            {
                                OnMessageReceived?.Invoke(mesbody);
                                break;
                            }
                        case (byte)MessageType.Join:
                            {
                                break;
                            }
                        case (byte)MessageType.Leave:
                            {
                                break;
                            }
                        case (byte)MessageType.Heartbeat:
                            {
                                break;
                            }
                        case (byte)MessageType.System:
                            {
                                break;
                            }

                    }
                }
                await Task.Delay(1000);
            }
        }

        public byte[] PackMsg(byte messageType, int sender, int receiver, string body)
        {
            int bodyCount = Encoding.UTF8.GetByteCount(body);
            byte[] buffer = new byte[9 + bodyCount];
            buffer[0] = messageType;
            Array.Copy(BitConverter.GetBytes(sender), 0, buffer, 1, 4);
            Array.Copy(BitConverter.GetBytes(receiver), 0, buffer, 5, 4);
            Array.Copy(Encoding.UTF8.GetBytes(body), 0, buffer, 9, bodyCount);
            return buffer;
        }

    }
}
