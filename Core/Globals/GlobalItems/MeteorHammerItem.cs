using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Players;

namespace TwilightEgress.Core.Globals.GlobalItems
{
    public class MeteorHammerItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.WoodenHammer;

        public override bool AltFunctionUse(Item item, Player player) => true;

        public override bool? UseItem(Item item, Player player)
        {
            NPC closest = null;
            float distanceToNPC = 9999999f;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.ModNPC is not Meteoroid || Vector2.DistanceSquared(Main.MouseWorld, npc.Center) >= distanceToNPC)
                    continue;

                distanceToNPC = Vector2.DistanceSquared(Main.MouseWorld, npc.Center);
                closest = npc;
            }

            // Make it shoot grenades for no reason
            if (player.altFunctionUse == 2 && closest is not null && player.GetModPlayer<MeteorHammerPlayer>().MeteorToHammer == -1)
            {
                bool canHit = Collision.CanHit(player.Center, 1, 1, closest.Center, 1, 1);

                if (!canHit)
                    return null;

                closest.GetGlobalNPC<MeteorHammerNPC>().IsNabbedForHammering = true;
                closest.GetGlobalNPC<MeteorHammerNPC>().PlayerHammered = player.whoAmI;

                return true;
            }
            else if (closest is not null && closest.GetGlobalNPC<MeteorHammerNPC>().IsNabbedForHammering)
            {
                closest.GetGlobalNPC<MeteorHammerNPC>().IsNabbedForHammering = false;
                Vector2 velocity = Main.MouseWorld - player.Center;
                velocity.Normalize();
                closest.velocity = velocity * 2 * 16;
            }

            return null;
        }
    }
}
