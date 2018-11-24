// SampSharp.RakNet
// Copyright 2018 Danil Zelyutin
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class BulletSync : ISync
    {
        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
        public int HitType { get; set; }
        public int HitId { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 HitPosition { get; set; }
        public Vector3 Offsets { get; set; }
        public int WeaponId { get; set; }

        public BulletSync(BitStream bs)
        {
            BS = bs;
        }
        public void ReadIncoming()
        {
            Read(false);
        }
        public void ReadOutcoming()
        {
            Read(true);
        }
        public void WriteIncoming()
        {
            Write(false);
        }
        public void WriteOutcoming()
        {
            Write(true);
        }
        private void Read(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt8, "hitType",
                ParamType.UInt16, "hitId",
                ParamType.Float, "origin_0",
                ParamType.Float, "origin_1",
                ParamType.Float, "origin_2",
                ParamType.Float, "hitPosition_0",
                ParamType.Float, "hitPosition_1",
                ParamType.Float, "hitPosition_2",
                ParamType.Float, "offsets_0",
                ParamType.Float, "offsets_1",
                ParamType.Float, "offsets_2",
                ParamType.UInt8, "weaponId",
            };
            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, "fromPlayerId");
            }

            var result = BS.ReadValue(arguments.ToArray());

            PacketId = (int)result["packetId"];
            if (outcoming)
            {
                FromPlayerId = (int)result["fromPlayerId"];
            }

            HitType = (int)result["hitType"];
            HitId = (int)result["hitId"];
            Origin = new Vector3((float)result["origin_0"], (float)result["origin_1"], (float)result["origin_2"]);
            HitPosition = new Vector3((float)result["hitPosition_0"], (float)result["hitPosition_1"], (float)result["hitPosition_2"]);
            Offsets = new Vector3((float)result["offsets_0"], (float)result["offsets_1"], (float)result["offsets_2"]);

            WeaponId = (int)result["weaponId"];
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, PacketId,
                ParamType.UInt8, HitType,
                ParamType.UInt16, HitId,
                ParamType.Float, Origin.X,
                ParamType.Float, Origin.Y,
                ParamType.Float, Origin.Z,
                ParamType.Float, HitPosition.X,
                ParamType.Float, HitPosition.Y,
                ParamType.Float, HitPosition.Z,
                ParamType.Float, Offsets.X,
                ParamType.Float, Offsets.Y,
                ParamType.Float, Offsets.Z,
                ParamType.UInt8, WeaponId,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());
        }
    }
}
