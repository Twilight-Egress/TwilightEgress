using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TwilightEgress.Core;
using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Globals.GlobalNPCs;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.Manaphage;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    public class SprayingInk(FiniteStateMachine stateMachine, Manaphage manaphage) : EntityState<Manaphage>(stateMachine, manaphage)
    {
        public override void Enter(float[] arguments = null)
        {
            Entity.NPC.ai[0] = (int)ManaphageBehavior.SprayingInk;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPCAimedTarget target = Entity.NPC.GetTargetData();

            ref float additionalAggroRange = ref Entity.NPC.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];
            ref float manaSuckTimer = ref Entity.NPC.TwilightEgress().ExtraAI[ManaSuckTimerIndex];
            ref float spriteStretchX = ref Entity.NPC.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref Entity.NPC.TwilightEgress().ExtraAI[SpriteStretchYIndex];

            float aggroRange = 400f + additionalAggroRange;
            bool targetOutOfRange = Entity.NPC.Distance(target.Center) >= aggroRange;
            if (target.Invalid || target.Type != Terraria.Enums.NPCTargetType.Player || targetOutOfRange)
            {
                ManaphageBehavior randomIdleState = Utils.SelectRandom(Main.rand, ManaphageBehavior.JellyfishPropulsion, ManaphageBehavior.LazeAround);
                FiniteStateMachine.SetCurrentState((int)randomIdleState, [Entity.NPC.whoAmI]);
                return;
            }

            Entity.ShouldTargetNPCs = false;
            manaSuckTimer = Entity.ManaRatio > 0.5f ? MaxManaSuckTimerOverFifty : MaxManaSuckTimerUnderFifty;

            Entity.NPC.rotation = Entity.NPC.rotation.AngleLerp(Entity.NPC.AngleTo(target.Center) - 1.57f, 0.2f);
            Vector2 areaAroundPlayer = target.Center + Entity.NPC.DirectionFrom(target.Center) * 250f;
            // Increase the Manaphage's movement speed depending on how much additional aggro range
            // it has ammased.
            float movementSpeed = MathHelper.Lerp(2f, 6f, Utils.GetLerpValue(0f, 1f, additionalAggroRange / 500f, true));
            Entity.NPC.SimpleMove(areaAroundPlayer, movementSpeed, 20f);

            if (Entity.Timer % 5 == 0)
            {
                Entity.CurrentManaCapacity -= 0.25f;

                Vector2 spawnPosition = Entity.NPC.Center + Vector2.UnitY.RotatedBy(Entity.NPC.rotation) * 30f;
                Vector2 inkVelocity = (target.Center - Entity.NPC.Center).SafeNormalize(Vector2.UnitX) * 14f + Entity.NPC.velocity;

                Entity.NPC.BetterNewProjectile(spawnPosition, inkVelocity, ModContent.ProjectileType<ManaInk>(), (int)(Entity.NPC.defDamage * 0.45f), 0f, damageChanges: true);
            }

            Entity.UpdateAnimationFrames(ManaphageAnimation.Attack, 5f);
            spriteStretchX = MathHelper.Lerp(1f, 0.85f, EasingFunctions.SineEaseInOut(Entity.Timer / 10f));
            spriteStretchY = MathHelper.Lerp(0.95f, 1.05f, EasingFunctions.SineEaseInOut(Entity.Timer / 10f));

            Entity.SwitchBehavior_Fleeing(target);
        }
    }
}
