using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyClinet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("client hello !");

            var client = new EasyClient();
            client.Initialize(new MyReceiveFilter(), (request) => {
                // handle the received request
                Console.WriteLine(request.Key);
            });

        }
    }
}
