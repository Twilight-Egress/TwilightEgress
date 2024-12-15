using System.Collections.Generic;

namespace TwilightEgress.Core.Behavior
{
    public class FiniteStateMachine<T>
    {
        protected Dictionary<T, State<T>> states;
        protected State<T> currentState;

        public FiniteStateMachine()
        {
            states = [];
        }

        public void Add(State<T> state) => states.Add(state.ID, state);

        public void Add(T stateID, State<T> state) => states.Add(stateID, state);

        public void Update(T[] arguments) => currentState?.Update(arguments);

        public State<T> GetState(T stateID) => states.TryGetValue(stateID, out State<T> value) ? value : null;

        public void SetCurrentState(State<T> state)
        {
            if (currentState == state)
                return;

            currentState?.Exit();
            currentState = state;
            currentState?.Enter();
        }

        public void TrySetCurrentState(T stateID)
        {
            if (stateID.Equals(currentState.ID))
                return;

            if (states.TryGetValue(stateID, out State<T> value)) ;
            SetCurrentState(value);
        }
    }

    public abstract class State<T>
    {
        public string Name { get; set; }

        public T ID { get; private set; }

        public delegate void DelegateNoArguments();
        public delegate void DelegateMethod(T[] arguments);

        public DelegateNoArguments OnEnter;
        public DelegateNoArguments OnExit;
        public DelegateMethod OnUpdate;

        public State(T id)
        {
            ID = id;
        }

        public State(T id, string name) : this(id)
        {
            Name = name;
        }

        public State(T id, DelegateNoArguments onEnter, DelegateNoArguments onExit = null, DelegateMethod onUpdate = null) : this(id)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
        }

        public State(T id, string name, DelegateNoArguments onEnter, DelegateNoArguments onExit = null, DelegateMethod onUpdate = null) : this(id, name)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
        }

        public virtual void Enter() => OnEnter?.Invoke();

        public virtual void Exit() => OnExit?.Invoke();

        public virtual void Update(T[] arguments = null) => OnUpdate?.Invoke(arguments);
    }
}
