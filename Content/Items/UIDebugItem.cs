using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items
{
    public class UIDebugItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override string Texture => "CalamityMod/Items/Weapons/Magic/LightGodsBrilliance";
        public override void SetDefaults()
        {
            Item.width = 108;
            Item.height = 108;
            Item.noMelee = true;
            Item.useAnimation = 1;
            Item.useTime = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
        public override bool? UseItem(Player player)
        {
            return true;
        }
    }
}
