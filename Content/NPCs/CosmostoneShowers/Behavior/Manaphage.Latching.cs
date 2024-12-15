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
    public class Latching<ManaphageBehavior> : State<ManaphageBehavior>
    {
        public Latching(ManaphageBehavior id) : base(id)
        {
        }

        public override void Enter(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];

            npc.ai[0] = (int)CosmostoneShowers.ManaphageBehavior.Latching;
            npc.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];
            Manaphage manaphage = npc.ModNPC as Manaphage;
            NPCAimedTarget target = npc.GetTargetData();

            ref float spriteStretchX = ref npc.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref npc.TwilightEgress().ExtraAI[SpriteStretchYIndex];
            ref float initialRotation = ref npc.TwilightEgress().ExtraAI[InitialRotationIndex];

            if (manaphage.AsteroidToSucc is null)
            {
                int[] cosmostoneAsteroidTypes = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];
                List<NPC> cosmostoneAsteroids = Main.npc.Take(Main.maxNPCs).Where(asteroid => asteroid.active && cosmostoneAsteroidTypes.Contains(asteroid.type) && npc.Distance(asteroid.Center) <= 300).ToList();
                if (cosmostoneAsteroids.Count <= 0)
                    return;

                manaphage.AsteroidToSucc = Main.npc[cosmostoneAsteroids.FirstOrDefault().whoAmI];
            }

            Rectangle asteroidHitbox = new((int)manaphage.AsteroidToSucc.position.X, (int)manaphage.AsteroidToSucc.position.Y, (int)(manaphage.AsteroidToSucc.width * 0.75f), (int)(manaphage.AsteroidToSucc.height * 0.75f));

            // Reset if the asteroid target variable is null.
            if (target.Invalid || !manaphage.AsteroidToSucc.active || manaphage.ManaRatio >= 1f)
            {
                manaphage.AsteroidToSucc = null;
                npc.velocity = Vector2.UnitY.RotatedBy(npc.rotation) * -2f;
                CosmostoneShowers.ManaphageBehavior randomIdleState = Utils.SelectRandom(Main.rand, CosmostoneShowers.ManaphageBehavior.JellyfishPropulsion, CosmostoneShowers.ManaphageBehavior.LazeAround);
                manaphage.stateMachine.TrySetCurrentState(randomIdleState, [npc.whoAmI]);
                return;
            }

            // Pre-succ.
            if (manaphage.LocalAIState == 0f)
            {
                if (manaphage.Timer >= 45)
                {
                    // Quickly move towards the selected asteroid and stop moving once the two hitboxes
                    // intersect with each other. 
                    npc.SimpleMove(manaphage.AsteroidToSucc.Center, 12f, 6f);
                    if (npc.Hitbox.Intersects(asteroidHitbox))
                    {
                        initialRotation = npc.rotation - manaphage.AsteroidToSucc.rotation;
                        manaphage.LocalAIState = 1f;
                        manaphage.Timer = 0f;
                        npc.netUpdate = true;
                    }
                }

                if (manaphage.Timer <= 15f)
                {
                    int frameY = (int)Math.Floor(MathHelper.Lerp(0f, 4f, manaphage.Timer / 15f));
                    manaphage.UpdateAnimationFrames(ManaphageAnimation.Inject, 0f, frameY);
                }

            }

            // Post-succ.
            if (manaphage.LocalAIState == 1f)
            {
                if (manaphage.Timer % 30 == 0)
                {
                    int damageToAsteroid = Main.rand.Next(10, 15);
                    manaphage.AsteroidToSucc.SimpleStrikeNPC(damageToAsteroid, 0, noPlayerInteraction: true);
                }

                Vector2 positionAroundAsteroid = manaphage.AsteroidToSucc.Center - Vector2.UnitY.RotatedBy(initialRotation + manaphage.AsteroidToSucc.rotation) * asteroidHitbox.Size();
                npc.SimpleMove(positionAroundAsteroid, 20f, 0f);

                manaphage.CurrentManaCapacity = MathHelper.Clamp(manaphage.CurrentManaCapacity + 0.1f, 0f, manaphage.MaximumManaCapacity);
                manaphage.UpdateAnimationFrames(ManaphageAnimation.Suck, 5f);
            }

            if (spriteStretchX > 1f)
                spriteStretchX *= 0.98f;
            if (spriteStretchX < 1f)
                spriteStretchX *= 1.02f;

            if (spriteStretchY > 1f)
                spriteStretchY *= 0.98f;
            if (spriteStretchY < 1f)
                spriteStretchY *= 1.02f;

            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(manaphage.AsteroidToSucc.Center) - 1.57f, 0.2f);
            manaphage.SwitchBehavior_Fleeing(target);
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
