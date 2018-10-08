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
        [Callback]
        public void OnTestRemote(int playerid)
        {
            Console.WriteLine("Calling remote");
        }
        #region Overrides of BaseMode
        protected override void OnInitialized(EventArgs e)
        {
            var loader = this.Client.NativeLoader;
            var native = loader.Load("CallRemoteFunction", new[]
            {
                NativeParameterInfo.ForType(typeof(string)),
                NativeParameterInfo.ForType(typeof(string))
            });
            var args = new object[] { "OnTestLocal", "" };
            native.Invoke(args);

            var raknet = Services.GetService<IRakNet>();
        }
        #endregion
    }
}