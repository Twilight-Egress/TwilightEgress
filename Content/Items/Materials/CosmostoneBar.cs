using Terraria.ID;

namespace TwilightEgress.Content.Items.Materials
{
    public class CosmostoneBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CosmostoneBar>());
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.material = true;
            Item.value = Item.sellPrice(silver: 14);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
	    {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Cosmostone>(),3)
                .AddTile(TileID.Furnaces)
                .Register();
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.LightBlue.ToVector3());    
        }
    }
}