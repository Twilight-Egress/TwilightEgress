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
using TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior;
using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Globals.GlobalNPCs;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    public class Manaphage : ModNPC
    {
        #region Enums
        public enum ManaphageBehavior
        {
            JellyfishPropulsion = 0,
            LazeAround = 1,
            Fleeing = 2,
            Latching = 3,
            SprayingInk = 4
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

        public const int JellyfishPropulsionsLeftIndex = 7;

        public const int ManaTankShaderTimeIndex = 8;

        public const int InitialRotationIndex = 9;

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

        public float SpriteStretchX;

        public float SpriteStretchY;

        public NPC AsteroidToSucc;

        public float ManaRatio => CurrentManaCapacity / MaximumManaCapacity;

        public float LifeRatio => NPC.life / (float)NPC.lifeMax;

        public Player NearestPlayer => Main.player[NPC.target];


        public FiniteStateMachine stateMachine = null;

        public ref float Timer => ref NPC.ai[1];

        public ref float AIState => ref NPC.ai[0];

        public ref float LocalAIState => ref NPC.ai[2];

        public virtual float MaximumManaCapacity => 100f;

        #endregion

        #region Overrides
        public override string LocalizationCategory => "NPCs.CosmostoneShowers";

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
            ref float manaTankShaderTime = ref NPC.TwilightEgress().ExtraAI[ManaTankShaderTimeIndex];
            ref float jellyfishMovementAngle = ref NPC.TwilightEgress().ExtraAI[JellyfishMovementAngleIndex];

            stateMachine = new FiniteStateMachine();

            stateMachine.Add((int)ManaphageBehavior.JellyfishPropulsion, new JellyfishPropulsion(stateMachine, this));
            stateMachine.Add((int)ManaphageBehavior.LazeAround, new LazeAround(stateMachine, this));
            stateMachine.Add((int)ManaphageBehavior.Fleeing, new Fleeing(stateMachine, this));
            stateMachine.Add((int)ManaphageBehavior.Latching, new Latching(stateMachine, this));
            stateMachine.Add((int)ManaphageBehavior.SprayingInk, new SprayingInk(stateMachine, this));

            stateMachine.ForEach((keyValuePair) =>
            {
                keyValuePair.Value.OnExit += Exit;
            });

            stateMachine.SetCurrentState((int)Utils.SelectRandom(Main.rand, ManaphageBehavior.JellyfishPropulsion, ManaphageBehavior.LazeAround), [NPC.whoAmI]);

            CurrentManaCapacity = Main.rand.NextBool(25) ? Main.rand.NextFloat(75f, 100f) : Main.rand.NextFloat(60f, 15f);
            SpriteStretchX = 1f;
            SpriteStretchY = 1f;
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
            NPC.AdvancedNPCTargeting(true, MaximumPlayerSearchDistance, ShouldTargetNPCs, MaximumNPCSearchDistance,
                ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>());

            // Don't bother targetting any asteroids at 60% and above mana capacity.
            if (ManaRatio > 0.6f)
                ShouldTargetNPCs = false;
            else
                // If the Manaphage is under 50% of it's maximum mana capacity, start detecting asteroids
                // only if the Manaphage isn't currently fleeing from anything.
                ShouldTargetNPCs = AIState != (float)ManaphageBehavior.Fleeing;

            stateMachine?.Update([NPC.whoAmI]);
            stateMachine?.SetCurrentState((int)AIState);

            float tankLightLevel = MathHelper.Lerp(0.3f, 1.25f, ManaRatio);
            Vector2 tankPosition = NPC.Center - Vector2.UnitY.RotatedBy(NPC.rotation) * 31f * SpriteStretchY;
            Lighting.AddLight(tankPosition, Color.Cyan.ToVector3() * tankLightLevel);

            CurrentManaCapacity = MathHelper.Clamp(CurrentManaCapacity, 0f, MaximumManaCapacity);
            ManageExtraTimers();
            Timer++;
        }

        #region AI Methods
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
                    stateMachine.SetCurrentState((int)ManaphageBehavior.SprayingInk);
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
                bool canSuckMana = ManaRatio < 0.3f || ManaRatio < 0.6f && manaSuckTimer <= 0;
                if (canSuckMana)
                    stateMachine.SetCurrentState((int)ManaphageBehavior.Latching, [cosmostoneAsteroids.FirstOrDefault().whoAmI]);
            }
        }

        public void SwitchBehavior_Fleeing(NPCAimedTarget target)
        {
            ref float additionalAggroRange = ref NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];

            float maxDetectionDistance = (AIState == (float)ManaphageBehavior.Latching ? AggroRangeWhileLatching : DefaultAggroRange) + additionalAggroRange;
            bool canFlee = target.Type == Terraria.Enums.NPCTargetType.Player && NPC.Distance(target.Center) <= maxDetectionDistance && AIState != (float)ManaphageBehavior.Fleeing;

            if (ManaRatio < 0.3f && canFlee || LifeRatio < 0.2f && canFlee)
                stateMachine.SetCurrentState((int)ManaphageBehavior.Fleeing);
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
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition;
            Vector2 stretchFactor = new(SpriteStretchX, SpriteStretchY);

            Rectangle rectangle = texture.Frame(6, Main.npcFrameCount[Type], FrameX, FrameY);
            Main.EntitySpriteDraw(texture, drawPosition, rectangle, NPC.GetAlpha(drawColor), NPC.rotation, rectangle.Size() / 2f, NPC.scale * stretchFactor, 0);
        }

        public void DrawManaTank()
        {
            ref float manaTankShaderTime = ref NPC.TwilightEgress().ExtraAI[ManaTankShaderTimeIndex];

            Texture2D manaphageTank = AssetRegistry.Textures.CosmostoneShowers.Manaphage_Tank.Value;
            Texture2D manaphageTankMask = AssetRegistry.Textures.CosmostoneShowers.Manaphage_Tank_Mask.Value;

            Vector2 stretchFactor = new(SpriteStretchX, SpriteStretchY);
            Vector2 origin = manaphageTank.Size() / 2f;
            Vector2 drawPosition = NPC.Center - Main.screenPosition - Vector2.UnitY.RotatedBy(NPC.rotation) * 31f * SpriteStretchY;

            float manaCapacityInterpolant = Utils.GetLerpValue(1f, 0f, CurrentManaCapacity / MaximumManaCapacity, true);

            Vector4[] colors =
            [
                new Color(96, 188, 246).ToVector4(),
                new Color(81, 158, 245).ToVector4(),
                new Color(76, 131, 242).ToVector4(),
                new Color(3, 96, 243).ToVector4(),
                new Color(48, 65, 197).ToVector4(),
                new Color(104, 94, 228).ToVector4(),
                new Color(157, 113, 239).ToVector4(),
                new Color(0, 0, 0, 0).ToVector4()
            ];

            Main.spriteBatch.PrepareForShaders();
            ShaderManager.TryGetShader("TwilightEgress.ManaphageTankShader", out ManagedShader manaTankShader);
            manaTankShader.TrySetParameter("time", Main.GlobalTimeWrappedHourly * manaTankShaderTime);
            manaTankShader.TrySetParameter("manaCapacity", manaCapacityInterpolant);
            manaTankShader.TrySetParameter("pixelationFactor", 0.075f);
            manaTankShader.TrySetParameter("palette", colors);
            manaTankShader.SetTexture(manaphageTankMask, 0);
            manaTankShader.SetTexture(AssetRegistry.Textures.Gradients.NeuronNebulaGalaxy, 1, SamplerState.AnisotropicWrap);
            manaTankShader.SetTexture(AssetRegistry.Textures.Gradients.SwirlyNoise, 2, SamplerState.AnisotropicWrap);
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
                stateMachine.SetCurrentState((int)ManaphageBehavior.JellyfishPropulsion);
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

        public void Exit(float[] arguments = null)
        {
            this.Timer = 0f;
            this.LocalAIState = 0f;
            this.FoundValidRotationAngle = false;

            if (arguments?.Length < 1)
                this.AsteroidToSucc = Main.npc[(int)arguments[0]];

            this.NPC.netUpdate = true;
        }
        #endregion
    }
}
