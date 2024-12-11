using CalamityMod;
using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;

namespace TwilightEgress.Content.Items.Dedicated.Marv
{
    public class BoltStrike : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];

        public ref float ScaleControl => ref Projectile.ai[1];

        public ref float AIPhase => ref Projectile.localAI[0];

        public ref float ColorTimer => ref Projectile.localAI[1];

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "TwilightEgress/Content/Items/Dedicated/Marv/ElectricSkyBoltExplosion";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            bool isChanneling = (Owner.channel || Owner.Calamity().mouseRight) && Owner.active && Owner.HeldItem.type == ModContent.ItemType<ThunderousFury>();
            if (!isChanneling && ScaleControl < 0.25f)
            {
                // Disentegrate into a worthless little puff of dust if under a certain scale.
                Projectile.Kill();
                return;
            }

            const int MaxChargeTime = 480;
            if (AIPhase == 0f)
            {
                if (isChanneling)
                {
                    Projectile.Center = Owner.MountedCenter + Owner.MountedCenter.AngleTo(Owner.Calamity().mouseWorld).ToRotationVector2() * 130f;
                    if (Timer < MaxChargeTime + 1)
                    {
                        // Slowly increase scale and opacity over time.
                        // Color is also adjusted accordingly in the Drawcode below.
                        ScaleControl = 2f * Utils.GetLerpValue(0.1f, 1f, Timer / MaxChargeTime, true);
                        Projectile.Opacity = MathHelper.Lerp(0f, 1f, Timer / 60);
                        Timer++;
                        ColorTimer++;
                    }

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<ThunderousFuryHoldout>() && projectile.timeLeft <= 2)
                        {
                            // Fire and switch the next AI Phase if the weapon is channeled for too long.
                            AIPhase = 1f;
                            Timer = 0f;
                            Projectile.velocity = Projectile.DirectionTo(Owner.Calamity().mouseWorld) * 35f;
                            SoundEngine.PlaySound(CommonCalamitySounds.PlasmaBlastSound, Projectile.Center);
                            //Main.LocalPlayer.Calamity().GeneralScreenShakePower = 7f * Projectile.scale;
                            ScreenShakeSystem.StartShake(7f * Projectile.scale, shakeStrengthDissipationIncrement: 0.185f);

                            Color shockwaveColor = Color.Lerp(Color.Yellow, Color.Cyan, TwilightEgressUtilities.SineEaseInOut(ColorTimer / 480));
                            int lifespan = (int)MathHelper.Lerp(10, 45, Timer / 480);
                            new RoaringShockwaveParticle(lifespan, Projectile.Center, Vector2.Zero, shockwaveColor, 0.1f, Main.rand.NextFloat(MathHelper.TwoPi)).SpawnCasParticle();
                            Projectile.netUpdate = true;
                        }
                    }
                }
                else
                {
                    // Fire off towards the cursor and play a sound with some visuals.
                    AIPhase = 1f;
                    Timer = 0f;
                    Projectile.velocity = Projectile.DirectionTo(Owner.Calamity().mouseWorld) * 35f;
                    SoundEngine.PlaySound(CommonCalamitySounds.PlasmaBlastSound, Projectile.Center);
                    //Main.LocalPlayer.Calamity().GeneralScreenShakePower = 7f * Projectile.scale;
                    ScreenShakeSystem.StartShake(7f * Projectile.scale, shakeStrengthDissipationIncrement: 0.185f);

                    Color shockwaveColor = Color.Lerp(Color.Yellow, Color.Cyan, TwilightEgressUtilities.SineEaseInOut(ColorTimer / 480));
                    new RoaringShockwaveParticle(45, Projectile.Center, Vector2.Zero, shockwaveColor, 0.1f, Main.rand.NextFloat(MathHelper.TwoPi)).SpawnCasParticle();
                    Projectile.netUpdate = true;
                }
            }

            if (AIPhase == 1f)
            {
                // Speed up a bit.
                if (Projectile.velocity.Length() < 45f)
                {
                    Projectile.velocity *= 1.02f;
                }

                // Die if it comes in contact with nothing.
                if (Timer >= 240f)
                {
                    Projectile.Kill();
                    return;
                }
                Timer++;
            }

            if (AIPhase == 2f)
            {
                // BIG BOOM
                ScaleControl += 0.07f;
                Projectile.Opacity = MathHelper.Lerp(1f, 0f, Timer / 180);

                if (Timer >= 180f)
                {
                    Projectile.Kill();
                }

                // Particles.
                Color particleColor = Color.Lerp(Color.Yellow, Color.Cyan, TwilightEgressUtilities.SineEaseInOut(ColorTimer / 480));
                if (Timer % 10 == 0)
                {
                    int lifespan = (int)MathHelper.Lerp(45, 100, Timer / 480);
                    RoaringShockwaveParticle shockwaveParticle = new(lifespan, Projectile.Center, Vector2.Zero, particleColor, 0.1f, Main.rand.NextFloat(MathHelper.TwoPi));
                    shockwaveParticle.SpawnCasParticle();
                }

                if (Timer % 3 == 0)
                {
                    int sparkLifespan = Main.rand.Next(20, 36);
                    float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6f, 55f);
                        SparkParticle electricSpark = new(Projectile.Center, sparkVelocity, particleColor, sparkScale, sparkLifespan);
                        electricSpark.SpawnCasParticle();
                    }
                }

                Timer++;
                Projectile.velocity *= 0.3f;
                ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 5f, shakeStrengthDissipationIncrement: 0.185f);
            }

            Projectile.scale = ScaleControl;
            // Resize the hitbox based on scale.
            int oldWidth = Projectile.width;
            int idealWidth = (int)(Projectile.scale * 100f);
            int idealHeight = (int)(Projectile.scale * 100f);
            if (idealWidth != oldWidth)
            {
                Projectile.position.X += Projectile.width / 2;
                Projectile.position.Y += Projectile.height / 2;
                Projectile.width = idealWidth;
                Projectile.height = idealHeight;
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;
            }

            // Passive dust visuals.
            for (int i = 0; i < 3; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.TintableDustLighted, speed * Main.rand.NextFloat(2f, 6f));
                d.noGravity = true;
                d.color = Color.Lerp(Color.Yellow, Color.Cyan, TwilightEgressUtilities.SineEaseInOut(ColorTimer / 480));
            }

            Projectile.rotation += MathHelper.Pi / 30f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool correctPlayerName = Owner.name == "Marv" || Owner.name == "EmolgaLover";
            if (AIPhase == 1f)
            {
                // Super Effective! VS. BIG SHOT
                SoundStyle boom = correctPlayerName ? AssetRegistry.Sounds.SuperEffective : new SoundStyle("CalamityMod/Sounds/Item/TeslaCannonFire");
                SoundEngine.PlaySound(boom, Projectile.Center);
                if (correctPlayerName)
                {
                    Rectangle rectangle = new Rectangle((int)target.Center.X, (int)target.Center.Y, target.width, target.height);
                    CombatText.NewText(rectangle, Color.Yellow, "Super Effective!", true);
                }

                AIPhase = 2f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (ScaleControl < 0.25f)
            {
                for (int i = 0; i < 30; i++)
                {
                    Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, speed * 5f);
                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D Vortex = AssetRegistry.Textures.GreyscaleVortex.Value;

            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            int individualFrame = texture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrame * Projectile.frame;
            Rectangle rec = new Rectangle(0, currentYFrame, texture.Width, individualFrame);

            Color color = Color.Lerp(Color.Yellow, Color.Cyan, TwilightEgressUtilities.SineEaseInOut(ColorTimer / 480)) * Projectile.Opacity;

            // Vortex 1.
            Main.EntitySpriteDraw(Vortex, drawPosition, Vortex.Frame(), Projectile.GetAlpha(color), rotation, Vortex.Size() / 2f, Projectile.scale * 3f, SpriteEffects.None, 0);
            //Vortex 2.
            Main.EntitySpriteDraw(Vortex, drawPosition, Vortex.Frame(), Projectile.GetAlpha(color), -rotation, Vortex.Size() / 2f, Projectile.scale * 4f, SpriteEffects.None, 0);

            // Draw pulsing backglow effects.
            for (int i = 0; i < 4; i++)
            {
                float backglowRadius = MathHelper.Lerp(2f, 5f, TwilightEgressUtilities.SineEaseInOut((float)(Main.timeForVisualEffects / 30f)));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 4) * backglowRadius;

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(texture, backglowDrawPositon, rec, Projectile.GetAlpha(color), rotation, rec.Size() / 2f, Projectile.scale, effects, 0);
                Main.spriteBatch.ResetToDefault();
            }

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, rec, Projectile.GetAlpha(color), rotation, rec.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }

        public float TrailWidthFunction(float trailLengthInterpolant) => 40f * Utils.GetLerpValue(0.75f, 0f, trailLengthInterpolant, true) * Projectile.scale * Projectile.Opacity;

        public Color TrailColorFunction(float trailLengthInterpolant) => Color.Lerp(Color.Lerp(Color.Yellow, Color.Cyan, TwilightEgressUtilities.SineEaseInOut(ColorTimer / 480)), Color.White, 0.45f) * Projectile.Opacity;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            if (AIPhase != 1)
                return;

            ShaderManager.TryGetShader("TwilightEgress.SmoothTextureMapTrail", out ManagedShader smoothTrail);
            smoothTrail.SetTexture(AssetRegistry.Textures.FadedStreak, 1, SamplerState.LinearWrap);
            smoothTrail.TrySetParameter("time", Main.GlobalTimeWrappedHourly * 2.5f);

            PrimitiveSettings settings = new(TrailWidthFunction, TrailColorFunction, _ => Projectile.Size * 0.5f, true, true, smoothTrail);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 100);
        }
    }
}
