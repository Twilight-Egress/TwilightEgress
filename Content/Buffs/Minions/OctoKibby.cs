using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Content.Items.Dedicated.Octo;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Buffs.Minions
{
    public class OctoKibby : ModBuff, ILocalizedModType
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
            player.GetModPlayer<BuffHandler>().OctoKibby = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<KibbyGirl>()] < 1)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
