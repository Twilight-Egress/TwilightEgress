using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace TwilightEgress.Core.Globals.GlobalNPCs
{
    public partial class TwilightEgressGlobalNPC
    {
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            for (int i = 0; i < TotalCustomAISlotsInUse; i++)
                binaryWriter.Write(ExtraAI[i]);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            for (int i = 0; i < TotalCustomAISlotsInUse; i++)
                ExtraAI[i] = binaryReader.ReadSingle();
        }
    }
}
