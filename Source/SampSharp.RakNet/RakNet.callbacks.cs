using System;
using System.Collections.Generic;
using System.Text;

using SampSharp.Core.Callbacks;
using SampSharp.GameMode.World;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        [Callback]
        internal void OnIncomingRPC(int playerid, int rpcid, int bs)
        {
            //player == -1 => broadcast
            //bs == 0 => no bs

            //var player = BasePlayer.FindOrCreate(playerid);
            //var rpc = new RPC(rpcid);
            Console.WriteLine($"HOOKING INCOMING RPC {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnOutcomingRPC(int playerid, int rpcid, int bs)
        {
            Console.WriteLine($"HOOKING OUTCOMING RPC {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnIncomingPacket(int playerid, int packetid, int bs)
        {
            Console.WriteLine($"HOOKING INCOMING PACKET {playerid}, {packetid}, {bs}");
        }
        [Callback]
        internal void OnOutcomingPacket(int playerid, int packetid, int bs)
        {
            Console.WriteLine($"HOOKING OUTCOMING PACKET {playerid}, {packetid}, {bs}");
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
