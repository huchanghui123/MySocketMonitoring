using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;

namespace Server
{
    public class MySession : AppSession<MySession>
    {
        public static int connect_num = 0;

        protected override void HandleException(Exception e)
        {
            this.Send("Application error: {0}", e.Message);
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            this.Send("Unknow request");
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }

        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }

        public override void Send(string message)
        {
            var msg = message + "###";
            Console.WriteLine("Server Send .....");
            base.Send(msg);
        }
    }
}
