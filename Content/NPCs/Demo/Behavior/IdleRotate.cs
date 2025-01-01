using Terraria;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.NPCs.Demo.Behavior
{
    public class IdleRotate : Node
    {
        private float rotationRate;
        private float velocityDamping;

        public IdleRotate(float rotationRate, float velocityDamping)
        {
            this.rotationRate = rotationRate;
            this.velocityDamping = velocityDamping;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            npc.rotation += rotationRate;
            npc.velocity *= velocityDamping;

            return NodeState.InProgress;
        }
    }
}
