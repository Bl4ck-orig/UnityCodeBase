using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SceneManagement
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Data/Config/Scene Config")]
    public class SO_SceneConfig : ScriptableObject
    {
        [SerializeField] private List<EScene> sceneOrder;
        [SerializeField] private List<SceneWrapper> sceneDetails;

        private static SO_SceneConfig instance;

        private SceneDetails currentScene;

        public static void SetSceneConfig(SO_SceneConfig _instance) => instance = _instance;

        private void OnEnable()
        {
            SceneClarification.ClarifyScene += SetCurrentScene;
        }

        private void OnDisable()
        {
            SceneClarification.ClarifyScene -= SetCurrentScene;
        }

        private void SetCurrentScene(EScene _sceneType, string _name) 
        {
            var scene = GetScenesOfType(_sceneType).Where(x => x.SceneName == _name);
            if (scene.Count() != 1)
                return;

            currentScene = scene.First();
        }

        #region Getting Scene Data -----------------------------------------------------------------
        public static int BakedSceneId(string _name)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            return instance.GetBakedSceneID(_name);
        }

        public static int BakedSceneId(EScene _sceneType, string _name)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            return instance.GetBakedSceneID(_sceneType, _name);
        }

        public static SceneDetails SceneByID(int _id)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            return instance.GetSceneByID(_id);
        }

        public static EScene SceneTypeByID(int _id)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            return instance.GetSceneTypeByID(_id);
        }

        public static SceneDetails FirstScene(EScene _sceneTpye)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            return instance.GetFirstScene(_sceneTpye);
        }

        public static SceneDetails Scene(EScene _sceneTpye, int _id)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            return instance.GetScene(_sceneTpye, _id);
        }

        public static bool NextScene(out SceneDetails _details)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            return instance.GetNextScene(out _details);
        }

        public static string AllScenesString()
        {
            string output = "";
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            instance.AllScenes().ForEach(x => output += x.ToString());
            return output;
        }
        #endregion -----------------------------------------------------------------


        #region Handle Scene Registration -----------------------------------------------------------------
        /// <summary>
        /// Handle when a scene clarifies itself.
        /// </summary>
        /// <param name="_sceneType">Type of the scene</param>
        /// <param name="_sceneName">Name of the scene</param>
        /// <param name="_path">Path of the scene</param>
        public static void UpdateScene(EScene _sceneType, string _sceneName, string _path)
        {
            if (instance == null)
                throw new NullReferenceException("Instance has not been set!");
            instance.HandleScene(_sceneType, _sceneName, _path);
        }

        /// <summary>
        /// Handle when a scene clarifies itself.
        /// </summary>
        /// <param name="_sceneType">Type of the scene</param>
        /// <param name="_sceneName">Name of the scene</param>
        /// <param name="_path">Path of the scene</param>
        public void HandleScene(EScene _sceneType, string _sceneName, string _path)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif

            FixMissingSceneTypes();



            AddScene(_sceneType, _sceneName, _path);

        }

        /// <summary>
        /// Fixes when scene types are missing in the dict.
        /// </summary>
        private void FixMissingSceneTypes()
        {
            List<EScene> sceneTypes = Enum.GetValues(typeof(EScene)).Cast<EScene>().ToList();
            
            if (sceneDetails.Count == sceneTypes.Count)
                return;

            sceneDetails = sceneTypes.Select(x => GetSceneDetailsForType(x)).ToList();
        }

        private SceneWrapper GetSceneDetailsForType(EScene _sceneType)
        {
            var scenesOfType = sceneDetails.Where(x => x.SceneType == _sceneType);
            return (scenesOfType.Count() == 0) ? new SceneWrapper(_sceneType, new List<SceneDetails>()) : scenesOfType.First();
        }
        
        private void AddScene(EScene _sceneType, string _sceneName, string _path)
        {
            SceneDetails scene = new SceneDetails(GetNextId(), _sceneName, _path, _sceneType);

            UpdateScene(scene);
        }

        private int GetNextId()
        {
            var allScenes = sceneDetails.SelectMany(x => x.Scenes);
            if (allScenes.Count() == 0)
                return 0;
            return allScenes.Max(y => y.Id) + 1;
        }

        private void UpdateScene(SceneDetails _sceneDetails)
        {
            List<SceneDetails> scenesOfType = GetScenesOfType(_sceneDetails.SceneType);
            List<SceneDetails> scenesWithName = scenesOfType.Where(x => x.SceneName == _sceneDetails.SceneName).ToList();

            if (scenesWithName.Count() > 1)
                HandleMultipleScenesForName(_sceneDetails, scenesOfType, scenesWithName);
            else if (scenesWithName.Count() == 0)
                scenesOfType.Add(_sceneDetails);
            else
                UpdateSceneDetails(_sceneDetails, scenesWithName.First());                
        }

        private void HandleMultipleScenesForName(SceneDetails _sceneDetails, 
            List<SceneDetails> _scenesOfType, List<SceneDetails> _scenesWithName)
        {
            Debug.LogError("Multiple scenes with name " + _sceneDetails.SceneName + " registered for type " +
                    _sceneDetails.SceneType + "! Removing all and creating new one.");

            _scenesWithName.ForEach(x => _scenesOfType.Remove(x));
        }

        private void UpdateSceneDetails(SceneDetails _newDetails, SceneDetails _oldDetails)
        {
            _oldDetails.ScenePath = _newDetails.ScenePath;
        }
        #endregion -----------------------------------------------------------------

        private List<SceneDetails> GetScenesOfType(EScene _sceneType) =>
            sceneDetails.Where(x => x.SceneType == _sceneType).First().Scenes;


        private List<SceneDetails> AllScenes() => sceneDetails.SelectMany(x => x.Scenes).ToList();

        private SceneDetails GetSceneByID(int _id)
        {
            var scenesWithName = AllScenes().Where(y => y.Id == _id);
            if (scenesWithName.Count() != 0)
                throw new ArgumentException("Wrong amount of scenes with id \"" + _id + "\" found.");
            return scenesWithName.First();
        }

        private SceneDetails GetBakeIdByID(int _id)
        {
            var scenesWithName = AllScenes().Where(y => y.Id == _id);
            if (scenesWithName.Count() != 0)
                throw new ArgumentException("Wrong amount of scenes with id \"" + _id + "\" found.");
            return scenesWithName.First();
        }

        private SceneDetails GetScene(string _name)
        {
            var scenesWithName = AllScenes().Where(y => y.SceneName == _name);
            if (scenesWithName.Count() == 0)
                throw new ArgumentException("No scenes with name \"" + _name + "\" found.");
            if (scenesWithName.Count() > 1)
                throw new ArgumentException("More than one scenes with name \"" + _name + "\" found.");
            return scenesWithName.First();
        }

        private SceneDetails GetScene(EScene _sceneType, string _name)
        {
            var scenesWithName = GetScenesOfType(_sceneType).Where(y => y.SceneName == _name);
            if (scenesWithName.Count() == 0)
                throw new ArgumentException("Wrong amount of scenes with name \"" + _name + "\" found.");
            if (scenesWithName.Count() > 1)
                throw new ArgumentException("More than one scenes with name \"" + _name + "\" found.");
            return scenesWithName.First();
        }

        private EScene GetSceneTypeByID(int _id) => GetSceneByID(_id).SceneType;

        private int GetBakedSceneID(string _name) => GetScene(_name).BakedId;

        private int GetBakedSceneID(EScene _sceneType, string _name) => GetScene(_sceneType, _name).BakedId;

        private SceneDetails GetFirstScene(EScene _sceneType) => GetScenesOfType(_sceneType).First();

        private SceneDetails GetScene(EScene _sceneType, int _id)
        {
            List<SceneDetails> scenes = GetScenesOfType(_sceneType);
            if(_id >= scenes.Count)
                throw new ArgumentException("Scene with id \"" + _id + "\" not found for type \"" + _sceneType + "\".");

            return scenes[_id];
        }

        private bool GetNextScene(out SceneDetails _details)
        {
            try
            {
                _details = GetScene(currentScene.SceneType, currentScene.Id + 1);
                return true;
            }
            catch (ArgumentException)
            {
                _details = null;
                return false;
            }
        }
        #region Handle Data -----------------------------------------------------------------
