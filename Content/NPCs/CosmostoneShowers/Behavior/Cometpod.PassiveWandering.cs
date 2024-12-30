using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids;
using TwilightEgress.Core.Behavior;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.ChunkyCometpod;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class PassiveWandering(FiniteStateMachine stateMachine, ChunkyCometpod cometpod) : EntityState<ChunkyCometpod>(stateMachine, cometpod)
    {
        public override void Enter(float[] arguments = null)
        {
            Entity.MaxPassiveWanderingTime = Main.rand.Next(480, 1200);
            Entity.AIState = (int)CometpodBehavior.PassiveWandering;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPCAimedTarget target = Entity.NPC.GetTargetData();

            // Move slowly in a random direction every few seconds.
            Entity.PassiveMovementTimer--;
            if (Entity.PassiveMovementTimer <= 0f)
            {
                Entity.PassiveMovementSpeed = Main.rand.NextFloat(5f, 151f) * 0.01f;
                Entity.PassiveMovementVectorX = Main.rand.NextFloat(-100f, 101f);
                Entity.PassiveMovementVectorY = Main.rand.NextFloat(-100f, 101f);
                Entity.PassiveMovementTimer = Main.rand.NextFloat(120f, 360f);
                Entity.NPC.netUpdate = true;
            }

            Entity.NPC.CheckForTurnAround(-MathHelper.PiOver2, MathHelper.PiOver2, 0.05f, out bool shouldTurnAround);
            Vector2 centerAhead = Entity.NPC.Center + Entity.NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            // Avoid tiles and leaving space. 
            if (shouldTurnAround || leavingSpace)
            {
                float distanceFromTileCollisionLeft = Utilities.DistanceToTileCollisionHit(Entity.NPC.Center, Entity.NPC.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                float distanceFromTileCollisionRight = Utilities.DistanceToTileCollisionHit(Entity.NPC.Center, Entity.NPC.velocity.RotatedBy(MathHelper.PiOver2)) ?? 1000f;
                int directionToMove = distanceFromTileCollisionLeft > distanceFromTileCollisionRight ? -1 : 1;
                Vector2 turnAroundVelocity = Entity.NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 * directionToMove);
                if (leavingSpace)
                    turnAroundVelocity = centerAhead.Y >= Main.maxTilesY + 750f ? Vector2.UnitY * -3f : centerAhead.Y < Main.maxTilesY * 0.34f ? Vector2.UnitY * 3f : Entity.NPC.velocity;

                // Setting these ensures that once the turnAround check becomes false, the normal idle velocity
                // won't conflict with the turn around velocity.
                Entity.PassiveMovementVectorX = turnAroundVelocity.X;
                Entity.PassiveMovementVectorY = turnAroundVelocity.Y;

                Entity.NPC.velocity = Vector2.Lerp(Entity.NPC.velocity, turnAroundVelocity, 0.1f);
            }
            else
            {
                float moveSpeed = Entity.PassiveMovementSpeed / MathF.Sqrt(MathF.Pow(Entity.PassiveMovementVectorX, 2) + MathF.Pow(Entity.PassiveMovementVectorY, 2));
                Entity.NPC.velocity = Vector2.Lerp(Entity.NPC.velocity, new Vector2(Entity.PassiveMovementVectorX * moveSpeed, Entity.PassiveMovementVectorY * moveSpeed) * 3f, 0.02f);
            }

            Entity.ShouldTargetNPCs = true;
            Entity.ShouldTargetPlayers = true;

            // Randomly select an asteroid and switch AI states.
            int[] asteroids = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];
            if (Main.rand.NextBool(1500) && Entity.ShouldTargetNPCs && target.Type == Terraria.Enums.NPCTargetType.NPC && !target.Invalid)
            {
                Entity.NearestAsteroid = Entity.NPC.FindClosestNPC(out float _, asteroids);
                FiniteStateMachine.SetCurrentState((int)CometpodBehavior.ChargeTowardsAsteroid, [1f]);
            }

            // Randomly select a player and switch AI states.
            int playerTargetChance = (int)(1500 - Entity.PlayerTargettingChanceReduction);
            if (Main.rand.NextBool(playerTargetChance) && Entity.ShouldTargetPlayers && target.Type == Terraria.Enums.NPCTargetType.Player && !target.Invalid)
                FiniteStateMachine.SetCurrentState((int)CometpodBehavior.ChargeTowardsPlayer, [1f]);

            if (Entity.Timer >= Entity.MaxPassiveWanderingTime && Main.rand.NextBool(5))
                FiniteStateMachine.SetCurrentState((int)CometpodBehavior.AimlessCharging, [0f]);
            else
            {
                Entity.Timer = 0f;
                Entity.NPC.netUpdate = true;
            }

            Entity.NPC.rotation = Entity.NPC.rotation.AngleLerp(Entity.NPC.velocity.ToRotation() - MathHelper.Pi, 0.2f);
        }
    }
}
