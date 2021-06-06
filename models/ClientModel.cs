using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Windows;
namespace Чат_tcp.models
{
    class ClientModel
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        private TcpClient client;
        public NetworkStream streamClient;
        private Server server;

        public ClientModel(Server s, TcpClient c)
        {
            Id =Guid.NewGuid().GetHashCode();
            client = c;
            server = s;
            server.addClient(this);
        }
        public void start()
        {
            string mes;
            try
            {
                streamClient = client.GetStream();
                //MessageBox.Show(streamClient.ToString()+"\n"+client.ToString());
                Name = readStream();
                mes = Name + " в чате.";
                server.broadcast(mes, Id);
                while (server.State)
                {
                    try
                    {
                        mes = readStream();
                        server.broadcast(Name+": "+mes,Id);
                    }
                    catch
                    {
                        server.broadcast(Name + " покинул чат.", Id);
                        break;
                    }
                }
            }catch(Exception ex)
            {
                server.delClient(Id);
                close();
            }
        }

        private string readStream()
        {
            byte[] data = new byte[255];
            while (streamClient.DataAvailable)
            {
                streamClient.Read(data, 0, data.Length);
            }
            return Encoding.UTF8.GetString(data);
        }
        public void close()
        {
            if (streamClient != null) streamClient.Close();
            if (client != null) client.Close();
        }
    }
}
