namespace TwilightEgress.Core.Behavior
{
    public class EntityState<T> : State
    {
        protected T Entity;

        public EntityState(FiniteStateMachine stateMachine, T entity) : base(stateMachine)
        {
            Entity = entity;
        }
    }
}
