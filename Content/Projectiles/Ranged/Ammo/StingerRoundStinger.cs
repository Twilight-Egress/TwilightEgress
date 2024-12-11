using Terraria.Audio;
using Terraria.ID;

namespace TwilightEgress.Content.Projectiles.Ranged.Ammo
{
    public class StingerRoundStinger : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => "Terraria/Images/Projectile_55";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 1;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
            TwilightEgressUtilities.CreateDustLoop(1, spawnPosition, Vector2.Zero, 18);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = hit.Crit ? 1 : 12;
            if (Main.rand.NextBool(chance))
            {
                TwilightEgressUtilities.CreateDustCircle(15, Projectile.Center, 18, 6f);
                target.AddBuff(BuffID.Poisoned, 360);
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
