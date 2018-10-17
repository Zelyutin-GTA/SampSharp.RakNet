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
            if(this.LoggingIncomingRPC) Console.WriteLine($"[SampSharp.RakNet] Hooking Incoming RPC {playerid}, {rpcid}, {bs}");   
        }

        [Callback]
        internal void OnOutcomingRPC(int playerid, int rpcid, int bs)
        {
            OutcomingRPC?.Invoke(this, new PacketRPCEventArgs(rpcid, playerid, bs));
            if (this.LoggingOutcomingRPC) Console.WriteLine($"[SampSharp.RakNet] Hooking Outcoming RPC {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnIncomingPacket(int playerid, int packetid, int bs)
        {
            IncomingPacket?.Invoke(this, new PacketRPCEventArgs(packetid, playerid, bs));
            if (this.LoggingIncomingPacket) Console.WriteLine($"[SampSharp.RakNet] Hooking Incoming Packet {playerid}, {packetid}, {bs}");
        }
        [Callback]
        internal void OnOutcomingPacket(int playerid, int packetid, int bs)
        {
            OutcomingPacket?.Invoke(this, new PacketRPCEventArgs(packetid, playerid, bs));
            if (this.LoggingOutcomingPacket) Console.WriteLine($"[SampSharp.RakNet] Hooking Outcoming Packet {playerid}, {packetid}, {bs}");
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
