using System.IO;
using Terraria.ModLoader.IO;

namespace TwilightEgress.Core.Globals.GlobalProjectiles
{
    public partial class TwilightEgressGlobalProjectile
    {
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < TotalCustomAISlotsInUse; i++)
                binaryWriter.Write(ExtraAI[i]);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < TotalCustomAISlotsInUse; i++)
                ExtraAI[i] = binaryReader.ReadSingle();
        }
    }
}
