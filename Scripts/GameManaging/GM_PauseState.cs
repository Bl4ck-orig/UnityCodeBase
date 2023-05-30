using Debugging;
using UnityEngine;

namespace GameManaging
{
    public class GM_PauseState: GameState
    {
        public GM_PauseState(GameManager _gameManager, EGameState _stateType, EScriptGroup _scriptGroup, int _id) : base(_gameManager, _stateType, _scriptGroup, _id)
        {
        }


        #region Entering / Exiting -----------------------------------------------------------------
        protected override void Entering()
        {
            Time.timeScale = 0;
        }

        protected override void Exiting()
        {
            Time.timeScale = 1;
        }
        #endregion -----------------------------------------------------------------
    }
}
