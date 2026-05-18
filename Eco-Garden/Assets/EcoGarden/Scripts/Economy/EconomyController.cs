using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Economy
{
    public sealed class EconomyController : MonoBehaviour
    {
        [SerializeField] private int startingGold;
        [SerializeField] private Text goldText;
        [SerializeField] private float goldPulseScale = 1.18f;
        [SerializeField] private float goldPulseDuration = 0.22f;
        [SerializeField] private Color goldPulseColor = new Color(1f, 0.86f, 0.24f, 1f);

        public int Gold { get; private set; }

        public event Action<int> GoldChanged;

        private Coroutine goldPulseRoutine;
        private Vector3 goldTextBaseScale = Vector3.one;
        private Color goldTextBaseColor = Color.white;

        private void Awake()
        {
            if (goldText == null)
            {
                GameObject goldObject = GameObject.Find("GoldText");
                if (goldObject != null)
                {
                    goldText = goldObject.GetComponent<Text>();
                }
            }

            CacheGoldTextBaseState();
            Gold = startingGold;
            RefreshGoldText();
        }

        public void AddGold(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Gold += amount;
            RefreshGoldText();
            PlayGoldPulse();
            GoldChanged?.Invoke(Gold);
        }

        public void SetGold(int amount)
        {
            Gold = amount < 0 ? 0 : amount;
            RefreshGoldText();
            GoldChanged?.Invoke(Gold);
        }

        public bool TrySpendGold(int amount)
        {
            if (amount < 0 || Gold < amount)
            {
                return false;
            }

            Gold -= amount;
            RefreshGoldText();
            GoldChanged?.Invoke(Gold);
            return true;
        }

        private void RefreshGoldText()
        {
            if (goldText != null)
            {
                goldText.text = "Gold " + Gold;
            }
        }

        private void CacheGoldTextBaseState()
        {
            if (goldText == null)
            {
                return;
            }

            goldTextBaseScale = goldText.transform.localScale;
            goldTextBaseColor = goldText.color;
        }

        private void PlayGoldPulse()
        {
            if (goldText == null)
            {
                return;
            }

            if (goldPulseRoutine != null)
            {
                StopCoroutine(goldPulseRoutine);
                goldText.transform.localScale = goldTextBaseScale;
                goldText.color = goldTextBaseColor;
            }

            goldPulseRoutine = StartCoroutine(GoldPulseRoutine());
        }

        private IEnumerator GoldPulseRoutine()
        {
            Vector3 peakScale = goldTextBaseScale * goldPulseScale;
            float halfDuration = goldPulseDuration * 0.5f;
            float elapsed = 0f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                goldText.transform.localScale = Vector3.Lerp(goldTextBaseScale, peakScale, t);
                goldText.color = Color.Lerp(goldTextBaseColor, goldPulseColor, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                goldText.transform.localScale = Vector3.Lerp(peakScale, goldTextBaseScale, t);
                goldText.color = Color.Lerp(goldPulseColor, goldTextBaseColor, t);
                yield return null;
            }

            goldText.transform.localScale = goldTextBaseScale;
            goldText.color = goldTextBaseColor;
            goldPulseRoutine = null;
        }
    }
}
