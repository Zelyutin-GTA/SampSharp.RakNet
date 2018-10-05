using System;
using System.Collections.Generic;
using System.Text;

using SampSharp.Core.Callbacks;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        //Callback example from streamer:
        /*[Callback]
        internal void OnDynamicObjectMoved(int objectid)
        {
            var @object = DynamicObject.Find(objectid);

            if (@object == null)
                return;

            OnDynamicObjectMoved(@object, EventArgs.Empty);
        }*/

        //RakNet callbacs
        /*
        OnIncomingPacket(playerid, packetid, BitStream:bs);
        OnIncomingRPC(playerid, rpcid, BitStream:bs);
        OnOutcomingPacket(playerid, packetid, BitStream:bs);
        OnOutcomingRPC(playerid, rpcid, BitStream:bs);
        */
    }
}
