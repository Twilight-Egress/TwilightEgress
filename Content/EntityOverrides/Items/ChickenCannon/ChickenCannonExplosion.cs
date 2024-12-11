using CalamityMod;
using Luminance.Core.Graphics;
using Terraria.DataStructures;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;

namespace TwilightEgress.Content.EntityOverrides.Items.ChickenCannon
{
    public class ChickenCannonExplosion : ModProjectile, ILocalizedModType
    {
        private ref float Timer => ref Projectile.ai[0];

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => AssetRegistry.ExtraTexturesPath + "EmptyPixel";

        public override void SetDefaults()
        {
            Projectile.width = 1000;
            Projectile.height = 1000;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = (int)22.5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Send out a huge spread of particles.
            PulseRingParticle explosionRing = new(Projectile.Center, Vector2.Zero, Color.White, 0.01f, 8f, 75);
            explosionRing.SpawnCasParticle();

            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(80f, 100f);
                Color initialColor = Color.Lerp(Color.WhiteSmoke, Color.DarkGray, Main.rand.NextFloat()) * Main.rand.NextFloat(0.2f, 0.5f);
                Color fadeColor = Color.DarkGray;
                float scale = Main.rand.NextFloat(10f, 20f);
                float opacity = Main.rand.NextFloat(0.6f, 1f);
                MediumMistParticle deathSmoke = new(Projectile.Center, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(180, 240));
                deathSmoke.SpawnCasParticle();
            }

            for (int i = 0; i < 50; i++)
            {
                Color fireColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0.2f, 0.8f)) * Main.rand.NextFloat(0.2f, 0.5f);
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(35f, 100f);
                float scale = Main.rand.NextFloat(4f, 6f);
                HeavySmokeParticle flames = new(Projectile.Center, velocity, fireColor, Main.rand.Next(120, 150), scale, Main.rand.NextFloat(0.7f, 1.75f), 0.06f, true, 0);
                flames.SpawnCasParticle();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 center = Projectile.Center;
            Vector2 size = Projectile.Size;
            return CalamityUtils.CircularHitboxCollision(center, size.Length() / 2f, targetHitbox);
        }

        public override void AI()
        {
            // Spawn a bunch of smaller explosions.
            if (Timer % 2 == 0)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<ChickenCannonMiniBoom>(), (int)(Projectile.damage * 0.65f), Projectile.knockBack, owner: Projectile.owner);
            }

            Timer++;
            ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 15f, shakeStrengthDissipationIncrement: 0.185f);
        }
    }
}
