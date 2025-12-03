using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

            // 发送数据
            string message = "Hello from client!";
            clientSocket.Send(Encoding.UTF8.GetBytes(message));

            // 接收服务器回应
            byte[] buffer = new byte[1024];
            int bytesReceived = clientSocket.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine("收到服务器回应: " + response);

            // 关闭连接
            clientSocket.Close();
            Console.ReadLine();
        }
    }
}
