using SuperSocket.ProtoBase;
using System.Text;

namespace Clinet
{
    class MyReceiveFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        string end;
        public MyReceiveFilter(string end) : base(Encoding.ASCII.GetBytes(end))
        {
            this.end = end;
        }
        
        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            //throw new NotImplementedException();
            //获取接收到的完整数据，包括头和尾
            var body = bufferStream.ReadString((int)bufferStream.Length, Encoding.ASCII);
            //去掉屁股的终止符
            body = body.Remove(body.Length - end.Length, end.Length);
            return new StringPackageInfo("", body, new string[] { });
        }
    }
}
