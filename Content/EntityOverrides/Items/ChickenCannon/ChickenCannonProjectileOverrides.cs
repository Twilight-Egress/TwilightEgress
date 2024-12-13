using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core;

namespace TwilightEgress.Content.EntityOverrides.Items.ChickenCannon
{
    public class ChickenCannonProjectileOverrides : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => lateInstantiation && (entity.type == ModContent.ProjectileType<ChickenExplosion>() || entity.type == ModContent.ProjectileType<ChickenRocket>());

        public override bool PreAI(Projectile projectile)
        {
            // Kill any old existing explosions.
            if (projectile.type == ModContent.ProjectileType<ChickenExplosion>())
            {
                projectile.Kill();
                return false;
            }

            return base.PreAI(projectile);
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            // Spawn the new explosion.
            if (projectile.type == ModContent.ProjectileType<ChickenRocket>())
                projectile.BetterNewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChickenCannonExplosion>(), projectile.damage, projectile.knockBack, SoundID.DD2_KoboldExplosion, null, projectile.owner);
        }
    }
}
