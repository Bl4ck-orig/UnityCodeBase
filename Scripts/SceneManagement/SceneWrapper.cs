using System.Collections.Generic;

namespace SceneManagement
{ 
    [System.Serializable]
    public class SceneWrapper
    {
        /// <summary>
        /// Type of the scene.
        /// </summary>
        public EScene SceneType;

        /// <summary>
        /// List of all scenes.
        /// </summary>
        public List<SceneDetails> Scenes;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_sceneType">Type of the scene</param>
        /// <param name="_scenes">Details about the scene</param>
        public SceneWrapper(EScene _sceneType, List<SceneDetails> _scenes)
        {
            SceneType = _sceneType;
            Scenes = _scenes;
        }

        public override string ToString()
        {
            string toString = SceneType.ToString() + ": ";
            Scenes.ForEach(x => toString += x.ToString());
            return toString;
        }
    }
}