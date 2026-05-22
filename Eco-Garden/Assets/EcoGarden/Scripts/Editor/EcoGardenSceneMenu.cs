using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Input;
using EcoGarden.Level;
using EcoGarden.Economy;
using EcoGarden.AI;
using EcoGarden.IAP;
using EcoGarden.Missions;
using EcoGarden.Progression;
using EcoGarden.Save;
using EcoGarden.Shop;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoGarden.Editor
{
    public static class EcoGardenSceneMenu
    {
        private const string Level15Path = "Assets/EcoGarden/ScriptableObjects/Levels/level_015_lotus_pond_corner.asset";
        private const string Level1Path = "Assets/EcoGarden/ScriptableObjects/Levels/level_001_first_sprouts.asset";
        private const string FirstReleaseLevelCatalogPath = "Assets/EcoGarden/ScriptableObjects/Levels/first_release_level_catalog.asset";
        private const string ScenePath = "Assets/EcoGarden/Scenes/EcoGarden_Level15_VerticalSlice.unity";
        private const string FirstReleaseScenePath = "Assets/EcoGarden/Scenes/EcoGarden_FirstRelease_Progression.unity";
        private const string ShopFolder = "Assets/EcoGarden/ScriptableObjects/Shop";
        private const string MissionFolder = "Assets/EcoGarden/ScriptableObjects/Missions";

        [MenuItem("Eco Garden/Create Scene/Level 15 Vertical Slice")]
        public static void CreateLevel15Scene()
        {
            EcoGardenAssetMenu.CreateLevel15Data();
            EcoGardenAssetMenu.CreateShopCatalogData();
            EcoGardenAssetMenu.CreateMissionData();

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
            gameRoot.AddComponent<LevelPlaytestMetricsController>();
            gameRoot.AddComponent<EconomyController>();
            gameRoot.AddComponent<MockIapProvider>();
            ShopController shopController = gameRoot.AddComponent<ShopController>();
            shopController.SetCatalogItems(LoadShopCatalogItems());
            MissionController missionController = gameRoot.AddComponent<MissionController>();
            missionController.SetMissionDefinitions(LoadMissionDefinitions());
            gameRoot.AddComponent<SaveController>();

            GameObject npcObject = new GameObject("CustomerNpc");
            NpcMovementController npc = npcObject.AddComponent<NpcMovementController>();
            npc.SetBoardController(boardController);

            CreateButterflies(boardRoot.transform.position);

            GameObject inputRoot = new GameObject("InputRoot");
            inputRoot.AddComponent<BoardInputController>();

            EcoGardenUiMenu.CreateGameHudSkeleton();
            boardController.LoadLevel();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorSceneManager.MarkSceneDirty(scene);

            Selection.activeGameObject = boardRoot;
        }

        [MenuItem("Eco Garden/Create Scene/First Release Progression")]
        public static void CreateFirstReleaseProgressionScene()
        {
            EcoGardenAssetMenu.CreateFirstReleaseLevelSetData();
            EcoGardenAssetMenu.CreateFirstReleaseLevelCatalogData();
            EcoGardenAssetMenu.CreateShopCatalogData();
            EcoGardenAssetMenu.CreateMissionData();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "EcoGarden_FirstRelease_Progression";

            CreateCamera();

            GameObject boardRoot = new GameObject("BoardRoot");
            boardRoot.transform.position = new Vector3(0f, -0.45f, 0f);
            boardRoot.AddComponent<BoardView>();
            BoardController boardController = boardRoot.AddComponent<BoardController>();
            boardController.SetLevelDefinition(AssetDatabase.LoadAssetAtPath<LevelDefinition>(Level1Path));

            GameObject gameRoot = new GameObject("GameRoot");
            gameRoot.AddComponent<GameBootstrapper>();
            gameRoot.AddComponent<LevelStateController>();
            gameRoot.AddComponent<LevelPlaytestMetricsController>();
            gameRoot.AddComponent<EconomyController>();
            gameRoot.AddComponent<MockIapProvider>();

            LevelCatalogController levelCatalogController = gameRoot.AddComponent<LevelCatalogController>();
            levelCatalogController.SetBoardController(boardController);
            levelCatalogController.SetCatalog(AssetDatabase.LoadAssetAtPath<LevelCatalogDefinition>(FirstReleaseLevelCatalogPath));

            ShopController shopController = gameRoot.AddComponent<ShopController>();
            shopController.SetCatalogItems(LoadShopCatalogItems());
            MissionController missionController = gameRoot.AddComponent<MissionController>();
            missionController.SetMissionDefinitions(LoadMissionDefinitions());
            gameRoot.AddComponent<SaveController>();

            GameObject npcObject = new GameObject("CustomerNpc");
            NpcMovementController npc = npcObject.AddComponent<NpcMovementController>();
            npc.SetBoardController(boardController);

            CreateButterflies(boardRoot.transform.position);

            GameObject inputRoot = new GameObject("InputRoot");
            inputRoot.AddComponent<BoardInputController>();

            EcoGardenUiMenu.CreateGameHudSkeleton();
            boardController.LoadLevel();

            EditorSceneManager.SaveScene(scene, FirstReleaseScenePath);
            EditorSceneManager.MarkSceneDirty(scene);

            Selection.activeGameObject = gameRoot;
        }

        [MenuItem("Eco Garden/Fix Scene/Add First Release Level Loader")]
        public static void AddFirstReleaseLevelLoaderToCurrentScene()
        {
            EcoGardenAssetMenu.CreateFirstReleaseLevelCatalogData();

            GameObject gameRoot = GameObject.Find("GameRoot");
            if (gameRoot == null)
            {
                gameRoot = new GameObject("GameRoot");
            }

            LevelCatalogController levelCatalogController = gameRoot.GetComponent<LevelCatalogController>();
            if (levelCatalogController == null)
            {
                levelCatalogController = gameRoot.AddComponent<LevelCatalogController>();
            }

            levelCatalogController.SetBoardController(Object.FindAnyObjectByType<BoardController>());
            levelCatalogController.SetCatalog(AssetDatabase.LoadAssetAtPath<LevelCatalogDefinition>(FirstReleaseLevelCatalogPath));

            if (gameRoot.GetComponent<LevelPlaytestMetricsController>() == null)
            {
                gameRoot.AddComponent<LevelPlaytestMetricsController>();
            }

            EditorUtility.SetDirty(gameRoot);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = gameRoot;
        }

        [MenuItem("Eco Garden/Fix Scene/Add Mission Controller")]
        public static void AddMissionControllerToCurrentScene()
        {
            EcoGardenAssetMenu.CreateMissionData();

            GameObject gameRoot = GameObject.Find("GameRoot");
            if (gameRoot == null)
            {
                gameRoot = new GameObject("GameRoot");
            }

            MissionController missionController = gameRoot.GetComponent<MissionController>();
            if (missionController == null)
            {
                missionController = gameRoot.AddComponent<MissionController>();
            }

            missionController.SetMissionDefinitions(LoadMissionDefinitions());
            EditorUtility.SetDirty(gameRoot);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Selection.activeGameObject = gameRoot;
        }

        private static ShopItemDefinition[] LoadShopCatalogItems()
        {
            string[] guids = AssetDatabase.FindAssets("t:ShopItemDefinition", new[] { ShopFolder });
            ShopItemDefinition[] items = new ShopItemDefinition[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                items[i] = AssetDatabase.LoadAssetAtPath<ShopItemDefinition>(path);
            }

            return items;
        }

        private static MissionDefinition[] LoadMissionDefinitions()
        {
            string[] guids = AssetDatabase.FindAssets("t:MissionDefinition", new[] { MissionFolder });
            MissionDefinition[] missions = new MissionDefinition[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                missions[i] = AssetDatabase.LoadAssetAtPath<MissionDefinition>(path);
            }

            return missions;
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

        private static void CreateButterflies(Vector3 boardCenter)
        {
            GameObject butterflyAObject = new GameObject("ButterflyA");
            ButterflyMovementController butterflyA = butterflyAObject.AddComponent<ButterflyMovementController>();
            butterflyA.ConfigureLoop(
                boardCenter + new Vector3(0f, 0.2f, 0f),
                new Vector2(4.45f, 2.15f),
                0.46f,
                0f,
                new Color(1f, 0.74f, 0.32f, 1f));

            GameObject butterflyBObject = new GameObject("ButterflyB");
            ButterflyMovementController butterflyB = butterflyBObject.AddComponent<ButterflyMovementController>();
            butterflyB.ConfigureHover(
                boardCenter + new Vector3(-3.2f, 2.9f, 0f),
                new Vector2(0.48f, 0.32f),
                0.82f,
                1.7f,
                new Color(0.68f, 0.86f, 1f, 1f));
        }
    }
}
