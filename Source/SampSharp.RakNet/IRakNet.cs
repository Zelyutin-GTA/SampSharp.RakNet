using System;
using SampSharp.GameMode;
using SampSharp.RakNet.Events;

namespace SampSharp.RakNet
{
    public interface IRakNet : IService
    {
        event EventHandler<PacketRpcEventArgs> IncomingRpc;
        event EventHandler<PacketRpcEventArgs> OutcomingRpc;
        event EventHandler<PacketRpcEventArgs> IncomingPacket;
        event EventHandler<PacketRpcEventArgs> OutcomingPacket;

        void SetLogging(bool incomingRpc, bool outcomingRpc, bool incomingPacket, bool outcomingPacket, bool blockingRpc, bool blockingPacket);
        void BlockRpc();
        void BlockPacket();
        void PostLoad(BaseMode gameMode);
    }
}
