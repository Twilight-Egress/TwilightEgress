using Terraria.ID;
using TwilightEgress.Core.Players.BuffHandlers;

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

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffHandler>().CurseOfNecromancy = true;
            if (Main.CurrentFrameFlags.AnyActiveBossNPC)
                player.buffTime[buffIndex] = 18000;

            if (!Main.CurrentFrameFlags.AnyActiveBossNPC && player.buffTime[buffIndex] > 3600)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
             
        }
    }
}
