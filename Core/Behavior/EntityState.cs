namespace TwilightEgress.Core.Behavior
{
    public class EntityState<T>(FiniteStateMachine stateMachine, T entity) : State(stateMachine)
    {
        protected T Entity = entity;
    }
}
