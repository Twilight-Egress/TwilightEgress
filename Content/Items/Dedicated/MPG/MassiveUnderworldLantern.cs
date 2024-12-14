using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;

namespace TwilightEgress.Content.Items.Dedicated.MPG
{
    public class MassiveUnderworldLantern : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private ref float HitCounter => ref Projectile.ai[2];

        private const int TimeBeforeCharging = 30;

        private const int MaxChargingTime = 300;

        private const int FadeoutTime = 60;

        public override string LocalizationCategory => "Items.Dedicated.MoonSpiritKhakkhara.Projectiles";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 56;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.Opacity = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 3f;
        }

        public override void AI()
        {
            Projectile.GetNearestTarget(3500f, 1750f, out bool foundTarget, out NPC closestTarget);
            if (!foundTarget || closestTarget is null)
            {
                // Just rapidly speed up in the direction its facing if there are no enemies.
                // Fade out after the usual charging time.
                Projectile.velocity *= 1.09f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.spriteDirection = Projectile.direction;

                if (Timer >= MaxChargingTime)
                {
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.03f);
                    Projectile.damage = 0;
                    Projectile.velocity *= 0.9f;
                    if (Timer >= MaxChargingTime + FadeoutTime)
                        Projectile.Kill();
                }

                Timer++;
                return;
            }

            if (AIState == 0f)
            {
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, EasingFunctions.SineEaseInOut(Timer / TimeBeforeCharging));
                Projectile.rotation = Projectile.AngleTo(closestTarget.Center);
                Projectile.velocity *= 0.9f;

                if (Timer == TimeBeforeCharging)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(closestTarget.Center) * 75f;
                    AIState = 1f;
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 1f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Timer >= MaxChargingTime || HitCounter >= 1f)
                {
                    //Main.LocalPlayer.Calamity().GeneralScreenShakePower = 8f;
                    ScreenShakeSystem.StartShake(8f, shakeStrengthDissipationIncrement: 0.185f);
                    AIState = 2f;
                    Timer = 0f;

                    // Some particles to mimic an explosion like effect.
                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(15f, 25f);
                        Color color = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                        float scale = Main.rand.NextFloat(3f, 6f);
                        HeavySmokeParticle deathSmoke = new(Projectile.Center, velocity, color, Main.rand.Next(75, 140), scale, Main.rand.NextFloat(0.35f, 1f), 0.06f, true, 0);
                        deathSmoke.SpawnCasParticle();
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f) * 25f;
                        Color normalColor = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                        Color bloomColor = Color.Lerp(Color.PowderBlue, Color.LightSkyBlue, Main.rand.NextFloat());
                        float scale = Main.rand.NextFloat(0.45f, 4f);
                        int lifespan = Main.rand.Next(15, 45);
                        SparkleParticle sparkle = new(Projectile.Center, velocity, normalColor, bloomColor, scale, lifespan, 0.25f, bloomScale: scale);
                        sparkle.SpawnCasParticle();
                    }
                }
            }

            if (AIState == 2f)
            {
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.03f);
                Projectile.damage = 0;
                Projectile.velocity *= 0.9f;
                if (Timer >= FadeoutTime)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Timer++;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.AdjustProjectileHitboxByScale(48f, 56f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => HitCounter++;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D baseTexture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            int frameHeight = baseTexture.Height / Main.projFrames[Type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, frameY, baseTexture.Width, frameHeight);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Cyan * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.rotation, projRec.Size() / 2f, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            return false;
        }
    }
}
