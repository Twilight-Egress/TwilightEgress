using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using Terraria.DataStructures;
using Terraria.ID;

namespace TwilightEgress.Content.Items.Dedicated.Fluffy
{
    public class TheBastOffense : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 36;
            Item.damage = 200;
            Item.knockBack = 3f;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<TheBastOffenseHoldout>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<GiantBastStatue>()] < 3;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float attackType = player.altFunctionUse == 2 ? 1 : 0;
            // Adrenaline on this weapon is simply meant to boost its attack by +50%.
            int newDamage = player.Calamity().AdrenalineEnabled ? damage + (int)(damage * 0.5f) : damage;
            Projectile.NewProjectile(source, position, velocity, type, newDamage, knockback, player.whoAmI, ai2: attackType);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CatBast, 10)
                .AddIngredient(ItemID.StormTigerStaff)
                .AddIngredient(ItemID.LicenseCat)
                .AddIngredient(ModContent.ItemType<Meowthrower>())
                .AddIngredient(ItemID.Meowmere)
                .AddIngredient(ModContent.ItemType<ThrowingBrick>())
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
