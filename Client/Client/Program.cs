using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region 初始化
            // 连接到服务器的 IP 地址和端口
            string serverIP = "127.0.0.1"; // 本机地址
            int port = 8080;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);

            // 创建 TCP Socket
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 连接服务器
            clientSocket.Connect(endPoint);
            Console.WriteLine("已连接到服务器.");

            MessageManager messageManager = new MessageManager(clientSocket);
            #endregion


            while (true)
            {

            }


            clientSocket.Close();
        }
    }

    class MessageManager
    {
        int Id;

        enum MessageType : byte
        {
            Text = 1, //用户信息
            Join = 2, //用户加入
            Leave = 3, //用户离开
            System = 4, //系统消息
            Heartbeat = 5 //心跳信息
        }


        private Socket _socket;
        public MessageManager(Socket socket)
        {
            _socket = socket;

            Id = 114514;

            Thread recThread = new Thread(() => ReceiveMsg());
            recThread.IsBackground = true;
            recThread.Start();

            Thread SendThread = new Thread(() => SendMsg());
            SendThread.IsBackground = true;
            SendThread.Start();
        }

        public void SendMsg()
        {
            while (true)
            {
                string read = Console.ReadLine();
                _socket.Send(PackMsg((byte)MessageType.Text, Id, 0, read));
            }
        }

        public void ReceiveMsg()
        {
            while (true)
            {
                #region 接受并拆分信息
                byte[] buffer = new byte[1024];
                int bytesRead = _socket.Receive(buffer);
                byte[] mes = new byte[bytesRead];
                Array.Copy(buffer, 0, mes, 0, bytesRead);

                byte type = mes[0];
                int sender = BitConverter.ToInt32(mes, 1);
                int receiver = BitConverter.ToInt32(mes, 5);
                //Console.WriteLine(type + " " + sender + " " + receiver);
                byte[] body = new byte[mes.Length - 9];
                Array.Copy(mes, 9, body, 0, body.Length);
                #endregion

                string rec = Encoding.UTF8.GetString(mes, 9, bytesRead - 9);


                if (bytesRead != 0)
                {
                    if (rec == "ACK")
                    {
                        Console.WriteLine("已发送");
                    }
                    else
                    {
                        Console.WriteLine("收到：" + rec);
                    }
                }
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
