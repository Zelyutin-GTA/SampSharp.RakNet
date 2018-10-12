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
            // Add the raknet service to the service provider.
            Console.WriteLine("SampSharp.RakNet is loading");

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

        bool LoggingIncomingRPC = false;
        bool LoggingOutcomingRPC = false;
        bool LoggingIncomingPacket = false;
        bool LoggingOutcomingPacket = false;

        public void SetLogging(bool incomingRPC, bool outcomingRPC, bool incomingPacket, bool outcomingPacket)
        {
            this.LoggingIncomingRPC = incomingRPC;
            this.LoggingOutcomingRPC = outcomingRPC;
            this.LoggingIncomingPacket = incomingPacket;
            this.LoggingOutcomingPacket = outcomingPacket;
        }
    }
}
