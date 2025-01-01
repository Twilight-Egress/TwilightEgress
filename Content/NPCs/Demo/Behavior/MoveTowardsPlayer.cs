using Terraria;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.NPCs.Demo.Behavior
{
    public class MoveTowardsPlayer : Node
    {
        private float speed;

        public MoveTowardsPlayer(float speed)
        {
            this.speed = speed;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            npc.rotation = npc.rotation.AngleLerp(0f, 0.1f);

            npc.velocity = Main.player[npc.target].Center - npc.Center;
            npc.velocity.Normalize();
            npc.velocity *= speed;

            return NodeState.InProgress;
        }
    }
}
