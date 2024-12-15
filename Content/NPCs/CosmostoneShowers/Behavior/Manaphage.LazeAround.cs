using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TwilightEgress.Core;
using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Globals.GlobalNPCs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class LazeAround<ManaphageBehavior> : State<ManaphageBehavior>
    {
        public LazeAround(ManaphageBehavior id) : base(id)
        {
        }

        public override void Enter(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];

            ref float lazeMovementInterval = ref npc.TwilightEgress().ExtraAI[Manaphage.LazeMovementIntervalIndex];
            ref float idleMovementDirection = ref npc.TwilightEgress().ExtraAI[Manaphage.IdleMovementDirectionIndex];

            Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);

            lazeMovementInterval = Main.rand.Next(360, 720);
            idleMovementDirection = Main.rand.NextBool().ToDirectionInt();
            npc.velocity += velocity;
            npc.ai[0] = (int)CosmostoneShowers.ManaphageBehavior.LazeAround;
            npc.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];
            Manaphage manaphage = npc.ModNPC as Manaphage;

            ref float lazeMovementInterval = ref npc.TwilightEgress().ExtraAI[Manaphage.LazeMovementIntervalIndex];
            ref float idleMovementDirection = ref npc.TwilightEgress().ExtraAI[Manaphage.IdleMovementDirectionIndex];
            ref float spriteStretchX = ref npc.TwilightEgress().ExtraAI[Manaphage.SpriteStretchXIndex];
            ref float spriteStretchY = ref npc.TwilightEgress().ExtraAI[Manaphage.SpriteStretchYIndex];

            int idleSwitchInterval = 1800;
            Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);

            manaphage.CheckForTurnAround(out bool turnAround);
            if (turnAround)
            {
                Vector2 circularArea = npc.Center + npc.velocity.RotatedBy(MathHelper.TwoPi);
                Vector2 turnAroundVelocity = circularArea.Y >= Main.maxTilesY + 750f ? Vector2.UnitY * -0.15f : circularArea.Y < Main.maxTilesY * 0.34f ? Vector2.UnitY * 0.15f : npc.velocity;

                float distanceFromTileCollisionLeft = Utilities.DistanceToTileCollisionHit(npc.Center, npc.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                float distanceFromTileCollisionRight = Utilities.DistanceToTileCollisionHit(npc.Center, npc.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                int directionToMove = (distanceFromTileCollisionLeft > distanceFromTileCollisionRight).ToDirectionInt();
                if (distanceFromTileCollisionLeft <= 150 || distanceFromTileCollisionRight <= 150f)
                    turnAroundVelocity = Vector2.One.RotatedBy(MathHelper.PiOver2 * directionToMove) * 0.15f;

                npc.velocity = turnAroundVelocity;
            }

            if (manaphage.Timer % lazeMovementInterval == 0 && !turnAround)
                npc.velocity += velocity;

            if (npc.velocity.Length() > 0.13f)
                npc.velocity *= 0.98f;

            npc.rotation *= 0.98f;

            // Randomly switch to the other idle AI state.
            if (manaphage.Timer >= idleSwitchInterval && Main.rand.NextBool(2))
                manaphage.stateMachine.TrySetCurrentState(CosmostoneShowers.ManaphageBehavior.JellyfishPropulsion, [npc.whoAmI]);

            // Squash and stretch the sprite passively.
            spriteStretchX = MathHelper.Lerp(1f, 1.10f, EasingFunctions.SineEaseInOut(manaphage.Timer / 60f));
            spriteStretchY = MathHelper.Lerp(1f, 0.8f, EasingFunctions.SineEaseInOut(manaphage.Timer / 120f));

            manaphage.UpdateAnimationFrames(default, 10f);

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
