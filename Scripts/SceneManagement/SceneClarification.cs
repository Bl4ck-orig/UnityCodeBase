using GameManaging;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneClarification : MonoBehaviour
    {
        public static Action<EScene, string> ClarifyScene { get; set; }

        [SerializeField] private EScene scene;
        [SerializeField] private bool ignoreFromBuild = false;

        public static EScene ActiveScene;

        /// <summary>
        /// Invokes the clarification method.
        /// </summary>
        private void Start()
        {
            ActiveScene = scene;

            if (!ignoreFromBuild)
                SO_SceneConfig.UpdateScene(scene, SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().path);

            ClarifyScene?.Invoke(scene, SceneManager.GetActiveScene().name);
        }
    }
}

