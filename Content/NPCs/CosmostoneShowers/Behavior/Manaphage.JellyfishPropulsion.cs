using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;
using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Globals.GlobalNPCs;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class JellyfishPropulsion(FiniteStateMachine stateMachine, Manaphage manaphage) : EntityState<Manaphage>(stateMachine, manaphage)
    {
        public override void Enter(float[] arguments = null)
        {
            ref float jellyfishMovementInterval = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.JellyfishMovementIntervalIndex];
            ref float maxPropulsions = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.MaxPropulsionsIndex];

            jellyfishMovementInterval = Utils.SelectRandom(Main.rand, 60, 80, 100, 120);
            maxPropulsions = Main.rand.NextFloat(10f, 25f);
            Entity.NPC.ai[0] = (int)ManaphageBehavior.JellyfishPropulsion;
            Entity.NPC.ai[1] = 0f;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            ref float jellyfishMovementInterval = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.JellyfishMovementIntervalIndex];
            ref float jellyfishMovementAngle = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.JellyfishMovementAngleIndex];
            ref float spriteStretchX = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.SpriteStretchXIndex];
            ref float spriteStretchY = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.SpriteStretchYIndex];
            ref float jellyfishPropulsionCount = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.JellyfishPropulsionCountIndex];
            ref float maxPropulsions = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.MaxPropulsionsIndex];
            ref float frameSpeed = ref Entity.NPC.TwilightEgress().ExtraAI[Manaphage.FrameSpeedIndex];

            float propulsionSpeed = Main.rand.NextFloat(5f, 7f);
            Entity.CheckForTurnAround(out bool turnAround);

            if (Entity.NPC.ai[1] <= jellyfishMovementInterval)
            {
                // Squash the sprite slightly before the propulsion movement to give a
                // more cartoony, jellyfish-like feeling to the movement.
                float stretchInterpolant = Utils.GetLerpValue(0f, 1f, Entity.Timer / jellyfishMovementInterval, true);
                spriteStretchX = MathHelper.Lerp(spriteStretchX, 1.25f, EasingFunctions.SineEaseInOut(stretchInterpolant));
                spriteStretchY = MathHelper.Lerp(spriteStretchY, 0.75f, EasingFunctions.SineEaseInOut(stretchInterpolant));

                int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 1f, stretchInterpolant));
                Entity.UpdateAnimationFrames(default, 0f, frameY);

                // Pick a random angle to move towards before propelling forward.
                if (!Entity.FoundValidRotationAngle)
                {
                    // Set a random movement angle initially.
                    if (Entity.Timer == 1 && !turnAround)
                        jellyfishMovementAngle = Main.rand.NextFloat(MathF.Tau);

                    // Stop searching for valid rotation angles once the Manaphage no longer needs to turn around.
                    if (!turnAround)
                    {
                        Entity.FoundValidRotationAngle = true;
                        Entity.NPC.netUpdate = true;
                    }

                    // Keep rotating to find a valid angle to rotate towards.
                    Vector2 centerAhead = Entity.NPC.Center - Vector2.UnitY.RotatedBy(Entity.NPC.rotation) * 128f;
                    jellyfishMovementAngle += -centerAhead.ToRotation() * 6f;
                }
            }

            // Move forward every few seconds.
            if (Entity.Timer == jellyfishMovementInterval)
            {
                Vector2 velocity = Vector2.One.RotatedBy(jellyfishMovementAngle) * propulsionSpeed;
                Entity.NPC.velocity = velocity;

                Entity.UpdateAnimationFrames(default, 0f, 2);

                // Spawn some lil' visual particles everytime it ejects.
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                    Dust dust = Dust.NewDustPerfect(Entity.NPC.Center, DustID.BlueFairy, dustVelocity * 5f);
                    dust.noGravity = true;
                }

                PulseRingParticle propulsionRing = new(Entity.NPC.Center - Entity.NPC.SafeDirectionTo(Entity.NPC.Center) * 60f, Entity.NPC.SafeDirectionTo(Entity.NPC.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), Entity.NPC.velocity.ToRotation(), 45);
                propulsionRing.SpawnCasParticle();

                // Unstretch the sprite.
                spriteStretchX = 0.8f;
                spriteStretchY = 1.25f;

                jellyfishPropulsionCount++;
            }

            if (Entity.Timer >= jellyfishMovementInterval + 30)
            {
                float animationInterpolant = Utils.GetLerpValue(0f, 1f, (Entity.Timer - jellyfishMovementInterval + 45) / jellyfishMovementInterval + 30, true);
                int frameY = (int)Math.Floor(MathHelper.Lerp(1f, 4f, EasingFunctions.SineEaseIn(animationInterpolant)));
                Entity.UpdateAnimationFrames(default, 0f, frameY);
            }

            if (Entity.Timer >= jellyfishMovementInterval + 45)
            {
                Entity.Timer = 0f;
                Entity.FoundValidRotationAngle = false;
                Entity.NPC.netUpdate = true;
            }

            Entity.NPC.velocity *= 0.96f;
            Vector2 futureVelocity = Vector2.One.RotatedBy(jellyfishMovementAngle);
            Entity.NPC.rotation = Entity.NPC.rotation.AngleLerp(futureVelocity.ToRotation() + 1.57f, Entity.Timer / (float)jellyfishMovementInterval);

            // Randomly switch to the other idle AI state.
            if (jellyfishPropulsionCount >= maxPropulsions && Main.rand.NextBool(2))
                FiniteStateMachine.SetCurrentState((int)ManaphageBehavior.LazeAround, [Entity.NPC.whoAmI]);

            Entity.SwitchBehavior_Attacking(Entity.NPC.GetTargetData());
            Entity.SwitchBehavior_Latching(Entity.NPC.GetTargetData());
            Entity.SwitchBehavior_Fleeing(Entity.NPC.GetTargetData());
        }
    }
}
