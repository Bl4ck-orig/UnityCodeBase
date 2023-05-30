using System;
using Debugging;

namespace FSM
{
    public interface IStateFactory<T> where T : struct, IConvertible
    {
        public State<T> CreateState(object _stateMachineSubject, T _stateType, EScriptGroup _scriptGroup, int _id);
    }
}
