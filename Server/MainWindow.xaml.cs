using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int clientConn = 0;
        private List<Button> btnList = null;
        private List<int> disconnectList = new List<int>();
        private List<SessionButton> sessionList = new List<SessionButton>();
        private SessionButton sb = null;
        MyServer appServer;

        public MainWindow()
        {
            InitializeComponent();
            InitButton();
            StartServer();
        }

        private void InitButton()
        {
            //将所有按钮全部放入集合管理
            btnList = new List<Button>();
            foreach (UIElement element in this.GridBtn.Children)
            {
                if (element is Button)
                {
                    Button btn = (Button)element;
                    btn.Click += BtnClick;
                    btn.IsEnabled = false;
                    btnList.Add(btn);
                }
            }
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Console.WriteLine("Button_Click name:" + btn.Name);
            var sb = sessionList.Find((SessionButton s) => s.name.Equals(btn.Name));
            var session = appServer.GetSessionByID(sb.sessionid);
            Console.WriteLine("Click--------------"+ (session.RemoteEndPoint as IPEndPoint).Address+"-----"
                + (session.RemoteEndPoint as IPEndPoint).Port);
            var info = String.Format("name: {0} ip: {1} port: {2}", sb.name,
                (session.RemoteEndPoint as IPEndPoint).Address,
                (session.RemoteEndPoint as IPEndPoint).Port);
            clinet_name.Text = info;
            clinet_info.Text = "SessionID:"+session.SessionID;

            //通知客户端发送CPU信息
            var msg = "GETCPU";
            session.Send(msg);
        }

        private void StartServer()
        {
            appServer = new MyServer();
            if (!appServer.Setup(2020))
            {
                Console.WriteLine("Failed to setup!");
                return;
            }

            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                return;
            }
            appServer.NewSessionConnected += new SessionHandler<MySession>(NewSessionConnected);
            appServer.SessionClosed += new SessionHandler<MySession, CloseReason>(NewSessionClosed);
            appServer.NewRequestReceived += new RequestHandler<MySession, StringRequestInfo>(NewRequestReceived);
            Console.WriteLine("The server started successfully!");
        }

        private void NewRequestReceived(MySession session, StringRequestInfo requestInfo)
        {
            switch (requestInfo.Key.ToUpper())
            {
                case ("CPU"):
                    UpdateClientCpuInfo(requestInfo.Body);
                    break;
                default:
                    Console.WriteLine("NewRequestReceived:" + requestInfo.Key + "----" + requestInfo.Body);
                    break;
            }
        }

        private void NewSessionConnected(MySession session)
        {
            IPAddress clientIp = (session.RemoteEndPoint as IPEndPoint).Address;
            int clientPort = (session.RemoteEndPoint as IPEndPoint).Port;

            String message = String.Format("Welcome to SuperSocket Telnet Server!! client IP:{0} Port:{1} ID:{2}",
                clientIp, clientPort, session.SessionID);
            //Console.WriteLine(message);
            session.Send(message);

            UpdateSeesionList(clientConn, session, "Connected");
            clientConn++;
            UpdateConnect(clientConn, session, "Connected");
        }

        private void NewSessionClosed(MySession session, CloseReason value)
        {
            clientConn--;
            IPAddress clientIp = (session.RemoteEndPoint as IPEndPoint).Address;
            int clientPort = (session.RemoteEndPoint as IPEndPoint).Port;

            String message = String.Format("Client: {0} {1} session: {2} is closed for {3} connect number: {4}",
                clientIp, clientPort, session.SessionID, value, clientConn);
            Console.WriteLine(message);

            UpdateSeesionList(clientConn, session, "Closed");
            UpdateConnect(clientConn, session, "Closed");
        }

        public void UpdateSeesionList(int index, MySession session, string status)
        {
            Console.WriteLine("UpdateSeesionList.......index:" + index + " status:"+ status);
            sb = new SessionButton();
            if (status.Equals("Connected"))
            {
                if (disconnectList.Count > 0)
                {
                    var i = disconnectList.First();
                    sb.index = i;
                    disconnectList.Remove(i);
                }
                else
                {
                    sb.index = index;
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    sb.name = btnList[sb.index].Name;
                    btnList[sb.index].Background = Brushes.Green;
                    btnList[sb.index].IsEnabled = true;
                }));
                sb.sessionid = session.SessionID;
                sessionList.Add(sb);
            }
            else if (status.Equals("Closed"))
            {
                var sb = sessionList.Find((SessionButton s) => s.sessionid.Equals(session.SessionID));
                Console.WriteLine("name:{0} id: {1} port: {2} seesionId: {3}", sb.name, sb.index,
                    (session.RemoteEndPoint as IPEndPoint).Port, session.SessionID);
                disconnectList.Add(sb.index);
                sessionList.Remove(sb);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    btnList[sb.index].Background = Brushes.White;
                    btnList[sb.index].IsEnabled = false;
                }));
            }
            disconnectList.Sort();
            foreach (int i in disconnectList)
            {
                Console.WriteLine("foreach disconnectList: " + i);
            }
        }

        public void UpdateConnect(int conn, MySession session, string status)
        {
            IPAddress ip = (session.RemoteEndPoint as IPEndPoint).Address;
            int port = (session.RemoteEndPoint as IPEndPoint).Port;
            this.Dispatcher.Invoke(new Action(() =>
            {
                connect_num.Content = "当前连接数：" + conn;
                clinet_name.Text = String.Format("Client Ip: {0} Port: {1} Status: {2}", ip, port, status);
                clinet_info.Text = String.Format("Session Id: {0}", session.SessionID);
            }));
            Console.WriteLine("UpdateConnectNumber..............." + conn);
        }

        private void UpdateClientCpuInfo(string info)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var msg = Encoding.GetEncoding("GB2312").GetString(Encoding.ASCII.GetBytes(info));
                Console.WriteLine(msg);
                clinet_cpu.Text = String.Format("{0}", msg);
            }));
        }

        private void AppClose(object sender, EventArgs e)
        {
            if (appServer != null)
            {
                appServer.Stop();
            }
        }
    }

    
}
