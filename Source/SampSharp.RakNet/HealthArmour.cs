using System;

namespace SampSharp.RakNet
{
    public class HealthArmour
    {
        public static void GetFromByte(byte healthArmour, ref int health, ref int armour)
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
