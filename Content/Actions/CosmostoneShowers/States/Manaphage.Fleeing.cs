﻿using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TwilightEgress.Content.NPCs.CosmostoneShowers;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;
using TwilightEgress.Core.Behavior;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.Manaphage;

namespace TwilightEgress.Content.Actions.CosmostoneShowers.States
{
    public class Fleeing(FiniteStateMachine stateMachine, Manaphage manaphage) : EntityState<Manaphage>(stateMachine, manaphage)
    {
        public override void Enter(float[] arguments = null)
        {
            Entity.NPC.ai[0] = (int)ManaphageBehavior.Fleeing;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPCAimedTarget target = Entity.NPC.GetTargetData();

            int maxTime = 45;
            int timeBeforePropulsion = 30;
            float avoidanceSpeedInterpolant = Utils.GetLerpValue(0f, 1f, Entity.LifeRatio / 0.2f, true);
            bool targetIsFarEnoughAway = Entity.NPC.Distance(target.Center) >= 400f + Entity.AdditionalAggroRange;

            if (targetIsFarEnoughAway || target.Type != Terraria.Enums.NPCTargetType.Player || target.Invalid)
            {
                ManaphageBehavior randomIdleState = Utils.SelectRandom(Main.rand, ManaphageBehavior.SprayingInk, ManaphageBehavior.LazeAround);
                FiniteStateMachine.SetCurrentState((int)randomIdleState);
                return;
            }

            Entity.CheckForTurnAround(out bool turnAround);

            // Same code found in the DoBehavior_JellyfishPropulsion method above.
            if (Entity.Timer <= timeBeforePropulsion)
            {
                float stretchInterpolant = Utils.GetLerpValue(0f, 1f, (float)(Entity.Timer / timeBeforePropulsion), true);
                Entity.SpriteStretchX = MathHelper.Lerp(Entity.SpriteStretchX, 1.25f, EasingFunctions.SineEaseInOut(stretchInterpolant));
                Entity.SpriteStretchY = MathHelper.Lerp(Entity.SpriteStretchY, 0.75f, EasingFunctions.SineEaseInOut(stretchInterpolant));

                if (!Entity.FoundValidRotationAngle)
                {
                    Vector2 vectorToPlayer = Entity.NPC.SafeDirectionTo(target.Center);
                    if (Entity.Timer == 1 && !turnAround)
                        Entity.JellyfishMovementAngle = vectorToPlayer.ToRotation();

                    int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 1f, stretchInterpolant));
                    Entity.UpdateAnimationFrames(default, 0f, frameY);

                    if (!turnAround)
                    {
                        Entity.FoundValidRotationAngle = true;
                        Entity.NPC.netUpdate = true;
                    }

                    Vector2 centerAhead = Entity.NPC.Center - Vector2.UnitY.RotatedBy(Entity.NPC.rotation) * 128f;
                    Entity.JellyfishMovementAngle += -centerAhead.ToRotation() * 12f;
                }
            }

            if (Entity.Timer == timeBeforePropulsion)
            {
                float avoidanceSpeed = MathHelper.Lerp(-3f, -7f, avoidanceSpeedInterpolant);
                Vector2 fleeVelocity = Vector2.UnitY.RotatedBy(Entity.NPC.rotation) * avoidanceSpeed;
                Entity.NPC.velocity = fleeVelocity;

                Entity.UpdateAnimationFrames(default, 0f, 2);

                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                    Dust dust = Dust.NewDustPerfect(Entity.NPC.Center, DustID.BlueFairy, dustVelocity * 5f);
                    dust.noGravity = true;
                }

                PulseRingParticle propulsionRing = new(Entity.NPC.Center - Entity.NPC.SafeDirectionTo(Entity.NPC.Center) * 60f, Entity.NPC.SafeDirectionTo(Entity.NPC.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), Entity.NPC.velocity.ToRotation(), 45);
                propulsionRing.SpawnCasParticle();

                Entity.SpriteStretchX = 0.8f;
                Entity.SpriteStretchY = 1.25f;
            }

            if (Entity.Timer >= timeBeforePropulsion)
            {
                float animationInterpolant = Utils.GetLerpValue(0f, 1f, (Entity.Timer - maxTime) / timeBeforePropulsion + 30, true);
                int frameY = (int)Math.Floor(MathHelper.Lerp(1f, 4f, EasingFunctions.SineEaseIn(animationInterpolant)));
                Entity.UpdateAnimationFrames(default, 0f, frameY);
            }

            if (Entity.Timer >= maxTime)
            {
                Entity.Timer = 0f;
                Entity.FoundValidRotationAngle = false;
                Entity.NPC.netUpdate = true;
            }

            Entity.NPC.velocity *= 0.98f;
            Vector2 futureVelocity = Vector2.One.RotatedBy(Entity.JellyfishMovementAngle);
            Entity.NPC.rotation = Entity.NPC.rotation.AngleLerp(futureVelocity.ToRotation() - 1.57f, 0.1f);
        }
    }
}
