using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 设置监听的 IP 地址和端口号
            int port = 8080;
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

            // 创建 TCP Socket
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //

            // 绑定端口并开始监听
            listener.Bind(endPoint);
            listener.Listen(10);
            Console.WriteLine("服务器已启动，等待连接...");

            // 等待并接收客户端连接
            Socket clientSocket = listener.Accept();//
            Console.WriteLine("客户端已连接.");


            ConnectManager connect = ConnectManager.Instance;
            connect.AddConnection(clientSocket);





            // 接收客户端数据
            byte[] buffer = new byte[1024];
            //int bytesReceived = clientSocket.Receive(buffer);
            int bytesReceived = connect.GetSocketById().Receive(buffer);
            string data = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine("收到数据: " + data);

            // 向客户端发送回应
            string response = "Hello from server!";
            connect.GetSocketById().Send(Encoding.UTF8.GetBytes(response));

            // 关闭连接
            connect.RemoveConnection(connect.GetSocketById());
            clientSocket.Close();
            listener.Close();
            Console.ReadLine();

        }
    }
}
