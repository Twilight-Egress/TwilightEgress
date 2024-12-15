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
    public class SprayingInk<ManaphageBehavior> : State<ManaphageBehavior>
    {
        public SprayingInk(ManaphageBehavior id) : base(id)
        {
        }

        public override void Enter(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];

            npc.ai[0] = (int)CosmostoneShowers.ManaphageBehavior.SprayingInk;
            npc.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            NPC npc = Main.npc[(int)arguments[0]];
            Manaphage manaphage = npc.ModNPC as Manaphage;
            NPCAimedTarget target = npc.GetTargetData();

            ref float additionalAggroRange = ref npc.TwilightEgress().ExtraAI[AdditionalAggroRangeIndex];
            ref float manaSuckTimer = ref npc.TwilightEgress().ExtraAI[ManaSuckTimerIndex];
            ref float spriteStretchX = ref npc.TwilightEgress().ExtraAI[SpriteStretchXIndex];
            ref float spriteStretchY = ref npc.TwilightEgress().ExtraAI[SpriteStretchYIndex];

            float aggroRange = 400f + additionalAggroRange;
            bool targetOutOfRange = npc.Distance(target.Center) >= aggroRange;
            if (target.Invalid || target.Type != Terraria.Enums.NPCTargetType.Player || targetOutOfRange)
            {
                CosmostoneShowers.ManaphageBehavior randomIdleState = Utils.SelectRandom(Main.rand, CosmostoneShowers.ManaphageBehavior.JellyfishPropulsion, CosmostoneShowers.ManaphageBehavior.LazeAround);
                manaphage.stateMachine.TrySetCurrentState(randomIdleState, [npc.whoAmI]);
                return;
            }

            manaphage.ShouldTargetNPCs = false;
            manaSuckTimer = manaphage.ManaRatio > 0.5f ? MaxManaSuckTimerOverFifty : MaxManaSuckTimerUnderFifty;

            npc.rotation = npc.rotation.AngleLerp(npc.AngleTo(target.Center) - 1.57f, 0.2f);
            Vector2 areaAroundPlayer = target.Center + npc.DirectionFrom(target.Center) * 250f;
            // Increase the Manaphage's movement speed depending on how much additional aggro range
            // it has ammased.
            float movementSpeed = MathHelper.Lerp(2f, 6f, Utils.GetLerpValue(0f, 1f, additionalAggroRange / 500f, true));
            npc.SimpleMove(areaAroundPlayer, movementSpeed, 20f);

            if (manaphage.Timer % 5 == 0)
            {
                manaphage.CurrentManaCapacity -= 0.25f;

                Vector2 spawnPosition = npc.Center + Vector2.UnitY.RotatedBy(npc.rotation) * 30f;
                Vector2 inkVelocity = (target.Center - npc.Center).SafeNormalize(Vector2.UnitX) * 14f + npc.velocity;

                npc.BetterNewProjectile(spawnPosition, inkVelocity, ModContent.ProjectileType<ManaInk>(), (int)(npc.defDamage * 0.45f), 0f, damageChanges: true);
            }

            manaphage.UpdateAnimationFrames(ManaphageAnimation.Attack, 5f);
            spriteStretchX = MathHelper.Lerp(1f, 0.85f, EasingFunctions.SineEaseInOut(manaphage.Timer / 10f));
            spriteStretchY = MathHelper.Lerp(0.95f, 1.05f, EasingFunctions.SineEaseInOut(manaphage.Timer / 10f));

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
