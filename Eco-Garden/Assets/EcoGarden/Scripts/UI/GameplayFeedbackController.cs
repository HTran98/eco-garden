using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.UI
{
    public sealed class GameplayFeedbackController : MonoBehaviour
    {
        [SerializeField] private Text hudFeedbackText;
        [SerializeField] private float worldTextDuration = 0.45f;
        [SerializeField] private float hudMessageDuration = 1.1f;
        [SerializeField] private Vector3 worldTextTravel = new Vector3(0f, 0.48f, 0f);

        private readonly Queue<WorldTextEntry> worldTextPool = new Queue<WorldTextEntry>();
        private Coroutine hudRoutine;

        private void Awake()
        {
            if (hudFeedbackText == null)
            {
                GameObject feedbackObject = GameObject.Find("FeedbackText");
                if (feedbackObject != null)
                {
                    hudFeedbackText = feedbackObject.GetComponent<Text>();
                }
            }
        }

        public void PlayHudMessage(string message)
        {
            if (hudFeedbackText == null)
            {
                return;
            }

            if (hudRoutine != null)
            {
                StopCoroutine(hudRoutine);
            }

            hudRoutine = StartCoroutine(HudMessageRoutine(message));
        }

        public void PlayWorldText(Vector3 worldPosition, string message, Color color)
        {
            WorldTextEntry entry = GetWorldTextEntry();
            entry.GameObject.transform.position = worldPosition + new Vector3(0f, 0f, -0.35f);
            entry.GameObject.SetActive(true);
            entry.Text.text = message;
            entry.Text.color = color;

            StartCoroutine(WorldTextRoutine(entry, color));
        }

        public void Pulse(Transform target, float scaleMultiplier = 1.16f, float duration = 0.18f)
        {
            if (target != null)
            {
                StartCoroutine(PulseRoutine(target, scaleMultiplier, duration));
            }
        }

        private IEnumerator HudMessageRoutine(string message)
        {
            hudFeedbackText.text = message;
            yield return new WaitForSeconds(hudMessageDuration);

            if (hudFeedbackText != null && hudFeedbackText.text == message)
            {
                hudFeedbackText.text = string.Empty;
            }

            hudRoutine = null;
        }

        private IEnumerator WorldTextRoutine(WorldTextEntry entry, Color color)
        {
            Vector3 start = entry.GameObject.transform.position;
            Vector3 end = start + worldTextTravel;
            float elapsed = 0f;

            while (elapsed < worldTextDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / worldTextDuration);
                float eased = 1f - Mathf.Pow(1f - t, 2f);
                entry.GameObject.transform.position = Vector3.Lerp(start, end, eased);
                entry.Text.color = new Color(color.r, color.g, color.b, 1f - t);
                yield return null;
            }

            entry.GameObject.SetActive(false);
            worldTextPool.Enqueue(entry);
        }

        private IEnumerator PulseRoutine(Transform target, float scaleMultiplier, float duration)
        {
            Vector3 startScale = target.localScale;
            Vector3 peakScale = startScale * scaleMultiplier;
            float halfDuration = duration * 0.5f;
            float elapsed = 0f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(startScale, peakScale, Mathf.Clamp01(elapsed / halfDuration));
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(peakScale, startScale, Mathf.Clamp01(elapsed / halfDuration));
                yield return null;
            }

            target.localScale = startScale;
        }

        private WorldTextEntry GetWorldTextEntry()
        {
            if (worldTextPool.Count > 0)
            {
                return worldTextPool.Dequeue();
            }

            GameObject textObject = new GameObject("WorldFeedbackText");
            TextMesh text = textObject.AddComponent<TextMesh>();
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.characterSize = 0.18f;
            text.fontSize = 72;

            MeshRenderer renderer = text.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = 120;
            }

            textObject.SetActive(false);
            return new WorldTextEntry(textObject, text);
        }

        private sealed class WorldTextEntry
        {
            public WorldTextEntry(GameObject gameObject, TextMesh text)
            {
                GameObject = gameObject;
                Text = text;
            }

            public GameObject GameObject { get; private set; }
            public TextMesh Text { get; private set; }
        }
    }
}
