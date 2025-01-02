using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Events;

namespace TwilightEgress.Content.Buffs.CosmostoneShowers
{
    public class Starstruck : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<StarstruckPlayer>().active = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<StarstruckNPC>().active = true;
        }
    }

    public class StarstruckPlayer : ModPlayer
    {
        public bool active;

        public override void PreUpdateBuffs()
        {
            if (EventHandlerManager.SpecificEventIsActive<Events.CosmostoneShowers.CosmostoneShowers>())
                Player.AddBuff(ModContent.BuffType<Starstruck>(), 10);
        }

        public override void PostUpdateBuffs()
        {
            if (active)
            {
                Player.tileSpeed += 0.75f;
                Player.wallSpeed += 0.75f;
                Player.blockRange += 8;
                Player.gravity = 0.2f;
            }
        }

        public override void ResetEffects() => active = false;
    }

    public class StarstruckNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool active;

        public override bool PreAI(NPC npc)
        {
            if (EventHandlerManager.SpecificEventIsActive<Events.CosmostoneShowers.CosmostoneShowers>())
                npc.AddBuff(ModContent.BuffType<Starstruck>(), 10);

            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (active)
                npc.GravityMultiplier *= 0.5f;
        }

        public override void ResetEffects(NPC npc) => active = false;
    }
}
