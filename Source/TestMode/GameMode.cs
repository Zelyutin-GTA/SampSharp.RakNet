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
using SampSharp.RakNet.Syncs;

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
            //raknet.OutcomingRPC += (sender, args) => OnOutcomingRPC(args);
            raknet.IncomingPacket += (sender, args) => OnIncomingPacket(args);
            raknet.OutcomingPacket += (sender, args) => OnOutcomingPacket(args);

            BaseVehicle.Create((VehicleModelType)429, new Vector3(5, 0, 5), 0, 0, 0);
            BaseVehicle.Create((VehicleModelType)461, new Vector3(10, 0, 5), 0, 0, 0);
            BaseVehicle.Create((VehicleModelType)488, new Vector3(15, 0, 5), 0, 0, 0);
            base.OnInitialized(e);
        }
        protected override void OnPlayerSpawned(BasePlayer player, SpawnEventArgs e)
        {
            player.Position = new Vector3(100, 100, 5);
            player.GiveWeapon(Weapon.AK47, 50);
            base.OnPlayerSpawned(player, e);
        }
        #endregion

        #region Test Commands
        [Command("setnamenonrpc")]
        public static void SetNameNonRPCCommand(BasePlayer sender, string name)
        {
            sender.Name = name;
            sender.SendClientMessage("Changing name without RPC!");
        }
        [Command("setname")]
        public static void SetNameCommand(BasePlayer sender, string name)
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

        [Command("createplayer")]
        public static void CreatePlayerCommand(BasePlayer sender, int playerID, string name, int skin)
        {
            var bs = BitStream.New();
            bs.WriteValue(ParamType.UINT16, playerID, ParamType.INT32, 0, ParamType.UINT8, 0, ParamType.UINT8, name.Length, ParamType.STRING, name);

            int serverJoin = 137;
            bs.SendRPC(serverJoin, sender.Id);

            
            bs = BitStream.New();

            int team = sender.Team;
            float x = sender.Position.X;
            float y = sender.Position.Y;
            float z = sender.Position.Z;
            float angle = 0;
            int color = Color.Aqua;
            int fight = 0;
            bs.WriteValue(ParamType.UINT16, playerID, ParamType.UINT8, team, ParamType.UINT32, skin, ParamType.FLOAT, x, ParamType.FLOAT, y, ParamType.FLOAT, z, ParamType.FLOAT, angle, ParamType.UINT32, color, ParamType.UINT8, fight);

            int worldPlayerAdd = 32;
            bs.SendRPC(worldPlayerAdd, sender.Id);
            sender.SendClientMessage("Creating Player with RPC!");
        }

        [Command("setanimnonrpc")]
        public static void SetAnimNonRPCCommand(BasePlayer sender, string animlib, string animname)
        {
            sender.ApplyAnimation(animlib, animname, 1.0f, true, false, false, false, 0);
            sender.SendClientMessage("Applying animation without RPC!");
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
                    var nicknameLen = (string)args.Result["nickname"];
                    var nickname = (int)args.Result["nicknameLen"];

                    Console.WriteLine($"Nickname changed. ID: {ID}, Nickname: {nickname}; Len: {nicknameLen}");
                };

                BS.ReadValue(ParamType.UINT16, "playerID", ParamType.UINT8, "nicknameLen", ParamType.STRING, "nickname");

                /*
                var type_ID = (int)ParamType.UINT16;
                var type_nickname = (int)ParamType.STRING;
                var type_nicknameLen = (int)ParamType.UINT8;
                
                var ID = 0;
                var nickname = "";
                var nicknameLen = 0;

                var types = new Type[] { typeof(int), typeof(int).MakeByRefType(), typeof(int).MakeByRefType(), typeof(int).MakeByRefType(), typeof(int).MakeByRefType()};
                var values = new object[] { bs, type_ID, ID, type_nicknameLen, nicknameLen};
                var Read = (Instance as IHasClient).GameModeClient.NativeLoader.Load("BS_ReadValue", null, types);
                Read.Invoke(values);
                nicknameLen = (int)values[4]+1;

                types = new Type[] { typeof(int), typeof(int).MakeByRefType(), typeof(string).MakeByRefType(), typeof(int).MakeByRefType() };
                values = new object[] { bs, type_nickname, nickname, nicknameLen };
                Read = (Instance as IHasClient).GameModeClient.NativeLoader.Load("BS_ReadValue", new uint[] { 3 }, types);
                Read.Invoke(values);
                *.
                nickname = (string)values[2];

                Console.WriteLine($"Read nickname: {nickname};"); */
            }

            if (rpcid == 86)
            {
                var BS = new BitStream(bs);
                BS.ReadCompleted += (sender, args) =>
                {
                    var ID = (int)args.Result["playerID"];
                    var AnimLib = (string)args.Result["AnimLib"];
                    var AnimName = (string)args.Result["AnimName"];

                    Console.WriteLine($"Animation applied. ID: {ID}; Anim Lib: {AnimLib}; Anim Name: {AnimName}");
                };
                BS.ReadValue(
                ParamType.UINT16, "playerID",
                ParamType.UINT8, "AnimLibLength",
                ParamType.STRING, "AnimLib",
                ParamType.UINT8, "AnimNameLength",
                ParamType.STRING, "AnimName",
                ParamType.FLOAT, "fDelta",
                ParamType.BOOL, "loop",
                ParamType.BOOL, "lockx",
                ParamType.BOOL, "locky",
                ParamType.BOOL, "freeze",
                ParamType.UINT32, "dTime"
                );

                
                /*var type_ID = (int)ParamType.UINT16;
                var type_animLibLen = (int)ParamType.UINT8;
                var type_animLib = (int)ParamType.STRING;
                var type_animNameLen = (int)ParamType.UINT8;
                var type_animName = (int)ParamType.STRING;
                var type_fDelta = (int)ParamType.FLOAT;
                var type_loop = (int)ParamType.BOOL;
                var type_lockx = (int)ParamType.BOOL;
                var type_locky = (int)ParamType.BOOL;
                var type_freeze = (int)ParamType.BOOL;
                var type_dTime = (int)ParamType.UINT32;

                var pID = 0;
                var animLibLen = 0;
                var animLib = "";
                var animNameLen = 0;
                var animName = "";
                var fDelta = 0.0f;
                var loop = false;
                var lockx = false;
                var locky = false;
                var freeze = false;
                var dTime = 0;
                
                var types = new Type[] {
                typeof(int),
                typeof(int).MakeByRefType(), typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(int).MakeByRefType()
                };
                var values = new object[] { bs,
                type_ID, pID,
                type_animLibLen, animLibLen };
                var Read = (Instance as IHasClient).GameModeClient.NativeLoader.Load("BS_ReadValue", null, types);
                Read.Invoke(values);
                animLibLen = (int)values[4];

                var animLibInt = new int[0];
                types = new Type[] {
                typeof(int),
                typeof(int).MakeByRefType(), typeof(int[]).MakeByRefType(), typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(int).MakeByRefType()
                };
                values = new object[] { bs,
                type_animLib, animLibInt, animLibLen,
                type_animNameLen, animNameLen
                };
                Read = (Instance as IHasClient).GameModeClient.NativeLoader.Load("BS_ReadValue", new uint[] { 3 }, types);
                foreach (var t in values)
                {
                    Console.WriteLine(t);
                }
                Read.Invoke(values);

                animLibInt = (int[])values[2];
                var animLibChar = new char[animLibLen];
                for (int i = 0; i < animLibLen; i++)
                {
                    animLibChar[i] = (char)animLibInt[i];
                }
                animLib = new string(animLibChar);
                
                animNameLen = (int)values[5];
                foreach(var t in values)
                {
                    Console.WriteLine(t);
                }
                Console.WriteLine("Len: "+animNameLen);

                var animNameInt = new int[0];

                types = new Type[] {
                typeof(int),
                typeof(int).MakeByRefType(), typeof(int[]).MakeByRefType(), typeof(int).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(float).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(bool).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(bool).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(bool).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(bool).MakeByRefType(),
                typeof(int).MakeByRefType(), typeof(int).MakeByRefType(),
                };
                values = new object[] {
                bs,
                type_animName, animNameInt, animNameLen,
                type_fDelta, fDelta,
                type_loop, loop,
                type_lockx, lockx,
                type_locky, locky,
                type_freeze, freeze,
                type_dTime, dTime};
                Read = (Instance as IHasClient).GameModeClient.NativeLoader.Load("BS_ReadValue", new uint[] { 3 }, types);
                Read.Invoke(values);

                animNameInt = (int[])values[2];
                var animNameChar = new char[animNameLen];
                for(int i = 0; i < animNameLen; i++)
                {
                    animNameChar[i] = (char)animNameInt[i];
                }
                animName = new string(animNameChar);

                Console.WriteLine($"Read anim: LIB: {animLib}; NAME: {animName};");
                */
            }
        }

        void OnIncomingPacket(PacketRPCEventArgs e)
        {
            var bs = e.BitStreamID;
            var packetid = e.ID;
            var playerID = e.PlayerID;

            var BS = new BitStream(bs);
            switch (packetid)
            {
                case (int)PacketIdentifiers.ONFOOT_SYNC: 
                {
                    var onFoot = new OnFootSync(BS);
                    onFoot.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming OnFootSync. Position: {onFoot.position};");
                    };
                    onFoot.ReadIncoming();

                    break;
                }
                case (int)PacketIdentifiers.DRIVER_SYNC:
                {
                    var driver = new DriverSync(BS);
                    driver.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming DriverSync. Position: {driver.position};");
                    };
                    driver.ReadIncoming();

                    break;
                }
                case (int)PacketIdentifiers.AIM_SYNC:
                {
                    var aim = new AimSync(BS);
                    aim.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming AimSync. Camera looks at: {aim.cameraFrontVector};");
                    };
                    aim.ReadIncoming();

                    break;
                }
            }
        }

        void OnOutcomingPacket(PacketRPCEventArgs e)
        {
            var bs = e.BitStreamID;
            var packetid = e.ID;
            var playerID = e.PlayerID;

            var BS = new BitStream(bs);
            switch (packetid)
            {
                case (int)PacketIdentifiers.ONFOOT_SYNC:
                {
                    var onFoot = new OnFootSync(BS);
                    onFoot.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming OnFootSync. Position: {onFoot.position};");
                    };
                    onFoot.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.DRIVER_SYNC:
                {
                    var driver = new DriverSync(BS);
                    driver.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming DriverSync. Position: {driver.position};");
                    };
                    driver.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.AIM_SYNC:
                {
                    var aim = new AimSync(BS);
                    aim.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming AimSync. Camera looks at: {aim.cameraFrontVector};");
                    };
                    aim.ReadOutcoming();

                    break;
                }
            }
        }
        #endregion
    }
}