using EcoGarden.Board;
using EcoGarden.UI;
using EcoGarden.Level;
using EcoGarden.Economy;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace EcoGarden.Input
{
    public sealed class BoardInputController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private AbilityHudController abilityHudController;
        [SerializeField] private LevelStateController levelStateController;
        [SerializeField] private EconomyController economyController;
        [SerializeField] private CoinBurstFeedback coinBurstFeedback;
        [SerializeField] private DraggedItemCanvasGhost draggedItemCanvasGhost;
        [SerializeField] private Camera inputCamera;
        [SerializeField] private Color selectedItemTint = new Color(1f, 1f, 1f, 0.65f);
        [SerializeField] private float dropAnimationSeconds = 0.12f;
        [SerializeField] private float sellAnimationSeconds = 0.22f;

        private bool isDraggingItem;
        private GridPosition dragStartPosition;
        private Color originalSelectedColor;
        private SpriteRenderer selectedRenderer;
        private ItemView selectedItemView;
        private Coroutine dropRoutine;
        private ExternalDropZone highlightedDropZone;
        private bool sellGhostAnimating;
        private bool externalDragGhostActive;

        private void Reset()
        {
            boardController = FindAnyObjectByType<BoardController>();
            abilityHudController = FindAnyObjectByType<AbilityHudController>();
            levelStateController = FindAnyObjectByType<LevelStateController>();
            economyController = FindAnyObjectByType<EconomyController>();
            coinBurstFeedback = FindAnyObjectByType<CoinBurstFeedback>();
            draggedItemCanvasGhost = FindAnyObjectByType<DraggedItemCanvasGhost>();
            inputCamera = Camera.main;
        }

        private void Awake()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (abilityHudController == null)
            {
                abilityHudController = FindAnyObjectByType<AbilityHudController>();
            }

            if (levelStateController == null)
            {
                levelStateController = FindAnyObjectByType<LevelStateController>();
            }

            if (economyController == null)
            {
                economyController = FindAnyObjectByType<EconomyController>();
            }

            if (coinBurstFeedback == null)
            {
                coinBurstFeedback = FindAnyObjectByType<CoinBurstFeedback>();
            }

            if (draggedItemCanvasGhost == null)
            {
                draggedItemCanvasGhost = FindAnyObjectByType<DraggedItemCanvasGhost>();
            }

            if (inputCamera == null)
            {
                inputCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (boardController == null || boardController.BoardState == null)
            {
                return;
            }

            if (levelStateController != null && !levelStateController.IsPlaying)
            {
                return;
            }

            if (WasPointerPressedThisFrame(out Vector2 pressPosition))
            {
                HandlePress(pressPosition);
            }

            if (isDraggingItem && TryGetPointerPosition(out Vector2 dragPosition))
            {
                HandleDrag(dragPosition);
            }

            if (WasPointerReleasedThisFrame(out Vector2 releasePosition))
            {
                HandleRelease(releasePosition);
            }
        }

        private void HandlePress(Vector2 screenPosition)
        {
            if (dropRoutine != null || sellGhostAnimating)
            {
                return;
            }

            if (IsPointerOverUi())
            {
                return;
            }

            if (!TryScreenToGrid(screenPosition, out GridPosition gridPosition))
            {
                return;
            }

            BoardCell cell = boardController.BoardState.GetCell(gridPosition);
            if (cell == null)
            {
                return;
            }

            if (abilityHudController != null && abilityHudController.HasSelectedAbility)
            {
                abilityHudController.TryUseSelectedAbility(gridPosition);
                return;
            }

            if (cell.Item != null)
            {
                isDraggingItem = true;
                dragStartPosition = gridPosition;
                SelectItemVisual(gridPosition);
                return;
            }

            if (cell.Kind == CellKind.Producer)
            {
                boardController.TrySpawnFromProducer(gridPosition, Time.time);
            }
        }

        private void HandleRelease(Vector2 screenPosition)
        {
            if (!isDraggingItem)
            {
                return;
            }

            isDraggingItem = false;
            SetHighlightedDropZone(null);

            if (TryGetExternalDropZone(screenPosition, out ExternalDropZone externalDropZone))
            {
                HandleExternalDrop(externalDropZone);
                return;
            }

            if (IsPointerOverUi())
            {
                AnimateDrop(dragStartPosition, false);
                return;
            }

            if (!TryScreenToGrid(screenPosition, out GridPosition targetPosition) ||
                targetPosition == dragStartPosition)
            {
                AnimateDrop(dragStartPosition, false);
                return;
            }

            bool changed = boardController.TryMoveOrMerge(dragStartPosition, targetPosition, false);
            AnimateDrop(changed ? targetPosition : dragStartPosition, changed);
        }

        private void HandleDrag(Vector2 screenPosition)
        {
            if (selectedItemView == null || inputCamera == null)
            {
                return;
            }

            Vector3 world = ScreenToWorld(screenPosition);
            UpdateExternalDropZoneHighlight(screenPosition);

            if (highlightedDropZone != null && highlightedDropZone.ZoneKind == ExternalDropZoneKind.SellBasket)
            {
                ShowExternalDragGhost(screenPosition);
                return;
            }

            HideExternalDragGhost(true);
            selectedItemView.SetDragWorldPosition(world);
        }

        private bool TryScreenToGrid(Vector2 screenPosition, out GridPosition gridPosition)
        {
            gridPosition = default;

            if (inputCamera == null)
            {
                inputCamera = Camera.main;
            }

            if (inputCamera == null)
            {
                return false;
            }

            return boardController.TryGetGridPosition(ScreenToWorld(screenPosition), out gridPosition);
        }

        private Vector3 ScreenToWorld(Vector2 screenPosition)
        {
            Vector3 world = inputCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -inputCamera.transform.position.z));
            world.z = 0f;
            return world;
        }

        private static bool WasPointerPressedThisFrame(out Vector2 screenPosition)
        {
            if (Touchscreen.current != null)
            {
                TouchControl primaryTouch = Touchscreen.current.primaryTouch;
                if (primaryTouch.press.wasPressedThisFrame)
                {
                    screenPosition = primaryTouch.position.ReadValue();
                    return true;
                }
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }

            screenPosition = default;
            return false;
        }

        private static bool WasPointerReleasedThisFrame(out Vector2 screenPosition)
        {
            if (Touchscreen.current != null)
            {
                TouchControl primaryTouch = Touchscreen.current.primaryTouch;
                if (primaryTouch.press.wasReleasedThisFrame)
                {
                    screenPosition = primaryTouch.position.ReadValue();
                    return true;
                }
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }

            screenPosition = default;
            return false;
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void SelectItemVisual(GridPosition gridPosition)
        {
            ClearSelectedVisual();

            if (!boardController.TryGetItemView(gridPosition, out selectedItemView))
            {
                return;
            }

            selectedRenderer = selectedItemView.SpriteRenderer;
            if (selectedRenderer != null)
            {
                originalSelectedColor = selectedRenderer.color;
                selectedRenderer.color = selectedItemTint;
            }

            selectedItemView.BeginDrag();
        }

        private void ClearSelectedVisual()
        {
            if (selectedRenderer != null)
            {
                selectedRenderer.color = originalSelectedColor;
                selectedRenderer = null;
            }

            HideExternalDragGhost(false);
            selectedItemView = null;
        }

        private void AnimateDrop(GridPosition destination, bool refreshAfterDrop)
        {
            if (dropRoutine != null)
            {
                StopCoroutine(dropRoutine);
            }

            if (selectedItemView == null)
            {
                if (refreshAfterDrop)
                {
                    boardController.RefreshView();
                }

                ClearSelectedVisual();
                return;
            }

            Vector3 destinationWorld = boardController.GetCellWorldPosition(destination);
            dropRoutine = StartCoroutine(AnimateDropRoutine(selectedItemView, destinationWorld, refreshAfterDrop));
        }

        private void AnimateExternalDrop(Vector3 destinationWorld, bool refreshAfterDrop)
        {
            if (dropRoutine != null)
            {
                StopCoroutine(dropRoutine);
            }

            if (selectedItemView == null)
            {
                if (refreshAfterDrop)
                {
                    boardController.RefreshView();
                }

                ClearSelectedVisual();
                return;
            }

            dropRoutine = StartCoroutine(AnimateDropRoutine(selectedItemView, destinationWorld, refreshAfterDrop));
        }

        private IEnumerator AnimateDropRoutine(ItemView itemView, Vector3 destinationWorld, bool refreshAfterDrop)
        {
            Vector3 start = itemView.transform.position;
            float elapsed = 0f;

            while (elapsed < dropAnimationSeconds)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / dropAnimationSeconds);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                Vector3 position = Vector3.Lerp(start, destinationWorld + new Vector3(0f, 0f, -0.1f), eased);
                itemView.SetDragWorldPosition(position);
                yield return null;
            }

            itemView.EndDrag(destinationWorld, boardController.BoardView.CellSize * 0.68f);

            if (refreshAfterDrop)
            {
                boardController.RefreshView();
            }

            ClearSelectedVisual();
            dropRoutine = null;
        }

        private void HandleExternalDrop(ExternalDropZone externalDropZone)
        {
            if (externalDropZone.ZoneKind != ExternalDropZoneKind.SellBasket)
            {
                AnimateDrop(dragStartPosition, false);
                return;
            }

            if (!boardController.TrySellItem(dragStartPosition, out int goldValue, false))
            {
                AnimateDrop(dragStartPosition, false);
                return;
            }

            if (economyController != null)
            {
                economyController.AddGold(goldValue);
            }

            PlaySellAnimation(externalDropZone);

            if (coinBurstFeedback != null)
            {
                coinBurstFeedback.Play(externalDropZone.GetWorldCenter(null), goldValue);
            }
        }

        private bool TryGetExternalDropZone(Vector2 screenPosition, out ExternalDropZone dropZone)
        {
            ExternalDropZone[] zones = FindObjectsByType<ExternalDropZone>(FindObjectsInactive.Exclude);
            for (int i = 0; i < zones.Length; i++)
            {
                if (zones[i].ContainsScreenPoint(screenPosition, null))
                {
                    dropZone = zones[i];
                    return true;
                }
            }

            dropZone = null;
            return false;
        }

        private void UpdateExternalDropZoneHighlight(Vector2 screenPosition)
        {
            if (TryGetExternalDropZone(screenPosition, out ExternalDropZone zone))
            {
                SetHighlightedDropZone(zone);
                return;
            }

            SetHighlightedDropZone(null);
        }

        private void SetHighlightedDropZone(ExternalDropZone zone)
        {
            if (highlightedDropZone == zone)
            {
                return;
            }

            if (highlightedDropZone != null)
            {
                highlightedDropZone.SetHighlighted(false);
            }

            highlightedDropZone = zone;

            if (highlightedDropZone != null)
            {
                highlightedDropZone.SetHighlighted(true);
            }
        }

        private void PlaySellAnimation(ExternalDropZone externalDropZone)
        {
            if (selectedItemView == null)
            {
                boardController.RefreshView();
                ClearSelectedVisual();
                return;
            }

            if (draggedItemCanvasGhost == null)
            {
                Vector3 targetWorld = ScreenToWorld(externalDropZone.GetScreenCenter(null));
                AnimateSellDrop(targetWorld);
                return;
            }

            Vector2 startScreen = inputCamera != null
                ? inputCamera.WorldToScreenPoint(selectedItemView.transform.position)
                : Vector2.zero;
            if (externalDragGhostActive && TryGetPointerPosition(out Vector2 pointerPosition))
            {
                startScreen = pointerPosition;
            }

            Vector2 endScreen = externalDropZone.GetScreenCenter(null);
            Sprite sprite = selectedItemView.SpriteRenderer != null ? selectedItemView.SpriteRenderer.sprite : null;
            Color color = selectedItemView.SpriteRenderer != null ? selectedItemView.SpriteRenderer.color : Color.white;
            int level = selectedItemView.Item != null ? selectedItemView.Item.Level : 0;

            selectedItemView.SetAlpha(0f);
            sellGhostAnimating = true;
            draggedItemCanvasGhost.PlaySell(sprite, color, level, startScreen, endScreen, CompleteSellAnimation);
        }

        private void CompleteSellAnimation()
        {
            sellGhostAnimating = false;
            boardController.RefreshView();
            ClearSelectedVisual();
            dropRoutine = null;
        }

        private void ShowExternalDragGhost(Vector2 screenPosition)
        {
            if (draggedItemCanvasGhost == null || selectedItemView == null)
            {
                if (selectedItemView != null)
                {
                    selectedItemView.SetDragWorldPosition(ScreenToWorld(screenPosition));
                }

                return;
            }

            Sprite sprite = selectedItemView.SpriteRenderer != null ? selectedItemView.SpriteRenderer.sprite : null;
            Color color = selectedItemView.SpriteRenderer != null ? selectedItemView.SpriteRenderer.color : Color.white;
            int level = selectedItemView.Item != null ? selectedItemView.Item.Level : 0;

            if (!externalDragGhostActive)
            {
                selectedItemView.SetAlpha(0f);
                draggedItemCanvasGhost.ShowDrag(sprite, color, level, screenPosition);
                externalDragGhostActive = true;
                return;
            }

            draggedItemCanvasGhost.MoveTo(screenPosition);
        }

        private void HideExternalDragGhost(bool restoreWorldItem)
        {
            if (!externalDragGhostActive)
            {
                return;
            }

            externalDragGhostActive = false;

            if (draggedItemCanvasGhost != null)
            {
                draggedItemCanvasGhost.Hide();
            }

            if (restoreWorldItem && selectedItemView != null)
            {
                selectedItemView.SetAlpha(1f);
            }
        }

        private void AnimateSellDrop(Vector3 destinationWorld)
        {
            if (dropRoutine != null)
            {
                StopCoroutine(dropRoutine);
            }

            if (selectedItemView == null)
            {
                boardController.RefreshView();
                ClearSelectedVisual();
                return;
            }

            dropRoutine = StartCoroutine(AnimateSellDropRoutine(selectedItemView, destinationWorld));
        }

        private IEnumerator AnimateSellDropRoutine(ItemView itemView, Vector3 destinationWorld)
        {
            Vector3 start = itemView.transform.position;
            float startScale = itemView.transform.localScale.x;
            float elapsed = 0f;

            while (elapsed < sellAnimationSeconds)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / sellAnimationSeconds);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                Vector3 position = Vector3.Lerp(start, destinationWorld, eased);
                itemView.SetDragWorldPosition(position, -0.35f);

                float scale = Mathf.Lerp(startScale, startScale * 0.18f, eased);
                itemView.transform.localScale = new Vector3(scale, scale, 1f);
                itemView.SetAlpha(1f - t);
                yield return null;
            }

            boardController.RefreshView();
            ClearSelectedVisual();
            dropRoutine = null;
        }

        private static bool TryGetPointerPosition(out Vector2 screenPosition)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            if (Mouse.current != null)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }

            screenPosition = default;
            return false;
        }
    }
}
