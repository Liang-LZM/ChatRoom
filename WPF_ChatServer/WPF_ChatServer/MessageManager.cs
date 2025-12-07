using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WPF_ChatServer
{
    internal class MessageManager
    {
        // 单例实现
        private static readonly Lazy<MessageManager> _instance =
            new Lazy<MessageManager>(() => new MessageManager());

        Queue<string> _message = new Queue<string>();

        private List<Client> _clientScoket = new List<Client>();

        public static MessageManager Instance => _instance.Value;

        private MessageManager()
        {
            try
            {
                Thread DealMsgThread = new Thread(() => DealMsg());
                DealMsgThread.IsBackground = true;
                DealMsgThread.Start();

                Thread ReceiveMsgThread = new Thread(() => ReceiveMsg());
                ReceiveMsgThread.IsBackground = true;
                ReceiveMsgThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void DealMsg()
        {
            while (true)
            {
                if (_message.Count > 0)
                {
                    Console.WriteLine("Dealing message");
                    _message.Dequeue();
                }
            }
        }

        public void ReceiveMsg()
        {
            while (true)
            {
                if (_clientScoket.Count > 0)
                {
                    foreach (var client in _clientScoket)
                    {
                        try
                        {
                            byte[] buffer = new byte[1024];
                            int bytesReceive = client.Socket.Receive(buffer);
                            string mes = Encoding.UTF8.GetString(buffer, 0, bytesReceive);
                            Console.WriteLine(mes);
                            _message.Enqueue(mes);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            RemoveConnection(client);
                        }
                    }
                }
            }
        }

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
