using CalamityMod.Sounds;
using Luminance.Common.Easings;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Players.BuffHandlers;
using EasingType = Luminance.Common.Easings.EasingType;

namespace TwilightEgress.Content.Items.Dedicated.Enchilada
{
    public class MechonSlayerHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float WeaponState => ref Projectile.ai[1];

        private const int SwingTime = 45;
        private const int MaxTime = 60;

        public static readonly PiecewiseCurve ThrustCurve = new PiecewiseCurve()
            .Add(EasingCurves.Sine, EasingType.Out, -0.125f, 0.5f)
            .Add(EasingCurves.Exp, EasingType.Out, 1f, 1f, 0.125f);

        private bool Initialized { get; set; } = false;

        public new string LocalizationCategory => "Projectiles.Misc";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Owner.dead || Owner.CCed || Owner.noItems || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<MechonSlayer>())
            {
                Projectile.Kill();
                return;
            }

            InitializeFields(MaxTime, -MathHelper.PiOver2);
            Vector2 holdPosition = Owner.Top + new Vector2(0f, -20f * ThrustCurve.Evaluate(Timer / SwingTime) + 35f);
            Projectile.Center = Owner.RotatedRelativePoint(holdPosition, true);

            Timer++;
            if (Timer == SwingTime / 2)
            {
                // Apply the art specific buffs.
                Owner.GetModPlayer<BuffHandler>().ApplyMechonSlayerArt((int)WeaponState);

                // Visuals.
                for (int i = 0; i < 15; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 8f);
                    SparkleParticle sparkle = new(Owner.RotatedRelativePoint(Owner.MountedCenter), velocity, GetArtColor(Color.Cyan), GetArtColor(Color.Cyan) * 0.75f, Main.rand.NextFloat(0.65f, 1.25f), Main.rand.Next(30, 45), 0.03f);
                    sparkle.SpawnCasParticle();
                }

                for (int i = 0; i < 10; i++)
                {
                    Vector2 basePosition = Owner.RotatedRelativePoint(Owner.MountedCenter);
                    Vector2 endPosition = basePosition + Main.rand.NextVector2CircularEdge(200f, 200f);
                    float scale = Main.rand.NextFloat(15f, 20f);
                    LightningArcParticle lightningAcrs = new(basePosition, endPosition, 80f, 1f, scale, GetArtColor(Color.Cyan), 25, true, true);
                    lightningAcrs.SpawnCasParticle();
                }

                MechonSlayerArtParticle artSymbol = new(Owner.RotatedRelativePoint(Owner.MountedCenter), 0.65f, 3f, (int)WeaponState, 60);
                artSymbol.SpawnCasParticle();
                PulseRingParticle pulseRing = new(Owner.RotatedRelativePoint(Owner.MountedCenter), Vector2.Zero, GetArtColor(Color.Cyan), 0f, 2f, 45);
                pulseRing.SpawnCasParticle();

                ScreenShakeSystem.StartShakeAtPoint(Owner.Center, 3f, shakeStrengthDissipationIncrement: 0.3f, intensityTaperEndDistance: 2000);
                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, Projectile.Center);
            }

            if (Timer >= SwingTime / 2)
            {
                // Shake a bit after the visual effects.
                Projectile.position += Main.rand.NextVector2Circular(5f, 5f);
                // Fade out.
                if (Timer is >= 30 and <= MaxTime)
                    Projectile.Opacity = Utils.GetLerpValue(1f, 0f, (Timer - 30f) / 30f, true);
            }

            // Set the owner's held projectile index as our projectile's index.
            Owner.heldProj = Projectile.whoAmI;
        }

        public void InitializeFields(int timeLeft, float rotation, SoundStyle? soundToPlay = null)
        {
            if (!Initialized)
            {
                Initialized = true;
                SoundEngine.PlaySound(soundToPlay ?? null, Projectile.Center);
                Projectile.timeLeft = timeLeft;
                Projectile.rotation = rotation;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBlade();
            return false;
        }

        public Color GetArtColor(Color? baseColor = null)
        {
            Color glowColor = baseColor ?? Color.Transparent;
            switch (WeaponState)
            {
                // Armor.
                case 0:
                    glowColor = Color.Orange;
                    break;

                // Eater.
                case 1:
                    glowColor = Color.LightSlateGray;
                    break;

                // Enchant.
                case 2:
                    glowColor = Color.Magenta;
                    break;

                // Purge.
                case 3:
                    glowColor = Color.LimeGreen;
                    break;

                // Speed.
                case 4:
                    glowColor = Color.LightSkyBlue;
                    break;
            }

            return glowColor;
        }

        public void DrawBlade()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D baseMechonSlayerSprite = ModContent.Request<Texture2D>("TwilightEgress/Content/Items/Dedicated/Enchilada/MechonSlayer").Value;

            float baseDrawAngle = Projectile.rotation;
            float drawRotation = baseDrawAngle + MathHelper.PiOver4;

            Vector2 origin = new(0f, texture.Height);
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw the main sprite.
            Main.EntitySpriteDraw(WeaponState == -1 ? baseMechonSlayerSprite : texture, drawPosition, null, Projectile.GetAlpha(GetArtColor(Color.White)), drawRotation, origin, Projectile.scale, 0);
        }
    }
}
