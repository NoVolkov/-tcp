using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using Чат_tcp.models;

namespace Чат_tcp
{
    class Server
    {
        private TcpListener listner;
        List<ClientModel> clients;
        private string host = "127.0.0.1";
        private int port = 8888;
        public bool State { get; set; } = false;

        public Server(int port)
        {
            clients = new List<ClientModel>();
            State = true;
            this.port = port;
        }

        public void addClient(ClientModel c)
        {
            clients.Add(c);
        }
        public void delClient(int id)
        {
            ClientModel c=clients.FirstOrDefault(x => x.Id == id);
            if (c != null) clients.Remove(c);
        }
        public void listen()
        {
            try
            {
                listner = new TcpListener(IPAddress.Any,port);
                listner.Start();
                while (State)
                {
                    TcpClient tcp = listner.AcceptTcpClient();
                    //MessageBox.Show(tcp.ToString());
                    ClientModel c = new ClientModel(this,tcp);
                    Thread thrClient = new Thread(new ThreadStart(c.start));
                    thrClient.Start();
                }
            }
            catch(Exception ex)
            {
                disconnect();
            }
        }
        public void broadcast(string mes, int id)
        {
            byte[] data = Encoding.UTF8.GetBytes(mes);
            foreach(ClientModel c in clients)
            {
                if (c.Id != id)
                {
                    c.streamClient.Write(data, 0, data.Length);
                }
            }
        }
        public void disconnect()
        {
            listner.Stop();
            foreach(ClientModel c in clients)
            {
                c.close();
            }
        }
    }
}
