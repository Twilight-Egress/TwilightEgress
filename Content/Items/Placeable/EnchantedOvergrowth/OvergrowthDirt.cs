namespace TwilightEgress.Content.Items.Placeable.EnchantedOvergrowth
{
    public class OvergrowthDirt : ModItem
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.EnchantedOvergrowth.OvergrowthDirt>());
            Item.width = 16;
            Item.height = 16;
        }
    }
}
