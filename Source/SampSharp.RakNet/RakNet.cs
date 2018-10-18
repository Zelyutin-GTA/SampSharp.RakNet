using System;

using SampSharp.GameMode;
using SampSharp.GameMode.API;
using SampSharp.Core;

using SampSharp.RakNet;
using SampSharp.RakNet.Definitions;


[assembly: SampSharpExtension(typeof(RakNet))]

namespace SampSharp.RakNet
{
    public partial class RakNet : Extension, IRakNet
    {
        internal static BaseMode Mode;
        internal static IGameModeClient Client => ((IHasClient)Mode).GameModeClient;

        #region Implementation of IService

        /// <summary>
        ///     Gets the game mode.
        /// </summary>
        public BaseMode GameMode { get; private set; }

        #endregion

        #region Overrides of Extension

        /// <summary>
        ///     Loads services provided by this extensions.
        /// </summary>
        /// <param name="gameMode">The game mode.</param>
        public override void LoadServices(BaseMode gameMode)
        {
            if (!typeof(IHasClient).IsAssignableFrom(gameMode.GetType()))
            {
                throw new RakNetException("[SampSharp.RakNet] Gamemode should implement IHasClient interface to use SampSharp.RakNet");
            }

            GameMode = gameMode;
            RakNet.Mode = gameMode;
            gameMode.Services.AddService<IRakNet>(this);

            base.LoadServices(gameMode);
        }

        #endregion

        public bool LoggingIncomingRpc { get; set; } = false;
        public bool LoggingOutcomingRpc { get; set; } = false;
        public bool LoggingIncomingPacket { get; set; } = false;
        public bool LoggingOutcomingPacket { get; set; } = false;
        public bool LoggingBlockingRpc { get; set; } = false;
        public bool LoggingBlockingPacket { get; set; } = false;

        public void SetLogging(bool incomingRpc, bool outcomingRpc, bool incomingPacket, bool outcomingPacket, bool blockingRpc, bool blockingPacket)
        {
            LoggingIncomingRpc = incomingRpc;
            LoggingOutcomingRpc = outcomingRpc;
            LoggingIncomingPacket = incomingPacket;
            LoggingOutcomingPacket = outcomingPacket;
            LoggingBlockingRpc = blockingRpc;
            LoggingBlockingPacket = blockingPacket;
        }

        public void BlockRpc()
        {
            if (LoggingBlockingRpc) Console.WriteLine($"[S#] Blocking next Rpc");
            Internal.CallRemoteFunction("BlockNextRpc", "");
        }
        public void BlockPacket()
        {
            if (LoggingBlockingPacket) Console.WriteLine($"[S#] Blocking next Packet");
            Internal.CallRemoteFunction("BlockNextPacket", "");
        }

        #region Sending RPCs and Packets
        public bool SendRpc(BitStream bs, int rpcId, int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            bool result = Internal.BS_RPC(bs.Id, playerId, rpcId, (int)priority, (int)reliability);
            return result;
        }
        public bool SendRpc(int rpcId, int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            bool result = Internal.BS_RPC(0, playerId, rpcId, (int)priority, (int)reliability);
            return result;
        }
        public bool SendPacket(BitStream bs, int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            bool result = Internal.BS_Send(bs.Id, playerId, (int)priority, (int)reliability);
            return result;
        }
        public bool SendPacket(int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            bool result = Internal.BS_Send(0, playerId, (int)priority, (int)reliability);
            return result;
        }
        #endregion
    }
}
