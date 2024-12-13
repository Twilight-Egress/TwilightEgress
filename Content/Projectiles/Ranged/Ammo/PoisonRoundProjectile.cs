using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Projectiles.Ranged.Ammo
{
    public class PoisonRoundProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 2;
            Projectile.Calamity().pointBlankShotDuration = 18;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = hit.Crit ? 1 : 4;
            if (Main.rand.NextBool(chance))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustRotation = Vector2.Normalize(Vector2.UnitY).RotatedBy((i - (15 / 2 - 1) * MathHelper.TwoPi / 15)) + Projectile.Center;
                    Vector2 dustVelocity = dustRotation - Projectile.Center;
                    Dust dust = Dust.NewDustPerfect(dustRotation + dustVelocity, 18, Vector2.Normalize(dustVelocity) * 3f);
                    dust.noGravity = true;
                }
                target.AddBuff(BuffID.Poisoned, 120);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
}
