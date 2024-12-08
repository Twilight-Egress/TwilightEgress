using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightEgress.Content.Items.Placeable.EnchantedOvergrowth
{
    public class Manastone : ModItem
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.EnchantedOvergrowth.Manastone>());
            Item.width = 16;
            Item.height = 16;
        }
    }
}
