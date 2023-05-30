using System;
using System.Collections.Generic;
using System.Linq;
using Debugging;

namespace FSM
{
    public class StateMachineHandler<T> where T : struct, IConvertible
    {
   
        public State<T> CurrentState { get => stateMachine.CurrentState; } 

        public T CurrentStateType { get => CurrentState.StateType; }

        public EScriptGroup ScriptGroup { get; private set; }

        public int Id { get; private set; }

        private T standardState;
        private List<T> states; 
        private T startState;

        private Dictionary<T, State<T>> statesDict;
        private Dictionary<Type, State<T>> statesTypeDict;
        protected StateMachine<T, State<T>> stateMachine;
        protected Queue<State<T>> StatesQueue;
        protected bool initialized = false;

        #region Initilization -----------------------------------------------------------------
        public StateMachineHandler(IStateFactory<T> _stateFactory, object _stateMachineSubject, 
            EScriptGroup _scriptGroup, int _id, T _standardState, List<T> _states, T _startState)
        {
            SetupVariables(_id, _scriptGroup, _standardState, _states, _startState);

            InitializeStates(_stateFactory, _stateMachineSubject, _scriptGroup, _id);

            initialized = true;
        }

        private void SetupVariables(int _id, EScriptGroup _scriptGroup, T _standardState, List<T> _states, T _startState)
        {
            Id = _id;
            ScriptGroup = _scriptGroup;
            standardState = _standardState;
            states = _states;
            startState = _startState;
            stateMachine = new StateMachine<T, State<T>>();
        }
    
        private void InitializeStates(IStateFactory<T> _stateFactory, object _stateMachineSubject, EScriptGroup _scriptGroup, int _id)
        {
            StatesQueue = new Queue<State<T>>();
            statesDict = new Dictionary<T, State<T>>();
            statesTypeDict = new Dictionary<Type, State<T>>();

            foreach (T state in states)
            {
                if (!statesDict.ContainsKey(state))
                {
                    statesDict.Add(state, _stateFactory.CreateState(_stateMachineSubject, state, _scriptGroup, _id));
                    statesTypeDict.Add(statesDict[state].GetType(), statesDict[state]);
                }
            }
        }

        public void StartStateMachine()
        {
            stateMachine.Initialize(statesDict[startState]);
        }
        #endregion -----------------------------------------------------------------

        #region Updating -----------------------------------------------------------------
        public void Update() => stateMachine.CurrentState.LogicUpdate();

        public void FixedUpdate() => stateMachine.CurrentState.PhysicsUpdate();
        #endregion -----------------------------------------------------------------

        #region Changing States -----------------------------------------------------------------
        public virtual void ChangeState(T _state)
        {
            if (statesDict.ContainsKey(_state))
            {
                if (stateMachine.CurrentState != statesDict[_state])
                {
                    stateMachine.ChangeState(statesDict[_state]);
                }
                return;
            }
            
            DebugManager.OutputWarning(ScriptGroup, this, "Trying to change to state " + _state + " which is not known!");
        }

        public virtual void ChangeStateIgnoreCurrentState(T _state)
        {
            if (statesDict.ContainsKey(_state))
            {
                stateMachine.ChangeState(statesDict[_state]);
                return;
            }

            DebugManager.OutputWarning(ScriptGroup, this, "Trying to change to state " + _state + " which is not known!");
        }

        public void ChangeState(State<T> _state) => ChangeState(_state.StateType);

        public void ChangeToNextState()
        {
            if (StatesQueue.Count > 0)
                ChangeState(StatesQueue.Dequeue());
            else
                ChangeState(standardState);
        }

        public void SetStandardState(T _state) => standardState = _state;
        #endregion -----------------------------------------------------------------

        #region Queueing -----------------------------------------------------------------
        public void EnqueueState(T _state)
        {
            if (statesDict.ContainsKey(_state))
                StatesQueue.Enqueue(statesDict[_state]);
            else
                DebugManager.OutputWarning(ScriptGroup, this, "Trying to enqueue state " + _state + " which is not known!");
        }

        public void ClearStatesQueue()
        {
            DebugManager.Output(ScriptGroup, this, "Id = " + Id + " Resetting states queue");
            StatesQueue.Clear();
        }

        public void ClearStatesQueueAndReset()
        {
            ClearStatesQueue();
            ChangeToNextState();
        }
        #endregion -----------------------------------------------------------------

        #region Getting Values -----------------------------------------------------------------
        public V GetState<V>(T _stateType) where V : State<T> => (V)statesDict[_stateType];

        public V GetState<V>() where V : State<T> => (V)statesTypeDict[typeof(V)];

        public bool IsInState(params T[] _states) => _states.Contains(CurrentState.StateType);
        #endregion -----------------------------------------------------------------


    }
}
