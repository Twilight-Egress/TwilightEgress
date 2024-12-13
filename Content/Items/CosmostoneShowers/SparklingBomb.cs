using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.CosmostoneShowers
{
    public class SparklingBomb : ModItem
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileID.Bomb;
            Item.width = 22;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Stargel>()
                .AddIngredient(ItemID.Bomb)
                .Register();
        }
    }
}
