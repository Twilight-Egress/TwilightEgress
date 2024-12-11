using Terraria.DataStructures;
using TwilightEgress.Core.EntityOverridingSystem;

namespace TwilightEgress.Content.EntityOverrides.Items.ChickenCannon
{
    public class ChickenCannonOverride : ItemOverride
    {
        public override int TypeToOverride => ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.ChickenCannon>();

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.One, ModContent.ProjectileType<ChickenCannonHoldout>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
