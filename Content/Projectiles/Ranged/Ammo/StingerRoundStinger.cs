using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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

            Dust dust = Dust.NewDustPerfect(spawnPosition, 18, Vector2.Zero);
            dust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = hit.Crit ? 1 : 12;
            if (Main.rand.NextBool(chance))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustRotation = Vector2.Normalize(Vector2.UnitY).RotatedBy((i - (15 / 2 - 1) * MathHelper.TwoPi / 15)) + Projectile.Center;
                    Vector2 dustVelocity = dustRotation - Projectile.Center;
                    Dust dust = Dust.NewDustPerfect(dustRotation + dustVelocity, 18, Vector2.Normalize(dustVelocity) * 6f);
                    dust.noGravity = true;
                }
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
