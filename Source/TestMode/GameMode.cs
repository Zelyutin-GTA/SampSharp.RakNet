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
            //raknet.IncomingRPC += (sender, args) => OnIncomingRPC(args);
            raknet.OutcomingRPC += (sender, args) => OnOutcomingRPC(args);
            base.OnInitialized(e);
        }
        #endregion
        [Command("setname")]
        public static void ChangeNameCommand(BasePlayer sender, string name)
        {
            var BS = BitStream.New();

            BS.WriteValue(ParamType.UINT16, sender.Id, ParamType.UINT8, name.Length, ParamType.STRING, name);

            BS.SendRPC(11, sender.Id);
            //Console.WriteLine($"Nickname changed. ID: {ID}, Nickname: {nickname}; Len: {nicknameLen}");
            sender.SendClientMessage("Changing name with RPC!");
        }
        [Command("setpos")]
        public static void SetPosCommand(BasePlayer sender, float x, float y, float z)
        {
            var BS = BitStream.New();

            BS.WriteValue(ParamType.FLOAT, x, ParamType.FLOAT, y, ParamType.FLOAT, z);

            int setPosRPC = 12;
            BS.SendRPC(setPosRPC, sender.Id);
            sender.SendClientMessage("Setting position with RPC!");
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
            sender.SendClientMessage("Creating Explosion with RPC!");
        }

        [Command("join")]
        public static void JoinCommand(BasePlayer sender)
        {
            var bs = BitStream.New();
            int playerID = 228;
            string name = "Daniele_Stradivari";
            bs.WriteValue(ParamType.UINT16, playerID, ParamType.INT32, 0, ParamType.UINT8, 0, ParamType.UINT8, name.Length, ParamType.STRING, name);

            int serverJoin = 137;
            bs.SendRPC(serverJoin, sender.Id);

            bs = BitStream.New();
            playerID = 322;
            name = "Matteo_Machiavelli";
            bs.WriteValue(ParamType.UINT16, playerID, ParamType.INT32, 0, ParamType.UINT8, 0, ParamType.UINT8, name.Length, ParamType.STRING, name);

            bs.SendRPC(serverJoin, sender.Id);
            sender.SendClientMessage("Creating Joining with RPC!");
        }
        [Command("player")]
        public static void PlayerCommand(BasePlayer sender)
        {
            var bs = BitStream.New();
            int playerID = 228;
            int team = sender.Team;
            int skin = 228;
            float x = sender.Position.X;
            float y = sender.Position.Y;
            float z = sender.Position.Z;
            float angle = 0;
            int color = Color.Aqua;
            int fight = 0;
            bs.WriteValue(ParamType.UINT16, playerID, ParamType.UINT8, team, ParamType.UINT32, skin, ParamType.FLOAT, x, ParamType.FLOAT, y, ParamType.FLOAT, z, ParamType.FLOAT, angle, ParamType.UINT32, color, ParamType.UINT8, fight);

            int worldPlayerAdd = 32;
            bs.SendRPC(worldPlayerAdd, sender.Id);

            bs = BitStream.New();
            playerID = 322;
            team = sender.Team;
            skin = 228;
            x = sender.Position.X;
            y = sender.Position.Y;
            z = sender.Position.Z;
            angle = 0;
            color = Color.Aqua;
            fight = 0;
            bs.WriteValue(ParamType.UINT16, playerID, ParamType.UINT8, team, ParamType.UINT32, skin, ParamType.FLOAT, x, ParamType.FLOAT, y, ParamType.FLOAT, z, ParamType.FLOAT, angle, ParamType.UINT32, color, ParamType.UINT8, fight);

            bs.SendRPC(worldPlayerAdd, sender.Id);
            sender.SendClientMessage("Creating RPC Player!");
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

        void OnOutcomingRPC(PacketRPCEventArgs e)
        {
            var bs = e.BitStreamID;
            var rpcid = e.ID;
            var playerID = e.PlayerID;

            if (rpcid == 11)
            {
                var BS = new BitStream(bs);
                BS.ReadCompleted += (sender, args) =>
                {
                    var ID = (int)args.Result["playerID"];
                    var nicknameLen = (int)args.Result["nickname"];
                    var nickname = (string)args.Result["nicknameLen"];

                    Console.WriteLine($"Nickname changed. ID: {ID}, Nickname: {nickname}; Len: {nicknameLen}");
                };

                BS.ReadValue(ParamType.UINT16, "playerID", ParamType.UINT8, "nicknameLen", ParamType.STRING, "nickname");
            }
        }
    }
}