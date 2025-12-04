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

        private List<Client> _clientScoket = new List<Client>();

        private ConnectManager() { }
        public void AddConnection(Client client)
        {
            if (!_clientScoket.Contains(client))
            {
                _clientScoket.Add(client);
                Console.WriteLine(client.IpAddress + "Connect");
            }
        }

        public void RemoveConnection(Client client)
        {
            if (_clientScoket.Contains(client))
            {
                _clientScoket.Remove(client);
                Console.WriteLine(client.IpAddress + client.ConnectTime + "Disconnect");
            }
        }

        public Client GetClientById(int id)
        {

            return _clientScoket.FirstOrDefault(c => c.Id == id);
        }

        public List<Client> GetAllClient()
        {
            return _clientScoket;
        }

        public void ChangeAuthenticated(Client client, bool TF)
        {
            client.IsAuthenticated = TF;
        }
    }

    class Client
    {
        public int Id { get; set; }
        public Socket Socket { get; set; }

        public string IpAddress { get; set; }

        public bool IsAuthenticated { get; set; }

        public DateTime ConnectTime { get; set; }

        public Client(Socket socket, int id = 0)
        {
            Id = id;
            IsAuthenticated = false;
            Socket = socket;
            IpAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            ConnectTime = DateTime.Now;
        }
    }
}
