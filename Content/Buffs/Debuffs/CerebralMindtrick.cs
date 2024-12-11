using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Buffs.Debuffs
{
    public class CerebralMindtrick : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs.Debuffs";

        public override string Texture => "Terraria/Images/Buff_321";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cerebral Mindtrick");
            // Description.SetDefault("Your head is spinning. You're beginning to see illusions!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<BuffHandler>().CerebralMindtrick = true;
    }
}
