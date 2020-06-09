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
        bool cycleSend = false;
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
                //收到服务器的发送CPU状态通知
                if (request.Body.ToUpper().IndexOf("GETCPU") > -1)
                {
                    if (!cycleSend) {
                        StartSend();
                    }
                }
                //收到服务器的终止发送通知
                else if (request.Body.ToUpper().IndexOf("STOPSENDING") > -1)
                {
                    cycleSend = false;
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
            var data = "hello this is super client\r\n";
            var connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
            if (connected)
            {
                client.Send(Encoding.ASCII.GetBytes(data));
            }
        }

        public void StartSend()
        {
            cycleSend = true;
            System.Threading.Thread _thread = new System.Threading.Thread(SendData);
            _thread.IsBackground = true;
            _thread.Start();
        }

        //发送数据到服务器
        public void SendData()
        {
            while (cycleSend)
            {
                if (!client.IsConnected)
                {
                    ConnectToServer(ip, port);
                }
                else
                {
                    var message = "CPU " + GetCpuTemperature() + "\r\n";
                    client.Send(Encoding.ASCII.GetBytes(message));
                }
                System.Threading.Thread.Sleep(2000);
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
