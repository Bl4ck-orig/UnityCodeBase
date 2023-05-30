using Debugging;

namespace GameManaging
{
    public class GM_ExitState : GM_DenyInputState
    {
        public GM_ExitState(GameManager _gameManager, EGameState _stateType, EScriptGroup _scriptGroup, int _id) : base(_gameManager, _stateType, _scriptGroup, _id)
        {
        }



        /// <summary>
        /// Starts the crossfade animation for exiting and inbvkes an event in some cases.
        /// </summary>
        protected override void Entering()
        {
            base.Entering();
            GameManager.EnterExitState?.Invoke();
        }
    }
}
