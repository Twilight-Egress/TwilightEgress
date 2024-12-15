using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids;
using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Globals.GlobalNPCs;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.ChunkyCometpod;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class AimlessCharging(FiniteStateMachine stateMachine, ChunkyCometpod cometpod) : EntityState<ChunkyCometpod>(stateMachine, cometpod)
    {
        public override void Enter(float[] arguments = null)
        {
            ref float maxAimlessCharges = ref Entity.NPC.TwilightEgress().ExtraAI[MaxAimlessChargesIndex];
            ref float aimlessChargeCounter = ref Entity.NPC.TwilightEgress().ExtraAI[AimlessChargeCounterIndex];

            maxAimlessCharges = Main.rand.Next(3, 6);
            aimlessChargeCounter = 0f;
            Entity.AIState = (int)CometpodBehavior.AimlessCharging;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            ref float chargeAngle = ref Entity.NPC.TwilightEgress().ExtraAI[ChargeAngleIndex];
            ref float maxAimlessCharges = ref Entity.NPC.TwilightEgress().ExtraAI[MaxAimlessChargesIndex];
            ref float aimlessChargeCounter = ref Entity.NPC.TwilightEgress().ExtraAI[AimlessChargeCounterIndex];

            NPCAimedTarget target = Entity.NPC.GetTargetData();

            int lineUpTime = 75;
            int chargeTime = 240;
            int postBonkCooldownTime = 180;
            int[] asteroids = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];

            Entity.ShouldTargetPlayers = true;
            Entity.ShouldTargetNPCs = true;

            Vector2 centerAhead = Entity.NPC.Center + Entity.NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            if (Entity.LocalAIState == 0f)
            {
                // Pick a random angle to turn towards and charge at.
                if (Entity.Timer is 1)
                    chargeAngle = Main.rand.NextFloat(MathF.Tau);

                Entity.NPC.rotation = Entity.NPC.rotation.AngleLerp(chargeAngle - MathHelper.Pi, 0.2f);
                Entity.NPC.velocity *= 0.9f;

                if (Entity.Timer >= lineUpTime)
                {
                    Entity.Timer = 0f;
                    Entity.LocalAIState = 1f;
                    Entity.NPC.netUpdate = true;
                }
            }

            if (Entity.LocalAIState == 1f)
            {
                Vector2 chargeVelocity = chargeAngle.ToRotationVector2() * 10f;
                Entity.NPC.velocity = Vector2.Lerp(Entity.NPC.velocity, chargeVelocity, 0.06f);
                Entity.NearestAsteroid = Entity.NPC.FindClosestNPC(out _, asteroids);

                void switchToCooldown(ref float aimlessChargeCounter)
                {
                    aimlessChargeCounter++;
                    Entity.Timer = 0f;
                    Entity.LocalAIState = 2f;
                    Entity.NPC.netUpdate = true;
                }

                // Simply stop if either the max time is reached or if the cometpod is outside of the space boundaries.
                if (Entity.Timer >= chargeTime || leavingSpace)
                    switchToCooldown(ref aimlessChargeCounter);

                // Bump into any tiles.
                if (Entity.NPC.collideX || Entity.NPC.collideY)
                {
                    Entity.NPC.velocity = Entity.NPC.oldVelocity * Entity.CalculateCollisionBounceSpeed(-0.42f);
                    switchToCooldown(ref aimlessChargeCounter);
                }

                // Bump into nearby cometpods if collision between the two occurs.
                if (Entity.NearestCometpod is not null && Entity.NPC.Hitbox.Intersects(Entity.NearestCometpod.Hitbox))
                {
                    Entity.NPC.velocity = Entity.NPC.DirectionFrom(Entity.NearestCometpod.Center) * Entity.CalculateCollisionBounceSpeed(0.8f);
                    Entity.NearestCometpod.velocity = Entity.NearestCometpod.DirectionFrom(Entity.NPC.Center) * Entity.CalculateCollisionBounceSpeed(1f);

                    // Stun the other cometpod.
                    if (Entity.NearestCometpod.ModNPC is ChunkyCometpod cometpod && cometpod.AIState != (float)CometpodBehavior.AimlessCharging)
                        FiniteStateMachine.SetCurrentState((int)CometpodBehavior.Starstruck, [0f]);

                    switchToCooldown(ref aimlessChargeCounter);
                }

                // Bump into the player if collision between the two occurs.
                if (!target.Invalid && target.Type == Terraria.Enums.NPCTargetType.Player && Entity.NPC.Hitbox.Intersects(target.Hitbox))
                {
                    Entity.NPC.velocity = Entity.NPC.DirectionFrom(target.Center) * Entity.CalculateCollisionBounceSpeed(0.8f);
                    target.Velocity = target.Center.DirectionFrom(Entity.NPC.Center) * Entity.CalculateCollisionBounceSpeed(2f);
                    switchToCooldown(ref aimlessChargeCounter);
                }

                // Bump into any nearby asteroids if collision between the two occurs.
                if (Entity.NearestAsteroid is not null && Entity.NPC.Hitbox.Intersects(Entity.NearestAsteroid.Hitbox))
                {
                    Entity.NPC.velocity = Entity.NPC.DirectionFrom(Entity.NearestAsteroid.Center) * Entity.CalculateCollisionBounceSpeed(0.86f);
                    Entity.NearestAsteroid.velocity = Entity.NearestAsteroid.DirectionFrom(Entity.NPC.Center) * Entity.CalculateCollisionBounceSpeed(1f);

                    int damageTaken = (int)(Main.rand.Next(1, 3) * Entity.NPC.velocity.Length());
                    Entity.NPC.SimpleStrikeNPC(damageTaken, Entity.NPC.direction, noPlayerInteraction: true);
                    Entity.NearestAsteroid.SimpleStrikeNPC(damageTaken * 8, -Entity.NPC.direction, noPlayerInteraction: true);
                    switchToCooldown(ref aimlessChargeCounter);
                }
            }

            if (Entity.LocalAIState == 2f)
            {
                Entity.NPC.rotation += Entity.NPC.velocity.X * 0.03f;
                Entity.NPC.velocity *= 0.98f;

                if (Entity.Timer >= postBonkCooldownTime)
                {
                    if (aimlessChargeCounter >= maxAimlessCharges || leavingSpace)
                    {
                        FiniteStateMachine.SetCurrentState((int)CometpodBehavior.PassiveWandering, [0f]);
                    }
                    else
                    {
                        Entity.Timer = 0f;
                        Entity.LocalAIState = 0f;
                        Entity.NPC.netUpdate = true;
                    }
                }
            }
        }
    }
}
