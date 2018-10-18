using SampSharp.Core.Natives.NativeObjects;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        protected static RakNetInternal Internal { get; set; }

        static RakNet()
        {
            Internal = NativeObjectProxyFactory.CreateInstance<RakNetInternal>();
        }

        public class RakNetInternal
        {
            [NativeMethod]
            public virtual int CallRemoteFunction(string functionName, string argumentsFormat)
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int PR_RegHandler(int name, string functionName, int type)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual bool BS_RPC(int bs, int playerid, int rpcid, int priority, int reliability)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual bool BS_Send(int bs, int playerid, int priority, int reliability)
            {
                throw new NativeNotImplementedException();
            }
        }
    }
}
