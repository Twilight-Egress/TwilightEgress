using TwilightEgress.Content.Projectiles.Ranged.Ammo;

namespace TwilightEgress.Content.Items.Ammo
{
    public class PoisonArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 32;
            Item.damage = 11;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<PoisonArrowProjectile>();
            Item.shootSpeed = 5f;
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ItemID.WoodenArrow, 50)
                .AddIngredient(ItemID.Stinger, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
