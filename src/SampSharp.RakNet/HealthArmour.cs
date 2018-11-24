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

namespace SampSharp.RakNet
{
    public class HealthArmour
    {
        public static void GetFromByte(byte healthArmour, out int health, out int armour)
        {
            byte byteHealth, byteArmour;
            byte byteArmTemp = 0, byteHlTemp = 0;

            byteArmTemp = (byte)(healthArmour & 0x0F);
            byteHlTemp = (byte)(healthArmour >> 4);

            if (byteArmTemp == 0xF) byteArmour = 100;
            else if (byteArmTemp == 0) byteArmour = 0;
            else byteArmour = (byte)(byteArmTemp * 7);

            if (byteHlTemp == 0xF) byteHealth = 100;
            else if (byteHlTemp == 0) byteHealth = 0;
            else byteHealth = (byte)(byteHlTemp * 7);

            health = byteHealth;
            armour = byteArmour;
        }
        public static byte SetInByte(int health, int armour)
        {
            byte healthArmour = 0;
            byte byteHealth = Convert.ToByte(health), byteArmour = Convert.ToByte(armour);
            if (byteHealth > 0 && byteHealth < 100)
            {
                healthArmour = (byte)(((byte)(byteHealth / 7)) << 4);
            }
            else if (byteHealth >= 100)
            {
                healthArmour = 0xF << 4;
            }

            if (byteArmour > 0 && byteArmour < 100)
            {
                healthArmour |= (byte)(byteArmour / 7);
            }
            else if (byteArmour >= 100)
            {
                healthArmour |= 0xF;
            }

            return healthArmour;
        }
    }
}
