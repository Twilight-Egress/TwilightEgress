namespace TwilightEgress.Core.Globals
{
    public partial class TwilightEgressGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public float[] ExtraAI = new float[100];

        internal bool[] IsCustomAISlotBeingUsed = new bool[100];

        internal int TotalCustomAISlotsInUse => IsCustomAISlotBeingUsed.Count(slot => slot);

        public int? SpecificNPCTypeToCheckOnHit = null;

        public bool HasStruckSpecificNPC = false;

        public override void SetDefaults(Projectile projectile)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
                ExtraAI[i] = 0f;

            SpecificNPCTypeToCheckOnHit = null;
            HasStruckSpecificNPC = false;

            base.SetDefaults(projectile);
        }

        public override void PostAI(Projectile projectile)
        {
            for (int i = 0; i < ExtraAI.Length; i++)
            {
                if (ExtraAI[i] != 0f)
                    IsCustomAISlotBeingUsed[i] = true;
            }

            // Reset every frame as to ensure this bool doesn't linger as active.
            HasStruckSpecificNPC = false;

            base.PostAI(projectile);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Checks for a specific NPC type that is set in the below field.
            if (SpecificNPCTypeToCheckOnHit.HasValue)
            {
                // If the target is alive and the types match up, then we activate a bool
                // that tells us that the projectile has struck the specific npc type 
                // that it was given.
                if (target.active && target.type == SpecificNPCTypeToCheckOnHit.Value)
                    HasStruckSpecificNPC = true;
            }

            base.OnHitNPC(projectile, target, hit, damageDone);
        }
    }
}
