using CalamityMod;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;
using TwilightEgress.Core.Globals.GlobalNPCs;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Manaphages
{
    public class Manaphage : ModNPC
    {
        #region Enums
        public enum ManaphageBehavior
        {
            Idle_JellyfishPropulsion,
            Idle_LazeAround,
            Fleeing,
            Latching,
            Attacking,
        }

        public enum ManaphageAnimation
        {
            Idle,
            Inject,
            Attack,
            LookRight,
            LookLeft,
            Suck,
        }
        #endregion

        #region Fields and Properties
        public const int JellyfishMovementIntervalIndex = 0;

        public const int LazeMovementIntervalIndex = 1;

        public const int IdleMovementDirectionIndex = 2;

        public const int AdditionalAggroRangeIndex = 3;

        public const int AggroRangeTimerIndex = 4;

        public const int ManaSuckTimerIndex = 5;

        public const int JellyfishMovementAngleIndex = 6;

        public const int SpriteStretchXIndex = 7;

        public const int SpriteStretchYIndex = 8;

        public const int JellyfishPropulsionCountIndex = 9;

        public const int MaxPropulsionsIndex = 10;

        public const int FrameSpeedIndex = 11;

        public const int ManaTankShaderTimeIndex = 12;

        public const int InitialRotationIndex = 13;

        public const float MaximumPlayerSearchDistance = 1200f;

        public const float MaximumNPCSearchDistance = 1200f;

        // Manaphages wait 12 seconds to search for asteroids if they are over 50% mana,
        // and 7 seconds to search for asteroids if they are under 50%
        public const float MaxManaSuckTimerOverFifty = 720f;

        public const float MaxManaSuckTimerUnderFifty = 420f;

        public const float DefaultAggroRange = 150f;

        public const float AggroRangeWhileLatching = 50f;

        public float CurrentManaCapacity;

        public bool ShouldTargetPlayers;

        public bool ShouldTargetNPCs;

        public bool FoundValidRotationAngle;

        public int FrameX;

        public int FrameY;

        public NPC AsteroidToSucc;

        public float ManaRatio => CurrentManaCapacity / MaximumManaCapacity;

        public float LifeRatio => NPC.life / (float)NPC.lifeMax;

        public Player NearestPlayer => Main.player[NPC.target];

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];

        public ref float LocalAIState => ref NPC.ai[2];

        public virtual float MaximumManaCapacity => 100f;

        #endregion

        #region Overrides
        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.UsesNewTargetting[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 34;
            NPC.damage = 25;
            NPC.defense = 3;
            NPC.lifeMax = 90;
            NPC.knockBackResist = 0.8f;
            NPC.value = 9f;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath25;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];
            ref float manaTankShaderTime = ref NPC.TwilightEgress().ExtraAI[ManaTankShaderTimeIndex];
            ref float jellyfishMovementAngle = ref NPC.TwilightEgress().ExtraAI[JellyfishMovementAngleIndex];

            AIState = (float)Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround);
            CurrentManaCapacity = Main.rand.NextBool(25) ? Main.rand.NextFloat(75f, 100f) : Main.rand.NextFloat(60f, 15f);
            spriteStretchX = 1f;
            spriteStretchY = 1f;
            manaTankShaderTime = Main.rand.NextFloat(0.25f, 0.75f) * Main.rand.NextBool().ToDirectionInt();
            jellyfishMovementAngle = Main.rand.NextFloat(MathHelper.TwoPi);
            NPC.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(CurrentManaCapacity);
            writer.Write(ShouldTargetPlayers);
            writer.Write(ShouldTargetNPCs);
            writer.Write(FoundValidRotationAngle);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            CurrentManaCapacity = reader.ReadSingle();
            ShouldTargetPlayers = reader.ReadBoolean();
            ShouldTargetNPCs = reader.ReadBoolean();
            FoundValidRotationAngle = reader.ReadBoolean();
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) => OnHitEffects(hit);

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) => OnHitEffects(hit);
        #endregion

        public override void AI()
        {
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];

            NPC.AdvancedNPCTargeting(true, MaximumPlayerSearchDistance, ShouldTargetNPCs, MaximumNPCSearchDistance, 
                ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>());
            NPCAimedTarget target = NPC.GetTargetData();

            // Don't bother targetting any asteroids at 60% and above mana capacity.
            if (ManaRatio > 0.6f)
                ShouldTargetNPCs = false;
            else
                // If the Manaphage is under 50% of it's maximum mana capacity, start detecting asteroids
                // only if the Manaphage isn't currently fleeing from anything.
                ShouldTargetNPCs = AIState != (float)ManaphageBehavior.Fleeing;

            switch ((ManaphageBehavior)AIState)
            {
                case ManaphageBehavior.Idle_JellyfishPropulsion:
                    DoBehavior_JellyfishPropulsionIdle(target); 
                    break;

                case ManaphageBehavior.Idle_LazeAround:
                    DoBehavior_LazeAroundIdle(target);
                    break;

                case ManaphageBehavior.Fleeing:
                    DoBehavior_Fleeing(target);
                    break;

                case ManaphageBehavior.Latching:
                    DoBehavior_Latching(target);
                    break;

                case ManaphageBehavior.Attacking:
                    DoBehavior_Attacking(target);
                    break;
            }

            float tankLightLevel = MathHelper.Lerp(0.3f, 1.25f, ManaRatio);
            Vector2 tankPosition = NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 31f * spriteStretchY;
            Lighting.AddLight(tankPosition, Color.Cyan.ToVector3() * tankLightLevel);

            CurrentManaCapacity = MathHelper.Clamp(CurrentManaCapacity, 0f, MaximumManaCapacity);
            ManageExtraTimers();
            Timer++;
        }

        #region AI Methods
        public void DoBehavior_JellyfishPropulsionIdle(NPCAimedTarget target)
        {
            ref float jellyfishMovementInterval = ref NPC.TwilightEgress().ExtraAI[JellyfishMovementIntervalIndex];
            ref float jellyfishMovementAngle = ref NPC.TwilightEgress().ExtraAI[JellyfishMovementAngleIndex];
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];
            ref float jellyfishPropulsionCount = ref NPC.TwilightEgress().ExtraAI[JellyfishPropulsionCountIndex];
            ref float maxPropulsions = ref NPC.TwilightEgress().ExtraAI[MaxPropulsionsIndex];
            ref float frameSpeed = ref NPC.TwilightEgress().ExtraAI[FrameSpeedIndex];

            float propulsionSpeed = Main.rand.NextFloat(5f, 7f);
            CheckForTurnAround(out bool turnAround);

            if (LocalAIState == 0f)
            {
                jellyfishMovementInterval = Utils.SelectRandom(Main.rand, 60, 80, 100, 120);
                maxPropulsions = Main.rand.NextFloat(10f, 25f);
                LocalAIState = 1f;
                Timer = 0f;
                NPC.netUpdate = true;
            }

            if (LocalAIState == 1f)
            {                
                if (Timer <= jellyfishMovementInterval)
                {
                    // Squash the sprite slightly before the propulsion movement to give a
                    // more cartoony, jellyfish-like feeling to the movement.
                    float stretchInterpolant = Utils.GetLerpValue(0f, 1f, Timer / jellyfishMovementInterval, true);
                    spriteStretchX = MathHelper.Lerp(spriteStretchX, 1.25f, EasingFunctions.SineEaseInOut(stretchInterpolant));
                    spriteStretchY = MathHelper.Lerp(spriteStretchY, 0.75f, EasingFunctions.SineEaseInOut(stretchInterpolant));

                    int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 1f, stretchInterpolant));
                    UpdateAnimationFrames(default, 0f, frameY);

                    // Pick a random angle to move towards before propelling forward.
                    if (!FoundValidRotationAngle)
                    {
                        // Set a random movement angle initially.
                        if (Timer == 1 && !turnAround)
                            jellyfishMovementAngle = Main.rand.NextFloat(MathF.Tau);

                        // Stop searching for valid rotation angles once the Manaphage no longer needs to turn around.
                        if (!turnAround)
                        {
                            FoundValidRotationAngle = true;
                            NPC.netUpdate = true;
                        }

                        // Keep rotating to find a valid angle to rotate towards.
                        Vector2 centerAhead = NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 128f;
                        jellyfishMovementAngle += -centerAhead.ToRotation() * 6f;
                    }
                }

                // Move forward every few seconds.
                if (Timer == jellyfishMovementInterval)
                {
                    Vector2 velocity = Vector2.One.RotatedBy(jellyfishMovementAngle) * propulsionSpeed;
                    NPC.velocity = velocity;

                    UpdateAnimationFrames(default, 0f, 2);

                    // Spawn some lil' visual particles everytime it ejects.
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                        Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.BlueFairy, dustVelocity * 5f);
                        dust.noGravity = true;
                    }

                    PulseRingParticle propulsionRing = new(NPC.Center - NPC.SafeDirectionTo(NPC.Center) * 60f, NPC.SafeDirectionTo(NPC.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), NPC.velocity.ToRotation(), 45);
                    propulsionRing.SpawnCasParticle();

                    // Unstretch the sprite.
                    spriteStretchX = 0.8f;
                    spriteStretchY = 1.25f;

                    jellyfishPropulsionCount++;
                }

                if (Timer >= jellyfishMovementInterval + 30)
                {
                    float animationInterpolant = Utils.GetLerpValue(0f, 1f, (Timer - jellyfishMovementInterval + 45) / jellyfishMovementInterval + 30, true);
                    int frameY = (int)Math.Floor(MathHelper.Lerp(1f, 4f, EasingFunctions.SineEaseIn(animationInterpolant)));
                    UpdateAnimationFrames(default, 0f, frameY);
                }

                if (Timer >= jellyfishMovementInterval + 45)
                {
                    Timer = 0f;
                    FoundValidRotationAngle = false;
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 0.96f;
                Vector2 futureVelocity = Vector2.One.RotatedBy(jellyfishMovementAngle);
                NPC.rotation = NPC.rotation.AngleLerp(futureVelocity.ToRotation() + 1.57f, Timer / (float)jellyfishMovementInterval);

                // Randomly switch to the other idle AI state.
                if (jellyfishPropulsionCount >= maxPropulsions && Main.rand.NextBool(2))
                    SwitchBehaviorState(ManaphageBehavior.Idle_LazeAround);
            }

            SwitchBehavior_Attacking(target);
            SwitchBehavior_Latching(target);
            SwitchBehavior_Fleeing(target);
        }

        public void DoBehavior_LazeAroundIdle(NPCAimedTarget target)
        {
            ref float lazeMovementInterval = ref NPC.TwilightEgress().ExtraAI[LazeMovementIntervalIndex];
            ref float idleMovementDirection = ref NPC.TwilightEgress().ExtraAI[IdleMovementDirectionIndex];
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];

            int idleSwitchInterval = 1800;
            Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);

            if (Timer is 0)
            {
                lazeMovementInterval = Main.rand.Next(360, 720);
                idleMovementDirection = Main.rand.NextBool().ToDirectionInt();
                NPC.velocity += velocity;
            }

            CheckForTurnAround(out bool turnAround);
            if (turnAround)
            {
                Vector2 circularArea = NPC.Center + NPC.velocity.RotatedBy(MathHelper.TwoPi);
                Vector2 turnAroundVelocity = circularArea.Y >= Main.maxTilesY + 750f ? Vector2.UnitY * -0.15f : circularArea.Y < Main.maxTilesY * 0.34f ?  Vector2.UnitY * 0.15f : NPC.velocity;

                float distanceFromTileCollisionLeft = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                float distanceFromTileCollisionRight = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                int directionToMove = (distanceFromTileCollisionLeft > distanceFromTileCollisionRight).ToDirectionInt();
                if (distanceFromTileCollisionLeft <= 150 || distanceFromTileCollisionRight <= 150f)
                    turnAroundVelocity = Vector2.One.RotatedBy(MathHelper.PiOver2 * directionToMove) * 0.15f;

                NPC.velocity = turnAroundVelocity;
            }
            
            if (Timer % lazeMovementInterval == 0 && !turnAround)
                NPC.velocity += velocity;

            if (NPC.velocity.Length() > 0.13f)
                NPC.velocity *= 0.98f;

            NPC.rotation *= 0.98f;

            // Randomly switch to the other idle AI state.
            if (Timer >= idleSwitchInterval && Main.rand.NextBool(2))
                SwitchBehaviorState(ManaphageBehavior.Idle_JellyfishPropulsion);

            // Squash and stretch the sprite passively.
            spriteStretchX = MathHelper.Lerp(1f, 1.10f, EasingFunctions.SineEaseInOut(Timer / 60f));
            spriteStretchY = MathHelper.Lerp(1f, 0.8f, EasingFunctions.SineEaseInOut(Timer / 120f));

            UpdateAnimationFrames(default, 10f);

            SwitchBehavior_Attacking(target);
            SwitchBehavior_Latching(target);
            SwitchBehavior_Fleeing(target);
        }

        public void DoBehavior_Fleeing(NPCAimedTarget target)
        {
            ref float additionalAggroRange = ref NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];
            ref float jellyfishMovementAngle = ref NPC.TwilightEgress().ExtraAI[JellyfishMovementAngleIndex];
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];

            int maxTime = 45;
            int timeBeforePropulsion = 30;
            float avoidanceSpeedInterpolant = Utils.GetLerpValue(0f, 1f, LifeRatio / 0.2f, true);
            bool targetIsFarEnoughAway = NPC.Distance(target.Center) >= 400f + additionalAggroRange;

            if (targetIsFarEnoughAway || target.Type != Terraria.Enums.NPCTargetType.Player || target.Invalid)
            {
                ManaphageBehavior randomIdleState = Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround);
                SwitchBehaviorState(randomIdleState);
                return;
            }

            CheckForTurnAround(out bool turnAround);

            // Same code found in the DoBehavior_JellyfishPropulsion method above.
            if (Timer <= timeBeforePropulsion)
            {
                float stretchInterpolant = Utils.GetLerpValue(0f, 1f, (float)(Timer / timeBeforePropulsion), true);
                spriteStretchX = MathHelper.Lerp(spriteStretchX, 1.25f, EasingFunctions.SineEaseInOut(stretchInterpolant));
                spriteStretchY = MathHelper.Lerp(spriteStretchY, 0.75f, EasingFunctions.SineEaseInOut(stretchInterpolant));

                if (!FoundValidRotationAngle)
                {
                    Vector2 vectorToPlayer = NPC.SafeDirectionTo(target.Center);
                    if (Timer == 1 && !turnAround)
                        jellyfishMovementAngle = vectorToPlayer.ToRotation();

                    int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 1f, stretchInterpolant));
                    UpdateAnimationFrames(default, 0f, frameY);

                    if (!turnAround)
                    {
                        FoundValidRotationAngle = true;
                        NPC.netUpdate = true;
                    }

                    Vector2 centerAhead = NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 128f;
                    jellyfishMovementAngle += -centerAhead.ToRotation() * 12f;
                }
            }

            if (Timer == timeBeforePropulsion)
            {
                float avoidanceSpeed = MathHelper.Lerp(-3f, -7f, avoidanceSpeedInterpolant);
                Vector2 fleeVelocity = Vector2.UnitY.RotatedBy(NPC.rotation) * avoidanceSpeed;
                NPC.velocity = fleeVelocity;

                UpdateAnimationFrames(default, 0f, 2);

                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                    Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.BlueFairy, dustVelocity * 5f);
                    dust.noGravity = true;
                }

                PulseRingParticle propulsionRing = new(NPC.Center - NPC.SafeDirectionTo(NPC.Center) * 60f, NPC.SafeDirectionTo(NPC.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), NPC.velocity.ToRotation(), 45);
                propulsionRing.SpawnCasParticle();

                spriteStretchX = 0.8f;
                spriteStretchY = 1.25f;
            }

            if (Timer >= timeBeforePropulsion)
            {
                float animationInterpolant = Utils.GetLerpValue(0f, 1f, (Timer - maxTime) / timeBeforePropulsion + 30, true);
                int frameY = (int)Math.Floor(MathHelper.Lerp(1f, 4f, EasingFunctions.SineEaseIn(animationInterpolant)));
                UpdateAnimationFrames(default, 0f, frameY);
            }

            if (Timer >= maxTime)
            {
                Timer = 0f;
                FoundValidRotationAngle = false;
                NPC.netUpdate = true;
            }

            NPC.velocity *= 0.98f;
            Vector2 futureVelocity = Vector2.One.RotatedBy(jellyfishMovementAngle);
            NPC.rotation = NPC.rotation.AngleLerp(futureVelocity.ToRotation() - 1.57f, 0.1f);
        }

        public void DoBehavior_Latching(NPCAimedTarget target)
        {
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];
            ref float initialRotation = ref NPC.TwilightEgress().ExtraAI[InitialRotationIndex];

            Rectangle asteroidHitbox = new((int)AsteroidToSucc.position.X, (int)AsteroidToSucc.position.Y, (int)(AsteroidToSucc.width * 0.75f), (int)(AsteroidToSucc.height * 0.75f));

            // Reset if the asteroid target variable is null.
            if (target.Invalid || !AsteroidToSucc.active || ManaRatio >= 1f)
            {
                AsteroidToSucc = null;
                NPC.velocity = Vector2.UnitY.RotatedBy(NPC.rotation) * -2f;
                SwitchBehaviorState(Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround));
                return;
            }

            // Pre-succ.
            if (LocalAIState == 0f)
            {
                if (Timer >= 45)
                {
                    // Quickly move towards the selected asteroid and stop moving once the two hitboxes
                    // intersect with each other. 
                    NPC.SimpleMove(AsteroidToSucc.Center, 12f, 6f);
                    if (NPC.Hitbox.Intersects(asteroidHitbox))
                    {
                        initialRotation = NPC.rotation - AsteroidToSucc.rotation;
                        LocalAIState = 1f;
                        Timer = 0f;
                        NPC.netUpdate = true;
                    }
                }

                if (Timer <= 15f)
                {
                    int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 4f, Timer / 15f));
                    UpdateAnimationFrames(ManaphageAnimation.Inject, 0f, frameY);
                }

            }

            // Post-succ.
            if (LocalAIState == 1f)
            {                
                if (Timer % 30 == 0)
                {
                    int damageToAsteroid = Main.rand.Next(10, 15);
                    AsteroidToSucc.SimpleStrikeNPC(damageToAsteroid, 0, noPlayerInteraction: true);
                }

                Vector2 positionAroundAsteroid = AsteroidToSucc.Center - Vector2.UnitY.RotatedBy(initialRotation + AsteroidToSucc.rotation) * asteroidHitbox.Size();
                NPC.SimpleMove(positionAroundAsteroid, 20f, 0f);

                CurrentManaCapacity = MathHelper.Clamp(CurrentManaCapacity + 0.1f, 0f, MaximumManaCapacity);
                UpdateAnimationFrames(ManaphageAnimation.Suck, 5f);               
            }
            
            if (spriteStretchX > 1f)
                spriteStretchX *= 0.98f;
            if (spriteStretchX < 1f)
                spriteStretchX *= 1.02f;

            if (spriteStretchY > 1f)
                spriteStretchY *= 0.98f;
            if (spriteStretchY < 1f)
                spriteStretchY *= 1.02f;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(AsteroidToSucc.Center) - 1.57f, 0.2f);
            SwitchBehavior_Fleeing(target);
        }

        public void DoBehavior_Attacking(NPCAimedTarget target)
        {
            ref float additionalAggroRange = ref NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];
            ref float manaSuckTimer = ref NPC.TwilightEgress().ExtraAI[ManaSuckTimerIndex];
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];

            float aggroRange = 400f + additionalAggroRange;
            bool targetOutOfRange = NPC.Distance(target.Center) >= aggroRange;
            if (target.Invalid || target.Type != Terraria.Enums.NPCTargetType.Player || targetOutOfRange)
            {
                SwitchBehaviorState(Utils.SelectRandom(Main.rand, ManaphageBehavior.Idle_JellyfishPropulsion, ManaphageBehavior.Idle_LazeAround));
                return;
            }

            ShouldTargetNPCs = false;
            manaSuckTimer = ManaRatio > 0.5f ? MaxManaSuckTimerOverFifty : MaxManaSuckTimerUnderFifty;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target.Center) - 1.57f, 0.2f);    
            Vector2 areaAroundPlayer = target.Center + NPC.DirectionFrom(target.Center) * 250f;
            // Increase the Manaphage's movement speed depending on how much additional aggro range
            // it has ammased.
            float movementSpeed = MathHelper.Lerp(2f, 6f, Utils.GetLerpValue(0f, 1f, additionalAggroRange / 500f, true));
            NPC.SimpleMove(areaAroundPlayer, movementSpeed, 20f);

            if (Timer % 5 == 0)
            {
                CurrentManaCapacity -= 0.25f;

                Vector2 spawnPosition = NPC.Center + Vector2.UnitY.RotatedBy(NPC.rotation) * 30f;
                Vector2 inkVelocity = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 14f + NPC.velocity;

                NPC.BetterNewProjectile(spawnPosition, inkVelocity, ModContent.ProjectileType<ManaInk>(), (int)(NPC.defDamage * 0.45f), 0f, damageChanges: true);
            }

            UpdateAnimationFrames(ManaphageAnimation.Attack, 5f);
            spriteStretchX = MathHelper.Lerp(1f, 0.85f, EasingFunctions.SineEaseInOut(Timer / 10f));
            spriteStretchY = MathHelper.Lerp(0.95f, 1.05f, EasingFunctions.SineEaseInOut(Timer / 10f));

            SwitchBehavior_Fleeing(target);
        }

        public void SwitchBehavior_Attacking(NPCAimedTarget target)
        {
            ref float additionalAggroRange = ref NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];
            ref float manaSuckTimer = ref NPC.TwilightEgress().ExtraAI[ManaSuckTimerIndex];

            bool canAtack = ManaRatio > 0.3f && LifeRatio > 0.2f;
            if (canAtack && target.Type == Terraria.Enums.NPCTargetType.Player)
            {
                float maxDetectionDistance = (AIState == (float)ManaphageBehavior.Latching ? AggroRangeWhileLatching : DefaultAggroRange) + additionalAggroRange;
                bool playerWithinRange = Vector2.Distance(NPC.Center, target.Center) < maxDetectionDistance;
                if (playerWithinRange)
                    SwitchBehaviorState(ManaphageBehavior.Attacking);
            }
        }

        public void SwitchBehavior_Latching(NPCAimedTarget target)
        {
            ref float manaSuckTimer = ref NPC.TwilightEgress().ExtraAI[ManaSuckTimerIndex];
            int[] cosmostoneAsteroidTypes = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];

            // If the Manaphage starts becoming low on mana, start looking for nearby Asteroids.
            if (target.Type == Terraria.Enums.NPCTargetType.NPC)
            {
                List<NPC> cosmostoneAsteroids = Main.npc.Take(Main.maxNPCs).Where(npc => npc.active && cosmostoneAsteroidTypes.Contains(npc.type) && NPC.Distance(npc.Center) <= 300).ToList();
                if (cosmostoneAsteroids.Count <= 0)
                    return;

                // Once the Manaphage reaches a low enough mana capacity, find the nearest asteroid and latch onto it.
                bool canSuckMana = ManaRatio < 0.3f || (ManaRatio < 0.6f && manaSuckTimer <= 0);
                if (canSuckMana)
                    SwitchBehaviorState(ManaphageBehavior.Latching, cosmostoneAsteroids.FirstOrDefault());
            }
        }

        public void SwitchBehavior_Fleeing(NPCAimedTarget target)
        {
            ref float additionalAggroRange = ref NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];

            float maxDetectionDistance = (AIState == (float)ManaphageBehavior.Latching ? AggroRangeWhileLatching : DefaultAggroRange) + additionalAggroRange;
            bool canFlee = target.Type == Terraria.Enums.NPCTargetType.Player && NPC.Distance(target.Center) <= maxDetectionDistance && AIState != (float)ManaphageBehavior.Fleeing;

            if ((ManaRatio < 0.3f && canFlee) || (LifeRatio < 0.2f && canFlee))
                SwitchBehaviorState(ManaphageBehavior.Fleeing);
        }
        #endregion

        #region Drawing and Animation
        public void UpdateAnimationFrames(ManaphageAnimation manaphageAnimation, float frameSpeed, int? specificYFrame = null)
        {
            int frameX = manaphageAnimation switch
            {
                ManaphageAnimation.Inject => 1,
                ManaphageAnimation.Attack => 2,
                ManaphageAnimation.LookRight => 3,
                ManaphageAnimation.LookLeft => 4,
                ManaphageAnimation.Suck => 5,
                _ => 0
            };

            FrameX = frameX;
            FrameY = specificYFrame ?? (int)Math.Floor(Timer / frameSpeed) % Main.npcFrameCount[Type];
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawManaTank();
            DrawMainSprite(drawColor);
            return false;
        }

        public void DrawMainSprite(Color drawColor)
        {
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];

            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 stretchFactor = new(spriteStretchX, spriteStretchY);

            Rectangle rectangle = texture.Frame(6, Main.npcFrameCount[Type], FrameX, FrameY);
            Main.EntitySpriteDraw(texture, drawPosition, rectangle, NPC.GetAlpha(drawColor), NPC.rotation, rectangle.Size() / 2f, NPC.scale * stretchFactor, 0);
        }

        public void DrawManaTank()
        {
            ref float spriteStretchX = ref NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];
            ref float manaTankShaderTime = ref NPC.TwilightEgress().ExtraAI[ManaTankShaderTimeIndex];

            Texture2D manaphageTank = ModContent.Request<Texture2D>("TwilightEgress/Assets/Textures/NPCs/CosmostoneShowers/Manaphages/Manaphage_Tank").Value;
            Texture2D manaphageTankMask = ModContent.Request<Texture2D>("TwilightEgress/Assets/Textures/NPCs/CosmostoneShowers/Manaphages/Manaphage_Tank_Mask").Value;

            Vector2 stretchFactor = new(spriteStretchX, spriteStretchY);
            Vector2 origin = manaphageTank.Size() / 2f;
            Vector2 drawPosition = NPC.Center - Main.screenPosition - Vector2.UnitY.RotatedBy(NPC.rotation) * 31f * spriteStretchY;

            float manaCapacityInterpolant = Utils.GetLerpValue(1f, 0f, CurrentManaCapacity / MaximumManaCapacity, true);

            Main.spriteBatch.PrepareForShaders();
            ShaderManager.TryGetShader("TwilightEgress.ManaphageTankShader", out ManagedShader manaTankShader);
            manaTankShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly * manaTankShaderTime);
            manaTankShader.TrySetParameter("manaCapacity", manaCapacityInterpolant);
            manaTankShader.TrySetParameter("pixelationFactor", 0.075f);
            manaTankShader.SetTexture(AssetRegistry.Textures.BlueCosmicGalaxy, 1, SamplerState.AnisotropicWrap);
            manaTankShader.SetTexture(AssetRegistry.Textures.SmudgyNoise, 2, SamplerState.AnisotropicWrap);
            manaTankShader.Apply();

            // Draw the tank mask with the shader applied to it.
            Main.EntitySpriteDraw(manaphageTankMask, drawPosition, null, Color.Black, NPC.rotation, origin, NPC.scale * 0.98f, 0);
            Main.spriteBatch.ExitShaderRegion();

            // Draw the tank itself.
            Main.EntitySpriteDraw(manaphageTank, drawPosition, null, Color.White * NPC.Opacity, NPC.rotation, origin, NPC.scale, 0);
        }
        #endregion

        #region Helper Methods
        public void OnHitEffects(NPC.HitInfo hit)
        {
            ref float additionalAggroRange = ref NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];
            ref float aggroRangeTimer = ref NPC.TwilightEgress().ExtraAI[AggroRangeTimerIndex];
            ref float manaSuckTimer = ref NPC.TwilightEgress().ExtraAI[ManaSuckTimerIndex];

            // Apply an extra 50 pixels of aggro range everytime the Manaphage is attacked.
            // 100 is added instead if the hit was critical.
            // 250 is added instead if the hit happened during the Manaphage's latching stage.
            additionalAggroRange += AIState == (float)ManaphageBehavior.Latching ? 250f : hit.Crit ? 100f : 50f;
            if (additionalAggroRange > 500f)
                additionalAggroRange = 500f;

            // Timer resetting.
            aggroRangeTimer = 720f;
            manaSuckTimer = ManaRatio > 0.5f ? MaxManaSuckTimerOverFifty : MaxManaSuckTimerUnderFifty;

            // Manaphages can be knocked out of their latching phase if hit.
            if (AIState == (float)ManaphageBehavior.Latching)
                SwitchBehaviorState(ManaphageBehavior.Idle_JellyfishPropulsion);
        }

        public void ManageExtraTimers()
        {
            // Manages the extra additional aggro range and mana suck timers.
            ref float additionalAggroRange = ref NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];
            ref float aggroRangeTimer = ref NPC.TwilightEgress().ExtraAI[AggroRangeTimerIndex];
            ref float manaSuckTimer = ref NPC.TwilightEgress().ExtraAI[ManaSuckTimerIndex];

            // This timer controls how long the additional aggro range is applied for.
            aggroRangeTimer = MathHelper.Clamp(aggroRangeTimer - 1f, 0f, 1200f);
            if (aggroRangeTimer <= 0f)
            {
                aggroRangeTimer = 0f;
                additionalAggroRange = MathHelper.Clamp(additionalAggroRange - 1f, 0f, 500f);
            }

            // This timer controls if a Manaphage should target an asteroid and absorb
            // mana at 50% mana capacity. Manaphages typically start fleeing and looking 
            // Asteroids at aunder 30% mana capacity.
            manaSuckTimer = MathHelper.Clamp(manaSuckTimer - 1f, 0f, 720f);
        }

        public void CheckForTurnAround(out bool turnAround)
        {
            turnAround = false;
            for (int i = 0; i < 8; i++)
            {
                // Avoid leaving the world and avoid running into tiles.
                Vector2 centerAhead = NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 128f * i;
                bool leavingWorldBounds = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;
                turnAround = leavingWorldBounds;

                if (!Collision.CanHit(NPC.Center, NPC.width, NPC.height, centerAhead, NPC.width, NPC.height))
                    turnAround = true;
            }
        }

        public void SwitchBehaviorState(ManaphageBehavior nextBehaviorState, NPC asteroidToTarget = null)
        {
            Timer = 0f;
            LocalAIState = 0f;
            AIState = (float)nextBehaviorState;
            FoundValidRotationAngle = false;
            AsteroidToSucc = asteroidToTarget;
            NPC.netUpdate = true;
        }
#endregion
    }
}
