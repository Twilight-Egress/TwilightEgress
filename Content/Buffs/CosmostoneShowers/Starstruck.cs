using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Buffs.CosmostoneShowers
{
    public class Starstruck : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
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

        public override void PostUpdateBuffs()
        {
            if (active)
            {
                Player.tileSpeed += 0.75f;
                Player.wallSpeed += 0.75f;
                Player.blockRange += 8;
                Player.gravity = 0.1f;
            }
        }

        public override void ResetEffects() => active = false;
    }

    public class StarstruckNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool active;

        public override void PostAI(NPC npc)
        {
            if (active)
                npc.GravityMultiplier *= 0.25f;
        }

        public override void ResetEffects(NPC npc) => active = false;
    }
}
