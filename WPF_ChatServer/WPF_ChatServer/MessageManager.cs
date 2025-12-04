using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ChatServer
{
    internal class MessageManager
    {
        // 单例实现
        private static readonly Lazy<MessageManager> _instance =
            new Lazy<MessageManager>(() => new MessageManager());
        Queue<string> _message = new Queue<string>();
        public static MessageManager Instance => _instance.Value;

        private MessageManager()
        {

        }

        public void DealMsg()
        {

        }

        public void ReceiveMsg()
        {
            var client = ConnectManager.Instance.GetAllClient();
            foreach (var item in client)
            {
                Debug.WriteLine("准备接收");
                Debug.WriteLine(item);
                byte[] receive = new byte[1024];
                int bytesReceived = ConnectManager.Instance.GetClientById(item.Id).Socket.Receive(receive);
                string mes = Encoding.UTF8.GetString(receive, 0, bytesReceived);
                Console.WriteLine(mes);
                Debug.WriteLine("收到信息");
            }
        }
    }
}
