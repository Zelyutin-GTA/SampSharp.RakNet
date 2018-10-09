using System;

using SampSharp.GameMode;
using SampSharp.RakNet;

using SampSharp.Core;
using SampSharp.Core.Natives;
using SampSharp.GameMode.World;
using SampSharp.Core.Callbacks;

namespace TestMode
{
    public class GameMode : BaseMode
    {
        #region Overrides of BaseMode
        protected override void OnInitialized(EventArgs e)
        {
            var raknet = Services.GetService<IRakNet>();
        }
        #endregion
    }
}