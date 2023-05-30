using Debugging;

namespace GameManaging
{
    public class GM_EnterState : GM_DenyInputState
    {
        private const int WAIT_FOR_START_FRAMES = 3;

        private int framesInState = 0;

        public GM_EnterState(GameManager _gameManager, EGameState _stateType, EScriptGroup _scriptGroup, int _id) : base(_gameManager, _stateType, _scriptGroup, _id)
        {
        }

        protected override void Entering()
        {
            base.Entering();
            framesInState = 0;
            
        }

        public override void LogicUpdate()
        {
            if (framesInState++ != WAIT_FOR_START_FRAMES)
                return;

            GameManager.EnterEnterState?.Invoke();
        }
    }
}
