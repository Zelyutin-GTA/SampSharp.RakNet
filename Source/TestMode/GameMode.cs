using System;

using SampSharp.GameMode;
using SampSharp.RakNet;

namespace TestMode
{
    public class GameMode : BaseMode
    {
        #region Overrides of BaseMode
        protected override void OnInitialized(EventArgs e)
        {
            var raknet = Services.GetService<IRakNet>();

            base.OnInitialized(e);
        }
        #endregion
    }
}