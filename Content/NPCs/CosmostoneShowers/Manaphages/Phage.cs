using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using TwilightEgress.Core.Globals.GlobalNPCs;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Manaphages
{
    public abstract class Phage : ModNPC
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
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.UsesNewTargetting[Type] = true;
        }

        public override void SetDefaults()
        {
            SetPhageDefaults();
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

        #region Virtuals
        public virtual void SetPhageDefaults()
        {

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
