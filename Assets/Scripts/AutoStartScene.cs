#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoStartScene
{
    private const string START_SCENE_PATH = "Assets/Scenes/MainMenu.unity"; // baþlangýç sahnesi
    private const string PREVIOUS_SCENE_KEY = "PreviousScenePath";

    static AutoStartScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Þu anki sahneyi kaydet
            string currentScene = EditorSceneManager.GetActiveScene().path;
            EditorPrefs.SetString(PREVIOUS_SCENE_KEY, currentScene);

            // Baþlangýç sahnesine geç
            if (currentScene != START_SCENE_PATH)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(START_SCENE_PATH);
                }
                else
                {
                    EditorApplication.isPlaying = false; // Ýptal et
                }
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Play bitince önceki sahneye geri dön
            if (EditorPrefs.HasKey(PREVIOUS_SCENE_KEY))
            {
                string previousScene = EditorPrefs.GetString(PREVIOUS_SCENE_KEY);
                if (previousScene != START_SCENE_PATH) // Tekrar menüye dönmesin
                {
                    EditorSceneManager.OpenScene(previousScene);
                }
                EditorPrefs.DeleteKey(PREVIOUS_SCENE_KEY);
            }
        }
    }
}
#endif
