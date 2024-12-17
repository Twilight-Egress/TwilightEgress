using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Dedicated.Jacob
{
    public class TomeOfTheTank : ModItem
    {
        public override string LocalizationCategory => "Items.Dedicated";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 64;
            Item.damage = 2000;
            Item.crit = 15;
            Item.knockBack = 3f;
            Item.useTime = 180;
            Item.useAnimation = 180;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<TomeOfTheTankHoldout>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<RampartofDeities>())
                .AddIngredient(ModContent.ItemType<DraedonsHeart>())
                .AddIngredient(ItemID.ExplosivePowder, 10)
                .AddIngredient(ModContent.ItemType<ExoPrism>(), 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
