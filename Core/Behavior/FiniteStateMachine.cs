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

        public void Update(float[] arguments) => currentState?.Update(arguments);

        public State<T> GetState(T stateID) => states.TryGetValue(stateID, out State<T> value) ? value : null;

        public void SetCurrentState(State<T> state, float[] arguments = null)
        {
            if (currentState == state)
                return;

            currentState?.Exit(arguments);
            currentState = state;
            currentState?.Enter(arguments);
        }

        public void TrySetCurrentState(T stateID, float[] arguments = null)
        {
            if (stateID.Equals(currentState.ID))
                return;

            if (states.TryGetValue(stateID, out State<T> value)) ;
            SetCurrentState(value, arguments);
        }
    }

    public abstract class State<T>
    {
        public string Name { get; set; }

        public T ID { get; private set; }

        public delegate void DelegateNoArguments();
        public delegate void DelegateMethod(float[] arguments);

        public DelegateMethod OnEnter;
        public DelegateMethod OnExit;
        public DelegateMethod OnUpdate;

        public State(T id)
        {
            ID = id;
        }

        public State(T id, string name) : this(id)
        {
            Name = name;
        }

        public State(T id, DelegateMethod onEnter, DelegateMethod onExit = null, DelegateMethod onUpdate = null) : this(id)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
        }

        public State(T id, string name, DelegateMethod onEnter, DelegateMethod onExit = null, DelegateMethod onUpdate = null) : this(id, name)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
        }

        public virtual void Enter(float[] arguments = null) => OnEnter?.Invoke(arguments);

        public virtual void Exit(float[] arguments = null) => OnExit?.Invoke(arguments);

        public virtual void Update(float[] arguments = null) => OnUpdate?.Invoke(arguments);
    }
}
