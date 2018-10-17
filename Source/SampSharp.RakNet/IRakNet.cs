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

        void SetLogging(bool incomingRPC, bool outcomingRPC, bool incomingPacket, bool outcomingPacket, bool blockingRPC, bool blockingPacket);
        void BlockRPC();
        void BlockPacket();
        void PostLoad(BaseMode gameMode);
    }
}
