using EcoGarden.Board;
using EcoGarden.Config;
using EcoGarden.Save;
using UnityEngine;

namespace EcoGarden.Progression
{
    public sealed class LevelCatalogController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private LevelCatalogDefinition levelCatalog;
        [SerializeField] private bool selectHighestUnlockedOnAwake = true;
        [SerializeField] private bool reloadBoardWhenSelecting;

        private LevelCatalogService catalogService;

        public LevelDefinition SelectedLevel { get; private set; }
        public LevelCatalogService Catalog
        {
            get
            {
                if (catalogService == null)
                {
                    catalogService = new LevelCatalogService(levelCatalog);
                }

                return catalogService;
            }
        }

        private void Reset()
        {
            boardController = FindAnyObjectByType<BoardController>();
        }

        private void Awake()
        {
            ResolveReferences();

            if (selectHighestUnlockedOnAwake)
            {
                SelectHighestUnlockedLevel(SaveService.Load());
            }
        }

        public void SetBoardController(BoardController controller)
        {
            boardController = controller;
        }

        public void SetCatalog(LevelCatalogDefinition catalog)
        {
            levelCatalog = catalog;
            catalogService = null;
        }

        public bool SelectHighestUnlockedLevel(SaveData saveData)
        {
            if (!Catalog.TryGetHighestUnlockedLevel(saveData, out LevelDefinition level))
            {
                return false;
            }

            return SelectLevel(level, saveData);
        }

        public bool SelectLevel(int levelId, SaveData saveData)
        {
            if (!Catalog.TryGetLevel(levelId, out LevelDefinition level))
            {
                return false;
            }

            return SelectLevel(level, saveData);
        }

        public bool SelectLevel(LevelDefinition level, SaveData saveData)
        {
            if (!LevelProgressionService.IsLevelUnlocked(saveData, level))
            {
                return false;
            }

            ResolveReferences();
            if (boardController == null)
            {
                return false;
            }

            SelectedLevel = level;
            boardController.SetLevelDefinition(level);

            if (reloadBoardWhenSelecting || boardController.BoardState != null)
            {
                boardController.LoadLevel();
            }

            return true;
        }

        public bool SelectLevelAfterUnlock(LevelDefinition level)
        {
            if (level == null)
            {
                return false;
            }

            ResolveReferences();
            if (boardController == null)
            {
                return false;
            }

            SelectedLevel = level;
            boardController.SetLevelDefinition(level);
            boardController.LoadLevel();
            return true;
        }

        private void ResolveReferences()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }
        }
    }
}
