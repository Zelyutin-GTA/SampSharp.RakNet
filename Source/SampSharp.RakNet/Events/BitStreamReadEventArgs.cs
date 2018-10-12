using System;
using System.Collections.Generic;

namespace SampSharp.RakNet.Events
{
    public class BitStreamReadEventArgs : EventArgs
    {
        public Dictionary<string, object> Result { get; private set; }
        public BitStreamReadEventArgs(Dictionary<string, object> result)
        {
            this.Result = result;
        }
    }
}
