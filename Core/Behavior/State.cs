﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightEgress.Core.Behavior
{
    public abstract class State
    {
        protected FiniteStateMachine FiniteStateMachine { get; private set; }

        public State(FiniteStateMachine stateMachine)
        {
            FiniteStateMachine = stateMachine;
        }

        public delegate void StateDelegate(float[] arguments);
        public StateDelegate OnEnter { get; set; } = null;
        public StateDelegate OnExit { get; set; } = null;
        public StateDelegate OnUpdate { get; set; } = null;


        public virtual void Enter(float[] arguments = null) => OnEnter?.Invoke(arguments);

        public virtual void Exit(float[] arguments = null) => OnExit?.Invoke(arguments);

        public virtual void Update(float[] arguments = null) => OnUpdate?.Invoke(arguments);
    }
}
