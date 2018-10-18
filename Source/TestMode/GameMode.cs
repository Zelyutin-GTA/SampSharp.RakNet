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
            RakNet.IncomingRpc += (sender, args) => OnIncomingRpc(args);
            RakNet.OutcomingRpc += (sender, args) => OnOutcomingRpc(args);
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
        public static void SetNameNonRpcCommand(BasePlayer sender, string name)
        {
            sender.Name = name;
            sender.SendClientMessage("Changing name without Rpc!");
        }
        [Command("setname")]
        public static void SetNameCommand(BasePlayer sender, string name)
        {
            var BS = BitStream.New();

            BS.WriteValue(ParamType.UInt16, sender.Id, ParamType.UInt8, name.Length, ParamType.String, name);

            BS.SendRpc(11, sender.Id);
            sender.SendClientMessage("Changing name with Rpc!");
        }
        [Command("setpos")]
        public static void SetPosCommand(BasePlayer sender, float x, float y, float z)
        {
            var BS = BitStream.New();

            BS.WriteValue(ParamType.Float, x, ParamType.Float, y, ParamType.Float, z);

            int setPosRpc = 12;
            BS.SendRpc(setPosRpc, sender.Id);
            sender.SendClientMessage("Setting position with Rpc!");
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

            bs.WriteValue(ParamType.Float, x, ParamType.Float, y, ParamType.Float, z, ParamType.UInt16, type, ParamType.Float, radius);

            int createExplosionRpc = 79;
            bs.SendRpc(createExplosionRpc, sender.Id);
            sender.SendClientMessage("Creating Explosion with Rpc!");
        }

        [Command("createplayer")]
        public static void CreatePlayerCommand(BasePlayer sender, int playerId, string name, int skin)
        {
            var bs = BitStream.New();
            bs.WriteValue(ParamType.UInt16, playerId, ParamType.Int32, 0, ParamType.UInt8, 0, ParamType.UInt8, name.Length, ParamType.String, name);

            int serverJoin = 137;
            bs.SendRpc(serverJoin, sender.Id);

            
            bs = BitStream.New();

            int team = sender.Team;
            float x = sender.Position.X;
            float y = sender.Position.Y;
            float z = sender.Position.Z;
            float angle = 0;
            int color = Color.Aqua;
            int fight = 0;
            bs.WriteValue(ParamType.UInt16, playerId, ParamType.UInt8, team, ParamType.UInt32, skin, ParamType.Float, x, ParamType.Float, y, ParamType.Float, z, ParamType.Float, angle, ParamType.UInt32, color, ParamType.UInt8, fight);

            int worldPlayerAdd = 32;
            bs.SendRpc(worldPlayerAdd, sender.Id);
            sender.SendClientMessage("Creating Player with Rpc!");
        }

        [Command("setanimnonrpc")]
        public static void SetAnimNonRpcCommand(BasePlayer sender, string animlib, string animname)
        {
            sender.ApplyAnimation(animlib, animname, 1.0f, true, false, false, false, 0);
            sender.SendClientMessage("Applying animation without Rpc!");
        }

        [Command("spectate")]
        public static void SpectateCommand(BasePlayer sender, int specPlayerId)
        {
            var specPlayer = BasePlayer.Find(specPlayerId);
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
        #endregion
        
        #region Rpc/Packet handlers
        void OnIncomingRpc(PacketRpcEventArgs e)
        {
            var bs = e.BitStreamId;
            var rpcid = e.Id;
            var playerId = e.PlayerId;

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
                    ParamType.UInt8, "mod",
                    ParamType.UInt8, "nicknameLen",
                    ParamType.String, "nickname",
                    ParamType.UInt32, "challengeResponse",
                    ParamType.UInt8, "authKeyLen",
                    ParamType.String, "authKey",
                    ParamType.UInt8, "clientVersionLen",
                    ParamType.String, "clientVersion"
                );
            }
        }

        void OnOutcomingRpc(PacketRpcEventArgs e)
        {
            var bs = e.BitStreamId;
            var rpcid = e.Id;
            var playerId = e.PlayerId;

            if (rpcid == 11)
            {
                var BS = new BitStream(bs);
                BS.ReadCompleted += (sender, args) =>
                {
                    var Id = (int)args.Result["playerId"];
                    var nicknameLen = (string)args.Result["nickname"];
                    var nickname = (int)args.Result["nicknameLen"];

                    Console.WriteLine($"Nickname changed. Id: {Id}, Nickname: {nickname}; Len: {nicknameLen}");
                };

                BS.ReadValue(ParamType.UInt16, "playerId", ParamType.UInt8, "nicknameLen", ParamType.String, "nickname");
            }

            if (rpcid == 86)
            {
                var BS = new BitStream(bs);
                BS.ReadCompleted += (sender, args) =>
                {
                    var Id = (int)args.Result["playerId"];
                    var AnimLib = (string)args.Result["AnimLib"];
                    var AnimName = (string)args.Result["AnimName"];

                    Console.WriteLine($"Animation applied. Id: {Id}; Anim Lib: {AnimLib}; Anim Name: {AnimName}");
                };
                BS.ReadValue(
                ParamType.UInt16, "playerId",
                ParamType.UInt8, "AnimLibLength",
                ParamType.String, "AnimLib",
                ParamType.UInt8, "AnimNameLength",
                ParamType.String, "AnimName",
                ParamType.Float, "fDelta",
                ParamType.Bool, "loop",
                ParamType.Bool, "lockx",
                ParamType.Bool, "locky",
                ParamType.Bool, "freeze",
                ParamType.UInt32, "dTime"
                );
            }
        }

        void OnIncomingPacket(PacketRpcEventArgs e)
        {
            var bs = e.BitStreamId;
            var packetid = e.Id;
            var playerId = e.PlayerId;

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
                        Console.WriteLine($"Reading incoming PassengerSync. VehicleId: {passenger.vehicleId}; Position: {passenger.position}; DriveBy: {passenger.driveBy};");
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

        void OnOutcomingPacket(PacketRpcEventArgs e)
        {
            var bs = e.BitStreamId;
            var packetid = e.Id;
            var playerId = e.PlayerId;

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
                        Console.WriteLine($"Reading outcoming PassengerSync. VehicleId: {passenger.vehicleId}; Position: {passenger.position}; DriveBy: {passenger.driveBy};");
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