using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.IAP;
using EcoGarden.Input;
using EcoGarden.Missions;
using EcoGarden.Save;
using EcoGarden.Shop;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace EcoGarden.Editor
{
    public static class EcoGardenSceneAudit
    {
        private const string Level15ScenePath = "Assets/EcoGarden/Scenes/EcoGarden_Level15_VerticalSlice.unity";

        [MenuItem("Eco Garden/Validation/Audit Level 15 Scene")]
        public static void AuditLevel15Scene()
        {
            List<string> issues = AuditLevel15SceneReferences();
            if (issues.Count > 0)
            {
                string message = "Eco Garden scene audit failed:\n- " + string.Join("\n- ", issues);
                Debug.LogError(message);
                throw new InvalidOperationException(message);
            }

            Debug.Log("Eco Garden scene audit passed.");
        }

        public static List<string> AuditLevel15SceneReferences()
        {
            Scene scene = EditorSceneManager.OpenScene(Level15ScenePath, OpenSceneMode.Single);
            List<string> issues = new List<string>();

            RequireObject<BoardController>(issues, "BoardController");
            RequireObject<BoardView>(issues, "BoardView");
            RequireObject<BoardInputController>(issues, "BoardInputController");
            RequireObject<EconomyController>(issues, "EconomyController");
            RequireObject<SaveController>(issues, "SaveController");
            RequireObject<ShopController>(issues, "ShopController");
            RequireObject<MissionController>(issues, "MissionController");
            RequireObject<MockIapProvider>(issues, "MockIapProvider");

            BoardController boardController = UnityEngine.Object.FindAnyObjectByType<BoardController>();
            if (boardController != null && boardController.LevelDefinition == null)
            {
                issues.Add("BoardController is missing LevelDefinition.");
            }

            EventSystem eventSystem = UnityEngine.Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                issues.Add("EventSystem is missing.");
            }
            else if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                issues.Add("EventSystem is missing InputSystemUIInputModule.");
            }

            RequireDropZone(issues, ExternalDropZoneKind.SellBasket, "Sell drop zone");
            RequireDropZone(issues, ExternalDropZoneKind.Delivery, "Delivery drop zone");
            FindMissingScripts(scene, issues);

            return issues;
        }

        private static void RequireObject<T>(List<string> issues, string label) where T : UnityEngine.Object
        {
            if (UnityEngine.Object.FindAnyObjectByType<T>() == null)
            {
                issues.Add(label + " is missing.");
            }
        }

        private static void RequireDropZone(List<string> issues, ExternalDropZoneKind kind, string label)
        {
            ExternalDropZone[] zones = UnityEngine.Object.FindObjectsByType<ExternalDropZone>(FindObjectsInactive.Include);
            for (int i = 0; i < zones.Length; i++)
            {
                if (zones[i] != null && zones[i].ZoneKind == kind)
                {
                    return;
                }
            }

            issues.Add(label + " is missing.");
        }

        private static void FindMissingScripts(Scene scene, List<string> issues)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                FindMissingScriptsRecursive(roots[i].transform, issues);
            }
        }

        private static void FindMissingScriptsRecursive(Transform transform, List<string> issues)
        {
            Component[] components = transform.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    issues.Add("Missing script on GameObject: " + GetPath(transform));
                    break;
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                FindMissingScriptsRecursive(transform.GetChild(i), issues);
            }
        }

        private static string GetPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }
    }
}
