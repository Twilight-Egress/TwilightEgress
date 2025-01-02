using Microsoft.Xna.Framework;
using Terraria;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions
{
    public class MoveTowardsTarget : Node
    {
        private float speed;

        public MoveTowardsTarget(float speed)
        {
            this.speed = speed;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            Vector2 targetCenter = npc.HasNPCTarget ? Main.npc[npc.TranslatedTargetIndex].Center : Main.player[npc.target].Center;

            npc.velocity = targetCenter - npc.Center;
            npc.velocity.Normalize();
            npc.velocity *= speed;

            return NodeState.InProgress;
        }
    }
}
