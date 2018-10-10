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

        public override void PostLoad(BaseMode gameMode)
        {
            Console.WriteLine("SampSharp.RakNet was loaded!");

            if(!typeof(IHasClient).IsAssignableFrom(gameMode.GetType()))
            {
                throw new Exception("Gamemode should implement IHasClient interface to use SampSharp.RakNet");
            }
            RakNet.Mode = gameMode;

            base.PostLoad(gameMode);
        }

        #region Implementation of IService

        /// <summary>
        ///     Gets the game mode.
        /// </summary>
        public BaseMode GameMode { get; private set; }

        #endregion
    }
}
