using TwilightEgress.Content.Items.Dedicated.MPG;

namespace TwilightEgress.Core.Globals.GlobalItems
{
    public class MSKUseFix : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.DamageType == DamageClass.Summon && player.HeldItem.type != ModContent.ItemType<MoonSpiritKhakkhara>())
                return player.ownedProjectileCounts[ModContent.ProjectileType<UnderworldLantern>()] < 1;
            return base.CanUseItem(item, player);
        }
    }
}
