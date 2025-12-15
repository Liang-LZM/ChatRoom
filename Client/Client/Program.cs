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

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit") break;
            }
            clientSocket.Close();
        }
    }

    class MessageManager
    {


        private Socket _socket;
        public MessageManager(Socket socket)
        {
            _socket = socket;

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
                _socket.Send(Encoding.UTF8.GetBytes(read));
            }
        }

        public void ReceiveMsg()
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = _socket.Receive(buffer);
                string rec = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (bytesRead != 0)
                {
                    Console.WriteLine(rec);
                }
            }

        }
    }
}
