using System;
using SampSharp.GameMode;
using SampSharp.GameMode.API;

using SampSharp.RakNet;

using SampSharp.Core;
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

            this.GameMode = gameMode;
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
            this.LoggingIncomingRpc = incomingRpc;
            this.LoggingOutcomingRpc = outcomingRpc;
            this.LoggingIncomingPacket = incomingPacket;
            this.LoggingOutcomingPacket = outcomingPacket;
            this.LoggingBlockingRpc = blockingRpc;
            this.LoggingBlockingPacket = blockingPacket;
        }

        public void BlockRpc()
        {
            if (this.LoggingBlockingRpc) Console.WriteLine($"[S#] Blocking next Rpc");
            Internal.CallRemoteFunction("BlockNextRpc", "");
        }
        public void BlockPacket()
        {
            if (this.LoggingBlockingPacket) Console.WriteLine($"[S#] Blocking next Packet");
            Internal.CallRemoteFunction("BlockNextPacket", "");
        }

    }
}
