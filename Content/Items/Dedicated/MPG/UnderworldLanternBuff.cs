using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Items.Dedicated.MPG
{
    public class UnderworldLanternBuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";

        public override string Texture => "Terraria/Images/Buff";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffHandler>().MoonSpiritLantern = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<UnderworldLantern>()] < 1)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
