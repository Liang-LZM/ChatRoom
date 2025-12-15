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
using System.Collections.Concurrent;

namespace WPF_ChatServer
{
    internal class MessageManager
    {
        // 单例实现
        private static readonly Lazy<MessageManager> _instance =
            new Lazy<MessageManager>(() => new MessageManager());

        private ConcurrentQueue<string> _message = new ConcurrentQueue<string>();

        private List<Client> _client = new List<Client>();

        public static MessageManager Instance => _instance.Value;

        private MessageManager()
        {
            try
            {

                Task.Run(() => DealMsg());


                Task.Run(() => ReceiveMsg());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public async void DealMsg()
        {
            while (true)
            {
                if (_message.Count > 0)
                {

                    string mes;
                    if (_message.TryDequeue(out mes))
                    {
                        List<Client> clientCopy;
                        lock (_client)
                        {
                            clientCopy = new List<Client>(_client);
                        }
                        foreach (Client client in clientCopy)
                        {
                            if (client != null)
                            {
                                Console.WriteLine("Dealing message");
                                //异步发送信息，避免阻塞线程
                                await Task.Run(() => { client.Socket.Send(Encoding.UTF8.GetBytes(mes)); });
                            }
                        }
                    }

                }
                await Task.Delay(100);
            }
        }

        public async void ReceiveMsg()
        {
            while (true)
            {
                if (_client.Count > 0)
                {
                    List<Client> clientCopy;
                    lock (_client)
                    {
                        clientCopy = new List<Client>(_client);
                    }

                    foreach (var client in clientCopy)
                    {
                        try
                        {
                            byte[] buffer = new byte[1024];
                            int bytesReceive = client.Socket.Receive(buffer);
                            string mes = Encoding.UTF8.GetString(buffer, 0, bytesReceive);
                            if (bytesReceive != 0)
                            {
                                Console.WriteLine(mes);
                                _message.Enqueue(mes);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            RemoveConnection(client);
                        }
                    }
                }
                await Task.Delay(10);
            }
        }

        public void AddConnection(Client client)
        {
            lock (_client)
            {
                if (!_client.Contains(client))
                {
                    _client.Add(client);
                    Console.WriteLine(client.IpAddress + "Connect");
                }
            }
        }

        public void RemoveConnection(Client client)
        {
            lock (_client)
            {
                if (_client.Contains(client))
                {
                    _client.Remove(client);
                    Console.WriteLine(client.IpAddress + client.ConnectTime + "Disconnect");
                }
            }
        }

        public Client GetClientById(int id)
        {

            return _client.FirstOrDefault(c => c.Id == id);
        }

        public List<Client> GetAllClient()
        {
            return _client;
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
