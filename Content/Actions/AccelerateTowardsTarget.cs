using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions
{
    public class AccelerateTowardsTarget : Node
    {
        private float acceleration;
        private float maxSpeed;

        public AccelerateTowardsTarget(float acceleration, float maxSpeed)
        {
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            Vector2 targetCenter = npc.HasNPCTarget ? Main.npc[npc.TranslatedTargetIndex].Center : Main.player[npc.target].Center;

            Vector2 accelerationVector = targetCenter - npc.Center;
            accelerationVector.Normalize();
            accelerationVector *= acceleration;

            npc.velocity += accelerationVector;
            npc.velocity = npc.velocity.ClampLength(0, maxSpeed);

            return NodeState.InProgress;
        }
    }
}
