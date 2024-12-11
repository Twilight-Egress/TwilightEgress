using TwilightEgress.Assets;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids;
using TwilightEgress.Core.BaseEntities.ModNPCs;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Manaphages
{
    public class Manaphage : BasePhage
    {
        public override void SetPhageDefaults()
        {
            NPC.width = 32;
            NPC.height = 34;
            NPC.damage = 25;
            NPC.defense = 3;
            NPC.lifeMax = 90;
            NPC.knockBackResist = 0.8f;
            NPC.value = 9f;
        }

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

            float tankLightLevel = Lerp(0.3f, 1.25f, ManaRatio);
            Vector2 tankPosition = NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 31f * spriteStretchY;
            Lighting.AddLight(tankPosition, Color.Cyan.ToVector3() * tankLightLevel);

            CurrentManaCapacity = Clamp(CurrentManaCapacity, 0f, MaximumManaCapacity);
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
                    spriteStretchX = Lerp(spriteStretchX, 1.25f, TwilightEgressUtilities.SineEaseInOut(stretchInterpolant));
                    spriteStretchY = Lerp(spriteStretchY, 0.75f, TwilightEgressUtilities.SineEaseInOut(stretchInterpolant));

                    int frameY = (int)Floor(Lerp(0f, 1f, stretchInterpolant));
                    UpdateAnimationFrames(default, 0f, frameY);

                    // Pick a random angle to move towards before propelling forward.
                    if (!FoundValidRotationAngle)
                    {
                        // Set a random movement angle initially.
                        if (Timer == 1 && !turnAround)
                            jellyfishMovementAngle = Main.rand.NextFloat(Tau);

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
                    TwilightEgressUtilities.CreateRandomizedDustExplosion(15, NPC.Center, DustID.BlueFairy);
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
                    int frameY = (int)Floor(Lerp(1f, 4f, TwilightEgressUtilities.SineEaseIn(animationInterpolant)));
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
            Vector2 velocity = Vector2.One.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);

            if (Timer is 0)
            {
                lazeMovementInterval = Main.rand.Next(360, 720);
                idleMovementDirection = Main.rand.NextBool().ToDirectionInt();
                NPC.velocity += velocity;
            }

            CheckForTurnAround(out bool turnAround);
            if (turnAround)
            {
                Vector2 circularArea = NPC.Center + NPC.velocity.RotatedBy(TwoPi);
                Vector2 turnAroundVelocity = circularArea.Y >= Main.maxTilesY + 750f ? Vector2.UnitY * -0.15f : circularArea.Y < Main.maxTilesY * 0.34f ?  Vector2.UnitY * 0.15f : NPC.velocity;

                float distanceFromTileCollisionLeft = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-PiOver2)) ?? 1000f;
                float distanceFromTileCollisionRight = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-PiOver2)) ?? 1000f;
                int directionToMove = (distanceFromTileCollisionLeft > distanceFromTileCollisionRight).ToDirectionInt();
                if (distanceFromTileCollisionLeft <= 150 || distanceFromTileCollisionRight <= 150f)
                    turnAroundVelocity = Vector2.One.RotatedBy(PiOver2 * directionToMove) * 0.15f;

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
            spriteStretchX = Lerp(1f, 1.10f, TwilightEgressUtilities.SineEaseInOut(Timer / 60f));
            spriteStretchY = Lerp(1f, 0.8f, TwilightEgressUtilities.SineEaseInOut(Timer / 120f));

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
                spriteStretchX = Lerp(spriteStretchX, 1.25f, TwilightEgressUtilities.SineEaseInOut(stretchInterpolant));
                spriteStretchY = Lerp(spriteStretchY, 0.75f, TwilightEgressUtilities.SineEaseInOut(stretchInterpolant));

                if (!FoundValidRotationAngle)
                {
                    Vector2 vectorToPlayer = NPC.SafeDirectionTo(target.Center);
                    if (Timer == 1 && !turnAround)
                        jellyfishMovementAngle = vectorToPlayer.ToRotation();

                    int frameY = (int)Floor(Lerp(0f, 1f, stretchInterpolant));
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
                float avoidanceSpeed = Lerp(-3f, -7f, avoidanceSpeedInterpolant);
                Vector2 fleeVelocity = Vector2.UnitY.RotatedBy(NPC.rotation) * avoidanceSpeed;
                NPC.velocity = fleeVelocity;

                UpdateAnimationFrames(default, 0f, 2);

                TwilightEgressUtilities.CreateRandomizedDustExplosion(15, NPC.Center, DustID.BlueFairy);
                PulseRingParticle propulsionRing = new(NPC.Center - NPC.SafeDirectionTo(NPC.Center) * 60f, NPC.SafeDirectionTo(NPC.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), NPC.velocity.ToRotation(), 45);
                propulsionRing.SpawnCasParticle();

                spriteStretchX = 0.8f;
                spriteStretchY = 1.25f;
            }

            if (Timer >= timeBeforePropulsion)
            {
                float animationInterpolant = Utils.GetLerpValue(0f, 1f, (Timer - maxTime) / timeBeforePropulsion + 30, true);
                int frameY = (int)Floor(Lerp(1f, 4f, TwilightEgressUtilities.SineEaseIn(animationInterpolant)));
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
                    int frameY = (int)Floor(Lerp(0f, 4f, Timer / 15f));
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

                CurrentManaCapacity = Clamp(CurrentManaCapacity + 0.1f, 0f, MaximumManaCapacity);
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
            float movementSpeed = Lerp(2f, 6f, Utils.GetLerpValue(0f, 1f, additionalAggroRange / 500f, true));
            NPC.SimpleMove(areaAroundPlayer, movementSpeed, 20f);

            if (Timer % 5 == 0)
            {
                CurrentManaCapacity -= 0.25f;

                Vector2 spawnPosition = NPC.Center + Vector2.UnitY.RotatedBy(NPC.rotation) * 30f;
                Vector2 inkVelocity = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 14f + NPC.velocity;

                NPC.BetterNewProjectile(spawnPosition, inkVelocity, ModContent.ProjectileType<ManaInk>(), NPC.defDamage.GetPercentageOfInteger(0.45f), 0f);
            }

            UpdateAnimationFrames(ManaphageAnimation.Attack, 5f);
            spriteStretchX = Lerp(1f, 0.85f, TwilightEgressUtilities.SineEaseInOut(Timer / 10f));
            spriteStretchY = Lerp(0.95f, 1.05f, TwilightEgressUtilities.SineEaseInOut(Timer / 10f));

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

            Texture2D manaphageTank = ModContent.Request<Texture2D>("TwilightEgress/Content/NPCs/CosmostoneShowers/Manaphages/Manaphage_Tank").Value;
            Texture2D manaphageTankMask = ModContent.Request<Texture2D>("TwilightEgress/Content/NPCs/CosmostoneShowers/Manaphages/Manaphage_Tank_Mask").Value;

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
    }
}
