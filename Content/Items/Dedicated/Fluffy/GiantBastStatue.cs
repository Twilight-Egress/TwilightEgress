using CalamityMod;
using Luminance.Common.Utilities;
using System;

namespace TwilightEgress.Content.Items.Dedicated.Fluffy
{
    public class GiantBastStatue : ModProjectile, ILocalizedModType
    {
        public ref float Timer => ref Projectile.ai[0];

        public ref float BounceLimit => ref Projectile.ai[1];

        public ref float KibbyCount => ref Projectile.ai[2];

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => "TwilightEgress/Content/Items/Dedicated/Fluffy/HomingBastStatue";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CanHitPastShimmer[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 10;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale = 3f;
                Projectile.localAI[0] = 1f;
                KibbyCount = 6;
                BounceLimit = 10;
            }

            if (Timer % 15 == 0)
            {

            }

            if (Projectile.velocity.Y < 16f)
                Projectile.velocity.Y += 0.3f;

            // Speed up over time.
            Projectile.velocity *= 1.002f;
            Projectile.velocity.ClampMagnitude(10f, 15f);

            // Adjust the hitbox.
            Projectile.AdjustProjectileHitboxByScale(32, 32);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }

        public override void OnKill(int timeLeft)
        {
            // K  A  B  O  O  M
            for (int i = 0; i < 12; i++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<Bastsplosion>(), Projectile.damage, Projectile.knockBack);
            }

            // kibby
            for (int i = 0; i < KibbyCount; i++)
            {
                Vector2 kibbyVelocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(13f, 19f);
                Projectile.BetterNewProjectile(Projectile.Center, kibbyVelocity, ModContent.ProjectileType<HomingBastStatue>(), Projectile.damage, Projectile.knockBack);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Land a critical hit if the projectile is on it's last bounce.
            if (BounceLimit == 1)
                hit.Crit = true;

            // Increase the amount of Bast Statues that are fired when this thing explodes.
            // If it crits increase it even more.
            KibbyCount += 2;
            if (hit.Crit)
                KibbyCount += 4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            BounceLimit--;
            KibbyCount += 4;
            if (BounceLimit <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundStyle meow = Utils.SelectRandom(Main.rand, SoundID.Item57, SoundID.Item58);
                SoundEngine.PlaySound(meow with { MaxInstances = 0 }, Projectile.position);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);

            Projectile.DrawBackglow(Projectile.GetAlpha(Color.Gold * 0.45f), 2f);
            Utilities.DrawAfterimagesCentered(Projectile, 0, Projectile.GetAlpha(Color.Gold));

            Main.spriteBatch.ResetToDefault();

            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(lightColor), Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }
    }
}
