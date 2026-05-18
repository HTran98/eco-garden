using EcoGarden.Board;
using EcoGarden.AI;
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
            EnsureHudSkin();
            EnsureSaveController();
            EnsureAndroidHudLayout();
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
    }
}
