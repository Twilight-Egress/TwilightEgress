using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Rarities;
using Terraria.DataStructures;

namespace TwilightEgress.Content.Items.Dedicated.Enchilada
{
    public class MechonSlayer : ModItem, ILocalizedModType
    {
        public static int WeaponState { get; set; }

        public new string LocalizationCategory => "Items.Support";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 108;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.shoot = ModContent.ProjectileType<MechonSlayerHoldout>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Switch weapon states.
            // -1 - None
            // 0 - Armor
            // 1 - Eater
            // 2 - Enchant
            // 3 - Purge
            // 4 - Speed
            WeaponState++;
            if (WeaponState > 4)
                WeaponState = -1;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, WeaponState);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AegisBlade>())
                .AddIngredient(ItemID.Ruby)
                .AddIngredient(ItemID.AdamantiteBar, 15)
                .AddIngredient(ItemID.FragmentStardust, 15)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient(ModContent.ItemType<DivineGeode>(), 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();

        }
    }
}
