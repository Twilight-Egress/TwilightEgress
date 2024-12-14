using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
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

        public override void Update(NPC npc, ref int buffIndex) => npc.GetGlobalNPC<BellbirdCryGlobalNPC>().BellbirdStun = true;
    }

    public class BellbirdCryModPlayer : ModPlayer
    {
        public bool BellbirdStun { get; set; }

        private int BellbirdStunTime;

        private const int BellbirdStunMaxTime = 240;

        private float BellbirdStunTimeRatio => BellbirdStunTime / (float)BellbirdStunMaxTime;

        public override void UpdateDead()
        {
            BellbirdStun = false;
            BellbirdStunTime = 0;
        }

        public override void ResetEffects()
        {
            BellbirdStun = false;
        }
    }

    public class BellbirdCryGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool BellbirdStun { get; set; }

        private int BellbirdStunTime { get; set; }

        private const int BellbirdStunMaxTime = 240;

        private float BellbirdStunTimeRatio => BellbirdStunTime / (float)BellbirdStunMaxTime;

        public override void ResetEffects(NPC npc)
        {
            BellbirdStun = false;
            if (!BellbirdStun)
                BellbirdStunTime = 0;
        }

        public override void PostAI(NPC npc)
        {
            if (BellbirdStun)
            {
                float statFuckeryInterpolant = MathHelper.Lerp(1f, 0.08f, BellbirdStunTimeRatio);
                float fallSpeedMultiplierInterpolant = MathHelper.Lerp(1f, 5f, BellbirdStunTimeRatio);

                npc.velocity.X *= 0.8f * statFuckeryInterpolant;
                npc.MaxFallSpeedMultiplier *= 1f * fallSpeedMultiplierInterpolant;
            }

            base.PostAI(npc);
        }
    }
}
