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

        public static MessageManager Instance => _instance.Value;

        private ConcurrentQueue<byte[]> _message = new ConcurrentQueue<byte[]>();

        private List<Client> _client = new List<Client>();

        private SemaphoreSlim _semaphore = new SemaphoreSlim(10, 10); // 限制最多10个并发任务

        enum MessageType : byte
        {
            Text = 1, //用户信息
            Join = 2, //用户加入
            Leave = 3, //用户离开
            System = 4, //系统消息
            Heartbeat = 5 //心跳信息
        }

        private MessageManager()
        {
            try
            {
                Task.Run(() => DealMsg());
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
                byte[] mes;
                if (_message.TryDequeue(out mes))
                {
                    List<Client> clientCopy;
                    lock (_client)
                    {
                        clientCopy = new List<Client>(_client);
                    }

                    #region 解析信息
                    byte type = mes[0];
                    int sender = BitConverter.ToInt32(mes, 1);
                    int receiver = BitConverter.ToInt32(mes, 5);
                    Console.WriteLine(type + " " + sender + " " + receiver);
                    byte[] body = new byte[mes.Length - 9];
                    Array.Copy(mes, 9, body, 0, body.Length);
                    #endregion

                    switch (type)
                    {
                        case (byte)MessageType.Text:
                            {
                                foreach (Client client in clientCopy)
                                {
                                    if (client != null)
                                    {
                                        Console.WriteLine("Dealing message");
                                        //异步发送信息，避免阻塞线程
                                        await Task.Run(() => { client.Socket.Send(mes); });
                                        //await Task.Run(() => { client.Socket.Send(body); });
                                    }
                                }
                                break;
                            }
                        case (byte)MessageType.Join:
                            {
                                break;
                            }
                        case (byte)MessageType.Leave:
                            {
                                break;
                            }
                        case (byte)MessageType.Heartbeat:
                            {
                                break;
                            }
                        case (byte)MessageType.System:
                            {
                                break;
                            }

                    }
                }
                await Task.Delay(1000);
            }
        }

        private void StartReceiveMsg(Client client)
        {
            Task.Run(() => ReceiveMsgForClient(client));
        }

        public async Task ReceiveMsgForClient(Client client)
        {
            while (true)
            {

                try
                {
                    await _semaphore.WaitAsync();
                    byte[] buffer = new byte[1024];
                    int bytesReceive = client.Socket.Receive(buffer);

                    //byte[] input = new byte[bytesReceive];//规范长度
                    //Array.Copy(buffer, 0, input, 0, bytesReceive);
                    string mes = Encoding.UTF8.GetString(buffer, 0, bytesReceive);
                    if (bytesReceive != 0)
                    {
                        Console.WriteLine(mes);
                        //_message.Enqueue(input);
                        _message.Enqueue(buffer);

                        // 向客户端发送确认
                        client.Socket.Send(PackMsg((byte)MessageType.System, 0, 0, "ACK"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    RemoveConnection(client);
                    break;
                }
                finally
                {
                    _semaphore.Release(); // 完成后释放信号量，允许其他任务继续执行
                }
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
                    StartReceiveMsg(client);
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

        public byte[] PackMsg(byte messageType, int sender, int receiver, string body)
        {
            int bodyCount = Encoding.UTF8.GetByteCount(body);
            byte[] buffer = new byte[9 + bodyCount];
            buffer[0] = messageType;
            Array.Copy(BitConverter.GetBytes(sender), 0, buffer, 1, 4);
            Array.Copy(BitConverter.GetBytes(receiver), 0, buffer, 5, 4);
            Array.Copy(Encoding.UTF8.GetBytes(body), 0, buffer, 9, bodyCount);
            return buffer;
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
