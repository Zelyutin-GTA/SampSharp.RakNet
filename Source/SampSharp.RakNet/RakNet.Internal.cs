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
        }
    }
}
