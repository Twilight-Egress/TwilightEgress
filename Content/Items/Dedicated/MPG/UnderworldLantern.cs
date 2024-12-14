using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Dedicated.MPG
{
    public class UnderworldLantern : ModProjectile, ILocalizedModType
    {
        public enum AttackState
        {
            Idling,
            UndeadSpiritTransformation
        }

        public bool ShouldDealContactDamage = false;

        public bool ShouldDrawUndeadSpirit = false;

        public NPC TargetToChase;

        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];

        public ref float AIState => ref Projectile.ai[1];

        public ref float LocalAIState => ref Projectile.ai[2];

        public const int IdleAngleIndex = 0;

        public const int UndeadSpiritFrameIndex = 1;

        public const int UndeadSpiritFrameCounterIndex = 2;

        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.timeLeft = 9999;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
        }

        public override bool? CanCutTiles() => false;

        public override bool MinionContactDamage() => ShouldDealContactDamage;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ShouldDealContactDamage);
            writer.Write(ShouldDrawUndeadSpirit);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ShouldDealContactDamage = reader.ReadBoolean();
            ShouldDrawUndeadSpirit = reader.ReadBoolean();
        }

        public override void AI()
        {
            if (!CheckActive(Owner))
                return;

            // AI methods.
            switch ((AttackState)AIState)
            {
                case AttackState.Idling:
                    DoBehavior_Idle();
                    break;

                case AttackState.UndeadSpiritTransformation:
                    DoBehavior_UndeadSpiritTransformation();
                    break;
            }

            Timer++;
            Projectile.AdjustProjectileHitboxByScale(24f, 40f);
        }

        public void DoBehavior_Idle()
        {
            int timeBeforeSwitchingAI = 30;
            ref float idleAngle = ref Projectile.TwilightEgress().ExtraAI[IdleAngleIndex];

            // Get a list of all active Underworld Lanterns and sort them according to their 
            // minionPos field. Then, get the proper order of each minion and space them
            // out evenly in a elliptical shape above the specified idle position.
            Vector2 idlePosition = Owner.Top;
            List<Projectile> brotherMinions = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile brotherProj = Main.projectile[i];
                if (brotherProj.active && brotherProj.type == Projectile.type && brotherProj.owner == Projectile.owner)
                {
                    brotherMinions.Add(brotherProj);
                }
            }

            brotherMinions.Sort((x, y) => x.minionPos - y.minionPos);
            int minionCount = brotherMinions.Count;
            if (minionCount > 0)
            {
                int order = brotherMinions.IndexOf(Projectile);
                idleAngle = MathHelper.TwoPi * order / minionCount;
                idleAngle += MathHelper.TwoPi * Main.GlobalTimeWrappedHourly / 8f;
                idlePosition.X += 140f * MathF.Cos(idleAngle);
                idlePosition.Y += -125f - 75f * MathF.Sin(idleAngle) + Owner.gfxOffY;
            }

            Projectile.Center = Vector2.Lerp(Projectile.Center, idlePosition, 0.225f);
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);
            Projectile.scale = MathHelper.Clamp(Projectile.scale - 0.1f, 1f, 1.75f);
            Projectile.rotation *= 0.9f;
            ShouldDrawUndeadSpirit = false;

            // Search for any nearby targets.
            NPC target = Projectile.GetNearestMinionTarget(Owner, 2500f, 300f, out bool foundTarget);
            if (foundTarget)
            {
                TargetToChase = target;
                Projectile.TwilightEgress().SpecificNPCTypeToCheckOnHit = target.type;
            }

            if (Timer >= timeBeforeSwitchingAI && !foundTarget)
                Timer = timeBeforeSwitchingAI - 1;

            // Switch to attack mode after some time.
            if (Timer >= timeBeforeSwitchingAI && foundTarget)
                SwitchAIStates(1);
        }

        public void DoBehavior_UndeadSpiritTransformation()
        {
            int floatTime = 45;
            int chaseTime = 720;
            int returnTime = 30;
            int cooldownTime = 15;
            float maxChaseSpeed = 75f;
            float maxTurnResistance = 15f;

            // Immediately move into the return phase if there is nothing to target.
            if ((TargetToChase is null || !TargetToChase.active) && LocalAIState != 2f)
            {
                LocalAIState = 2f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }

            // Initially float upwards and fade out.
            if (LocalAIState == 0f)
            {
                if (Timer <= floatTime)
                {
                    // Float upwards.
                    if (Timer == 1f)
                        Projectile.velocity.Y -= 12f;

                    Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity - 0.05f, 0f, 1f);

                    if (Timer == floatTime)
                    {
                        LocalAIState = 1f;
                        Timer = 0f;
                        Projectile.netUpdate = true;
                    }
                }
            }

            // Emerge from the player's body and dash towards the enemy.
            if (LocalAIState == 1f)
            {
                if (Timer <= chaseTime)
                {
                    if (Timer == 0f)
                        Projectile.Center = Owner.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 60f;

                    // Fade back in and chase the enemy.
                    Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);
                    Projectile.scale = MathHelper.Clamp(Projectile.scale + 0.1f, 0f, 1.75f);
                    Projectile.SimpleMove(TargetToChase.Center, maxChaseSpeed, maxTurnResistance);
                    Projectile.rotation = Projectile.velocity.X * 0.03f;

                    // Adjust the hitbox to accommodate for the new sprite.
                    Projectile.AdjustProjectileHitboxByScale(42f, 50f);

                    // Switch if time is up or the Lantern has hit the target.
                    if (Timer == chaseTime || !TargetToChase.active || Projectile.TwilightEgress().HasStruckSpecificNPC)
                    {
                        LocalAIState = 2f;
                        Timer = 0f;
                        Projectile.netUpdate = true;
                    }
                }

                ShouldDrawUndeadSpirit = true;
                ShouldDealContactDamage = true;
            }

            // Target was hit, the minion lost its target or chase time was up; retreat to the player's center and reset.
            if (LocalAIState == 2f)
            {
                // Ease back to the player and fade out.
                if (Timer <= returnTime)
                {
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, EasingFunctions.SineEaseInOut(Timer / returnTime));
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, EasingFunctions.SineEaseInOut(Timer / returnTime));
                    Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, EasingFunctions.SineEaseInOut(Timer / returnTime));
                }

                // Head back to usual idle AI.
                if (Timer >= returnTime + cooldownTime)
                {
                    SwitchAIStates(0);
                    // Velocity is reset here in order to not screw up the alignment of each lantern for the idle AI code.
                    Projectile.velocity *= 0f;
                }

                // Turn off contact damage.
                ShouldDealContactDamage = false;
            }

            Projectile.spriteDirection = TargetToChase is not null ? (TargetToChase.Center.X < Projectile.Center.X).ToDirectionInt() : Projectile.direction;
            AnimateSpirit();
        }

        public void SwitchAIStates(int aiState)
        {
            AIState = aiState;
            Timer = 0f;
            LocalAIState = 0f;
            ShouldDealContactDamage = false;
            ShouldDrawUndeadSpirit = false;
            Projectile.netUpdate = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == Projectile.TwilightEgress().SpecificNPCTypeToCheckOnHit.Value)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.LouderSwingWoosh);

                Vector2 stretchFactor = new(1f, 3f);
                Color slashColor = Color.Lerp(Color.Cyan, Color.LightSkyBlue, Main.rand.NextFloat());
                Color bloomColor = Color.Lerp(slashColor, Color.Transparent, 0.15f);
                new SwordSlashParticle(target.Center, slashColor, bloomColor, Main.rand.NextFloat(MathF.Tau), stretchFactor, 2f, 20).Spawn();
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 8f);
                Color color = Color.Lerp(Color.Cyan, Color.CornflowerBlue, Main.rand.NextFloat());
                float scale = Main.rand.NextFloat(0.25f, 1.25f);
                HeavySmokeParticle deathSmoke = new(Projectile.Center, velocity, color, Main.rand.Next(75, 140), scale, Main.rand.NextFloat(0.35f, 1f), 0.06f, true, 0);
                deathSmoke.SpawnCasParticle();
            }
        }

        public void AnimateSpirit()
        {
            ref float undeadSpiritFrame = ref Projectile.TwilightEgress().ExtraAI[UndeadSpiritFrameIndex];
            ref float undeadSpiritFrameCounter = ref Projectile.TwilightEgress().ExtraAI[UndeadSpiritFrameCounterIndex];

            undeadSpiritFrameCounter++;
            if (undeadSpiritFrameCounter >= 4f)
            {
                undeadSpiritFrame++;
                if (undeadSpiritFrame >= 4f)
                    undeadSpiritFrame = 0f;
                undeadSpiritFrameCounter = 0f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float undeadSpiritFrame = ref Projectile.TwilightEgress().ExtraAI[UndeadSpiritFrameIndex];
            ref float undeadSpiritFrameCounter = ref Projectile.TwilightEgress().ExtraAI[UndeadSpiritFrameCounterIndex];

            Texture2D baseTexture = TextureAssets.Projectile[Type].Value;
            Texture2D spiritTexture = TextureAssets.Npc[NPCID.PirateGhost].Value;

            SpriteEffects spiritEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            int frameHeight = baseTexture.Height / Main.projFrames[Type];
            int frameY = frameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, frameY, baseTexture.Width, frameHeight);

            Rectangle spiritRec = spiritTexture.Frame(1, 4, 0, (int)(undeadSpiritFrame % 4));

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Cyan * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);

                if (ShouldDrawUndeadSpirit)
                    Main.EntitySpriteDraw(spiritTexture, drawPosition, spiritRec, Projectile.GetAlpha(trailColor), Projectile.rotation, spiritRec.Size() / 2f, Projectile.scale, spiritEffects, 0);
                else
                    Main.EntitySpriteDraw(baseTexture, drawPosition, projRec, Projectile.GetAlpha(trailColor), Projectile.rotation, projRec.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ResetToDefault();

            return false;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.HasBuff(ModContent.BuffType<UnderworldLanternBuff>()))
            {
                Projectile.timeLeft = 2;
                return true;
            }
            else
            {
                Projectile.Kill();
                return false;
            }
        }
    }
}
