using EcoGarden.Abilities;
using EcoGarden.Board;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class AbilityHudController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private Button shovelButton;
        [SerializeField] private Button magicWandButton;
        [SerializeField] private Button sortingMagnetButton;
        [SerializeField] private Text feedbackText;

        public bool HasSelectedAbility { get; private set; }
        public AbilityKind SelectedAbility { get; private set; }

        private void Awake()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            AutoWireReferences();
            WireButtons();
            Refresh();
        }

        private void Start()
        {
            Refresh();
        }

        public bool TryUseSelectedAbility(GridPosition targetPosition)
        {
            if (!HasSelectedAbility || boardController == null)
            {
                return false;
            }

            bool used = boardController.TryUseAbility(SelectedAbility, targetPosition);
            if (used)
            {
                ClearSelection();
                SetFeedback(string.Empty);
                Refresh();
                return true;
            }

            SetFeedback("Invalid target");
            return false;
        }

        public void SelectAbility(AbilityKind abilityKind)
        {
            if (boardController == null ||
                boardController.AbilityInventory == null ||
                boardController.AbilityInventory.GetCount(abilityKind) <= 0)
            {
                SetFeedback("No uses left");
                return;
            }

            HasSelectedAbility = true;
            SelectedAbility = abilityKind;
            SetFeedback("Select target");
            RefreshButtonColors();
        }

        public void ClearSelection()
        {
            HasSelectedAbility = false;
            RefreshButtonColors();
        }

        public void Refresh()
        {
            if (boardController == null || boardController.AbilityInventory == null)
            {
                return;
            }

            SetButtonLabel(shovelButton, "Shovel x" + boardController.AbilityInventory.GetCount(AbilityKind.Shovel));
            SetButtonLabel(magicWandButton, "Wand x" + boardController.AbilityInventory.GetCount(AbilityKind.MagicWand));
            SetButtonLabel(sortingMagnetButton, "Magnet x" + boardController.AbilityInventory.GetCount(AbilityKind.SortingMagnet));
            RefreshButtonColors();
        }

        private void AutoWireReferences()
        {
            if (shovelButton == null)
            {
                shovelButton = FindButton("ShovelButton");
            }

            if (magicWandButton == null)
            {
                magicWandButton = FindButton("MagicWandButton");
            }

            if (sortingMagnetButton == null)
            {
                sortingMagnetButton = FindButton("SortingMagnetButton");
            }

            if (feedbackText == null)
            {
                GameObject feedbackObject = GameObject.Find("FeedbackText");
                if (feedbackObject != null)
                {
                    feedbackText = feedbackObject.GetComponent<Text>();
                }
            }
        }

        private void WireButtons()
        {
            if (shovelButton != null)
            {
                shovelButton.onClick.RemoveListener(SelectShovel);
                shovelButton.onClick.AddListener(SelectShovel);
            }

            if (magicWandButton != null)
            {
                magicWandButton.onClick.RemoveListener(SelectMagicWand);
                magicWandButton.onClick.AddListener(SelectMagicWand);
            }

            if (sortingMagnetButton != null)
            {
                sortingMagnetButton.onClick.RemoveListener(SelectSortingMagnet);
                sortingMagnetButton.onClick.AddListener(SelectSortingMagnet);
            }
        }

        private void SelectShovel()
        {
            SelectAbility(AbilityKind.Shovel);
        }

        private void SelectMagicWand()
        {
            SelectAbility(AbilityKind.MagicWand);
        }

        private void SelectSortingMagnet()
        {
            SelectAbility(AbilityKind.SortingMagnet);
        }

        private void RefreshButtonColors()
        {
            SetButtonColor(shovelButton, HasSelectedAbility && SelectedAbility == AbilityKind.Shovel);
            SetButtonColor(magicWandButton, HasSelectedAbility && SelectedAbility == AbilityKind.MagicWand);
            SetButtonColor(sortingMagnetButton, HasSelectedAbility && SelectedAbility == AbilityKind.SortingMagnet);
        }

        private static void SetButtonLabel(Button button, string label)
        {
            if (button == null)
            {
                return;
            }

            Text text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = label;
            }
        }

        private static void SetButtonColor(Button button, bool selected)
        {
            if (button == null)
            {
                return;
            }

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = selected
                    ? new Color(0.36f, 0.68f, 0.78f, 1f)
                    : new Color(0.24f, 0.48f, 0.62f, 1f);
            }
        }

        private void SetFeedback(string message)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
            }
        }

        private static Button FindButton(string objectName)
        {
            GameObject gameObject = GameObject.Find(objectName);
            return gameObject != null ? gameObject.GetComponent<Button>() : null;
        }
    }
}
