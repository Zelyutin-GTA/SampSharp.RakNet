using SampSharp.Core.Natives.NativeObjects;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        protected static RakNetInternal Internal;

        static RakNet()
        {
            Internal = NativeObjectProxyFactory.CreateInstance<RakNetInternal>(); // TODO: change to class extension as in BaseModeInternal
        }

        public class RakNetInternal
        {
            [NativeMethod]
            public virtual int CallRemoteFunction(string functionName, string argumentsFormat)
            {
                throw new NativeNotImplementedException();
            }
        }
    }
}
