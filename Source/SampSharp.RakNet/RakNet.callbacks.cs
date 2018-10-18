using System;

using SampSharp.Core.Callbacks;
using SampSharp.RakNet.Definitions;
using SampSharp.RakNet.Events;

namespace SampSharp.RakNet
{
    public partial class RakNet
    {
        public event EventHandler<PacketRpcEventArgs> IncomingRpc;
        public event EventHandler<PacketRpcEventArgs> OutcomingRpc;
        public event EventHandler<PacketRpcEventArgs> IncomingPacket;
        public event EventHandler<PacketRpcEventArgs> OutcomingPacket;

        [Callback]
        internal void OnIncomingRpc(int playerid, int rpcid, int bs)
        {
            IncomingRpc?.Invoke(this, new PacketRpcEventArgs(rpcid, playerid, bs));
            if(LoggingIncomingRpc) Console.WriteLine($"[SampSharp.RakNet] Hooking Incoming Rpc {playerid}, {rpcid}, {bs}");   
        }

        [Callback]
        internal void OnOutcomingRpc(int playerid, int rpcid, int bs)
        {
            OutcomingRpc?.Invoke(this, new PacketRpcEventArgs(rpcid, playerid, bs));
            if (LoggingOutcomingRpc) Console.WriteLine($"[SampSharp.RakNet] Hooking Outcoming Rpc {playerid}, {rpcid}, {bs}");
        }
        [Callback]
        internal void OnIncomingPacket(int playerid, int packetid, int bs)
        {
            IncomingPacket?.Invoke(this, new PacketRpcEventArgs(packetid, playerid, bs));
            if (LoggingIncomingPacket) Console.WriteLine($"[SampSharp.RakNet] Hooking Incoming Packet {playerid}, {packetid}, {bs}");
        }
        [Callback]
        internal void OnOutcomingPacket(int playerid, int packetid, int bs)
        {
            OutcomingPacket?.Invoke(this, new PacketRpcEventArgs(packetid, playerid, bs));
            if (LoggingOutcomingPacket) Console.WriteLine($"[SampSharp.RakNet] Hooking Outcoming Packet {playerid}, {packetid}, {bs}");
        }
        //RakNet callbacs
        /*
        OnIncomingPacket(playerid, packetid, BitStream:bs);
        OnIncomingRpc(playerid, rpcid, BitStream:bs);
        OnOutcomingPacket(playerid, packetid, BitStream:bs);
        OnOutcomingRpc(playerid, rpcid, BitStream:bs);
        */
    }
}
