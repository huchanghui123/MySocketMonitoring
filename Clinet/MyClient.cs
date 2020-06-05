using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;

namespace Clinet
{
    class MyClient
    {
        EasyClient client;
        //结束符
        string endFilter = "###";
        string ip;
        int port;
        public string data { get; set; } = "hello this is super client\r\n";
        CpuTemperatureReader cpuReader;
        HardwareSensors hs;

        public MyClient(string endFilter)
        {
            if (string.IsNullOrEmpty(endFilter))
            {
                this.endFilter = endFilter;
            }
            client = new EasyClient();
            client.Initialize(new MyReceiveFilter(endFilter), (request) => {
                Console.WriteLine("resposen:" + request.Body);
                if (request.Body.ToUpper().IndexOf("GETCPU") > -1)
                {
                    var message = "CPU " + GetCpuTemperature() + "\r\n";
                    client.Send(Encoding.ASCII.GetBytes(message));
                    //cpuReader.Dispose();
                }
            });

            cpuReader = new CpuTemperatureReader();
            hs = cpuReader.GetTemperaturesInCelsius();
        }

        //连接服务器
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
        //发送数据到服务器
        public void SendData(String message)
        {
            if (client.IsConnected)
            {
                client.Send(Encoding.ASCII.GetBytes(message));
            }
            
        }

        public bool IsConnetced()
        {
            return client.IsConnected;
        }

        public string GetCpuTemperature()
        {
            hs = cpuReader.GetTemperaturesInCelsius();
            var message = String.Format("CPU Package:{0}℃ Min:{1}℃ Max:{2}℃ CPU Speed:{3}GHz Memory Load:{4}%",
                                hs.temperature, hs.temperature_min, hs.temperature_max,
                                hs.cpu_clock.ToString("f2"), hs.mem_load);
            Console.WriteLine(message);
            return message;
        }

        public void CloseCpuReader()
        {
            try
            {
                cpuReader.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }
        //
    }
}
