using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyClinet
{
    class Program
    {
        static string ip = "127.0.0.1";
        static int port = 2020;
        static void Main(string[] args)
        {
            Console.WriteLine("client hello !");
            EasyClient client = new EasyClient();
            client.Initialize(new MyReceiveFilter(), (request) =>
            {
                // handle the received request
                Console.WriteLine(request.Key);
            });
            try
            {
                client.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
            
            client.Connected += NewConnected;
            client.Closed +=ClientClosed;
            client.Error += ClientError;

            
            Console.WriteLine(client.IsConnected);
            //client.Send(Encoding.ASCII.GetBytes("LOGIN kerry"));
            
            Console.ReadKey();
            

        }

        private static void NewConnected(object sender, EventArgs e)
        {
            Console.WriteLine("NewConnected..........."+ sender);
            EasyClient client = (EasyClient)sender;
            client.Send(Encoding.ASCII.GetBytes("LOGIN kerry"));
        }

        private static void ClientError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
        }

        private static void ClientClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Client Closed!");
        }

        
    }
}
