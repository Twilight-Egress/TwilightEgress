using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids;
using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Globals.GlobalNPCs;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.Manaphage;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class Latching(FiniteStateMachine stateMachine, Manaphage manaphage) : EntityState<Manaphage>(stateMachine, manaphage)
    {
        public override void Enter(float[] arguments = null)
        {
            Entity.NPC.ai[0] = (int)ManaphageBehavior.Latching;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPCAimedTarget target = Entity.NPC.GetTargetData();

            ref float initialRotation = ref Entity.NPC.TwilightEgress().ExtraAI[InitialRotationIndex];

            if (Entity.AsteroidToSucc is null)
            {
                int[] cosmostoneAsteroidTypes = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];
                List<NPC> cosmostoneAsteroids = Main.npc.Take(Main.maxNPCs).Where(asteroid => asteroid.active && cosmostoneAsteroidTypes.Contains(asteroid.type) && Entity.NPC.Distance(asteroid.Center) <= 300).ToList();
                if (cosmostoneAsteroids.Count <= 0)
                    return;

                Entity.AsteroidToSucc = Main.npc[cosmostoneAsteroids.FirstOrDefault().whoAmI];
            }

            Rectangle asteroidHitbox = new((int)Entity.AsteroidToSucc.position.X, (int)Entity.AsteroidToSucc.position.Y, (int)(Entity.AsteroidToSucc.width * 0.75f), (int)(Entity.AsteroidToSucc.height * 0.75f));

            // Reset if the asteroid target variable is null.
            if (target.Invalid || !Entity.AsteroidToSucc.active || Entity.ManaRatio >= 1f)
            {
                Entity.AsteroidToSucc = null;
                Entity.NPC.velocity = Vector2.UnitY.RotatedBy(Entity.NPC.rotation) * -2f;
                ManaphageBehavior randomIdleState = Utils.SelectRandom(Main.rand, ManaphageBehavior.JellyfishPropulsion, ManaphageBehavior.LazeAround);
                FiniteStateMachine.SetCurrentState((int)randomIdleState);
                return;
            }

            // Pre-succ.
            if (Entity.LocalAIState == 0f)
            {
                if (Entity.Timer >= 45)
                {
                    // Quickly move towards the selected asteroid and stop moving once the two hitboxes
                    // intersect with each other. 
                    Entity.NPC.SimpleMove(Entity.AsteroidToSucc.Center, 12f, 6f);
                    if (Entity.NPC.Hitbox.Intersects(asteroidHitbox))
                    {
                        initialRotation = Entity.NPC.rotation - Entity.AsteroidToSucc.rotation;
                        Entity.LocalAIState = 1f;
                        Entity.Timer = 0f;
                        Entity.NPC.netUpdate = true;
                    }
                }

                if (Entity.Timer <= 15f)
                {
                    int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 4f, Entity.Timer / 15f));
                    Entity.UpdateAnimationFrames(ManaphageAnimation.Inject, 0f, frameY);
                }

            }

            // Post-succ.
            if (Entity.LocalAIState == 1f)
            {
                if (Entity.Timer % 30 == 0)
                {
                    int damageToAsteroid = Main.rand.Next(10, 15);
                    Entity.AsteroidToSucc.SimpleStrikeNPC(damageToAsteroid, 0, noPlayerInteraction: true);
                }

                Vector2 positionAroundAsteroid = Entity.AsteroidToSucc.Center - Vector2.UnitY.RotatedBy(initialRotation + Entity.AsteroidToSucc.rotation) * asteroidHitbox.Size();
                Entity.NPC.SimpleMove(positionAroundAsteroid, 20f, 0f);

                Entity.CurrentManaCapacity = MathHelper.Clamp(Entity.CurrentManaCapacity + 0.1f, 0f, Entity.MaximumManaCapacity);
                Entity.UpdateAnimationFrames(ManaphageAnimation.Suck, 5f);
            }

            if (Entity.SpriteStretchX > 1f)
                Entity.SpriteStretchX *= 0.98f;
            if (Entity.SpriteStretchX < 1f)
                Entity.SpriteStretchX *= 1.02f;

            if (Entity.SpriteStretchY > 1f)
                Entity.SpriteStretchY *= 0.98f;
            if (Entity.SpriteStretchY < 1f)
                Entity.SpriteStretchY *= 1.02f;

            Entity.NPC.rotation = Entity.NPC.rotation.AngleLerp(Entity.NPC.AngleTo(Entity.AsteroidToSucc.Center) - 1.57f, 0.2f);
            Entity.SwitchBehavior_Fleeing(target);
        }
    }
}
