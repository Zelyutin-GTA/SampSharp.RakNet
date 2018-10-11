using System;

namespace SampSharp.RakNet.Events
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
