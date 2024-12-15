using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;
using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Globals.GlobalNPCs;
using Terraria.DataStructures;
using Luminance.Common.Utilities;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class Fleeing<ManaphageBehavior> : State<ManaphageBehavior>
    {
        public Fleeing(ManaphageBehavior id) : base(id)
        {
        }

        public override void Enter(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];

            npc.ai[0] = (int)CosmostoneShowers.ManaphageBehavior.Fleeing;
            npc.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];
            Manaphage manaphage = npc.ModNPC as Manaphage;
            NPCAimedTarget target = npc.GetTargetData();

            ref float additionalAggroRange = ref npc.TwilightEgress().ExtraAI[Manaphage.AdditionalAggroRangeIndex];
            ref float jellyfishMovementAngle = ref npc.TwilightEgress().ExtraAI[Manaphage.JellyfishMovementAngleIndex];
            ref float spriteStretchX = ref npc.TwilightEgress().ExtraAI[Manaphage.SpriteStretchXIndex];
            ref float spriteStretchY = ref npc.TwilightEgress().ExtraAI[Manaphage.SpriteStretchYIndex];

            int maxTime = 45;
            int timeBeforePropulsion = 30;
            float avoidanceSpeedInterpolant = Utils.GetLerpValue(0f, 1f, manaphage.LifeRatio / 0.2f, true);
            bool targetIsFarEnoughAway = npc.Distance(target.Center) >= 400f + additionalAggroRange;

            if (targetIsFarEnoughAway || target.Type != Terraria.Enums.NPCTargetType.Player || target.Invalid)
            {
                CosmostoneShowers.ManaphageBehavior randomIdleState = Utils.SelectRandom(Main.rand, CosmostoneShowers.ManaphageBehavior.JellyfishPropulsion, CosmostoneShowers.ManaphageBehavior.LazeAround);
                manaphage.stateMachine.TrySetCurrentState(randomIdleState, [npc.whoAmI]);
                return;
            }

            manaphage.CheckForTurnAround(out bool turnAround);

            // Same code found in the DoBehavior_JellyfishPropulsion method above.
            if (manaphage.Timer <= timeBeforePropulsion)
            {
                float stretchInterpolant = Utils.GetLerpValue(0f, 1f, (float)(manaphage.Timer / timeBeforePropulsion), true);
                spriteStretchX = MathHelper.Lerp(spriteStretchX, 1.25f, EasingFunctions.SineEaseInOut(stretchInterpolant));
                spriteStretchY = MathHelper.Lerp(spriteStretchY, 0.75f, EasingFunctions.SineEaseInOut(stretchInterpolant));

                if (!manaphage.FoundValidRotationAngle)
                {
                    Vector2 vectorToPlayer = npc.SafeDirectionTo(target.Center);
                    if (manaphage.Timer == 1 && !turnAround)
                        jellyfishMovementAngle = vectorToPlayer.ToRotation();

                    int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 1f, stretchInterpolant));
                    manaphage.UpdateAnimationFrames(default, 0f, frameY);

                    if (!turnAround)
                    {
                        manaphage.FoundValidRotationAngle = true;
                        npc.netUpdate = true;
                    }

                    Vector2 centerAhead = npc.Center - Vector2.UnitY.RotatedBy(npc.rotation) * 128f;
                    jellyfishMovementAngle += -centerAhead.ToRotation() * 12f;
                }
            }

            if (manaphage.Timer == timeBeforePropulsion)
            {
                float avoidanceSpeed = MathHelper.Lerp(-3f, -7f, avoidanceSpeedInterpolant);
                Vector2 fleeVelocity = Vector2.UnitY.RotatedBy(npc.rotation) * avoidanceSpeed;
                npc.velocity = fleeVelocity;

                manaphage.UpdateAnimationFrames(default, 0f, 2);

                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                    Dust dust = Dust.NewDustPerfect(npc.Center, DustID.BlueFairy, dustVelocity * 5f);
                    dust.noGravity = true;
                }

                PulseRingParticle propulsionRing = new(npc.Center - npc.SafeDirectionTo(npc.Center) * 60f, npc.SafeDirectionTo(npc.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), npc.velocity.ToRotation(), 45);
                propulsionRing.SpawnCasParticle();

                spriteStretchX = 0.8f;
                spriteStretchY = 1.25f;
            }

            if (manaphage.Timer >= timeBeforePropulsion)
            {
                float animationInterpolant = Utils.GetLerpValue(0f, 1f, (manaphage.Timer - maxTime) / timeBeforePropulsion + 30, true);
                int frameY = (int)Math.Floor(MathHelper.Lerp(1f, 4f, EasingFunctions.SineEaseIn(animationInterpolant)));
                manaphage.UpdateAnimationFrames(default, 0f, frameY);
            }

            if (manaphage.Timer >= maxTime)
            {
                manaphage.Timer = 0f;
                manaphage.FoundValidRotationAngle = false;
                npc.netUpdate = true;
            }

            npc.velocity *= 0.98f;
            Vector2 futureVelocity = Vector2.One.RotatedBy(jellyfishMovementAngle);
            npc.rotation = npc.rotation.AngleLerp(futureVelocity.ToRotation() - 1.57f, 0.1f);
        }

        public override void Exit(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];

            (npc.ModNPC as Manaphage).Timer = 0f;
            (npc.ModNPC as Manaphage).LocalAIState = 0f;
            (npc.ModNPC as Manaphage).FoundValidRotationAngle = false;

            if (arguments.Length < 1)
                (npc.ModNPC as Manaphage).AsteroidToSucc = Main.npc[(int)arguments[1]];

            npc.netUpdate = true;
        }
    }
}
