using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ChatServer
{
    internal class ConnectManager
    {
        private static readonly Lazy<ConnectManager> _instance =
            new Lazy<ConnectManager>(() => new ConnectManager());
        public static ConnectManager Instance => _instance.Value;

        private List<Socket> _clientScoket = new List<Socket>();

        private ConnectManager() { }
        public void AddConnection(Socket socket)
        {
            if (!_clientScoket.Contains(socket))
            {
                _clientScoket.Add(socket);
                Console.WriteLine(((IPEndPoint)socket.RemoteEndPoint).Address.ToString() + "Connect");
            }
        }

        public void RemoveConnection(Socket socket)
        {
            if (_clientScoket.Contains(socket))
            {
                _clientScoket.Remove(socket);
                Console.WriteLine(((IPEndPoint)socket.RemoteEndPoint).Address.ToString() + "Disconnect");
            }
        }

        public Socket GetSocketById()
        {
            return _clientScoket.FirstOrDefault();
        }
    }


}
