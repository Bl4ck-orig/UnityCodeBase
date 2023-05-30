using System;

namespace FSM
{
    public class StateMachine<U, T> where U : struct, IConvertible where T : State<U>
    {
        public T CurrentState { get; protected set; }

        public T PreviousState { get; protected set; }

        public void Initialize(T _startingState)
        {
            CurrentState = _startingState;
            CurrentState.Enter();
        }

        public void ChangeState(T _newState)
        {
            PreviousState = CurrentState;
            CurrentState.Exit();
            CurrentState = _newState;
            CurrentState.Enter();
        }
    }
}
