using UnityEngine;

namespace StateKraft
{
    [System.Serializable]
    public partial class State : ScriptableObject
    {
        [HideInInspector] public ushort Id;
        public StateMachine StateMachine { get; private set; }
        protected GameObject gameObject;
        protected Transform transform;
#if UNITY_EDITOR
        [SerializeField, HideInInspector] private UnityEditor.Editor _editor;
#endif

        public void InternalInitialize(object owner, StateMachine stateMachine)
        {
            StateMachine = stateMachine;
            if (!(owner is MonoBehaviour mono)) return;
            gameObject = mono.gameObject;
            transform = mono.transform;
        }
        public virtual void Initialize(object owner) { }
        public virtual void Enter() { }
        public virtual void StateUpdate() { }
        public virtual void Exit() { }

        public void TransitionTo()
        {
            StateMachine.TransitionTo(this);
        }
        public void TransitionTo<T>()
        {
            StateMachine.TransitionTo<T>();
        }
        public T GetState<T>()
        {
            return StateMachine.GetState<T>();
        }
        public void ForceState()
        {
            StateMachine.ForceState(this);
        }
        public void ForceState<T>()
        {
            StateMachine.ForceState<T>();
        }
    }
}

