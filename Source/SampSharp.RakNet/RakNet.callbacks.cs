using System;
using System.Collections.Generic;
using System.Text;

using SampSharp.Core.Callbacks;
using SampSharp.GameMode.World;

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

        [Callback]
        internal void OnIncomingRPC(int playerid, int rpcid, int bs)
        {
            //player == -1 => broadcast
            //bs == 0 => no bs

            //var player = BasePlayer.FindOrCreate(playerid);
            //var rpc = new RPC(rpcid);
            Console.WriteLine($"HOOKING INCOMING {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnOutcomingRPC(int playerid, int rpcid, int bs)
        {
            Console.WriteLine($"HOOKING OUTCOMING {playerid}, {rpcid}, {bs}");
        }

        //RakNet callbacs
        /*
        OnIncomingPacket(playerid, packetid, BitStream:bs);
        OnIncomingRPC(playerid, rpcid, BitStream:bs);
        OnOutcomingPacket(playerid, packetid, BitStream:bs);
        OnOutcomingRPC(playerid, rpcid, BitStream:bs);
        */
    }
}
