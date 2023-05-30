using Debugging;

namespace GameManaging
{
    public class GM_RunState : GM_AllowInputState
    {
        public GM_RunState(GameManager _gameManager, EGameState _stateType, EScriptGroup _scriptGroup, int _id) : base(_gameManager, _stateType, _scriptGroup, _id)
        {
        }
    }
}