#if (UNITY_EDITOR)
        /// <summary>
        /// Bakes the collected data to the build settings.
        /// </summary>
        public void BakeSceneDataToBuildSettings()
        {
            EditorUtility.SetDirty(this);

            List<EditorBuildSettingsScene> scenesForEditor = new List<EditorBuildSettingsScene>();
            int bakedId = 0;
            for (int i = 0; i < sceneOrder.Count; i++)
            {

                List<SceneDetails> scenes = GetSceneDetailsBySceneOrder(i);
                for (int j = 0; j < scenes.Count; j++)
                {
                    scenesForEditor.Add(new EditorBuildSettingsScene(scenes[j].ScenePath, true));
                    scenes[j].BakedId = bakedId;
                    bakedId++;
                }
            }
            EditorBuildSettings.scenes = scenesForEditor.ToArray();
        }

        /// <summary>
        /// Gets scene details by the iterator over the sceneorder.
        /// </summary>
        /// <param name="_sceneIterator">Iterator</param>
        /// <returns>List of all scenes by the iterator</returns>
        private List<SceneDetails> GetSceneDetailsBySceneOrder(int _sceneIterator) => 
            sceneDetails.Where(x => x.SceneType == sceneOrder[_sceneIterator]).Select(x => x.Scenes).First();
#endif
        #endregion -----------------------------------------------------------------
    }
}
