using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Items.Dedicated.Lynel
{
    public class BellbirdCry : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs.Debuffs";

        public override string Texture => "Terraria/Images/Buff_160";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BuffHandler>().BellbirdStun = true;

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<DebuffHandler>().BellbirdStun = true;
    }
}
