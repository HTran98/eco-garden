using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class CoinBurstFeedback : MonoBehaviour
    {
        [SerializeField] private Text floatingText;
        [SerializeField] private float duration = 0.55f;
        [SerializeField] private Vector2 travel = new Vector2(0f, 90f);

        public void Play(Vector3 worldPosition, int amount)
        {
            if (floatingText == null)
            {
                floatingText = GetComponent<Text>();
            }

            transform.position = worldPosition;
            floatingText.text = "+" + amount + " gold";
            floatingText.color = new Color(1f, 0.84f, 0.22f, 1f);
            StopAllCoroutines();
            StartCoroutine(PlayRoutine(worldPosition));
        }

        private IEnumerator PlayRoutine(Vector3 startPosition)
        {
            float elapsed = 0f;
            Color startColor = floatingText.color;
            Vector3 endPosition = startPosition + new Vector3(travel.x, travel.y, 0f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(startPosition, endPosition, 1f - Mathf.Pow(1f - t, 2f));
                floatingText.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
                yield return null;
            }

            floatingText.text = string.Empty;
        }
    }
}
