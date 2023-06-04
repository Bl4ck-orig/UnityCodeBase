#if UNITY_EDITOR
using SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SO_SceneConfig))]
    public class SO_SceneConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SO_SceneConfig sceneConfig = (SO_SceneConfig)target;
            if(GUILayout.Button("Bake Data To Build Settings"))
            {
                sceneConfig.BakeSceneDataToBuildSettings();
                
            }
        }
    }
}
#endif