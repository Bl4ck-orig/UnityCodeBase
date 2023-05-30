using System.Collections.Generic;

namespace SceneManagement
{
    [System.Serializable]
    public class SceneDetails
    {
        public int Id;

        public int BakedId = -1;

        /// <summary>
        /// Dafault value.
        /// </summary>
        public static SceneDetails NULL = new SceneDetails(0, "NULL", "NULL", (EScene)0);

        /// <summary>
        /// Name of the scene.
        /// </summary>
        public string SceneName;

        /// <summary>
        /// Path to the scene.
        /// </summary>
        public string ScenePath;

        /// <summary>
        /// Type of the scene.
        /// </summary>
        public EScene SceneType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_sceneName">Name of the scene</param>
        /// <param name="_scenePath">Path to the scene</param>
        /// <param name="_sceneType">Type of the scene</param>
        public SceneDetails(int _id, string _sceneName, string _scenePath, EScene _sceneType)
        {
            Id = _id;
            SceneName = _sceneName;
            ScenePath = _scenePath;
            SceneType = _sceneType;
        }

        /// <summary>
        /// Checks if the scene is null.
        /// </summary>
        /// <returns>True if it is null</returns>
        public bool IsNULL() => SceneName == NULL.SceneName && ScenePath == NULL.ScenePath && SceneType == NULL.SceneType;

        public override bool Equals(object obj)
        {
            SceneDetails other = obj as SceneDetails;
            if (other == null)
                return false;

            return SceneName == other.SceneName && ScenePath == other.ScenePath && SceneType == other.SceneType;
        }

        public override int GetHashCode()
        {
            int hashCode = -1055545897;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SceneName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ScenePath);
            hashCode = hashCode * -1521134295 + SceneType.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            string bakeString = (BakedId == -1) ? " - not baked " : " - baked id: " + BakedId;
            return "[" + Id + "]" + SceneName + ": type: "  + SceneType + bakeString + ", path: " + ScenePath;
        }
    }
}