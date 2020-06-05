using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;

namespace Clinet
{
    class Program
    {
        static string endfilter = "###";

        static MyClient client;
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(cancelHandler, true);
            //Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Console.WriteLine("start client!");

            client = new MyClient(endfilter);
            client.ConnectToServer("127.0.0.1",2020);

            Console.ReadKey();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("进程退出事件");
            CloseCpuReader();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("捕获Ctrl+C事件");
            Console.ReadKey();
        }

        public delegate bool ControlCtrlDelegate(int CtrlType);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭
                    CloseCpuReader();
                    break;
                case 2:
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭
                    CloseCpuReader();
                    break;
            }
            Console.ReadKey();
            return false;
        }

        private static void CloseCpuReader()
        {
            try
            {
                if (client != null)
                {
                    client.CloseCpuReader();
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
