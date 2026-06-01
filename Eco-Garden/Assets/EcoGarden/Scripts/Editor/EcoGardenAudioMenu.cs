using EcoGarden.Audio;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoGarden.Editor
{
    public static class EcoGardenAudioMenu
    {
        private const string AudioFolder = "Assets/EcoGarden/Audio";
        private static readonly string[] ScenePaths =
        {
            "Assets/EcoGarden/Scenes/EcoGarden_Level15_VerticalSlice.unity",
            "Assets/EcoGarden/Scenes/EcoGarden_FirstRelease_Progression.unity"
        };

        [MenuItem("Eco Garden/Audio/Assign Audio Clips To Current Scene")]
        public static void AssignAudioClipsToCurrentScene()
        {
            EcoGardenAudioController controller = Object.FindAnyObjectByType<EcoGardenAudioController>();
            if (controller == null)
            {
                GameObject gameRoot = GameObject.Find("GameRoot");
                if (gameRoot == null)
                {
                    gameRoot = new GameObject("GameRoot");
                }

                controller = gameRoot.GetComponent<EcoGardenAudioController>();
                if (controller == null)
                {
                    controller = gameRoot.AddComponent<EcoGardenAudioController>();
                }
            }

            int assignedCount = AssignAudioClips(controller);
            EditorUtility.SetDirty(controller);
            Scene activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
            }

            Debug.Log("Eco Garden audio assignment complete. Assigned " + assignedCount + " clip references.", controller);
            Selection.activeGameObject = controller.gameObject;
        }

        [MenuItem("Eco Garden/Audio/Assign Audio Clips To Release Scenes")]
        public static void AssignAudioClipsToReleaseScenes()
        {
            int sceneCount = 0;
            int clipReferenceCount = 0;

            for (int i = 0; i < ScenePaths.Length; i++)
            {
                string scenePath = ScenePaths[i];
                if (!File.Exists(scenePath))
                {
                    Debug.LogWarning("Eco Garden audio assignment skipped missing scene: " + scenePath);
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                EcoGardenAudioController controller = FindOrCreateAudioController();
                clipReferenceCount += AssignAudioClips(controller);
                EditorUtility.SetDirty(controller);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                sceneCount++;
            }

            Debug.Log("Eco Garden audio assignment complete for " + sceneCount + " scenes. Assigned " + clipReferenceCount + " clip references.");
        }

        public static int AssignAudioClips(EcoGardenAudioController controller)
        {
            if (controller == null)
            {
                return 0;
            }

            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { AudioFolder });
            int assignedCount = 0;
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip == null)
                {
                    continue;
                }

                string assetId = Path.GetFileNameWithoutExtension(path);
                controller.SetClipByAssetId(assetId, clip);
                assignedCount++;
            }

            return assignedCount;
        }

        private static EcoGardenAudioController FindOrCreateAudioController()
        {
            EcoGardenAudioController controller = Object.FindAnyObjectByType<EcoGardenAudioController>();
            if (controller != null)
            {
                return controller;
            }

            GameObject gameRoot = GameObject.Find("GameRoot");
            if (gameRoot == null)
            {
                gameRoot = new GameObject("GameRoot");
            }

            controller = gameRoot.GetComponent<EcoGardenAudioController>();
            if (controller == null)
            {
                controller = gameRoot.AddComponent<EcoGardenAudioController>();
            }

            return controller;
        }
    }
}
