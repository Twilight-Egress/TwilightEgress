using CalamityMod;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.FrostMoon
{
    public class HolidayHalberd : ModItem
    {
        public override string LocalizationCategory => "Items.FrostMoon";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

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
            Item.shoot = ModContent.ProjectileType<HolidayHalberdHoldout>();
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
