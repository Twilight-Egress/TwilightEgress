using CalamityMod;
using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Weapons.Rogue.HolidayHalberd
{
    public class HolidayHalbertHoldout : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private bool ShouldDespawn => Owner.dead || !Owner.channel || !Owner.active || Owner.CCed || Owner.HeldItem.type != ModContent.ItemType<HolidayHalberd>();

        private const int MaxSpinTimeThreshold = 50;

        private const int RotationSpeedIndex = 0;

        public new string LocalizationCategory => "Projectiles.Rogue";

        public override string Texture => "TwilightEgress/Content/Items/Weapons/Rogue/HolidayHalberd/HolidayHalberd";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.scale = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // idk wtf collision point does personally, don't ask me
            // - fryzahh
            float collisionPoint = 0f;

            // Get the length we want for our collision line. In this casee it should match the weapon.
            float lineLength = 78f * Projectile.scale;
            // We get the position where the blade is being held.
            Vector2 startPoint = Projectile.Center + Projectile.rotation.ToRotationVector2();
            // And finally, return a line collision check.
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, startPoint + Projectile.rotation.ToRotationVector2() * lineLength, 32, ref collisionPoint);
        }

        public override void AI()
        {
            if (ShouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            AttackBehavior();
            UpdatePlayerVariables();
        }

        public void AttackBehavior()
        {
            ref float rotationSpeed = ref Projectile.TwilightEgress().ExtraAI[RotationSpeedIndex];

            // Increase the spin speed over time.
            if (Timer <= 30f)
                rotationSpeed = MathHelper.Lerp(45f, 8f, Timer / 30f);


            // Fire and kill.
            if (Timer >= MaxSpinTimeThreshold)
            {
                Vector2 velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * 30f * Projectile.scale;
                Vector2 spawnPosition = Projectile.Center + Projectile.SafeDirectionTo(Main.MouseWorld) * 5f;
                int p = Projectile.BetterNewProjectile(spawnPosition, velocity, ModContent.ProjectileType<HolidayHalberdThrown>(), Projectile.damage,
                    Projectile.knockBack, CommonCalamitySounds.LouderSwingWoosh, null, Projectile.owner);

                if (Main.projectile.IndexInRange(p))
                {
                    if (Owner.Calamity().StealthStrikeAvailable())
                    {
                        Main.projectile[p].Calamity().stealthStrike = true;
                        Main.projectile[p].damage = (int)(Projectile.damage * 0.25f);
                        ConsumeStealthManually();
                    }
                }

                Projectile.Kill();
                return;
            }

            Projectile.Center = Owner.MountedCenter;
            Projectile.rotation += MathHelper.Pi / rotationSpeed * Owner.direction;
            Projectile.scale = MathHelper.Clamp(Projectile.scale + 0.05f, 0f, 1f);

            // Particles.
            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 78f + Main.rand.NextVector2Circular(65f, 65f) * Projectile.scale;
                float scale = Main.rand.NextFloat(0.2f, 0.8f) * Projectile.scale;
                int lifespan = Main.rand.Next(15, 30);
                SparkleParticle sparkleParticle = new(spawnPosition, Vector2.Zero, GetHalberdVisualColors(), GetHalberdVisualColors() * 0.35f, scale, lifespan, 0.25f, 1.25f);
                sparkleParticle.SpawnCasParticle();
            }
        }

        /// <summary>
        /// A copy of Calamity's ConsumeRogueStealth method. This is only to be used if your Rogue Weapon functions under a held projectile or some other mean
        /// that isn't taken into consideration during manual stealth updating. This was made because the original method is internal.
        /// </summary>
        public void ConsumeStealthManually()
        {
            Owner.Calamity().stealthStrikeThisFrame = true;
            Owner.Calamity().stealthAcceleration = 1f;
            float lossReductionRatio = (float)Owner.Calamity().flatStealthLossReduction / (Owner.Calamity().rogueStealthMax * 100f);
            float remainingStealth = Owner.Calamity().rogueStealthMax * lossReductionRatio;
            float stealthToLose = Owner.Calamity().rogueStealthMax - remainingStealth;
            if (stealthToLose < 0.01f)
            {
                stealthToLose = 0.01f;
            }
            if (Owner.Calamity().stealthStrikeHalfCost)
            {
                Owner.Calamity().rogueStealth -= 0.5f * stealthToLose;
                if (Owner.Calamity().rogueStealth <= 0f)
                {
                    Owner.Calamity().rogueStealth = 0f;
                }
            }
            else if (Owner.Calamity().stealthStrike75Cost)
            {
                Owner.Calamity().rogueStealth -= 0.75f * stealthToLose;
                if (Owner.Calamity().rogueStealth <= 0f)
                {
                    Owner.Calamity().rogueStealth = 0f;
                }
            }
            else if (Owner.Calamity().stealthStrike85Cost)
            {
                Owner.Calamity().rogueStealth -= 0.9f * stealthToLose;
                if (Owner.Calamity().rogueStealth <= 0f)
                {
                    Owner.Calamity().rogueStealth = 0f;
                }
            }
            else
            {
                Owner.Calamity().rogueStealth = remainingStealth;
            }
        }

        public void UpdatePlayerVariables()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.direction = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
           

            DrawHalberd();
            return false;
        }

        public Color GetHalberdVisualColors()
        {
            Color mainColor = Utilities.ColorSwap(Color.Red, Color.Lime, 2f);
            Color stealthColor = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f, Color.Cyan, Color.LightCyan, Color.LightSkyBlue, Color.LightBlue);
            if (Owner.Calamity().rogueStealth > 0f)
                mainColor = Color.Lerp(mainColor, stealthColor, Owner.Calamity().rogueStealth / Owner.Calamity().rogueStealthMax);

            return mainColor;
        }

        public void DrawHalberd()
        {           
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // This is all explained in MSK's file if you're having trouble understanding.
            float extraAngle = Owner.direction < 0 ? MathHelper.PiOver2 : 0f;
            float baseDrawAngle = Projectile.rotation;
            float drawRotation = baseDrawAngle + MathHelper.PiOver4 + extraAngle;

            Vector2 origin = new Vector2(Owner.direction < 0 ? texture.Width : 0f, texture.Height);
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw backglow effects. 
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 4) * 3f;
                Main.EntitySpriteDraw(texture, backglowDrawPositon, null, Projectile.GetAlpha(GetHalberdVisualColors()), drawRotation, origin, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), drawRotation, origin, Projectile.scale, effects, 0);
        }

        public float TrailWidthFunction(float trailLengthInterpolant) => 35f * Utils.GetLerpValue(1f, 0f, trailLengthInterpolant, true) * Projectile.scale;

        public Color TrailColorFunction(float trailLengthInterpolant) => Color.Lerp(Color.White, GetHalberdVisualColors(), 0.85f);

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            // Get the draw points for the primitive trail.
            // This needs to be done at the same time the trail is being drawn, otherwise not every point will be properly adjusted.
            for (int i = 0; i < 12; i++)
            {
                float localRotation = Projectile.oldRot[i];
                if (i == 0)
                    localRotation = Projectile.rotation;
                Projectile.oldPos[i] = Projectile.position + localRotation.ToRotationVector2() * 78f * Projectile.scale;
            }

            ShaderManager.TryGetShader("TwilightEgress.SmoothTextureMapTrail", out ManagedShader smoothTrail);
            smoothTrail.SetTexture(AssetRegistry.Textures.LightStreak, 1, SamplerState.LinearWrap);
            smoothTrail.TrySetParameter("time", Main.GlobalTimeWrappedHourly);

            PrimitiveSettings settings = new(TrailWidthFunction, TrailColorFunction, _ => Projectile.Size * 0.5f, true, true, smoothTrail);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 36);
        }
    }
}
