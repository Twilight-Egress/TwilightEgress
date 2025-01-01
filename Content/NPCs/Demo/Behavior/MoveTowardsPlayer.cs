using Terraria;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.NPCs.Demo.Behavior
{
    public class MoveTowardsPlayer : Node
    {
        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            npc.TargetClosest();

            npc.velocity = Main.player[npc.target].Center - npc.Center;
            npc.velocity.Normalize();
            npc.velocity *= 4f;

            return NodeState.InProgress;
        }
    }
}
