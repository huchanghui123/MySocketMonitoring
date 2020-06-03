using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;

namespace Server
{
    public class MyServer : AppServer<MySession>
    {
        protected override void OnStarted()
        {
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {

            return base.Setup(rootConfig, config);
        }
    }

    

}
