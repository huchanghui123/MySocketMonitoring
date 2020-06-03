using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;

namespace Clinet
{
    class Program
    {
        static string endfilter = "###";
        static CpuTemperatureReader cpuReader;
        static HardwareSensors hs;

        static void Main(string[] args)
        {
            Console.WriteLine("start client!");

            MyClient client = new MyClient(endfilter);
            client.ConnectToServer("127.0.0.1",2020);

            cpuReader = new CpuTemperatureReader();
            hs = cpuReader.GetTemperaturesInCelsius();
            string message = String.Format("CPU Package:{0}℃ Min:{1}℃ Max:{2}℃ CPU Speed:{3}GHz Memory Load:{4}%",
                                hs.temperature, hs.temperature_min, hs.temperature_max, 
                                hs.cpu_clock.ToString("f2"), hs.mem_load);

            while (!client.IsConnetced())
            {
                
            }
            client.SendData("message: "+message + "\r\n");
            cpuReader.Dispose();
            Console.ReadKey();

        }

        
    }
}
