using System;
using SampSharp.GameMode;
using SampSharp.GameMode.API;

using SampSharp.RakNet;

[assembly: SampSharpExtension(typeof(RakNet))]

namespace SampSharp.RakNet
{
    public partial class RakNet : Extension, IRakNet
    {
        public override void PostLoad(BaseMode gameMode)
        {
            Console.WriteLine("SampSharp.RakNet was loaded!");
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
