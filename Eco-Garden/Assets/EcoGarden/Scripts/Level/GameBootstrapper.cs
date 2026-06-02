using EcoGarden.Board;
using EcoGarden.AI;
using EcoGarden.Audio;
using EcoGarden.UI;
using EcoGarden.Save;
using UnityEngine;

namespace EcoGarden.Level
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;

        private void Reset()
        {
            boardController = FindAnyObjectByType<BoardController>();
        }

        private void Start()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (boardController != null && boardController.BoardState == null)
            {
                boardController.LoadLevel();
            }

            EnsureNpcMovement();
            EnsureButterflies();
            EnsureAudioListener();
            EnsureBackground();
            EnsureBoardBackdrop();
            EnsureHudSkin();
            EnsureAudio();
            EnsureSaveController();
            EnsureDecorations();
            EnsureInventoryUi();
            EnsureAndroidHudLayout();
            EnsureTutorialUi();
        }

        private void EnsureBackground()
        {
            EcoGardenBackgroundController background = FindAnyObjectByType<EcoGardenBackgroundController>();
            if (background == null)
            {
                GameObject backgroundObject = new GameObject("EcoGardenBackground");
                background = backgroundObject.AddComponent<EcoGardenBackgroundController>();
            }

            background.Configure(Camera.main);
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = new Color(0.70f, 0.86f, 0.78f, 1f);
            }
        }

        private void EnsureAudioListener()
        {
            if (FindAnyObjectByType<AudioListener>() != null)
            {
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                mainCamera = cameraObject.AddComponent<Camera>();
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 5.25f;
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = new Color(0.70f, 0.86f, 0.78f, 1f);
                cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            }

            mainCamera.gameObject.AddComponent<AudioListener>();
        }

        private void EnsureBoardBackdrop()
        {
            if (boardController == null)
            {
                return;
            }

            BoardBackdropController backdrop = FindAnyObjectByType<BoardBackdropController>();
            if (backdrop == null)
            {
                GameObject backdropObject = new GameObject("BoardBackdrop");
                backdrop = backdropObject.AddComponent<BoardBackdropController>();
            }

            backdrop.Configure(boardController);
        }

        private void EnsureNpcMovement()
        {
            if (FindAnyObjectByType<NpcMovementController>() != null || boardController == null)
            {
                return;
            }

            GameObject npcObject = new GameObject("CustomerNpc");
            NpcMovementController npc = npcObject.AddComponent<NpcMovementController>();
            npc.SetBoardController(boardController);
        }

        private void EnsureButterflies()
        {
            if (FindAnyObjectByType<ButterflyMovementController>() != null || boardController == null)
            {
                return;
            }

            Vector3 boardCenter = boardController.BoardView != null
                ? boardController.BoardView.transform.position
                : Vector3.zero;

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

        private void EnsureHudSkin()
        {
            HudSkinController skinController = FindAnyObjectByType<HudSkinController>();
            if (skinController == null)
            {
                GameObject skinObject = new GameObject("HudSkinController");
                skinController = skinObject.AddComponent<HudSkinController>();
            }

            skinController.Apply();
        }

        private void EnsureSaveController()
        {
            if (FindAnyObjectByType<SaveController>() != null)
            {
                return;
            }

            GameObject saveObject = new GameObject("SaveController");
            saveObject.AddComponent<SaveController>();
        }

        private void EnsureAudio()
        {
            if (FindAnyObjectByType<EcoGardenAudioController>() != null)
            {
                return;
            }

            GameObject audioObject = new GameObject("EcoGardenAudioController");
            audioObject.AddComponent<EcoGardenAudioController>();
        }

        private void EnsureAndroidHudLayout()
        {
            AndroidHudLayoutController layoutController = FindAnyObjectByType<AndroidHudLayoutController>();
            if (layoutController == null)
            {
                GameObject hudRoot = GameObject.Find("HUDRoot");
                if (hudRoot != null)
                {
                    layoutController = hudRoot.AddComponent<AndroidHudLayoutController>();
                }
            }

            if (layoutController != null)
            {
                layoutController.ApplyLayout();
            }
        }

        private void EnsureDecorations()
        {
            if (FindAnyObjectByType<DecorationController>() != null)
            {
                return;
            }

            GameObject decorationObject = new GameObject("DecorationController");
            decorationObject.AddComponent<DecorationController>();
        }

        private void EnsureInventoryUi()
        {
            if (FindAnyObjectByType<InventoryUiController>() != null)
            {
                return;
            }

            GameObject hudRoot = GameObject.Find("HUDRoot");
            if (hudRoot != null)
            {
                hudRoot.AddComponent<InventoryUiController>();
                HudSkinController skinController = FindAnyObjectByType<HudSkinController>();
                if (skinController != null)
                {
                    skinController.Apply();
                }
            }
        }

        private void EnsureTutorialUi()
        {
            if (FindAnyObjectByType<TutorialUiController>(FindObjectsInactive.Include) != null)
            {
                return;
            }

            GameObject tutorialObject = new GameObject("TutorialUiController");
            tutorialObject.AddComponent<TutorialUiController>();
        }
    }
}
