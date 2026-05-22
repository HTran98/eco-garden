using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Economy;
using UnityEngine;

namespace EcoGarden.Level
{
    public sealed class LevelPlaytestMetricsController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private LevelStateController levelStateController;
        [SerializeField] private EconomyController economyController;

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnEnable()
        {
            ResolveReferences();
            if (levelStateController != null)
            {
                levelStateController.LevelCompleted += LogCompleted;
                levelStateController.LevelFailed += LogFailed;
            }
        }

        private void OnDisable()
        {
            if (levelStateController != null)
            {
                levelStateController.LevelCompleted -= LogCompleted;
                levelStateController.LevelFailed -= LogFailed;
            }
        }

        private void LogCompleted()
        {
            LogResult("Completed");
        }

        private void LogFailed()
        {
            LogResult("Failed");
        }

        private void LogResult(string result)
        {
            if (boardController == null || boardController.LevelDefinition == null)
            {
                Debug.Log("EcoGarden Playtest: result=" + result + " level=unknown");
                return;
            }

            int shovel = boardController.AbilityInventory != null
                ? boardController.AbilityInventory.GetCount(AbilityKind.Shovel)
                : 0;
            int wand = boardController.AbilityInventory != null
                ? boardController.AbilityInventory.GetCount(AbilityKind.MagicWand)
                : 0;
            int magnet = boardController.AbilityInventory != null
                ? boardController.AbilityInventory.GetCount(AbilityKind.SortingMagnet)
                : 0;
            int gold = economyController != null ? economyController.Gold : 0;
            int gem = economyController != null ? economyController.Gem : 0;
            int remainingSeconds = levelStateController != null
                ? Mathf.CeilToInt(levelStateController.RemainingSeconds)
                : 0;

            Debug.Log(
                "EcoGarden Playtest: result=" + result +
                " levelId=" + boardController.LevelDefinition.LevelId +
                " levelName=\"" + boardController.LevelDefinition.LevelName + "\"" +
                " remainingSeconds=" + remainingSeconds +
                " gold=" + gold +
                " gem=" + gem +
                " shovel=" + shovel +
                " wand=" + wand +
                " magnet=" + magnet);
        }

        private void ResolveReferences()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (levelStateController == null)
            {
                levelStateController = FindAnyObjectByType<LevelStateController>();
            }

            if (economyController == null)
            {
                economyController = FindAnyObjectByType<EconomyController>();
            }
        }
    }
}
