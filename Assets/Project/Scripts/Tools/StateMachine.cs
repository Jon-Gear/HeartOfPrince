using UnityEngine;

namespace Tools
{
    public abstract class State
    {
        protected StateMachine stateMachine;

        public State(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void OnUpdate();
    }

    public class StateMachine : MonoBehaviour
    {
        private State currentState;

        public void SetState(State newState)
        {
            if (currentState == newState) return;

            currentState?.OnExit();
            currentState = newState;
            currentState?.OnEnter();
        }

        private void Update()
        {
            currentState?.OnUpdate();
        }
        public State GetCurrentState() => currentState;
    }

}

