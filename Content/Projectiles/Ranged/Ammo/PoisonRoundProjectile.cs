using CalamityMod;
using Terraria.Audio;
using Terraria.ID;

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
                TwilightEgressUtilities.CreateDustCircle(15, Projectile.Center, 18, 3f);
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
