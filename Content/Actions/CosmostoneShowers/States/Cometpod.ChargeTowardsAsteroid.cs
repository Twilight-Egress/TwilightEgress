using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TwilightEgress.Content.NPCs.CosmostoneShowers;
using TwilightEgress.Core.Behavior;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.ChunkyCometpod;

namespace TwilightEgress.Content.Actions.CosmostoneShowers.States
{
    public class ChargeTowardsAsteroid(FiniteStateMachine stateMachine, ChunkyCometpod cometpod) : EntityState<ChunkyCometpod>(stateMachine, cometpod)
    {
        public override void Enter(float[] arguments = null)
        {
            Entity.AIState = (int)CometpodBehavior.ChargeTowardsAsteroid;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPCAimedTarget target = Entity.NPC.GetTargetData();

            if (target.Invalid || Entity.NearestAsteroid is null)
            {
                FiniteStateMachine.SetCurrentState((int)CometpodBehavior.Starstruck, [0f]);
                return;
            }

            int lineUpTime = 75;
            int chargeTime = 240;

            Vector2 centerAhead = Entity.NPC.Center + Entity.NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            if (Entity.LocalAIState == 0f)
            {
                Entity.NPC.rotation = Entity.NPC.rotation.AngleLerp(Entity.NPC.AngleTo(target.Center) - MathHelper.Pi, 0.2f);
                Entity.NPC.velocity *= 0.9f;

                if (Entity.Timer >= lineUpTime)
                {
                    Entity.Timer = 0f;
                    Entity.LocalAIState = 1f;
                    Entity.NPC.velocity = Entity.NPC.SafeDirectionTo(target.Center);
                    Entity.NPC.netUpdate = true;
                }
            }

            if (Entity.LocalAIState == 1f)
            {
                Entity.NPC.rotation = Entity.NPC.velocity.ToRotation() - MathHelper.Pi;

                if (Entity.NPC.velocity.Length() < 10f)
                    Entity.NPC.velocity *= 1.06f;

                // Bounce off of the target when collision is made.
                if (Entity.NPC.Hitbox.Intersects(target.Hitbox))
                {
                    Entity.NPC.velocity = Entity.NPC.DirectionFrom(target.Center) * Entity.CalculateCollisionBounceSpeed(0.86f);
                    target.Velocity = target.Center.DirectionFrom(Entity.NPC.Center) * Entity.CalculateCollisionBounceSpeed(1f);

                    int damageTaken = (int)(Main.rand.Next(1, 3) * Entity.NPC.velocity.Length());
                    Entity.NPC.SimpleStrikeNPC(damageTaken, Entity.NPC.direction, noPlayerInteraction: true);
                    Entity.NearestAsteroid.SimpleStrikeNPC(damageTaken * 8, -Entity.NPC.direction, noPlayerInteraction: true);

                    Entity.NearestAsteroid = null;
                    FiniteStateMachine.SetCurrentState((int)CometpodBehavior.Starstruck, [0f]);
                }

                // If no collision is made or the Cometpod is out of bounds, simply switch AI states.
                if (leavingSpace || Entity.Timer >= chargeTime || Entity.NPC.collideX || Entity.NPC.collideY)
                {
                    if (Entity.NPC.collideX || Entity.NPC.collideY)
                        Entity.NPC.velocity = Entity.NPC.oldVelocity * Entity.CalculateCollisionBounceSpeed(-0.42f);

                    FiniteStateMachine.SetCurrentState((int)CometpodBehavior.Starstruck, [0f]);
                }

                // Bounce off of other Cometpods.
                if (Entity.NearestCometpod is not null && Entity.NPC.Hitbox.Intersects(Entity.NearestCometpod.Hitbox))
                {
                    Entity.NPC.velocity = Entity.NPC.oldVelocity * -0.8f;
                    Entity.NearestCometpod.velocity = Entity.NearestCometpod.DirectionFrom(Entity.NPC.Center) * Entity.NPC.velocity.Length();

                    FiniteStateMachine.SetCurrentState((int)CometpodBehavior.Starstruck, [0f]);
                }
            }
        }
    }
}
