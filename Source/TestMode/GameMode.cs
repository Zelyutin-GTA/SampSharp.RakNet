using System;

using SampSharp.Core;
using SampSharp.GameMode;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP;

using SampSharp.RakNet;
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

        [Command("explode")]
        public static void ExplodeCommand(BasePlayer sender)
        {
            var bs = BitStream.New();
            float x = sender.Position.X;
            float y = sender.Position.Y;
            float z = sender.Position.Z;
            int type = 1;
            float radius = 100.0f;

            bs.WriteValue(ParamType.FLOAT, x, ParamType.FLOAT, y, ParamType.FLOAT, z, ParamType.UINT16, type, ParamType.FLOAT, radius);

            int createExplosionRPC = 79;
            bs.SendRPC(createExplosionRPC, sender.Id);
            sender.SendClientMessage("Creating RPC Explosion!");
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
                    float x = (float)args.Result["x"];
                    float y = (float)args.Result["y"];
                    float z = (float)args.Result["z"];

                    Console.WriteLine($"Map Clicked X: {x}; Y: {y}; Z: {z};");
                };
                
                BS.ReadValue(ParamType.FLOAT, "x", ParamType.FLOAT, "y", ParamType.FLOAT, "z");
            }
            if (rpcid == 25)
            {
                var BS = new BitStream(bs);
                BS.ReadCompleted += (sender, args) =>
                {
                    var nickname = (string) args.Result["nickname"];
                    var clientVersion = (string)args.Result["clientVersion"];
                    var authKey = (string)args.Result["authKey"];

                    Console.WriteLine($"Client joined. Version: {clientVersion}; Auth Key: {authKey}; Nickname: {nickname};");
                };

                BS.ReadValue(
                    ParamType.INT32, "version",
                    ParamType.UINT8, "mod",
                    ParamType.UINT8, "nicknameLen",
                    ParamType.STRING, "nickname",
                    ParamType.UINT32, "challengeResponse",
                    ParamType.UINT8, "authKeyLen",
                    ParamType.STRING, "authKey",
                    ParamType.UINT8, "clientVersionLen",
                    ParamType.STRING, "clientVersion"
                );
            }
        }
        #endregion
    }
}