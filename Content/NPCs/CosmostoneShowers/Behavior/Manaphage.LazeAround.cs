using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using TwilightEgress.Core;
using TwilightEgress.Core.Behavior;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.Manaphage;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class LazeAround(FiniteStateMachine stateMachine, Manaphage manaphage) : EntityState<Manaphage>(stateMachine, manaphage)
    {
        public override void Enter(float[] arguments = null)
        {
            Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);

            Entity.LazeMovementInterval = Main.rand.Next(360, 720);
            Entity.IdleMovementDirection = Main.rand.NextBool().ToDirectionInt();
            Entity.NPC.velocity += velocity;
            Entity.NPC.ai[0] = (int)ManaphageBehavior.LazeAround;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            int idleSwitchInterval = 1800;
            Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.1f, 0.13f);

            Entity.CheckForTurnAround(out bool turnAround);
            if (turnAround)
            {
                Vector2 circularArea = Entity.NPC.Center + Entity.NPC.velocity.RotatedBy(MathHelper.TwoPi);
                Vector2 turnAroundVelocity = circularArea.Y >= Main.maxTilesY + 750f ? Vector2.UnitY * -0.15f : circularArea.Y < Main.maxTilesY * 0.34f ? Vector2.UnitY * 0.15f : Entity.NPC.velocity;

                float distanceFromTileCollisionLeft = Utilities.DistanceToTileCollisionHit(Entity.NPC.Center, Entity.NPC.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                float distanceFromTileCollisionRight = Utilities.DistanceToTileCollisionHit(Entity.NPC.Center, Entity.NPC.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                int directionToMove = (distanceFromTileCollisionLeft > distanceFromTileCollisionRight).ToDirectionInt();
                if (distanceFromTileCollisionLeft <= 150 || distanceFromTileCollisionRight <= 150f)
                    turnAroundVelocity = Vector2.One.RotatedBy(MathHelper.PiOver2 * directionToMove) * 0.15f;

                Entity.NPC.velocity = turnAroundVelocity;
            }

            if (Entity.Timer % Entity.LazeMovementInterval == 0 && !turnAround)
                Entity.NPC.velocity += velocity;

            if (Entity.NPC.velocity.Length() > 0.13f)
                Entity.NPC.velocity *= 0.98f;

            Entity.NPC.rotation *= 0.98f;

            // Randomly switch to the other idle AI state.
            if (Entity.Timer >= idleSwitchInterval && Main.rand.NextBool(2))
                FiniteStateMachine.SetCurrentState((int)ManaphageBehavior.JellyfishPropulsion);

            // Squash and stretch the sprite passively.
            Entity.SpriteStretchX = MathHelper.Lerp(1f, 1.10f, EasingFunctions.SineEaseInOut(Entity.Timer / 60f));
            Entity.SpriteStretchY = MathHelper.Lerp(1f, 0.8f, EasingFunctions.SineEaseInOut(Entity.Timer / 120f));

            Entity.UpdateAnimationFrames(default, 10f);

            Entity.SwitchBehavior_Attacking(Entity.NPC.GetTargetData());
            Entity.SwitchBehavior_Latching(Entity.NPC.GetTargetData());
            Entity.SwitchBehavior_Fleeing(Entity.NPC.GetTargetData());
        }
    }
}
