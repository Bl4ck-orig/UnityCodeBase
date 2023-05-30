using Debugging;
using FSM;

namespace GameManaging
{
    public class GameManagerStateFactory : IStateFactory<EGameState>
    {

        public State<EGameState> CreateState(object _stateMachineSubject, EGameState _stateType, EScriptGroup _scriptGroup, int _id)
        {
            switch (_stateType)
            {
                case EGameState.Enter:
                    return new GM_EnterState((GameManager)_stateMachineSubject, _stateType, _scriptGroup, _id);
                case EGameState.Exit:
                    return new GM_ExitState((GameManager)_stateMachineSubject, _stateType, _scriptGroup, _id);
                case EGameState.Load:
                    return new GM_LoadState((GameManager)_stateMachineSubject, _stateType, _scriptGroup, _id);
                case EGameState.Run:
                    return new GM_RunState((GameManager)_stateMachineSubject, _stateType, _scriptGroup, _id);
                case EGameState.Pause:
                    return new GM_PauseState((GameManager)_stateMachineSubject, _stateType, _scriptGroup, _id);
                case EGameState.DenyInput:
                    return new GM_DenyInputState((GameManager)_stateMachineSubject, _stateType, _scriptGroup, _id);
                case EGameState.AllowInput:
                    return new GM_AllowInputState((GameManager)_stateMachineSubject, _stateType, _scriptGroup, _id);
                default:
                    UnityEngine.Debug.LogError("State unknown!");
                    return null;

            }
        }
    }
}
