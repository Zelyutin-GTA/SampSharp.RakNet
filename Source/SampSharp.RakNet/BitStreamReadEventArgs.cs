using System;

namespace SampSharp.RakNet
{
    public class BitStreamReadEventArgs : EventArgs
    {
        public object[] Result { get; private set; }
        public BitStreamReadEventArgs(object[] result)
        {
            this.Result = result;
        }
    }
}
