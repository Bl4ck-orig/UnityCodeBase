using Debugging;
using Inputs;

namespace GameManaging
{
    public class GM_DenyInputState : GameState
    {
        public GM_DenyInputState(GameManager _gameManager, EGameState _stateType, EScriptGroup _scriptGroup, int _id) : base(_gameManager, _stateType, _scriptGroup, _id)
        {
        }

        protected override void Entering()
        {
            InputManager.OnDisableInputs?.Invoke();
        }
    }
}
