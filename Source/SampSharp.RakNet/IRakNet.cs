using System;
using SampSharp.GameMode;
using SampSharp.RakNet.Events;

namespace SampSharp.RakNet
{
    public interface IRakNet : IService
    {
        event EventHandler<PacketRPCEventArgs> IncomingRPC;
        event EventHandler<PacketRPCEventArgs> OutcomingRPC;
        event EventHandler<PacketRPCEventArgs> IncomingPacket;
        event EventHandler<PacketRPCEventArgs> OutcomingPacket;

        void PostLoad(BaseMode gameMode);
    }
}
