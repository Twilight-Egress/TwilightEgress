using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Items.Accessories.Elementals.TwinGeminiGenies
{
    public class GeminiGenieSandy : ModProjectile, ILocalizedModType
    {
        private enum AIStates
        {
            Idle,
            Attacking
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private ref float AIState => ref Projectile.ai[1];

        private ref float AttackState => ref Projectile.ai[2];

        private const int AttackCounterIndex = 0;

        private const int SandTwisterScaleIndex = 1;

        private const int SandTwisterOpacityIndex = 2;

        private const int SandTwisterFrameCounterIndex = 3;

        private const int SandTwisterFrameIndex = 4;

        private static Projectile myself;

        public static Projectile Myself
        {
            get
            {
                if (myself is not null && !myself.active)
                    return null;
                return myself;
            }

            private set => myself = value;
        }

        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 114;
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

        public override bool? CanDamage() => !Owner.GetModPlayer<BuffHandler>().GeminiGeniesVanity;

        public bool CheckActive()
        {
            BuffHandler buffHandler = Owner.GetModPlayer<BuffHandler>();

            if (buffHandler.GeminiGenies || buffHandler.GeminiGeniesVanity)
            {
                Projectile.timeLeft = 2;
                return true;
            }

            if (Owner.dead || !Owner.active)
            {
                buffHandler.GeminiGenies = false;
                Projectile.Kill();
                return false;
            }

            return false;
        }

        public override void AI()
        {
            if (!CheckActive())
                return;

            ref float attackCounter = ref Projectile.TwilightEgress().ExtraAI[AttackCounterIndex];

            // Set the global NPC instance.
            Myself = Projectile;

            // Search for nearby targets.
            NPC target = Projectile.GetNearestMinionTarget(Owner, 1750f, 500f, out bool foundTarget);

            // If the player is using the acceessory as vanity, make sure to always set this to false so no attacks are done.
            if (Owner.GetModPlayer<BuffHandler>().GeminiGeniesVanity)
                foundTarget = false;

            if (target == null && (AIStates)AttackState == AIStates.Attacking)
            {
                AttackState = (int)AIStates.Idle;
            }

            switch ((AIStates)AttackState)
            {
                case AIStates.Idle:
                    DoBehavior_Idle(foundTarget);
                    break;

                case AIStates.Attacking:
                    DoBehavior_Attacking(foundTarget, target.Center, ref attackCounter);
                    break;
            }

            Timer++;
            Projectile.rotation = Projectile.velocity.X * 0.03f;
        }

        public void DoBehavior_Idle(bool foundTarget)
        {
            ref float sandTwisterScale = ref Projectile.TwilightEgress().ExtraAI[SandTwisterScaleIndex];
            ref float sandTwisterOpacity = ref Projectile.TwilightEgress().ExtraAI[SandTwisterOpacityIndex];

            ResetTwisterVisuals();
            Projectile.AdjustProjectileHitboxByScale(54f, 114f);

            Vector2 idlePosition = Owner.Center - Vector2.UnitX * 175f;
            idlePosition.Y += MathHelper.Lerp(-15f, 15f, TwilightEgressUtilities.SineEaseInOut(Timer / 240f));

            float speed = 25f;
            Vector2 idealVelocity = idlePosition - Projectile.Center;
            float distance = idealVelocity.Length();

            if (distance > 2000f)
            {
                // Teleport when the player is too far away.
                Projectile.Center = Owner.Center;
                TwilightEgressUtilities.CreateRandomizedDustExplosion(36, Projectile.Center, DustID.GoldCoin, 10f);
            }

            if (distance > 70f)
            {
                idealVelocity.Normalize();
                idealVelocity *= speed;
                Projectile.velocity = (Projectile.velocity * 40f + idealVelocity) / 41f;
            }

            if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X = -0.18f;
                Projectile.velocity.Y = -0.08f;
            }

            if (foundTarget)
            {
                AttackState = 1f;
                Timer = 0f;
                AIState = 0f;
                Projectile.netUpdate = true;
            }

            Projectile.spriteDirection = -Projectile.direction;
        }

        public void DoBehavior_Attacking(bool foundTarget, Vector2 targetCenter, ref float attackCounter)
        {
            ref float sandTwisterScale = ref Projectile.TwilightEgress().ExtraAI[SandTwisterScaleIndex];
            ref float sandTwisterOpacity = ref Projectile.TwilightEgress().ExtraAI[SandTwisterOpacityIndex];
            ref float sandTwisterFrameCounter = ref Projectile.TwilightEgress().ExtraAI[SandTwisterFrameCounterIndex];
            ref float sandTwisterFrame = ref Projectile.TwilightEgress().ExtraAI[SandTwisterFrameIndex];

            // Immediately go back if there are no enemies.
            if (!foundTarget)
            {
                AttackState = 0f;
                AIState = 0f;
                Timer = 0f;
                Projectile.netUpdate = true;
                return;
            }

            if (AIState == 0f)
            {
                // Slow down and pick a random attack.
                Projectile.velocity *= 0.9f;

                if (Timer >= 45f)
                {
                    Timer = 0f;
                    attackCounter = 0f;
                    AIState = Utils.SelectRandom(Main.rand, 1f, 2f);
                    Projectile.netUpdate = true;
                }

                ResetTwisterVisuals();
                Projectile.AdjustProjectileHitboxByScale(54f, 114f);
            }

            // Sandnado Magics.
            if (AIState == 1f)
            {
                int sandnadoSummonInterval = 45;
                int maxSandnadoAttackTime = 600;

                // Stick to a radius around the target.
                Vector2 movePosition = targetCenter + Projectile.DirectionFrom(targetCenter) * 325f;
                Projectile.SimpleMove(movePosition, 30f, 45f);

                if (Timer <= maxSandnadoAttackTime && Timer % sandnadoSummonInterval == 0)
                {
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(200f, 200f);
                    Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<Sandnado>(), (int)(Projectile.damage * 0.65f), Projectile.knockBack, SoundID.Item60, null, Projectile.owner, ai1: Utils.SelectRandom(Main.rand, 0f, 1f));
                }

                if (Timer >= maxSandnadoAttackTime)
                {
                    // Rest if too many attacks are done at once.
                    attackCounter++;
                    if (attackCounter >= 4)
                        AIState = 0f;
                    else
                        AIState = 2f;

                    Timer = 0f;
                    Projectile.netUpdate = true;
                }

                Projectile.AdjustProjectileHitboxByScale(54f, 114f);
            }

            // Sand Twister Rush.
            if (AIState == 2f)
            {
                int rushTime = 360;
                int cooldownTime = 120;

                if (Timer == 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                    TwilightEgressUtilities.CreateDustCircle(36, Projectile.Center, DustID.GoldCoin, 10f);
                }

                if (Timer <= rushTime)
                {
                    // Speed towards the target, though quite sloppily.
                    Projectile.SimpleMove(targetCenter, 45f, 85f);
                    if (Timer <= 30f)
                    {
                        sandTwisterOpacity = MathHelper.Lerp(sandTwisterOpacity, 1f, TwilightEgressUtilities.SineEaseInOut(Timer / 30f));
                        sandTwisterScale = MathHelper.Lerp(5f, 2.25f, TwilightEgressUtilities.SineEaseInOut(Timer / 30f));
                        Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, TwilightEgressUtilities.SineEaseInOut(Timer / 30f));
                    }
                }

                if (Timer >= rushTime && Timer <= rushTime + cooldownTime)
                {
                    Projectile.velocity *= 0.9f;
                    if (Timer >= rushTime + cooldownTime)
                    {
                        attackCounter++;
                        if (attackCounter >= 4)
                            AIState = 0f;
                        else
                            AIState = 1f;

                        Timer = 0f;
                        Projectile.netUpdate = true;
                    }

                    ResetTwisterVisuals();
                }

                // Animate the twister.
                AnimateTwister();
                // Adjust the hitbox.
                Projectile.AdjustProjectileHitboxByScale(100f, 114f);
            }

            Projectile.spriteDirection = (targetCenter.X < Projectile.Center.X).ToDirectionInt();
        }

        public void ResetTwisterVisuals()
        {
            ref float sandTwisterScale = ref Projectile.TwilightEgress().ExtraAI[SandTwisterScaleIndex];
            ref float sandTwisterOpacity = ref Projectile.TwilightEgress().ExtraAI[SandTwisterOpacityIndex];

            sandTwisterOpacity = MathHelper.Clamp(sandTwisterOpacity - 0.05f, 0f, 1f);
            sandTwisterScale = MathHelper.Clamp(sandTwisterScale + 0.05f, 0f, 5f);
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.05f, 0f, 1f);
        }

        public void AnimateTwister()
        {
            ref float sandTwisterFrameCounter = ref Projectile.TwilightEgress().ExtraAI[SandTwisterFrameCounterIndex];
            ref float sandTwisterFrame = ref Projectile.TwilightEgress().ExtraAI[SandTwisterFrameIndex];

            sandTwisterFrameCounter++;
            if (sandTwisterFrameCounter >= 2f)
            {
                sandTwisterFrameCounter = 0f;
                if (++sandTwisterFrame >= 8)
                {
                    sandTwisterFrame = 0f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float sandTwisterOpacity = ref Projectile.TwilightEgress().ExtraAI[SandTwisterOpacityIndex];

            DrawDuna();
            if (sandTwisterOpacity > 0f)
                DrawSandTwister();
            return false;
        }

        public void DrawSandTwister()
        {
            ref float sandTwisterScale = ref Projectile.TwilightEgress().ExtraAI[SandTwisterScaleIndex];
            ref float sandTwisterOpacity = ref Projectile.TwilightEgress().ExtraAI[SandTwisterOpacityIndex];
            ref float sandTwisterFrameCounter = ref Projectile.TwilightEgress().ExtraAI[SandTwisterFrameCounterIndex];
            ref float sandTwisterFrame = ref Projectile.TwilightEgress().ExtraAI[SandTwisterFrameIndex];

            Texture2D sandTwister = TextureAssets.Projectile[ProjectileID.WeatherPainShot].Value;

            Rectangle rec = sandTwister.Frame(1, 8, 0, (int)sandTwisterFrame % 8);
            Vector2 origin = rec.Size() / 2f;

            // Draw the afterimage trail.
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                SpriteEffects effects = Projectile.oldSpriteDirection[i] > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 trailDrawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Lerp(Color.Gold, Color.White, 0.4f) * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(sandTwister, trailDrawPosition, rec, trailColor * sandTwisterOpacity, Projectile.oldRot[i], origin, sandTwisterScale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main texture.
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(sandTwister, drawPosition, rec, Color.Gold * sandTwisterOpacity, Projectile.rotation, origin, sandTwisterScale, SpriteEffects.None, 0);
        }

        public void DrawDuna()
        {
            Texture2D baseTexture = TextureAssets.Projectile[Type].Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture + "Glow").Value;

            int individualFrameHeight = baseTexture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle projRec = new Rectangle(0, currentYFrame, baseTexture.Width, individualFrameHeight);

            // Draw the afterimagee trail.
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Color trailColor = Color.Gold * 0.75f * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.EntitySpriteDraw(glowTexture, drawPosition, null, Projectile.GetAlpha(trailColor), Projectile.oldRot[i], glowTexture.Size() / 2f, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main texture.
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, spriteEffects, animated: true);
        }
    }
}
