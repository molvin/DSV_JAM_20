using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#pragma warning disable 649

namespace StateKraft
{
    [Serializable]
    public class StateMachine
    {
        [SerializeField] private State[] _states;
        private Dictionary<Type, State> _stateDictionary;
        private Dictionary<ushort, Type> _stateById;
        private Dictionary<Type, ushort> _idByState;
        private object _owner;
        public State CurrentState { get; private set; }
        public State[] UninstancedStates => _states;
        private bool _runFirstEnter = true;

        public void Initialize(object owner)
        {
            _owner = owner;
            _stateDictionary = new Dictionary<Type, State>();
            _stateById = new Dictionary<ushort, Type>();
            _idByState = new Dictionary<Type, ushort>();
            //Create copies of all states
            State firstState = null;
            ushort index = 0;
            foreach (State state in _states)
            {
                State instance = Object.Instantiate(state);
                if (firstState == null) firstState = instance;
                Type type = state.GetType();
                _stateDictionary[type] = instance;
                _stateById[index] = type;
                _idByState[type] = index;
                instance.Id = index;
                index++;
            }
            //Run init and set the state machine variable of all created states
            foreach (State state in _stateDictionary.Values)
            {
                state.InternalInitialize(owner, this);
                state.Initialize(owner);
            }
        }
        public void Update()
        {
            if (_runFirstEnter)
            {
                TransitionTo(_stateDictionary.Values.FirstOrDefault());
                _runFirstEnter = false;
            }
            
            if (CurrentState != null) 
                CurrentState.StateUpdate();
        }

        public T GetState<T>()
        {
            return (T)Convert.ChangeType(_stateDictionary[typeof(T)], typeof(T));
        }
        public State GetState(Type type)
        {
            return _stateDictionary[type];
        }

        public void TransitionTo(ushort id)
        {
            TransitionTo(GetState(_stateById[id]));
        }
        public void TransitionTo<T>()
        {
            TransitionTo(GetState<T>() as State);
        }
        public void TransitionTo(State state)
        {
            if (state == null) { Debug.LogWarning("Cannot transition to state null"); return; }
            if (CurrentState != null) CurrentState.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }

        public void ForceState(ushort id)
        {
            ForceState(GetState(_stateById[id]));
        }
        public void ForceState<T>()
        {
            ForceState(GetState<T>() as State);
        }
        public void ForceState(State state)
        {
            CurrentState = state;
        }

        public ushort GetId(State state)
        {
            return _idByState[state.GetType()];
        }
        public State GetState(ushort id)
        {
            return GetState(_stateById[id]);
        }

        public void ReinitializeState(State state)
        {
            if (_owner == null) { Debug.LogWarning("State machine has not been initialized with valid owner"); return; }

            Type type = state.GetType();
            if (!_stateDictionary.ContainsKey(type)) return;

            State instance = Object.Instantiate(state);
            instance.InternalInitialize(_owner, this);
            instance.Initialize(_owner);
            instance.Id = _idByState[type];

            if (CurrentState.GetType() == type)
                TransitionTo(instance);
            
            Object.Destroy(_stateDictionary[type]);
            _stateDictionary[type] = instance;
        }
    }
}

