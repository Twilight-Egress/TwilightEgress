using CalamityMod;
using Luminance.Common.Utilities;
using System;
using TwilightEgress.Assets;
using TwilightEgress.Content.Items.Dedicated.Raesh;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Dedicated.Fluffy
{
    public class TheBastOffenseHoldout : ModProjectile, ILocalizedModType
    {
        private enum AttackState
        {
            BastBarrage,
            BIGSHOTS
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float ChargeTimer => ref Projectile.ai[0];

        private ref float ResetAnimationTimer => ref Projectile.ai[1];

        private ref float AttackType => ref Projectile.ai[2];

        private ref float AIState => ref Projectile.localAI[0];

        public const int BastIncreaseDelayIndex = 0;

        public const int BastCatCountIndex = 1;

        public const int BackglowRotationIndex = 2;

        public const int WeaponShakeAngleIndex = 3;

        public const int OldRotationIndex = 4;

        public const int RecoilStrengthIndex = 5;

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override string Texture => "TwilightEgress/Content/Items/Dedicated/Fluffy/TheBastOffense";

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 3600;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            ref float bastIncreaseDelay = ref Projectile.TwilightEgress().ExtraAI[BastIncreaseDelayIndex];
            ref float bastCatCount = ref Projectile.TwilightEgress().ExtraAI[BastCatCountIndex];
            ref float weaponShakeAngle = ref Projectile.TwilightEgress().ExtraAI[WeaponShakeAngleIndex];
            ref float oldRotation = ref Projectile.TwilightEgress().ExtraAI[OldRotationIndex];
            ref float recoilStrength = ref Projectile.TwilightEgress().ExtraAI[RecoilStrengthIndex];

            bool shouldDespawn = !Owner.active || Owner.dead || Owner.CCed || Owner.HeldItem.type != ModContent.ItemType<TheBastOffense>();
            if (shouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            switch ((AttackState)AttackType)
            {
                case AttackState.BastBarrage:
                    DoBehavior_BastBarrage(ref bastIncreaseDelay, ref bastCatCount, ref weaponShakeAngle, ref oldRotation, ref recoilStrength);
                    break;

                case AttackState.BIGSHOTS:
                    DoBehavior_BIGSHOTS(ref oldRotation);
                    break;
            }

            UpdateProjectileVariablesAndFunctions();
            UpdatePlayerVariables();
        }

        public void UpdateProjectileVariablesAndFunctions()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

        public void DoBehavior_BastBarrage(ref float bastIncreaseDelay, ref float bastCatCount, ref float weaponShakeAngle, ref float oldRotation, ref float recoilStrength)
        {
            if (AIState == 0f)
            {
                // Blow up after some time.
                if (ChargeTimer >= 1200f)
                {
                    Projectile.Kill();
                    FuckingExplode();
                    return;
                }

                Projectile.rotation = Owner.MountedCenter.AngleTo(Main.MouseWorld);

                int increaseBastInterval = 5;
                int maxBastCatCount = 200;

                // Initialization.
                if (ChargeTimer == 0)
                {
                    bastIncreaseDelay = 60f;
                    Projectile.netUpdate = true;
                }

                // Everytime the charge timer has reached past both the interval and delay it is set back to 0
                // and the delay is decreased by 1, making it charge faster over time.
                if (ChargeTimer >= increaseBastInterval + bastIncreaseDelay && bastCatCount <= maxBastCatCount)
                {
                    PulseRingParticle chargeRing = new(Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f, Vector2.Zero, Color.Gold, 1.25f, 0.01f, new Vector2(0.5f, 1f), Projectile.rotation, 30);
                    chargeRing.SpawnCasParticle();
                    SoundEngine.PlaySound(SoundID.Item149 with { MaxInstances = 0 }, Projectile.Center);

                    // Play a different sound and a spawn a dust circle to indicate it's done charging.
                    if (bastCatCount == maxBastCatCount)
                    {
                        TwilightEgressUtilities.CreateDustCircle(30, Projectile.Center, DustID.Firework_Yellow, 10f, shouldDefyGravity: true);
                        SoundEngine.PlaySound(SoundID.ResearchComplete with { MaxInstances = 0 }, Projectile.Center);
                    }
                    bastIncreaseDelay = (int)MathHelper.Clamp(bastIncreaseDelay - 2f, 5f, 60f);
                    bastCatCount++;
                    ChargeTimer = increaseBastInterval;
                }

                // Shake progressively as the count increases.
                weaponShakeAngle = MathHelper.Lerp(0f, 12f, ChargeTimer / 720f);
                Projectile.rotation = Projectile.rotation + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(weaponShakeAngle)).ToRotation();

                // Spawn small smoke particles along with the shaking.
                if (ChargeTimer > 120f && Main.rand.NextBool(3))
                {
                    Vector2 smokeSpawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f + Main.rand.NextVector2Circular(15f, 15f);
                    Vector2 smokeVelocity = -Vector2.UnitY * Main.rand.NextFloat(3f, 8f);

                    float smokeScale = Main.rand.NextFloat(0.65f, 1f) * MathHelper.Lerp(0f, 1f, ChargeTimer / 720f);
                    float smokeOpacity = Main.rand.NextFloat(0.45f, 0.85f) * MathHelper.Lerp(0f, 1f, ChargeTimer / 720f);
                    int smokeLifespan = Main.rand.Next(30, 60);

                    TimedSmokeParticle smoke = new(smokeSpawnPosition, smokeVelocity, Color.Lerp(Color.DarkGray, Color.Black, 0.75f), Color.Gray, smokeScale, smokeOpacity, smokeLifespan, 0.02f);
                    smoke.SpawnCasParticle();
                }

                if (Owner.active && Owner.channel && Owner.HeldItem.type == ModContent.ItemType<TheBastOffense>())
                    ChargeTimer++;
                else
                {
                    AIState = 1f;
                    ChargeTimer = 0f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 1f)
            {
                recoilStrength = MathHelper.Lerp(1f, 3f, bastCatCount / 200f);

                // Play a fart sound if there were not cats loaded.
                if (ResetAnimationTimer == 0f)
                {
                    if (bastCatCount <= 0)
                        SoundEngine.PlaySound(SoundID.Item16, Projectile.Center);
                    oldRotation = Owner.MountedCenter.AngleTo(Main.MouseWorld);

                    // Buncha particles.
                    int smokeCount = (int)MathHelper.Lerp(3, 45, bastCatCount / 200f);
                    for (int i = 0; i < smokeCount; i++)
                    {
                        Vector2 smokeSpawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f;
                        Vector2 smokeVelocity = Owner.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(60f)) * Main.rand.NextFloat(10f, 25f);

                        float smokeScale = Main.rand.NextFloat(0.65f, 1.25f);
                        float smokeOpacity = Main.rand.NextFloat(0.45f, 0.85f);
                        int smokeLifespan = Main.rand.Next(30, 60);

                        TimedSmokeParticle smoke = new(smokeSpawnPosition, smokeVelocity, Color.Black, Color.Orange, smokeScale, smokeOpacity, smokeLifespan, 0.02f);
                        smoke.SpawnCasParticle();
                    }
                    Projectile.netUpdate = true;
                }

                // Fire the cat armada.
                if (bastCatCount > 0)
                {
                    for (int i = 0; i < bastCatCount; i++)
                    {
                        Vector2 velocity = Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(13f, 19f);
                        Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 50f;
                        Projectile.BetterNewProjectile(spawnPosition, velocity, ModContent.ProjectileType<HomingBastStatue>(), Projectile.originalDamage, Projectile.knockBack, SoundID.Item62, null, Projectile.owner);
                    }

                    Owner.velocity = -Owner.SafeDirectionTo(Main.MouseWorld) * MathHelper.Lerp(2f, 12f, bastCatCount / 200f);

                    ChargeTimer = 0f;
                    bastCatCount = 0f;
                    PulseRingParticle fireRing = new(Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f, Vector2.Zero, Color.Gold, 0.01f, recoilStrength, new Vector2(0.5f, 1f), Projectile.rotation, 60);
                    fireRing.SpawnCasParticle();
                }

                // Recoil animation.
                ResetAnimationTimer++;
                if (ResetAnimationTimer <= 45f)
                {
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, oldRotation + MathHelper.ToRadians(-85f) * Owner.direction * recoilStrength, TwilightEgressUtilities.ExpoEaseOut(ResetAnimationTimer / 25f));
                }

                if (ResetAnimationTimer >= 45f)
                {
                    Projectile.Kill();
                    return;
                }
            }
        }

        public void DoBehavior_BIGSHOTS(ref float oldRotation)
        {
            if (AIState == 0f)
            {
                int chargeTime = 120;
                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.MountedCenter.AngleTo(Main.MouseWorld), 0.2f);
                if (ChargeTimer <= chargeTime)
                {
                    if (ChargeTimer % 10 == 0)
                    {
                        float maxScale = ChargeTimer == chargeTime ? 0.01f : 1.25f;
                        float newScale = ChargeTimer == chargeTime ? 5f : 0.01f;
                        PulseRingParticle fireRing = new(Projectile.Center + Projectile.rotation.ToRotationVector2() * 35f, Vector2.Zero, Color.Gold, maxScale, newScale, new Vector2(0.5f, 1f), Projectile.rotation, 60);
                        fireRing.SpawnCasParticle();
                        SoundEngine.PlaySound(SoundID.Item75, Projectile.Center);
                    }
                }

                if (ChargeTimer >= chargeTime)
                {
                    AIState = 1f;
                    ChargeTimer = 0f;

                    // Apply some recoil to the player.
                    Owner.velocity = -Owner.SafeDirectionTo(Main.MouseWorld) * 15f;

                    Vector2 bastStatueMegaVelocity = Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY) * 10f;
                    Projectile.BetterNewProjectile(Projectile.Center + Projectile.rotation.ToRotationVector2() * 50f, bastStatueMegaVelocity, ModContent.ProjectileType<GiantBastStatue>(), (int)(Projectile.originalDamage * 4f), Projectile.knockBack, SoundID.Item62, null, Projectile.owner);
                }
                ChargeTimer++;
            }

            if (AIState == 1f)
            {
                if (ResetAnimationTimer == 0f)
                {
                    oldRotation = Owner.MountedCenter.AngleTo(Main.MouseWorld);
                    Projectile.netUpdate = true;
                }

                // Recoil animation.
                ResetAnimationTimer++;
                if (ResetAnimationTimer <= 45f)
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, oldRotation + MathHelper.ToRadians(-135f) * Owner.direction, TwilightEgressUtilities.ExpoEaseOut(ResetAnimationTimer / 35f));

                if (ResetAnimationTimer >= 45f)
                    Projectile.Kill();
            }
        }

        public void FuckingExplode()
        {
            ref float bastCatCount = ref Projectile.TwilightEgress().ExtraAI[BastCatCountIndex];

            // Hurt the player in the explosion.       
            Player.HurtInfo hurtInfo = new()
            {
                DamageSource = PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.TwilightEgress.Status.DeathReasons.BastOffenseExplosion")),
                Dodgeable = false,
                Damage = Main.zenithWorld ? 9000 : 150
            };
            Owner.Hurt(hurtInfo);
            Projectile.netUpdate = true;

            // Release the cats that built up.
            for (int i = 0; i < bastCatCount; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(Math.Tau) * Main.rand.NextFloat(10f, 15f);
                Projectile.BetterNewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<HomingBastStatue>(), Projectile.originalDamage, Projectile.knockBack, AssetRegistry.Sounds.KibbyExplosion, null, Projectile.owner);
            }

            // Particle effects.
            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(10f, 15f);
                Color initialColor = Color.Lerp(Color.WhiteSmoke, Color.Orange, Main.rand.NextFloat());
                Color fadeColor = Color.DarkGray;
                float scale = Main.rand.NextFloat(6f, 8f);
                float opacity = Main.rand.NextFloat(0.6f, 1f);
                MediumMistParticle deathSmoke = new(Projectile.Center, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(180, 240), 0.03f);
                deathSmoke.SpawnCasParticle();
            }

            for (int i = 0; i < 15; i++)
            {
                Color fireColor = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0.2f, 0.8f));
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(7f, 15f);
                float scale = Main.rand.NextFloat(5f, 7f);
                HeavySmokeParticle heavySmoke = new(Projectile.Center, velocity, fireColor, Main.rand.Next(120, 150), scale, Main.rand.NextFloat(0.7f, 1.75f), 0.06f, true, 0);
                heavySmoke.SpawnCasParticle();
            }

            PulseRingParticle deathRing = new(Projectile.Center, Vector2.Zero, Color.White, 0.01f, 8f, 75);
            deathRing.SpawnCasParticle();
        }

        public void Reset()
        {
            AIState = 0f;
            ChargeTimer = 0f;
            ResetAnimationTimer = 0f;
            Projectile.timeLeft = 2700;
        }

        public void UpdatePlayerVariables()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            if (AIState == 0f)
                Owner.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float backglowRotation = ref Projectile.TwilightEgress().ExtraAI[BackglowRotationIndex];

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? MathHelper.Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, 0f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            // Draw pulsing backglow effects.
            for (int i = 0; i < 4; i++)
            {
                backglowRotation += MathHelper.TwoPi / 300f;
                float backglowRadius = MathHelper.Lerp(2f, 5f, TwilightEgressUtilities.SineEaseInOut((float)(Main.timeForVisualEffects / 30f)));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(backglowRotation + MathHelper.TwoPi * i / 4) * backglowRadius;

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(texture, backglowDrawPositon, texture.Frame(), Projectile.GetAlpha(Color.LightYellow), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
                Main.spriteBatch.ResetToDefault();
            }

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }
    }
}
