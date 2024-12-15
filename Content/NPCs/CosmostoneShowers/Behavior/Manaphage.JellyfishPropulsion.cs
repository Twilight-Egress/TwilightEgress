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
    public class JellyfishPropulsion<ManaphageBehavior> : State<ManaphageBehavior>
    {
        public JellyfishPropulsion(ManaphageBehavior id) : base(id)
        {
        }

        public override void Enter(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];

            ref float jellyfishMovementInterval = ref npc.TwilightEgress().ExtraAI[Manaphage.JellyfishMovementIntervalIndex];
            ref float maxPropulsions = ref npc.TwilightEgress().ExtraAI[Manaphage.MaxPropulsionsIndex];

            jellyfishMovementInterval = Utils.SelectRandom(Main.rand, 60, 80, 100, 120);
            maxPropulsions = Main.rand.NextFloat(10f, 25f);
            npc.ai[0] = (int)CosmostoneShowers.ManaphageBehavior.JellyfishPropulsion;
            npc.ai[1] = 0f;
            npc.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];
            Manaphage manaphage = npc.ModNPC as Manaphage;

            ref float jellyfishMovementInterval = ref npc.TwilightEgress().ExtraAI[Manaphage.JellyfishMovementIntervalIndex];
            ref float jellyfishMovementAngle = ref npc.TwilightEgress().ExtraAI[Manaphage.JellyfishMovementAngleIndex];
            ref float spriteStretchX = ref npc.TwilightEgress().ExtraAI[Manaphage.SpriteStretchXIndex];
            ref float spriteStretchY = ref npc.TwilightEgress().ExtraAI[Manaphage.SpriteStretchYIndex];
            ref float jellyfishPropulsionCount = ref npc.TwilightEgress().ExtraAI[Manaphage.JellyfishPropulsionCountIndex];
            ref float maxPropulsions = ref npc.TwilightEgress().ExtraAI[Manaphage.MaxPropulsionsIndex];
            ref float frameSpeed = ref npc.TwilightEgress().ExtraAI[Manaphage.FrameSpeedIndex];

            float propulsionSpeed = Main.rand.NextFloat(5f, 7f);
            manaphage.CheckForTurnAround(out bool turnAround);

            if (npc.ai[1] <= jellyfishMovementInterval)
            {
                // Squash the sprite slightly before the propulsion movement to give a
                // more cartoony, jellyfish-like feeling to the movement.
                float stretchInterpolant = Utils.GetLerpValue(0f, 1f, manaphage.Timer / jellyfishMovementInterval, true);
                spriteStretchX = MathHelper.Lerp(spriteStretchX, 1.25f, EasingFunctions.SineEaseInOut(stretchInterpolant));
                spriteStretchY = MathHelper.Lerp(spriteStretchY, 0.75f, EasingFunctions.SineEaseInOut(stretchInterpolant));

                int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 1f, stretchInterpolant));
                manaphage.UpdateAnimationFrames(default, 0f, frameY);

                // Pick a random angle to move towards before propelling forward.
                if (!manaphage.FoundValidRotationAngle)
                {
                    // Set a random movement angle initially.
                    if (manaphage.Timer == 1 && !turnAround)
                        jellyfishMovementAngle = Main.rand.NextFloat(MathF.Tau);

                    // Stop searching for valid rotation angles once the Manaphage no longer needs to turn around.
                    if (!turnAround)
                    {
                        (npc.ModNPC as Manaphage).FoundValidRotationAngle = true;
                        npc.netUpdate = true;
                    }

                    // Keep rotating to find a valid angle to rotate towards.
                    Vector2 centerAhead = npc.Center - Vector2.UnitY.RotatedBy(npc.rotation) * 128f;
                    jellyfishMovementAngle += -centerAhead.ToRotation() * 6f;
                }
            }

            // Move forward every few seconds.
            if (manaphage.Timer == jellyfishMovementInterval)
            {
                Vector2 velocity = Vector2.One.RotatedBy(jellyfishMovementAngle) * propulsionSpeed;
                npc.velocity = velocity;

                manaphage.UpdateAnimationFrames(default, 0f, 2);

                // Spawn some lil' visual particles everytime it ejects.
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                    Dust dust = Dust.NewDustPerfect(npc.Center, DustID.BlueFairy, dustVelocity * 5f);
                    dust.noGravity = true;
                }

                PulseRingParticle propulsionRing = new(npc.Center - npc.SafeDirectionTo(npc.Center) * 60f, npc.SafeDirectionTo(npc.Center) * -5f, Color.DeepSkyBlue, 0f, 0.3f, new Vector2(0.5f, 2f), npc.velocity.ToRotation(), 45);
                propulsionRing.SpawnCasParticle();

                // Unstretch the sprite.
                spriteStretchX = 0.8f;
                spriteStretchY = 1.25f;

                jellyfishPropulsionCount++;
            }

            if (manaphage.Timer >= jellyfishMovementInterval + 30)
            {
                float animationInterpolant = Utils.GetLerpValue(0f, 1f, (manaphage.Timer - jellyfishMovementInterval + 45) / jellyfishMovementInterval + 30, true);
                int frameY = (int)Math.Floor(MathHelper.Lerp(1f, 4f, EasingFunctions.SineEaseIn(animationInterpolant)));
                manaphage.UpdateAnimationFrames(default, 0f, frameY);
            }

            if (manaphage.Timer >= jellyfishMovementInterval + 45)
            {
                manaphage.Timer = 0f;
                manaphage.FoundValidRotationAngle = false;
                npc.netUpdate = true;
            }

            npc.velocity *= 0.96f;
            Vector2 futureVelocity = Vector2.One.RotatedBy(jellyfishMovementAngle);
            npc.rotation = npc.rotation.AngleLerp(futureVelocity.ToRotation() + 1.57f, manaphage.Timer / (float)jellyfishMovementInterval);

            // Randomly switch to the other idle AI state.
            if (jellyfishPropulsionCount >= maxPropulsions && Main.rand.NextBool(2))
                manaphage.stateMachine.TrySetCurrentState(CosmostoneShowers.ManaphageBehavior.LazeAround, [npc.whoAmI]);

            manaphage.SwitchBehavior_Attacking(npc.GetTargetData());
            manaphage.SwitchBehavior_Latching(npc.GetTargetData());
            manaphage.SwitchBehavior_Fleeing(npc.GetTargetData());
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
