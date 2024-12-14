using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Dedicated.Lynel
{
    public class BellbirdBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff";

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<EarPiercingBellbird>());
        }
    }
}
