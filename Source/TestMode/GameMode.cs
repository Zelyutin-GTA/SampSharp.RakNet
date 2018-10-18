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
        IRakNet RakNet;
        #region Implementation of IHasClient
        public IGameModeClient GameModeClient => Client;
        #endregion

        #region Overrides of BaseMode
        protected override void OnInitialized(EventArgs e)
        {
            RakNet = Services.GetService<IRakNet>();
            RakNet.SetLogging(false, false, false, false, true, true);
            RakNet.IncomingRPC += (sender, args) => OnIncomingRPC(args);
            RakNet.OutcomingRPC += (sender, args) => OnOutcomingRPC(args);
            RakNet.IncomingPacket += (sender, args) => OnIncomingPacket(args);
            RakNet.OutcomingPacket += (sender, args) => OnOutcomingPacket(args);

            BaseVehicle.Create((VehicleModelType)429, new Vector3(5, 0, 5), 0, 0, 0);
            BaseVehicle.Create((VehicleModelType)461, new Vector3(10, 0, 5), 0, 0, 0);
            BaseVehicle.Create((VehicleModelType)488, new Vector3(15, 0, 5), 0, 0, 0);

            BaseVehicle.Create((VehicleModelType)403, new Vector3(25, 0, 5), 0, 0, 0);
            BaseVehicle.Create((VehicleModelType)435, new Vector3(30, 0, 5), 0, 0, 0);
            base.OnInitialized(e);
        }
        protected override void OnPlayerSpawned(BasePlayer player, SpawnEventArgs e)
        {
            player.Position = new Vector3(40, 0, 5);
            player.GiveWeapon(Weapon.AK47, 50);
            player.Health = 76;
            player.Armour = 156;
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

            BS.WriteValue(ParamType.Uint16, sender.Id, ParamType.Uint8, name.Length, ParamType.String, name);

            BS.SendRPC(11, sender.Id);
            sender.SendClientMessage("Changing name with RPC!");
        }
        [Command("setpos")]
        public static void SetPosCommand(BasePlayer sender, float x, float y, float z)
        {
            var BS = BitStream.New();

            BS.WriteValue(ParamType.Float, x, ParamType.Float, y, ParamType.Float, z);

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

            bs.WriteValue(ParamType.Float, x, ParamType.Float, y, ParamType.Float, z, ParamType.Uint16, type, ParamType.Float, radius);

            int createExplosionRPC = 79;
            bs.SendRPC(createExplosionRPC, sender.Id);
            sender.SendClientMessage("Creating Explosion with RPC!");
        }

        [Command("createplayer")]
        public static void CreatePlayerCommand(BasePlayer sender, int playerID, string name, int skin)
        {
            var bs = BitStream.New();
            bs.WriteValue(ParamType.Uint16, playerID, ParamType.Int32, 0, ParamType.Uint8, 0, ParamType.Uint8, name.Length, ParamType.String, name);

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
            bs.WriteValue(ParamType.Uint16, playerID, ParamType.Uint8, team, ParamType.Uint32, skin, ParamType.Float, x, ParamType.Float, y, ParamType.Float, z, ParamType.Float, angle, ParamType.Uint32, color, ParamType.Uint8, fight);

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

        [Command("spectate")]
        public static void SpectateCommand(BasePlayer sender, int specPlayerID)
        {
            var specPlayer = BasePlayer.Find(specPlayerID);
            if(specPlayer == null)
            {
                sender.SendClientMessage("Player not found!");
                return;
            }
            sender.ToggleSpectating(true);
            sender.SpectatePlayer(specPlayer);
            sender.SendClientMessage("Spectating player!");
        }
        [Command("unspectate")]
        public static void UnspectateCommand(BasePlayer sender)
        {
            sender.ToggleSpectating(false);
            sender.SendClientMessage("Spectating player!");
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
                
                BS.ReadValue(ParamType.Float, "x", ParamType.Float, "y", ParamType.Float, "z");
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
                    ParamType.Int32, "version",
                    ParamType.Uint8, "mod",
                    ParamType.Uint8, "nicknameLen",
                    ParamType.String, "nickname",
                    ParamType.Uint32, "challengeResponse",
                    ParamType.Uint8, "authKeyLen",
                    ParamType.String, "authKey",
                    ParamType.Uint8, "clientVersionLen",
                    ParamType.String, "clientVersion"
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

                BS.ReadValue(ParamType.Uint16, "playerID", ParamType.Uint8, "nicknameLen", ParamType.String, "nickname");
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
                ParamType.Uint16, "playerID",
                ParamType.Uint8, "AnimLibLength",
                ParamType.String, "AnimLib",
                ParamType.Uint8, "AnimNameLength",
                ParamType.String, "AnimName",
                ParamType.Float, "fDelta",
                ParamType.Bool, "loop",
                ParamType.Bool, "lockx",
                ParamType.Bool, "locky",
                ParamType.Bool, "freeze",
                ParamType.Uint32, "dTime"
                );
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
                case (int)PacketIdentifiers.OnFootSync: 
                {
                    var onFoot = new OnFootSync(BS);
                    onFoot.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming OnFootSync. Position: {onFoot.position}; Health: {onFoot.health}; Armour: {onFoot.armour}; Velocity: {onFoot.velocity};");
                    };
                    onFoot.ReadIncoming();
                    
                    break;
                }
                case (int)PacketIdentifiers.DriverSync:
                {
                    var driver = new DriverSync(BS);
                    driver.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming DriverSync. Position: {driver.position};");
                        
                    };
                    driver.ReadIncoming();
                    break;
                }
                case (int)PacketIdentifiers.AimSync:
                {
                    var aim = new AimSync(BS);
                    aim.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming AimSync. Camera looks at: {aim.cameraFrontVector};");
                    };
                    aim.ReadIncoming();
                    
                    break;
                }
                case (int)PacketIdentifiers.BulletSync:
                {
                    var bullet = new BulletSync(BS);
                    bullet.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming BulletSync. Bullet hit: {bullet.hitPosition};");
                        BS.ResetWritePointer();
                        bullet.origin = new Vector3(bullet.origin.X, bullet.origin.Y, bullet.origin.Z + 3);
                        bullet.offsets = new Vector3(bullet.offsets.X, bullet.offsets.Y, bullet.offsets.Z + 3);
                        bullet.hitPosition = new Vector3(bullet.hitPosition.X, bullet.hitPosition.Y, bullet.hitPosition.Z + 3);
                        bullet.WriteIncoming();
                    };
                    bullet.ReadIncoming();

                    break;
                }
                case (int)PacketIdentifiers.PassengerSync:
                {
                    var passenger = new PassengerSync(BS);
                    passenger.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming PassengerSync. VehicleID: {passenger.vehicleID}; Position: {passenger.position}; DriveBy: {passenger.driveBy};");
                    };
                    passenger.ReadIncoming();

                    break;
                }
                case (int)PacketIdentifiers.SpectatorSync:
                {
                    var spectator = new SpectatorSync(BS);
                    spectator.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming SpectatorSync. Position: {spectator.position};");
                    };
                    spectator.ReadIncoming();

                    break;
                }
                case (int)PacketIdentifiers.TrailerSync:
                {
                    var trailer = new TrailerSync(BS);
                    trailer.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming TrailerSync. Position: {trailer.position};");
                    };
                    trailer.ReadIncoming();

                    break;
                }
                case (int)PacketIdentifiers.UnoccupiedSync:
                {
                    var unoccupied = new UnoccupiedSync(BS);
                    unoccupied.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading incoming UnoccupiedSync. Position: {unoccupied.position}; Health: {unoccupied.vehicleHealth};");
                    };
                    unoccupied.ReadIncoming();

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
                case (int)PacketIdentifiers.OnFootSync:
                {
                    var onFoot = new OnFootSync(BS);
                    onFoot.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming OnFootSync. Position: {onFoot.position}; Health: {onFoot.health}; Armour: {onFoot.armour}; Velocity: {onFoot.velocity};");
                        BS.ResetWritePointer();
                        onFoot.position = new Vector3(onFoot.position.X, onFoot.position.Y, onFoot.position.Z + 10);
                        onFoot.WriteOutcoming();
                    };
                    onFoot.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.DriverSync:
                {
                    var driver = new DriverSync(BS);
                    driver.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming DriverSync. Position: {driver.position};");
                        BS.ResetWritePointer();
                        driver.position = new Vector3(driver.position.X, driver.position.Y, driver.position.Z + 10);
                        driver.WriteOutcoming();
                    };
                    driver.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.AimSync:
                {
                    var aim = new AimSync(BS);
                    aim.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming AimSync. Camera looks at: {aim.cameraFrontVector};");
                    };
                    aim.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.BulletSync:
                {
                    var bullet = new BulletSync(BS);
                    bullet.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming BulletSync. Bullet hit: {bullet.hitPosition};");
                    };
                    bullet.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.PassengerSync:
                {
                    var passenger = new PassengerSync(BS);
                    passenger.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming PassengerSync. VehicleID: {passenger.vehicleID}; Position: {passenger.position}; DriveBy: {passenger.driveBy};");
                    };
                    passenger.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.SpectatorSync:
                {
                    //Spectator syncronization does not have outcoming packets;

                    break;
                }
                case (int)PacketIdentifiers.TrailerSync:
                {
                    var trailer = new TrailerSync(BS);
                    trailer.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming TrailerSync. Position: {trailer.position};");
                    };
                    trailer.ReadOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.UnoccupiedSync:
                {
                    var unoccupied = new UnoccupiedSync(BS);
                    unoccupied.ReadCompleted += (sender, args) =>
                    {
                        Console.WriteLine($"Reading outcoming UnoccupiedSync. Position: {unoccupied.position}; Health: {unoccupied.vehicleHealth};");
                    };
                    unoccupied.ReadOutcoming();

                    break;
                }
            }
        }
        #endregion
    }
}