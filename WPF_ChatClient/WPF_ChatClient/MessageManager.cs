using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ChatClient
{
    class MessageManager
    {
        // 连接到服务器的 IP 地址和端口
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        // 创建 TCP Socket
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        #region 单例实现
        private static readonly Lazy<MessageManager> _instance =
            new Lazy<MessageManager>(() => new MessageManager());
        public static MessageManager Instance => _instance.Value;

        private MessageManager()
        {
            // 连接服务器
            clientSocket.Connect(endPoint);
        }
        #endregion

        private int Id = 0;





    }
}
