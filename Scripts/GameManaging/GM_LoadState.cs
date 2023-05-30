using Debugging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManaging
{
    public class GM_LoadState : GM_DenyInputState
    {
        private AsyncOperation operation;

        public GM_LoadState(GameManager _gameManager, EGameState _stateType, EScriptGroup _scriptGroup, int _id) : base(_gameManager, _stateType, _scriptGroup, _id)
        {
        }

        #region Entering / Exiting -----------------------------------------------------------------
        /// <summary>
        /// Invokes certain events.
        /// </summary>
        protected override void Entering()
        {
            base.Entering();
            if (!GameManager.HasSceneBeenSet)
                UnityEngine.Debug.LogError("Scene has not been set correctly before entering load state!");

            GameManager.LoadingStarted?.Invoke();
            operation = SceneManager.LoadSceneAsync(GameManager.SceneToLoad);
        }

        /// <summary>
        /// Resets variables.
        /// </summary>
        protected override void Exiting()
        {
            base.Exiting();
            GameManager.LoadingFinished?.Invoke();
            GameManager.HasSceneBeenSet = false;
        }
        #endregion -----------------------------------------------------------------

        #region Updating -----------------------------------------------------------------
        /// <summary>
        /// Handles loading scenes either networked or not.
        /// </summary>
        public override void LogicUpdate()
        {
            if (operation != null && operation.isDone)
                GameManager.ChangeState(EGameState.Enter);
        }
        #endregion -----------------------------------------------------------------

        #region Events -----------------------------------------------------------------
        public void SceneLoadDone() => GameManager.ChangeState(EGameState.Enter);
        #endregion -----------------------------------------------------------------
    }
}
