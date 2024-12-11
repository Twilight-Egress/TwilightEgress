using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria.DataStructures;
using Terraria.ID;

namespace TwilightEgress.Content.Items.Dedicated.Marv
{
    public class ThunderousFury : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Magic;
            Item.width = 124;
            Item.height = 52;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.channel = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<ThunderousFuryHoldout>();
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) => true;

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float attackType = player.altFunctionUse == 2 ? 1 : 0;
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ThunderousFuryHoldout>(), damage, knockback, player.whoAmI, ai2: attackType);
            return false;
        }
    }
}
