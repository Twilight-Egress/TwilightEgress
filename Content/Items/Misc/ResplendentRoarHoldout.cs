using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Sounds;
using Luminance.Common.Easings;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;
using TwilightEgress.Core.Graphics;
using TwilightEgress.Core.Players;

namespace TwilightEgress.Content.Items.Misc
{
    public class ResplendentRoarHoldout : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        private enum AttackTypes
        {
            RebirthSlashes,
            FinalDyingRoar
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AttackCounter => ref Projectile.ai[1];

        private ref float AttackType => ref Projectile.localAI[0];

        private ref float AIState => ref Projectile.localAI[1];

        private bool IsChannelingRMB => Owner.Calamity().mouseRight && Owner.active && Owner.HeldItem.type == ModContent.ItemType<ResplendentRoar>();

        private bool ShouldDespawn => Owner.dead || !Owner.active || Owner.CCed || Owner.HeldItem.type != ModContent.ItemType<ResplendentRoar>();

        private Vector2 Direction = Vector2.Zero;

        private Vector2 DistanceFromPlayer => Direction * -2f;

        private float SwingDirection => Projectile.ai[2] * Math.Sign(Direction.X);

        private float BaseRotation { get; set; }

        private bool Initialized { get; set; }

        private const float StartingAngle = -(MathHelper.PiOver2 + MathHelper.PiOver4);

        private const float LargeSwingAngle = -(MathHelper.Pi + MathHelper.PiOver4);

        private const int SmallSwingMaxTime = 35;

        private const int LargeSwingMaxTime = 45;

        private const int YharonFrameIndex = 0;

        private const int YharonFrameCounterIndex = 1;

        private const int YharonOpacityIndex = 2;

        private const int YharonScaleIndex = 3;

        private const int YharonDrawImageRotationIndex = 4;

        private const int YharonDrawImageRadiusIndex = 5;

        public static readonly PiecewiseCurve ThrustCurve = new PiecewiseCurve()
            .Add(EasingCurves.Circ, EasingType.In, 0.125f, 0.5f)
            .Add(EasingCurves.Exp, EasingType.Out, 1f, 1f);

        public float SwingRatio(bool small) => ThrustCurve.Evaluate(Timer / (small ? SmallSwingMaxTime : LargeSwingMaxTime));

        public new string LocalizationCategory => "Projectiles.Melee";

