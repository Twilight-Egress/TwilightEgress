using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Projectiles.Ranged.Ammo;

namespace TwilightEgress.Content.Items.Ammo
{
    public class PoisonRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 14;
            Item.damage = 8;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<PoisonRoundProjectile>();
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(70)
                .AddIngredient(ItemID.MusketBall, 70)
                .AddIngredient(ItemID.Stinger, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
