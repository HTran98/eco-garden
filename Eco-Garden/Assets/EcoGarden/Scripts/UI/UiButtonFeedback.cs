using EcoGarden.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class UiButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float pressedScale = 0.94f;

        private Vector3 baseScale;
        private bool initialized;

        private void Awake()
        {
            CacheBaseScale();
        }

        private void OnEnable()
        {
            CacheBaseScale();
        }

        private void OnDisable()
        {
            Restore();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable())
            {
                return;
            }

            CacheBaseScale();
            EcoGardenAudioController.Instance?.PlayButtonTap();
            transform.localScale = baseScale * pressedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Restore();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Restore();
        }

        private void CacheBaseScale()
        {
            if (initialized)
            {
                return;
            }

            baseScale = transform.localScale;
            initialized = true;
        }

        private void Restore()
        {
            if (initialized)
            {
                transform.localScale = baseScale;
            }
        }

        private bool IsInteractable()
        {
            Button button = GetComponent<Button>();
            return button == null || button.interactable;
        }
    }
}
