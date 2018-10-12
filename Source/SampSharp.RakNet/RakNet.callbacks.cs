using System;

using SampSharp.Core.Callbacks;
using SampSharp.RakNet.Definitions;
using SampSharp.RakNet.Events;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        public event EventHandler<PacketRPCEventArgs> IncomingRPC;
        public event EventHandler<PacketRPCEventArgs> OutcomingRPC;
        public event EventHandler<PacketRPCEventArgs> IncomingPacket;
        public event EventHandler<PacketRPCEventArgs> OutcomingPacket;

        [Callback]
        internal void OnIncomingRPC(int playerid, int rpcid, int bs)
        {
            IncomingRPC?.Invoke(this, new PacketRPCEventArgs(rpcid, playerid, bs));
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
            OutcomingRPC?.Invoke(this, new PacketRPCEventArgs(rpcid, playerid, bs));
            Console.WriteLine($"[S#] Hooking Outcoming RPC {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnIncomingPacket(int playerid, int packetid, int bs)
        {
            IncomingPacket?.Invoke(this, new PacketRPCEventArgs(packetid, playerid, bs));
            Console.WriteLine($"[S#] Hooking Incoming Packet {playerid}, {packetid}, {bs}");
            //BlockPacket();
        }
        [Callback]
        internal void OnOutcomingPacket(int playerid, int packetid, int bs)
        {
            OutcomingPacket?.Invoke(this, new PacketRPCEventArgs(packetid, playerid, bs));
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
