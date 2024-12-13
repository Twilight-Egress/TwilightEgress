using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Items.CosmostoneShowers;

namespace TwilightEgress.Content.Items.EnchantedOvergrowth
{
    public class SilicateClusterBrick : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TwilightEgress.Content.Tiles.CosmostoneShowers.SilicateClusterBrick>());
            Item.width = 12;
            Item.height = 12;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SilicateCluster>(2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
