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
            Console.WriteLine($"[S#] Hooking Incoming RPC {playerid}, {rpcid}, {bs}");
            //BlockRPC();
        }
        [Callback]
        internal void OnOutcomingRPC(int playerid, int rpcid, int bs)
        {
            Console.WriteLine($"[S#] Hooking Outcoming RPC {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnIncomingPacket(int playerid, int packetid, int bs)
        {
            Console.WriteLine($"[S#] Hooking Incoming Packet {playerid}, {packetid}, {bs}");
            BlockPacket();
        }
        [Callback]
        internal void OnOutcomingPacket(int playerid, int packetid, int bs)
        {
            Console.WriteLine($"[S#] Hooking Outcoming Packet {playerid}, {packetid}, {bs}");
        }
        internal void BlockRPC()
        {
            Console.WriteLine($"[S#] Block next RPC");
            Internal.CallRemoteFunction("BlockNextRPC", "");
        }
        internal void BlockPacket()
        {
            Console.WriteLine($"[S#] Block next Packet");
            Internal.CallRemoteFunction("BlockNextPacket", "");
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
