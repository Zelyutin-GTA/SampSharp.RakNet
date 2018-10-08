using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet
{
    class RPC
    {
        public int ID { get; private set; }

        public RPC(int rpcID)
        {
            this.ID = rpcID;
        }
    }
}
