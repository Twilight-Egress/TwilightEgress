namespace TwilightEgress.Core.Globals.GlobalNPCs
{
    public class DebuffHandler : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool BellbirdStun { get; set; }

        private int BellbirdStunTime { get; set; }

        private const int BellbirdStunMaxTime = 240;

        private float BellbirdStunTimeRatio => BellbirdStunTime / (float)BellbirdStunMaxTime;

        public override void ResetEffects(NPC npc)
        {
            BellbirdStun = false;
            if (!BellbirdStun)
                BellbirdStunTime = 0;
        }

        public override void PostAI(NPC npc)
        {
            if (BellbirdStun)
            {
                float statFuckeryInterpolant = MathHelper.Lerp(1f, 0.08f, BellbirdStunTimeRatio);
                float fallSpeedMultiplierInterpolant = MathHelper.Lerp(1f, 5f, BellbirdStunTimeRatio);

                npc.velocity.X *= 0.8f * statFuckeryInterpolant;
                npc.MaxFallSpeedMultiplier *= 1f * fallSpeedMultiplierInterpolant;
            }

            base.PostAI(npc);
        }
    }
}
