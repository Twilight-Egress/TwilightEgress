using TwilightEgress.Core.Behavior;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions
{
    public class RunStateMachine : Node
    {
        private FiniteStateMachine stateMachine;

        public RunStateMachine(FiniteStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public override NodeState Update(int whoAmI)
        {
            stateMachine?.Update();

            return NodeState.Success;
        }
    }
}
