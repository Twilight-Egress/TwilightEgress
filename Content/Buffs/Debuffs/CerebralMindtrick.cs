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

        public override void Update(Player Player, ref int buffIndex) => Player.TwilightEgress_Buffs().CerebralMindtrick = true;
    }
}
