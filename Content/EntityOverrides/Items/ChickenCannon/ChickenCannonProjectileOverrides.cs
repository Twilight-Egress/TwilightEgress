﻿using CalamityMod.Projectiles.Ranged;
using Cascade.Content.Projectiles.Ranged;

namespace Cascade.Content.EntityOverrides.Items.ChickenCannon
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

        public override void Kill(Projectile projectile, int timeLeft)
        {
            // Spawn the new explosion.
            if (projectile.type == ModContent.ProjectileType<ChickenRocket>())
                projectile.SpawnProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChickenCannonExplosion>(), projectile.damage, projectile.knockBack, true, SoundID.DD2_KoboldExplosion, projectile.owner);
        }
    }
}
