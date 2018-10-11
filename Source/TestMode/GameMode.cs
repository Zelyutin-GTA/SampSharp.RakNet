using System;

using SampSharp.GameMode;
using SampSharp.RakNet;

using SampSharp.Core;
using SampSharp.Core.Natives;
using SampSharp.GameMode.World;
using SampSharp.Core.Callbacks;

namespace TestMode
{
    public class GameMode : BaseMode, IHasClient
    {
        #region Implementation of IHasClient
        public IGameModeClient GameModeClient => Client;
        #endregion

        #region Overrides of BaseMode
        protected override void OnInitialized(EventArgs e)
        {
            var raknet = Services.GetService<IRakNet>();
            base.OnInitialized(e);
        }
        #endregion
    }
}