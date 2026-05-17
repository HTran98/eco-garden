using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Input;
using EcoGarden.Level;
using EcoGarden.Economy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoGarden.Editor
{
    public static class EcoGardenSceneMenu
    {
        private const string Level15Path = "Assets/EcoGarden/ScriptableObjects/Levels/level_015_lotus_pond_corner.asset";
        private const string ScenePath = "Assets/EcoGarden/Scenes/EcoGarden_Level15_VerticalSlice.unity";

        [MenuItem("Eco Garden/Create Scene/Level 15 Vertical Slice")]
        public static void CreateLevel15Scene()
        {
            EcoGardenAssetMenu.CreateLevel15Data();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "EcoGarden_Level15_VerticalSlice";

            CreateCamera();

            GameObject boardRoot = new GameObject("BoardRoot");
            boardRoot.transform.position = new Vector3(0f, -0.45f, 0f);
            BoardView boardView = boardRoot.AddComponent<BoardView>();
            BoardController boardController = boardRoot.AddComponent<BoardController>();
            boardController.SetLevelDefinition(AssetDatabase.LoadAssetAtPath<LevelDefinition>(Level15Path));

            GameObject gameRoot = new GameObject("GameRoot");
            gameRoot.AddComponent<GameBootstrapper>();
            gameRoot.AddComponent<LevelStateController>();
            gameRoot.AddComponent<EconomyController>();

            GameObject inputRoot = new GameObject("InputRoot");
            inputRoot.AddComponent<BoardInputController>();

            EcoGardenUiMenu.CreateGameHudSkeleton();
            boardController.LoadLevel();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorSceneManager.MarkSceneDirty(scene);

            Selection.activeGameObject = boardRoot;
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 5.25f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.30f, 0.43f, 0.60f, 1f);
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        }
    }
}
