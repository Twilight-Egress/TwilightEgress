using Luminance.Common.Utilities;
using Terraria;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Dedicated.Jacob
{
    public class TomeOfTheTankHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float ChargeTimer => ref Projectile.ai[0];

        private const int RitualCircleScaleIndex = 0;

        private const int RitualCircleOpacityIndex = 1;

        private const int OrbitingSummoningCircleRotationIndex = 2;

        private const int PulseRingInitialScaleIndex = 3;

        private bool IsManaThresholdMet => Owner.statMana >= 300;

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "TwilightEgress/Content/Items/Dedicated/Jacob/TomeOfTheTank";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 2700;
        }

        public override void AI()
        {
            bool shouldDespawn = !IsManaThresholdMet || !Owner.channel || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<TomeOfTheTank>();
            if (shouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            UpdateProjectileVariablesAndFunctions();
            UpdatePlayerVariables();
        }

        public void UpdateProjectileVariablesAndFunctions()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Projectile.rotation += 0.03f * Projectile.direction;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += Pi;
            }

            PerformAttack();
        }

        public void UpdatePlayerVariables()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir(Sign(Owner.AngleTo(Main.MouseWorld).ToRotationVector2().X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.Center.AngleTo(Main.MouseWorld) - PiOver2);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Owner.Center.AngleTo(Main.MouseWorld) - PiOver2);
        }

        public void PerformAttack()
        {
            ref float ritualCircleScale = ref Projectile.TwilightEgress().ExtraAI[RitualCircleScaleIndex];
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];
            ref float pulseRingInitialScale = ref Projectile.TwilightEgress().ExtraAI[PulseRingInitialScaleIndex];

            if (ChargeTimer == 0f)
            {
                pulseRingInitialScale = 2.5f;
                Projectile.netUpdate = true;
            }

            int chargeTime = 180;
            if (ChargeTimer <= chargeTime)
            {
                if (ChargeTimer % 15 == 0f)
                {
                    pulseRingInitialScale = Clamp(pulseRingInitialScale + 0.25f, 0.5f, 3.5f);
                    PulseRingParticle chargeUpRing = new(Owner.Center, Vector2.Zero, Color.CornflowerBlue, pulseRingInitialScale, 0.01f, 45);
                    chargeUpRing.SpawnCasParticle();

                    SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);

                    for (int i = 0; i < 36; i++)
                    {
                        Vector2 magicDustSpawnOffset = (TwoPi * i / 36f).ToRotationVector2() * 200f + Main.rand.NextVector2Circular(15f, 15f);
                        Dust dust = Dust.NewDustPerfect(Owner.Center + magicDustSpawnOffset, 267);
                        dust.color = Color.Lerp(Color.CornflowerBlue, Color.Fuchsia, Main.rand.NextFloat());
                        dust.noGravity = true;
                        dust.scale = 1f * Main.rand.NextFloat(1.1f, 1.25f);
                        dust.velocity = (Owner.Center - dust.position) * 0.062f;
                    }
                }

                // Start to make the ritual circle visible after 3 seconds.
                if (ChargeTimer >= 60)
                {
                    ritualCircleScale = Lerp(0f, 1f, TwilightEgressUtilities.SineEaseInOut(ChargeTimer / 175f));
                    ritualCircleOpacity = Lerp(0f, 1f, TwilightEgressUtilities.SineEaseInOut(ChargeTimer / 175f));
                }
            }

            else if (ChargeTimer >= chargeTime && ChargeTimer % 120 == 0)
            {
                if (Owner.CheckMana(300, true, false))
                    Owner.manaRegenDelay = Owner.maxRegenDelay;

                Vector2 spawnPosition = Owner.Center + Vector2.UnitY.RotatedByRandom(TwoPi) * 250f;
                Vector2 velocity = -spawnPosition.DirectionTo(Owner.Center).SafeNormalize(Vector2.UnitY) * 5f;
                Projectile.BetterNewProjectile(spawnPosition, velocity, ModContent.ProjectileType<Rampart>(), Projectile.damage, Projectile.knockBack, SoundID.Item105);
            }

            ChargeTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float ritualCircleScale = ref Projectile.TwilightEgress().ExtraAI[RitualCircleScaleIndex];
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];

            // Only draw the ritual circle if necessary.
            if (ritualCircleOpacity > 0 && ritualCircleScale > 0)
            {
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                DrawRitualCircle();
                Main.spriteBatch.ResetToDefault();
            }
            DrawBook(lightColor);
            return false;
        }

        public void DrawRitualCircle()
        {
            ref float ritualCircleScale = ref Projectile.TwilightEgress().ExtraAI[RitualCircleScaleIndex];
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];
            ref float orbitingSummoningCircleRotation = ref Projectile.TwilightEgress().ExtraAI[OrbitingSummoningCircleRotationIndex];

            Texture2D outerCircle = ModContent.Request<Texture2D>("TwilightEgress/Content/Items/Dedicated/Jacob/TankGodRitualCircle").Value;
            Texture2D innerCircle = ModContent.Request<Texture2D>("TwilightEgress/Content/Items/Dedicated/Jacob/TankGodRitualCircleInner").Value;
            Texture2D orbitingCircle = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/RancorMagicCircle").Value;
            Texture2D blurredOrbitingCircle = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/RancorMagicCircleGlowmask").Value;

            Vector2 mainRitualDrawPosition = Owner.Center + Vector2.UnitY * Owner.gfxOffY - Main.screenPosition;
            float scale = ritualCircleScale * Projectile.scale;

            Color outerRitualColor = Utilities.ColorSwap(Color.CornflowerBlue, Color.Goldenrod, 1.5f) * ritualCircleOpacity;
            Color innerRitualColor = Utilities.ColorSwap(Color.Crimson, Color.DarkSlateBlue, 3f) * ritualCircleOpacity;

            Main.EntitySpriteDraw(outerCircle, mainRitualDrawPosition, null, Projectile.GetAlpha(outerRitualColor), Projectile.rotation, outerCircle.Size() / 2f, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(innerCircle, mainRitualDrawPosition, null, Projectile.GetAlpha(innerRitualColor), -Projectile.rotation, innerCircle.Size() / 2f, scale, SpriteEffects.None);

            // Draw the three orbiting summoning circles.
            for (int i = 0; i < 3; i++)
            {
                orbitingSummoningCircleRotation += TwoPi / 1200f;
                Vector2 orbitingRitualDrawPosition = mainRitualDrawPosition - Vector2.UnitY.RotatedBy(orbitingSummoningCircleRotation + TwoPi * i / 3f) * 192f;

                Color outerOrbitingCircleColor = outerRitualColor * ritualCircleOpacity * 0.8f;
                Color blurredOrbitingCircleColor = innerRitualColor * ritualCircleOpacity;

                Main.EntitySpriteDraw(orbitingCircle, orbitingRitualDrawPosition, null, Projectile.GetAlpha(outerOrbitingCircleColor), Projectile.rotation, orbitingCircle.Size() / 2f, scale, SpriteEffects.None);
                Main.EntitySpriteDraw(blurredOrbitingCircle, orbitingRitualDrawPosition, null, Projectile.GetAlpha(blurredOrbitingCircleColor), -Projectile.rotation, blurredOrbitingCircle.Size() / 2f, scale, SpriteEffects.None);
            }
        }

        public void DrawBook(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float rotation = Owner.AngleTo(Main.MouseWorld) + (Owner.direction < 0 ? Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, -2f) + rotation.ToRotationVector2() * (Owner.direction < 0 ? -30f : 30f) - Main.screenPosition;

            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
        }
    }
}
