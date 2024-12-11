using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using TwilightEgress.Content.Items.Materials;
using Mono.Cecil;
using Terraria;
using Terraria.ID;
using System.Linq;

namespace TwilightEgress.Content.Items.Weapons.Ranged.SailorsSingularity
{
    public class SailorsSingularity : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 24;
            Item.damage = 20;
            Item.channel = true;
            Item.knockBack = 0.425f;
            Item.useTime = Item.useAnimation = 20;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SailorsSingularityHoldout>();
            Item.shootSpeed = 11f;
        }
        public override bool CanUseItem(Player player)
        {
            return !(player.altFunctionUse == 2 && Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<SailorVortex>()));
        }
        public override bool CanShoot(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => true;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CosmostoneBar>(), 8)
                .AddIngredient(ItemID.StarCannon)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool AltFunctionUse(Player player) => true;
    }
}
