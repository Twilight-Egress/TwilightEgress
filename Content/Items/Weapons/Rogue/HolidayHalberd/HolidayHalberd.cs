using CalamityMod.Items;

namespace TwilightEgress.Content.Items.Weapons.Rogue.HolidayHalberd
{
    public class HolidayHalberd : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Rogue";

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 72;
            Item.damage = 150;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.knockBack = 4f;
            Item.crit = 4;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<HolidayHalbertHoldout>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
