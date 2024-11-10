namespace TwilightEgress.Content.Buffs.Debuffs
{
    public class CurseOfNecromancy : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs.Debuffs";

        public override string Texture => "Terraria/Images/Buff";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player Player, ref int buffIndex)
        {
            Player.TwilightEgress_Buffs().CurseOfNecromancy = true;
            if (Main.CurrentFrameFlags.AnyActiveBossNPC)
                Player.buffTime[buffIndex] = 18000;

            if (!Main.CurrentFrameFlags.AnyActiveBossNPC && Player.buffTime[buffIndex] > 3600)
            {
                Player.DelBuff(buffIndex);
                buffIndex--;
            }
             
        }
    }
}
