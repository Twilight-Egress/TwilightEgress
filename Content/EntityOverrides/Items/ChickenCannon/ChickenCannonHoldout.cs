﻿using CalamityMod.Projectiles.Ranged;
using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;
using ChickenCannonItem = CalamityMod.Items.Weapons.Ranged.ChickenCannon;

namespace TwilightEgress.Content.EntityOverrides.Items.ChickenCannon
{
    public class ChickenCannonHoldout : ModProjectile
    {
        public static readonly SoundStyle YharonRoar = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonRoar");

        public static readonly SoundStyle YharonRoarShort = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonRoarShort");

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float OldRotation => ref Projectile.ai[1];

        private ref float BackglowRotation => ref Projectile.ai[2];

        private const int ChargeUpTime = 180;

        private const int TimeSpentReloading = 60;

        public override string LocalizationCategory => "Projectiles.ChickenCannon";

        public override string Texture => "CalamityMod/Projectiles/Ranged/ChickenCannonHeld";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            bool isChanneling = Owner.active && Owner.channel && Owner.HeldItem.type == ModContent.ItemType<ChickenCannonItem>();
            if (!isChanneling)
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            DoBehavior();
            UpdatePlayerVariables(Owner);
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

        public void DoBehavior()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true);

            if (Timer <= ChargeUpTime)
            {
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.AngleTo(Main.MouseWorld), 0.2f);
                if (Timer % 60f == 0)
                {
                    float maxScale = Timer == ChargeUpTime ? 0.01f : 1.25f;
                    float newScale = Timer == ChargeUpTime ? 5f : 0.01f;
                    PulseRingParticle chargeUpRing = new(Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f, Vector2.Zero, Color.Orange, maxScale + Main.rand.NextFloat(0.3f), newScale, new Vector2(0.5f, 1f), Projectile.rotation, 30);
                    chargeUpRing.SpawnCasParticle();

                    // Play a different yharon sound at every interval.
                    SoundStyle sound = YharonRoar;
                    if (Timer >= 180f)
                        sound = YharonRoarShort;
                    SoundEngine.PlaySound(sound, Projectile.Center);
                }

                if (Timer == ChargeUpTime)
                {
                    OldRotation = Projectile.AngleTo(Main.MouseWorld);
                    Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 60f;
                    Vector2 velocity = Projectile.SafeDirectionTo(Main.MouseWorld) * 15f;

                    Projectile.BetterNewProjectile(spawnPosition, velocity, ModContent.ProjectileType<ChickenRocket>(), Projectile.damage, Projectile.knockBack, CommonCalamitySounds.LargeWeaponFireSound, null, Projectile.owner);
                }
            }

            // Rotate backwards from recoil and animate the projectile.
            if (Timer >= ChargeUpTime && Timer <= ChargeUpTime + TimeSpentReloading)
            {
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, OldRotation + MathHelper.ToRadians(-65f) * Owner.direction, 0.3f);
                Projectile.UpdateProjectileAnimationFrames(0, 4, 4);
            }

            // Reset.
            if (Timer >= ChargeUpTime + 30f)
                Projectile.Kill();
        }

        public void UpdatePlayerVariables(Player owner)
        {
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            if (Timer <= ChargeUpTime)
                owner.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? MathHelper.Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, 0f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            int individualFrameHeight = texture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, currentYFrame, texture.Width, individualFrameHeight);

            // Draw pulsing backglow effects.
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                BackglowRotation += MathHelper.TwoPi / 300f;
                float backglowRadius = MathHelper.Lerp(2f, 5f, EasingFunctions.SineEaseInOut((float)(Main.timeForVisualEffects / 30f)));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(BackglowRotation + MathHelper.TwoPi * i / 4) * backglowRadius;

                Main.EntitySpriteDraw(texture, backglowDrawPositon, projRec, Projectile.GetAlpha(Color.Orange), rotation, projRec.Size() / 2f, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, projRec, Projectile.GetAlpha(lightColor), rotation, projRec.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }
    }
}
