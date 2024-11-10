using TwilightEgress.Content.Items.Dedicated.Lynel;

namespace TwilightEgress.Content.Buffs.Pets
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
