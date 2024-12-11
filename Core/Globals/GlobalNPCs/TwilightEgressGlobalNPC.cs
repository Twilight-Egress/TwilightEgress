
namespace TwilightEgress.Core.Globals.GlobalNPCs
{
    public partial class TwilightEgressGlobalNPC : GlobalNPC
    {
        public float[] ExtraAI = new float[100];

        internal bool[] IsCustomAISlotBeingUsed = new bool[100];

        internal int TotalCustomAISlotsInUse => IsCustomAISlotBeingUsed.Count(slot => slot);

        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC npc)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
                ExtraAI[i] = 0f;
        }

        public override void PostAI(NPC npc)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
            {
                if (ExtraAI[i] != 0f)
                    IsCustomAISlotBeingUsed[i] = true;
            }

            base.PostAI(npc);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.type == NPCID.IceQueen)
                drawColor = Color.White;
        }
    }

    public static class GlobalNPCExtensionMethod
    {
        public static TwilightEgressGlobalNPC TwilightEgress(this NPC npc) => npc.GetGlobalNPC<TwilightEgressGlobalNPC>();
    }
}
