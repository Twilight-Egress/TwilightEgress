using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.BaseEntities.ModNPCs;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Planetoids
{
    public class ShatteredPlanetoid : Planetoid, ILocalizedModType
    {
        public new string LocalizationCategory => "NPCs.Misc";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

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
            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustPosition = NPC.Center + Main.rand.NextVector2CircularEdge(totalAttractionRadius, totalAttractionRadius);
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Electric, Vector2.UnitX);
                    dust.noGravity = true;
                }
            }

            NPC.ShowNameOnHover = false;
        }
    }
}