        public override string Texture => "CalamityMod/Items/Weapons/Melee/TheBurningSky";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1.25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float lineLength = 146f * Projectile.scale;
            Vector2 startPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() + DistanceFromPlayer;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, startPoint + Projectile.rotation.ToRotationVector2() * lineLength, 60, ref collisionPoint);
        }

        public override void AI()
        {
            if (ShouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            // Left click and right click.
            switch ((AttackTypes)AttackType)
            {
                case AttackTypes.RebirthSlashes:
                    RebirthSlashes();
                    break;

                case AttackTypes.FinalDyingRoar:
                    FinalDyingRoar();
                    break;
            }

            Timer++;
            Projectile.Center = Owner.MountedCenter + DistanceFromPlayer;
            UpdatePlayerVariables(AttackType == 1f);
        }

        public void RebirthSlashes()
        {
            // The attack pattern.
            switch (AttackCounter)
            {
                case < 2:
                    DoBehavior_SmallSwing();
                    break;

                case >= 2 and < 4:
                    DoBehavior_LargeSwing();
                    break;
            }
        }

        public void DoBehavior_SmallSwing()
        {
            // Some initialization.
            if (!Initialized)
            {
                Initialized = true;
                Projectile.timeLeft = SmallSwingMaxTime;
                Direction = Projectile.velocity;
                Direction.Normalize();
                BaseRotation = Projectile.velocity.ToRotation();
                Projectile.rotation = StartingAngle * SwingDirection + Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            float startingAngle = StartingAngle * SwingDirection + BaseRotation;
            float endingAngle = -StartingAngle * SwingDirection + BaseRotation;
            Projectile.rotation = MathHelper.Lerp(startingAngle, endingAngle, SwingRatio(true));

            float distanceToMiddle = MathHelper.Distance(SwingRatio(true), 0.5f);
            if (Timer == SmallSwingMaxTime / 2)
            {
                SoundEngine.PlaySound(AssetRegistry.Sounds.YharonHurt with { PitchVariance = 1f }, Projectile.Center);
                SoundEngine.PlaySound(CommonCalamitySounds.LouderSwingWoosh, Projectile.Center);
            }

            Projectile.scale = 1.25f + (float)Math.Sin((double)(SwingRatio(true) * (float)Math.PI)) * 0.45f;
        }

        public void DoBehavior_LargeSwing()
        {
            if (!Initialized)
            {
                Initialized = true;
                Projectile.timeLeft = LargeSwingMaxTime;
                Direction = Projectile.velocity;
                Direction.Normalize();
                BaseRotation = Projectile.velocity.ToRotation();
                Projectile.rotation = LargeSwingAngle * SwingDirection + Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            float startingAngle = LargeSwingAngle * SwingDirection + BaseRotation;
            float endingAngle = -StartingAngle * SwingDirection + BaseRotation;
            Projectile.rotation = MathHelper.Lerp(startingAngle, endingAngle, SwingRatio(false));

            float distanceToMiddle = MathHelper.Distance(SwingRatio(false), 0.5f);
            if (Timer == LargeSwingMaxTime / 2)
            {
                SoundEngine.PlaySound(AssetRegistry.Sounds.YharonRoarShort with { PitchVariance = 0.15f }, Projectile.Center);
                SoundEngine.PlaySound(CommonCalamitySounds.LouderPhantomPhoenix, Projectile.Center);
            }

            Projectile.scale = 1.25f + (float)Math.Sin((double)(SwingRatio(false) * (float)Math.PI)) * 0.75f;
        }

        public void FinalDyingRoar()
        {
            ref float yharonFrame = ref Projectile.TwilightEgress().ExtraAI[YharonFrameIndex];
            ref float yharonFrameCounter = ref Projectile.TwilightEgress().ExtraAI[YharonFrameCounterIndex];
            ref float yharonOpacity = ref Projectile.TwilightEgress().ExtraAI[YharonOpacityIndex];
            ref float yharonScale = ref Projectile.TwilightEgress().ExtraAI[YharonScaleIndex];

            if (!Initialized)
            {
                Initialized = true;
                Direction = Projectile.velocity;
                Direction.Normalize();
                BaseRotation = Projectile.velocity.ToRotation();
                Projectile.rotation = BaseRotation;
                SoundEngine.PlaySound(AssetRegistry.Sounds.YharonRoarShort with { PitchVariance = 1f }, Projectile.Center);
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            if (AIState == 0f)
            {
                Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);

                if (++yharonFrameCounter >= 5)
                {
                    if (++yharonFrame >= 5)
                        yharonFrame = 0f;
                    yharonFrameCounter = 0f;
                }

                float maximumOpacity = 1f * Utils.GetLerpValue(0f, 1f, Owner.GetModPlayer<ResplendentRoarPlayer>().ResplendentRazeCharge / 100f, true);
                float maximumScale = 1.35f * Utils.GetLerpValue(0f, 1f, Owner.GetModPlayer<ResplendentRoarPlayer>().ResplendentRazeCharge / 100f, true);
                yharonOpacity = MathHelper.Lerp(yharonOpacity, maximumOpacity, Timer / 90f);
                yharonScale = MathHelper.Lerp(yharonScale, maximumScale, Timer / 90f);

                if (Timer >= 90f && !IsChannelingRMB || Timer >= 600f)
                {
                    AIState = 1f;
                    Timer = 0f;
                    SoundEngine.PlaySound(AssetRegistry.Sounds.YharonRoar with { Volume = 4f }, Projectile.Center);

                    for (int i = 0; i < 3; i++)
                    {
                        Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                        Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                        Color fireColor = Color.Lerp(colorGroup, secondColorGroup, Main.rand.NextFloat(0.2f, 0.8f));

                        float scale = i == 2 ? 9f : i == 1f ? 6f : 3f;
                        PulseRingParticle yharonRoar = new(Owner.Center, Vector2.Zero, fireColor, 0.01f, scale, 60);
                        yharonRoar.SpawnCasParticle();
                    }

                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 1f)
            {
                yharonFrame = 6;
                yharonFrameCounter = 0f;

                Owner.immuneTime = 30;
                Owner.velocity = Projectile.rotation.ToRotationVector2() * 55f;

                if (Timer >= 30f)
                {
                    Timer = 0f;
                    AIState = 2f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 2f)
            {
                yharonOpacity = MathHelper.Clamp(yharonOpacity - 0.025f, 0f, 1f);
                yharonScale = MathHelper.Clamp(yharonScale - 0.025f, 0f, 1.35f);
                Projectile.scale = MathHelper.Clamp(Projectile.scale - 0.025f, 0f, 1f);
                Owner.velocity *= 0.9f;

                if (Timer >= 30f)
                {
                    Projectile.Kill();
                    Owner.GetModPlayer<ResplendentRoarPlayer>().ResplendentRazeCharge = 0f;
                }
            }
            Owner.GetModPlayer<ResplendentRoarPlayer>().ResplendentRazeUpdateTimer = 0;
        }

        public void ParticleVisuals()
        {
            ResplendentRoarPlayer resplendentRoarPlayer = Owner.GetModPlayer<ResplendentRoarPlayer>();

            if (resplendentRoarPlayer.ResplendentRazeCharge >= 25f)
            {
                for (int i = 0; i < 3; i++)
                {
                    float lineLength = 146f * Projectile.scale;
                    Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * lineLength + DistanceFromPlayer + Main.rand.NextVector2Circular(80f, 80f);

                    Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                    Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                    Color fireColor = Color.Lerp(colorGroup, secondColorGroup, Main.rand.NextFloat(0.2f, 0.8f));

                    int lifespan = Main.rand.Next(15, 30);
                    float scale = Main.rand.NextFloat(0.25f, 0.85f) * (resplendentRoarPlayer.ResplendentRazeCharge / 100f) - 2.5f;
                    float opacity = Main.rand.NextFloat(0.9f, 4f) * (resplendentRoarPlayer.ResplendentRazeCharge / 100f) - 1f;

                    HeavySmokeParticle flames = new(spawnPosition, Vector2.Zero, fireColor, lifespan, scale, opacity, 0.03f, true);
                    flames.SpawnCasParticle();
                }

                for (int i = 0; i < 4; i++)
                {
                    float lineLength = 146f * Projectile.scale;
                    Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * lineLength + DistanceFromPlayer + Main.rand.NextVector2Circular(120f, 120f);

                    Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                    Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                    Color sparkleColor = Color.Lerp(colorGroup, secondColorGroup, Main.rand.NextFloat(0.2f, 0.8f));

                    int lifespan = Main.rand.Next(15, 30);
                    float scale = Main.rand.NextFloat(0.45f, 1f) * (resplendentRoarPlayer.ResplendentRazeCharge / 100f) - 3f;
                    float opacity = Main.rand.NextFloat(0.65f, 2f) * (resplendentRoarPlayer.ResplendentRazeCharge / 100f) - 1f;

                    SparkleParticle sparkles = new(spawnPosition, Vector2.Zero, sparkleColor * opacity, sparkleColor * 1.05f * opacity, scale, lifespan);
                    sparkles.SpawnCasParticle();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ResplendentRoarPlayer resplendentRoarPlayer = Owner.GetModPlayer<ResplendentRoarPlayer>();

            // Charge the resplendant raze.
            resplendentRoarPlayer.ResplendentRazeCharge += 2f;
            if (hit.Crit)
                resplendentRoarPlayer.ResplendentRazeCharge += 4f;

            // Add the Dragonfire Debuff after the charge reaches 75%
            if (resplendentRoarPlayer.ResplendentRazeCharge >= 75f)
            {
                target.AddBuff(ModContent.BuffType<Dragonfire>(), 180);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { PitchVariance = 1f, MaxInstances = 0 }, target.Center);
            }

            // Visual stuff.
            ScreenShakeSystem.StartShakeAtPoint(target.Center, 3f, shakeStrengthDissipationIncrement: 0.5f, intensityTaperEndDistance: 2000);
            for (int i = 0; i < Main.rand.Next(15, 25); i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(5f, 15f);
                float scale = Main.rand.NextFloat(0.85f, 1.25f);
                int lifespan = Main.rand.Next(25, 45);

                Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                Color sparkColor = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);

                SparkParticle sparks = new(target.Center, velocity, sparkColor, scale, lifespan);
                sparks.SpawnCasParticle();
            }

            if (resplendentRoarPlayer.ResplendentRazeCharge >= 75f)
            {
                for (int i = 0; i < Main.rand.Next(10, 12); i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 7f);
                    float scale = Main.rand.NextFloat(1f, 2f);
                    int lifespan = Main.rand.Next(25, 45);

                    Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                    Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                    Color flameColor = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);

                    HeavySmokeParticle flames = new(target.Center, velocity, flameColor, lifespan, scale, 1f, 0.03f, true);
                    flames.SpawnCasParticle();
                }
            }
        }

        public void UpdatePlayerVariables(bool updateDirectionByRotation = false)
        {
            Owner.heldProj = Projectile.whoAmI;
            if (updateDirectionByRotation)
                Owner.direction = Math.Sign(Projectile.rotation.ToRotationVector2().X);
            else
                Owner.direction = Math.Sign(Projectile.velocity.X);
            Owner.itemRotation = Projectile.rotation;
            if (Owner.direction != 1)
            {
                Owner.itemRotation -= (float)Math.PI;
            }
            Owner.itemRotation = MathHelper.WrapAngle(Projectile.rotation);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float yharonOpacity = ref Projectile.TwilightEgress().ExtraAI[YharonOpacityIndex];
            ref float yharonScale = ref Projectile.TwilightEgress().ExtraAI[YharonScaleIndex];

            // This sets the starting position of the primitive trail so that the other positions follow it accordingly
            Projectile.oldPos[0] = Projectile.position + Projectile.rotation.ToRotationVector2() * 128f * Projectile.scale;

            DrawHalberd();
            return false;
        }

        public void DrawHalberd()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            bool shouldFlipSprite = SwingDirection == -1;

            SpriteEffects effects = shouldFlipSprite ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float extraAngle = shouldFlipSprite ? MathHelper.PiOver2 : 0f;
            float baseDrawAngle = Projectile.rotation;
            float drawRotation = baseDrawAngle + MathHelper.PiOver4 + extraAngle;

            Vector2 origin = new Vector2(shouldFlipSprite ? texture.Width : 0f, texture.Height);
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw backglow effects. 
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 4) * 3f;

                Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
                Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
                Color backglowColor = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);

                Main.EntitySpriteDraw(texture, backglowDrawPositon, null, Projectile.GetAlpha(backglowColor), drawRotation, origin, Projectile.scale * 1.015f, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), drawRotation, origin, Projectile.scale, effects, 0);
        }

        /*public void DrawYharon(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            ref float yharonFrame = ref Projectile.TwilightEgress().ExtraAI[YharonFrameIndex];
            ref float yharonOpacity = ref Projectile.TwilightEgress().ExtraAI[YharonOpacityIndex];
            ref float yharonScale = ref Projectile.TwilightEgress().ExtraAI[YharonScaleIndex];
            ref float yharonImageRotation = ref Projectile.TwilightEgress().ExtraAI[YharonDrawImageRotationIndex];
            ref float yharonImageRadius = ref Projectile.TwilightEgress().ExtraAI[YharonDrawImageRadiusIndex];

            Texture2D baseTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/Projectiles/Melee/ResplendentRoarYharon").Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Rectangle projRec = baseTexture.Frame(1, 7, 0, (int)(yharonFrame % 7));
            Vector2 origin = new Vector2((Owner.direction < 0) ? projRec.Width * 0.4f : projRec.Width * 0.6f, projRec.Height * 0.6f);

            float extraAngle = (Owner.direction < 0 ? Pi : 0f);
            float rotation = Projectile.rotation - extraAngle; 

            Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
            Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
            Color chickenColor = Color.Lerp(MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup), Color.White, 0.3f);

            for (int i = 0; i < 6; i++)
            {
                yharonImageRotation += TwoPi / 1200f;
                Vector2 drawPosition = Owner.Center - Main.screenPosition + Vector2.UnitY.RotatedBy(yharonImageRotation + TwoPi * i / 6f) * 30f;
                Main.spriteBatch.Draw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(chickenColor * 0.9f) * yharonOpacity, rotation, origin, yharonScale, effects, 0);
            }
            orig.Invoke(self);
        }*/

        public float TrailWidthFunction(float completionRatio) => 40f * Utils.GetLerpValue(1f, 0f, completionRatio, true) * Projectile.scale;

        public Color TrailColorFunction(float completionRatio)
        {
            Color colorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.IndianRed, Color.Yellow, Color.Red);
            Color secondColorGroup = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.75f, Color.OrangeRed, Color.Sienna, Color.PaleVioletRed);
            Color trailColor = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.5f, colorGroup, secondColorGroup);
            return trailColor;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            // This sets the starting position of the primitive trail so that the other positions follow it accordingly
            Projectile.oldPos[0] = Projectile.position + Projectile.rotation.ToRotationVector2() * 128f * Projectile.scale;

            ShaderManager.TryGetShader("TwilightEgress.SmoothTextureMapTrail", out ManagedShader smoothTrail);
            smoothTrail.SetTexture(AssetRegistry.Textures.FlameStreak, 1, SamplerState.LinearWrap);
            smoothTrail.TrySetParameter("time", Main.GlobalTimeWrappedHourly);

            PrimitiveSettings settings = new(TrailWidthFunction, TrailColorFunction, _ => Projectile.Size * 0.5f, true, true, smoothTrail);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 48);
        }
    }
}
