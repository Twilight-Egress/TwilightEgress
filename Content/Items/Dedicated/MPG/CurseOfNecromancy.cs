using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Items.Dedicated.MPG
{
    public class CurseOfNecromancy : ModBuff
    {
        public override string LocalizationCategory => "Items.Dedicated.MoonSpiritKhakkhara.Buffs";

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
