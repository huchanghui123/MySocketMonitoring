using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;

namespace Clinet
{
    class MyClient
    {
        EasyClient client;
        string endFilter = "###";
        string ip;
        int port;
        public string data { get; set; } = "hello this is super client\r\n";

        public MyClient(string endFilter)
        {
            if (string.IsNullOrEmpty(endFilter))
            {
                this.endFilter = endFilter;
            }
            client = new EasyClient();
            client.Initialize(new MyReceiveFilter(endFilter), (request) => {
                Console.WriteLine(request.Key + "---------" + request.Body);
            });
        }

        public async void ConnectToServer(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
            if (connected)
            {
                client.Send(Encoding.ASCII.GetBytes(data));
            }
        }

        public void SendData(String message)
        {
            Console.WriteLine(client.IsConnected + message);
            if (client.IsConnected)
            {
                client.Send(Encoding.ASCII.GetBytes(message));
            }
            
        }

        public bool IsConnetced()
        {
            return client.IsConnected;
        }

    }
}
