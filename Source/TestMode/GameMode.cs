using System;

using SampSharp.GameMode;
using SampSharp.RakNet;

using SampSharp.Core;
using SampSharp.Core.Natives;
using SampSharp.GameMode.World;
using SampSharp.Core.Callbacks;
using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

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
            raknet.IncomingRPC += (sender, args) => OnIncomingRPC(args);
            base.OnInitialized(e);
        }

        void OnIncomingRPC(PacketRPCEventArgs e)
        {
            var bs = e.BitStreamID;
            var rpcid = e.ID;
            var playerID = e.PlayerID;

            if (rpcid == 119)
            {
                var BS = new BitStream(bs);
                BS.ReadCompleted += (sender, args) =>
                {
                    float _x = (float)args.Result[0];
                    float _y = (float)args.Result[1];
                    float _z = (float)args.Result[2];

                    Console.WriteLine($"Map Clicked {_x};{_y};{_z};");
                };
                float x = 0.0f, y = 0.0f, z = 0.0f;
                
                BS.ReadValue(ParamType.FLOAT, x, ParamType.FLOAT, y, ParamType.FLOAT, z);
            }
        }
        #endregion
    }
}