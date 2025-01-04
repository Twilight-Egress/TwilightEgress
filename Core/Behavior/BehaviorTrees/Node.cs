using System.Collections.Generic;

namespace TwilightEgress.Core.Behavior.BehaviorTrees
{
    public abstract class Node
    {
        private NodeState State { get; set; }
        public Node Parent;

        private List<Node> Children = new List<Node>();

        public virtual NodeState Update(int whoAmI) => NodeState.Failure;

        public Node()
        {

        }

        public Node(List<Node> children)
        {
            foreach (Node childNode in children)
            {
                childNode.Parent = this;
                Children.Add(childNode);
            }
        }
    }
}
