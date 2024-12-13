using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Core;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Accessories.Elementals.TwinGeminiGenies
{
    public class TelekineticallyControlledWeapon : ModProjectile, ILocalizedModType
    {
        private enum WeaponTypes
        {
            ForsakenSaber,
            SpiritFlame,
            SpearOfPaleolith
        }

        private Player Player => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private ref float WeaponState => ref Projectile.ai[2];

        private const int IdleAngleIndex = 0;

        private const int RotationSpeedIndex = 1;

        private const int RotationDirectionIndex = 2;

        private const int ForsakenSaberPositionRotationIndex = 3;

        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => AssetRegistry.ExtraTexturesPath + "EmptyPixel";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ref float rotationSpeed = ref Projectile.TwilightEgress().ExtraAI[RotationSpeedIndex];
            ref float rotationDirection = ref Projectile.TwilightEgress().ExtraAI[RotationDirectionIndex];

            rotationSpeed = Main.rand.NextFloat(100f, 360f);
            rotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override void AI()
        {
            Projectile Owner = GeminiGeniePsychic.Myself;
            Projectile.GetNearestTarget(1500f, 500f, out bool foundTarget, out NPC closestTarget);

            ref float rotationSpeed = ref Projectile.TwilightEgress().ExtraAI[RotationSpeedIndex];
            ref float rotationDirection = ref Projectile.TwilightEgress().ExtraAI[RotationDirectionIndex];

            if (Owner is null)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft = 2;
            }

            if (AIState == 0f)
            {
                Projectile.velocity *= 0.9f;
                GetIdlePosition(Owner, out Vector2 idlePosition);
                Projectile.Center = Vector2.Lerp(Projectile.Center, idlePosition, 0.3f);
                Projectile.rotation += MathHelper.Pi / rotationSpeed * rotationDirection;

                if (closestTarget is not null)
                {
                    AIState = 1f;
                    Projectile.netUpdate = true;
                }
            }

            if (AIState == 1f)
            {
                if (!foundTarget)
                {
                    Timer = 0f;
                    AIState = 0f;
                    Projectile.netUpdate = true;
                    return;
                }

                switch ((WeaponTypes)WeaponState)
                {
                    case WeaponTypes.ForsakenSaber:
                        DoBehavior_ForsakenSaber(closestTarget);
                        break;

                    case WeaponTypes.SpiritFlame:
                        DoBehavior_SpiritFlame(closestTarget);
                        break;
                }

                Timer++;
            }

            Projectile.spriteDirection = Projectile.direction;
        }

        public void DoBehavior_ForsakenSaber(NPC closestTarget)
        {
            int lineUpTime = 30;
            int dashTime = 25;
            int cooldownTime = 10;
            float extraAngle = Projectile.direction < 0 ? MathHelper.PiOver2 : 0f;

            ref float forsakenSaberPositionRotation = ref Projectile.TwilightEgress().ExtraAI[ForsakenSaberPositionRotationIndex];

            if (Timer == 1f)
            {
                forsakenSaberPositionRotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.netUpdate = true;
            }

            if (Timer <= lineUpTime)
            {
                Vector2 lineUpPosition = closestTarget.Center - Vector2.UnitY.RotatedBy(forsakenSaberPositionRotation) * 300f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, lineUpPosition, 0.3f);

                // Line up quickly around the target, then dash towards them.
                if (Timer == lineUpTime)
                {
                    Projectile.velocity = Projectile.SafeDirectionTo(closestTarget.Center) * 45f;
                    SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound with { PitchVariance = 1f }, Projectile.Center);
                }

                // Rotate towards the target.
                Projectile.rotation = Projectile.AngleTo(closestTarget.Center) + MathHelper.PiOver4 + extraAngle;
            }

            if (Timer >= lineUpTime)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + extraAngle;

            // Slowdown and reset shortly after.
            if (Timer >= lineUpTime + dashTime)
            {
                Projectile.velocity *= 0.9f;
                if (Timer >= lineUpTime + dashTime + cooldownTime)
                {
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }
            }
        }

        public void DoBehavior_SpiritFlame(NPC closestTarget)
        {
            int spiritFlameAttackInterval = 15;
            if (Timer % spiritFlameAttackInterval == 0)
            {
                // Summon Spirit Flames that home in on enemies.
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(100f, 100f);
                Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<SpiritFlame>(), (int)(Projectile.damage * 0.65f), Projectile.knockBack);
            }

            // Stick to a radius around the target.
            Vector2 movePosition = closestTarget.Center + Projectile.DirectionFrom(closestTarget.Center) * 175f;
            Projectile.SimpleMove(movePosition, 30f, 45f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

        public void GetIdlePosition(Projectile owner, out Vector2 idlePosition)
        {
            ref float idleAngle = ref Projectile.TwilightEgress().ExtraAI[IdleAngleIndex];

            // Idle movement.
            idlePosition = owner.Top;
            List<Projectile> brotherMinions = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile brotherProj = Main.projectile[i];
                if (brotherProj.active && brotherProj.type == Projectile.type && brotherProj.owner == Projectile.owner)
                {
                    brotherMinions.Add(brotherProj);
                }
            }

            int minionCount = brotherMinions.Count;
            if (minionCount > 0)
            {
                int order = brotherMinions.IndexOf(Projectile);
                idleAngle = MathHelper.TwoPi * order / minionCount;
                idleAngle += MathHelper.TwoPi * Main.GlobalTimeWrappedHourly / 5f;
                idlePosition.X += 140f * MathF.Cos(idleAngle);
                idlePosition.Y += -70f - 40f * MathF.Sin(idleAngle) + owner.gfxOffY;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D baseTexture = TextureAssets.Item[ItemID.SpiritFlame].Value;
            if (WeaponState == (float)WeaponTypes.ForsakenSaber)
                baseTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ForsakenSaber").Value;

            int individualFrameHeight = baseTexture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, currentYFrame, baseTexture.Width, individualFrameHeight);

            // Draw the afterimagee trail.
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                SpriteEffects effects = Projectile.oldSpriteDirection[i] < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Lerp(Color.Magenta, Color.White, 0.6f) * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.oldRot[i], baseTexture.Size() / 2f, Projectile.scale * 1.275f, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main texture.
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, spriteEffects, texture: baseTexture);
            return false;
        }
    }
}
