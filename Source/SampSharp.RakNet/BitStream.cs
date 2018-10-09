using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet
{
    partial class BitStream
    {
        public readonly int ID;
        public BitStream(int id)
        {
            this.ID = id;
        }
        /*public static BitStream New()
        {
            int id = BS_New();
            return new BitStream(id);
        }*/
    }
}
