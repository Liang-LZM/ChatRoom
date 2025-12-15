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
        static async Task Main(string[] args)
        {
            #region 初始化
            // 设置监听的 IP 地址和端口号
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 8080);
            // 创建 TCP Socket
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 绑定端口并开始监听
            listener.Bind(endPoint);
            //设置监听队列长度
            listener.Listen(10);
            Console.WriteLine("服务器已启动，等待连接...");
            //创建信息接受模块
            MessageManager messageManager = MessageManager.Instance;
            #endregion

            while (true)
            {
                Socket ClientSocket = await listener.AcceptAsync();
                messageManager.AddConnection(new Client(ClientSocket));
                Console.WriteLine("客户端已连接.");
                await Task.Delay(100);
            }
            listener.Close();
            Console.ReadLine();

        }
    }
}
