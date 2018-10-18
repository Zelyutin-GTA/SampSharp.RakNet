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
            var BS = BitStream.Create();

            BS.WriteValue(ParamType.UInt16, sender.Id, ParamType.UInt8, name.Length, ParamType.String, name);

            BS.SendRpc(11, sender.Id);
            sender.SendClientMessage("Changing name with Rpc!");
        }
        [Command("setpos")]
        public static void SetPosCommand(BasePlayer sender, float x, float y, float z)
        {
            var BS = BitStream.Create();

            BS.WriteValue(ParamType.Float, x, ParamType.Float, y, ParamType.Float, z);

            int setPosRpc = 12;
            BS.SendRpc(setPosRpc, sender.Id);
            sender.SendClientMessage("Setting position with Rpc!");
        }

        [Command("explode")]
        public static void ExplodeCommand(BasePlayer sender)
        {
            var bs = BitStream.Create();
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
            var bs = BitStream.Create();
            bs.WriteValue(ParamType.UInt16, playerId, ParamType.Int32, 0, ParamType.UInt8, 0, ParamType.UInt8, name.Length, ParamType.String, name);

            int serverJoin = 137;
            bs.SendRpc(serverJoin, sender.Id);

            
            bs = BitStream.Create();

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
                
                var result = BS.ReadValue(ParamType.Float, "x", ParamType.Float, "y", ParamType.Float, "z");
                float x = (float)result["x"];
                float y = (float)result["y"];
                float z = (float)result["z"];
                Console.WriteLine($"Map Clicked X: {x}; Y: {y}; Z: {z};");
            }
            if (rpcid == 25)
            {
                var BS = new BitStream(bs);
                var result = BS.ReadValue(
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
                var nickname = (string)result["nickname"];
                var clientVersion = (string)result["clientVersion"];
                var authKey = (string)result["authKey"];

                Console.WriteLine($"Client joined. Version: {clientVersion}; Auth Key: {authKey}; Nickname: {nickname};");
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
                var result = BS.ReadValue(ParamType.UInt16, "playerId", ParamType.UInt8, "nicknameLen", ParamType.String, "nickname");

                var Id = (int)result["playerId"];
                var nicknameLen = (string)result["nickname"];
                var nickname = (int)result["nicknameLen"];

                Console.WriteLine($"Nickname changed. Id: {Id}, Nickname: {nickname}; Len: {nicknameLen}");
            }

            if (rpcid == 86)
            {
                var BS = new BitStream(bs);
                var result = BS.ReadValue(
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

                var Id = (int)result["playerId"];
                var AnimLib = (string)result["AnimLib"];
                var AnimName = (string)result["AnimName"];

                Console.WriteLine($"Animation applied. Id: {Id}; Anim Lib: {AnimLib}; Anim Name: {AnimName}");
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

                    onFoot.ReadIncoming();
                    Console.WriteLine($"Reading incoming OnFootSync. Position: {onFoot.Position}; Health: {onFoot.Health}; Armour: {onFoot.Armour}; Velocity: {onFoot.Velocity};");

                    break;
                }
                case (int)PacketIdentifiers.DriverSync:
                {
                    var driver = new DriverSync(BS);

                    driver.ReadIncoming();
                    Console.WriteLine($"Reading incoming DriverSync. Position: {driver.Position};");

                    break;
                }
                case (int)PacketIdentifiers.AimSync:
                {
                    var aim = new AimSync(BS);

                    aim.ReadIncoming();
                    Console.WriteLine($"Reading incoming AimSync. Camera looks at: {aim.CameraFrontVector};");

                    break;
                }
                case (int)PacketIdentifiers.BulletSync:
                {
                    var bullet = new BulletSync(BS);
                    bullet.ReadIncoming();

                    Console.WriteLine($"Reading incoming BulletSync. Bullet hit: {bullet.HitPosition};");
                    BS.ResetWritePointer();
                    bullet.Origin = new Vector3(bullet.Origin.X, bullet.Origin.Y, bullet.Origin.Z + 3);
                    bullet.Offsets = new Vector3(bullet.Offsets.X, bullet.Offsets.Y, bullet.Offsets.Z + 3);
                    bullet.HitPosition = new Vector3(bullet.HitPosition.X, bullet.HitPosition.Y, bullet.HitPosition.Z + 3);
                    bullet.WriteIncoming();

                    break;
                }
                case (int)PacketIdentifiers.PassengerSync:
                {
                    var passenger = new PassengerSync(BS);

                    passenger.ReadIncoming();
                    Console.WriteLine($"Reading incoming PassengerSync. VehicleId: {passenger.VehicleId}; Position: {passenger.Position}; DriveBy: {passenger.DriveBy};");

                    break;
                }
                case (int)PacketIdentifiers.SpectatorSync:
                {
                    var spectator = new SpectatorSync(BS);

                    spectator.ReadIncoming();
                    Console.WriteLine($"Reading incoming SpectatorSync. Position: {spectator.position};");

                    break;
                }
                case (int)PacketIdentifiers.TrailerSync:
                {
                    var trailer = new TrailerSync(BS);

                    trailer.ReadIncoming();
                    Console.WriteLine($"Reading incoming TrailerSync. Position: {trailer.Position};");

                    break;
                }
                case (int)PacketIdentifiers.UnoccupiedSync:
                {
                    var unoccupied = new UnoccupiedSync(BS);

                    unoccupied.ReadIncoming();
                    Console.WriteLine($"Reading incoming UnoccupiedSync. Position: {unoccupied.Position}; Health: {unoccupied.VehicleHealth};");

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

                    onFoot.ReadOutcoming();
                    Console.WriteLine($"Reading outcoming OnFootSync. Position: {onFoot.Position}; Health: {onFoot.Health}; Armour: {onFoot.Armour}; Velocity: {onFoot.Velocity};");

                    BS.ResetWritePointer();
                    onFoot.Position = new Vector3(onFoot.Position.X, onFoot.Position.Y, onFoot.Position.Z + 10);
                    onFoot.WriteOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.DriverSync:
                {
                    var driver = new DriverSync(BS);

                    driver.ReadOutcoming();
                    Console.WriteLine($"Reading outcoming DriverSync. Position: {driver.Position};");

                    BS.ResetWritePointer();
                    driver.Position = new Vector3(driver.Position.X, driver.Position.Y, driver.Position.Z + 10);
                    driver.WriteOutcoming();

                    break;
                }
                case (int)PacketIdentifiers.AimSync:
                {
                    var aim = new AimSync(BS);

                    aim.ReadOutcoming();
                    Console.WriteLine($"Reading outcoming AimSync. Camera looks at: {aim.CameraFrontVector};");

                    break;
                }
                case (int)PacketIdentifiers.BulletSync:
                {
                    var bullet = new BulletSync(BS);

                    bullet.ReadOutcoming();
                    Console.WriteLine($"Reading outcoming BulletSync. Bullet hit: {bullet.HitPosition};");

                    break;
                }
                case (int)PacketIdentifiers.PassengerSync:
                {
                    var passenger = new PassengerSync(BS);

                    passenger.ReadOutcoming();
                    Console.WriteLine($"Reading outcoming PassengerSync. VehicleId: {passenger.VehicleId}; Position: {passenger.Position}; DriveBy: {passenger.DriveBy};");

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

                    trailer.ReadOutcoming();
                    Console.WriteLine($"Reading outcoming TrailerSync. Position: {trailer.Position};");

                    break;
                }
                case (int)PacketIdentifiers.UnoccupiedSync:
                {
                    var unoccupied = new UnoccupiedSync(BS);

                    unoccupied.ReadOutcoming();
                    Console.WriteLine($"Reading outcoming UnoccupiedSync. Position: {unoccupied.Position}; Health: {unoccupied.VehicleHealth};");

                    break;
                }
            }
        }
        #endregion
    }
}