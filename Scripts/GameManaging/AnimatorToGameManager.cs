using UnityEngine;

namespace GameManaging
{
    public class AnimatorToGameManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        public void OnEnterFinished() => gameManager.OnEnterFinished();

        public void OnExitFinished() => gameManager.OnExitFinished();
    }
}
