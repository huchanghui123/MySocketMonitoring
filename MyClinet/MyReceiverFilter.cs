using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace MyClinet
{
    class MyReceiveFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        public MyReceiveFilter() : base(Encoding.ASCII.GetBytes("||"))
        {
        }

        public MyReceiveFilter(byte[] terminator) : base(Encoding.ASCII.GetBytes("||"))
        {
        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            throw new NotImplementedException();
        }
    }
}
