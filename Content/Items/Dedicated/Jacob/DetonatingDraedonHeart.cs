using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Dedicated.Jacob
{
    public class DetonatingDraedonHeart : ModProjectile, ILocalizedModType
    {
        private ref float Timer => ref Projectile.ai[0];

        private ref float RandomizedExplosionDelay => ref Projectile.ai[1];

        private ref float FrameSpeed => ref Projectile.ai[2];

        private const int MaxChargeTime = 75;

        private const int DetonationDelay = 30;

        private const int PulseRingInitialScaleIndex = 0;

        private const int HeartBackglowOpacityIndex = 1;

        private const int HeartBackglowSpinIndex = 2;

        private const int HeartBackglowRadiusIndex = 3;

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "CalamityMod/Items/Accessories/DraedonsHeart";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
            Projectile.timeLeft = 300;
            Projectile.scale = 0f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            RandomizedExplosionDelay = Main.rand.NextFloat(10f, 46f);
            FrameSpeed = 10f;
        }

        public override void AI()
        {
            ref float pulseRingInitialScale = ref Projectile.TwilightEgress().ExtraAI[PulseRingInitialScaleIndex];
            ref float heartBackglowOpacity = ref Projectile.TwilightEgress().ExtraAI[HeartBackglowOpacityIndex];
            ref float heartBackglowRadius = ref Projectile.TwilightEgress().ExtraAI[HeartBackglowRadiusIndex];

            if (Timer <= MaxChargeTime)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1.5f, TwilightEgressUtilities.SineEaseInOut(Timer / MaxChargeTime));
            }

            if (Timer >= MaxChargeTime && Timer <= MaxChargeTime + DetonationDelay + (int)RandomizedExplosionDelay && Timer % 5 == 0)
            {
                pulseRingInitialScale = MathHelper.Clamp(pulseRingInitialScale + 0.5f, 0.5f, 3.5f);
                PulseRingParticle detonantionRing = new(Projectile.Center, Vector2.Zero, Color.Red, pulseRingInitialScale, 0.01f, 45);
                detonantionRing.SpawnCasParticle();

                SoundEngine.PlaySound(AssetRegistry.Sounds.AsrielTargetBeep, Projectile.Center);

                for (int i = 0; i < 36; i++)
                {
                    Vector2 magicDustSpawnOffset = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 200f + Main.rand.NextVector2Circular(15f, 15f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + magicDustSpawnOffset, 267);
                    dust.color = Color.Lerp(Color.Red, Color.Crimson, Main.rand.NextFloat());
                    dust.noGravity = true;
                    dust.scale = 1f * Main.rand.NextFloat(1.1f, 1.25f);
                    dust.velocity = (Projectile.Center - dust.position) * 0.062f;
                }

                // Backglow visuals.
                heartBackglowOpacity = MathHelper.Lerp(heartBackglowOpacity, 1f, TwilightEgressUtilities.SineEaseInOut(Timer / 30 + RandomizedExplosionDelay));
                heartBackglowRadius = MathHelper.Lerp(0f, 5f, TwilightEgressUtilities.SineEaseInOut(Timer / 30 + RandomizedExplosionDelay));

                // Decrease the frame speed to make the animation appear faster.
                FrameSpeed = MathHelper.Clamp(FrameSpeed - 1f, 1f, 10f);
            }

            if (Timer >= MaxChargeTime + DetonationDelay + (int)RandomizedExplosionDelay)
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.X * 0.03f;
            Projectile.AdjustProjectileHitboxByScale(42f, 40f);

            // Animation.
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= FrameSpeed)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }

            // In the chance that there is an enemy near, move REALLY slowly towards them.
            Projectile.GetNearestTarget(1000f, 500f, out _, out NPC target);
            if (target != null && Timer >= MaxChargeTime)
                Projectile.SimpleMove(target.Center, 10f, 200f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound, Projectile.Center);
            PulseRingParticle explosionRing = new(Projectile.Center, Vector2.Zero, Color.Red, 0.01f, 10f, 75);
            explosionRing.SpawnCasParticle();

            // K  A  B  O  O  M two, electric boogaloo.
            for (int i = 0; i < 12; i++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<Tanksplosion>(), Projectile.damage, Projectile.knockBack);
            }

            int sparkLifespan = Main.rand.Next(20, 36);
            float sparkScale = Main.rand.NextFloat(1.25f, 2.25f);
            Color sparkColor = Color.Lerp(Color.Red, Color.Goldenrod, Main.rand.NextFloat());
            for (int i = 0; i < 25; i++)
            {
                Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 16f);
                SparkParticle deathSparks = new(Projectile.Center, sparkVelocity, sparkColor, sparkScale, sparkLifespan);
                deathSparks.SpawnCasParticle();
            }

            ScreenShakeSystem.StartShake(10f, shakeStrengthDissipationIncrement: 0.185f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float heartBackglowOpacity = ref Projectile.TwilightEgress().ExtraAI[HeartBackglowOpacityIndex];
            ref float heartBackglowRadius = ref Projectile.TwilightEgress().ExtraAI[HeartBackglowRadiusIndex];
            ref float heartBackglowSpin = ref Projectile.TwilightEgress().ExtraAI[HeartBackglowSpinIndex];

            Texture2D heartGlow = ModContent.Request<Texture2D>("TwilightEgress/Content/Items/Dedicated/Jacob/DetonatingDraedonHeartGlow").Value;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 8; i++)
            {
                heartBackglowSpin += MathHelper.TwoPi / 240f;
                Vector2 heartBackglowDrawPosition = Projectile.Center + Vector2.UnitY.RotatedBy(heartBackglowSpin + MathHelper.TwoPi * i / 8f) * heartBackglowRadius + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                Color color = Color.Red;
                Main.EntitySpriteDraw(heartGlow, heartBackglowDrawPosition, null, color * heartBackglowOpacity, Projectile.rotation, heartGlow.Size() / 2f, Projectile.scale * 1.085f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ResetToDefault();

            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }
    }
}
