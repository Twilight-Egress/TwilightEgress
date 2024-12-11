using System;
using TwilightEgress.Core.BaseEntities.ModNPCs;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Planetoids
{
    public class ShatteredPlanetoid : BasePlanetoid, ILocalizedModType
    {
        public new string LocalizationCategory => "NPCs.Misc";

        public override float MaximumAttractionRadius => 176f;

        public override float WalkableRadius => 120f;

        public override void SafeSetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.lifeMax = 2;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.dontTakeDamage = true;
        }

        public override void SafeAI()
        {
            float totalAttractionRadius = MaximumAttractionRadius + WalkableRadius;
            Vector2 dustPosition = NPC.Center + Main.rand.NextVector2CircularEdge(totalAttractionRadius, totalAttractionRadius);
            if (Main.rand.NextBool(2))
                TwilightEgressUtilities.CreateDustLoop(15, dustPosition, Vector2.UnitX, DustID.Electric);

            NPC.rotation += MathF.Tau / 600f;
            NPC.ShowNameOnHover = false;
        }
    }
}
