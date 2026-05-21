using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Economy
{
    public sealed class EconomyController : MonoBehaviour
    {
        [SerializeField] private int startingGold;
        [SerializeField] private int startingGem;
        [SerializeField] private Text goldText;
        [SerializeField] private Text gemText;
        [SerializeField] private float goldPulseScale = 1.18f;
        [SerializeField] private float goldPulseDuration = 0.22f;
        [SerializeField] private Color goldPulseColor = new Color(1f, 0.86f, 0.24f, 1f);
        [SerializeField] private Color gemPulseColor = new Color(0.58f, 0.88f, 1f, 1f);

        public int Gold { get; private set; }
        public int Gem { get; private set; }

        public event Action<int> GoldChanged;
        public event Action<int> GemChanged;
        public event Action<CurrencyKind, int> CurrencyChanged;

        private Coroutine goldPulseRoutine;
        private Coroutine gemPulseRoutine;
        private Vector3 goldTextBaseScale = Vector3.one;
        private Color goldTextBaseColor = Color.white;
        private Vector3 gemTextBaseScale = Vector3.one;
        private Color gemTextBaseColor = Color.white;

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

            if (gemText == null)
            {
                GameObject gemObject = GameObject.Find("GemText");
                if (gemObject != null)
                {
                    gemText = gemObject.GetComponent<Text>();
                }
            }

            EnsureGemText();
            CacheTextBaseState();
            Gold = startingGold;
            Gem = startingGem;
            RefreshCurrencyText(CurrencyKind.Gold);
            RefreshCurrencyText(CurrencyKind.Gem);
        }

        public void AddGold(int amount)
        {
            AddCurrency(CurrencyKind.Gold, amount);
        }

        public void AddGem(int amount)
        {
            AddCurrency(CurrencyKind.Gem, amount);
        }

        public void SetGold(int amount)
        {
            SetCurrency(CurrencyKind.Gold, amount);
        }

        public void SetGem(int amount)
        {
            SetCurrency(CurrencyKind.Gem, amount);
        }

        public bool TrySpendGold(int amount)
        {
            return TrySpendCurrency(CurrencyKind.Gold, amount);
        }

        public bool TrySpendGem(int amount)
        {
            return TrySpendCurrency(CurrencyKind.Gem, amount);
        }

        public int GetBalance(CurrencyKind currencyKind)
        {
            return currencyKind == CurrencyKind.Gem ? Gem : Gold;
        }

        public void AddCurrency(CurrencyKind currencyKind, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            SetBalance(currencyKind, GetBalance(currencyKind) + amount);
            RefreshCurrencyText(currencyKind);
            PlayCurrencyPulse(currencyKind);
            InvokeCurrencyChanged(currencyKind);
        }

        public void SetCurrency(CurrencyKind currencyKind, int amount)
        {
            SetBalance(currencyKind, amount < 0 ? 0 : amount);
            RefreshCurrencyText(currencyKind);
            InvokeCurrencyChanged(currencyKind);
        }

        public bool TrySpendCurrency(CurrencyKind currencyKind, int amount)
        {
            if (amount < 0 || GetBalance(currencyKind) < amount)
            {
                return false;
            }

            SetBalance(currencyKind, GetBalance(currencyKind) - amount);
            RefreshCurrencyText(currencyKind);
            InvokeCurrencyChanged(currencyKind);
            return true;
        }

        private void SetBalance(CurrencyKind currencyKind, int amount)
        {
            if (currencyKind == CurrencyKind.Gem)
            {
                Gem = amount;
                return;
            }

            Gold = amount;
        }

        private void EnsureGemText()
        {
            if (gemText != null || goldText == null)
            {
                return;
            }

            RectTransform goldRect = goldText.GetComponent<RectTransform>();
            if (goldRect != null)
            {
                goldRect.anchorMin = new Vector2(0.25f, 0f);
                goldRect.anchorMax = new Vector2(0.50f, 1f);
                goldRect.anchoredPosition = Vector2.zero;
                goldRect.sizeDelta = Vector2.zero;
            }

            GameObject gemObject = new GameObject("GemText", typeof(RectTransform));
            gemObject.transform.SetParent(goldText.transform.parent, false);

            RectTransform gemRect = gemObject.GetComponent<RectTransform>();
            gemRect.anchorMin = new Vector2(0.50f, 0f);
            gemRect.anchorMax = new Vector2(0.75f, 1f);
            gemRect.pivot = new Vector2(0.5f, 0.5f);
            gemRect.anchoredPosition = Vector2.zero;
            gemRect.sizeDelta = Vector2.zero;

            gemText = gemObject.AddComponent<Text>();
            gemText.text = "Gem 0";
            gemText.alignment = TextAnchor.MiddleCenter;
            gemText.font = goldText.font;
            gemText.fontSize = goldText.fontSize;
            gemText.color = gemPulseColor;
            gemText.raycastTarget = false;
            gemText.resizeTextForBestFit = goldText.resizeTextForBestFit;
            gemText.resizeTextMinSize = goldText.resizeTextMinSize;
            gemText.resizeTextMaxSize = goldText.resizeTextMaxSize;
            gemText.horizontalOverflow = goldText.horizontalOverflow;
            gemText.verticalOverflow = goldText.verticalOverflow;
        }

        private void RefreshCurrencyText(CurrencyKind currencyKind)
        {
            if (currencyKind == CurrencyKind.Gem)
            {
                if (gemText != null)
                {
                    gemText.text = "Gem " + Gem;
                }

                return;
            }

            if (goldText != null)
            {
                goldText.text = "Gold " + Gold;
            }
        }

        private void CacheTextBaseState()
        {
            if (goldText != null)
            {
                goldTextBaseScale = goldText.transform.localScale;
                goldTextBaseColor = goldText.color;
            }

            if (gemText != null)
            {
                gemTextBaseScale = gemText.transform.localScale;
                gemTextBaseColor = gemText.color;
            }
        }

        private void PlayCurrencyPulse(CurrencyKind currencyKind)
        {
            if (currencyKind == CurrencyKind.Gem)
            {
                PlayGemPulse();
                return;
            }

            PlayGoldPulse();
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

        private void PlayGemPulse()
        {
            if (gemText == null)
            {
                return;
            }

            if (gemPulseRoutine != null)
            {
                StopCoroutine(gemPulseRoutine);
                gemText.transform.localScale = gemTextBaseScale;
                gemText.color = gemTextBaseColor;
            }

            gemPulseRoutine = StartCoroutine(GemPulseRoutine());
        }

        private IEnumerator GoldPulseRoutine()
        {
            yield return CurrencyPulseRoutine(goldText, goldTextBaseScale, goldTextBaseColor, goldPulseColor);
            goldPulseRoutine = null;
        }

        private IEnumerator GemPulseRoutine()
        {
            yield return CurrencyPulseRoutine(gemText, gemTextBaseScale, gemTextBaseColor, gemPulseColor);
            gemPulseRoutine = null;
        }

        private IEnumerator CurrencyPulseRoutine(Text targetText, Vector3 baseScale, Color baseColor, Color pulseColor)
        {
            Vector3 peakScale = baseScale * goldPulseScale;
            float halfDuration = goldPulseDuration * 0.5f;
            float elapsed = 0f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                targetText.transform.localScale = Vector3.Lerp(baseScale, peakScale, t);
                targetText.color = Color.Lerp(baseColor, pulseColor, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / halfDuration);
                targetText.transform.localScale = Vector3.Lerp(peakScale, baseScale, t);
                targetText.color = Color.Lerp(pulseColor, baseColor, t);
                yield return null;
            }

            targetText.transform.localScale = baseScale;
            targetText.color = baseColor;
        }

        private void InvokeCurrencyChanged(CurrencyKind currencyKind)
        {
            int balance = GetBalance(currencyKind);
            if (currencyKind == CurrencyKind.Gem)
            {
                GemChanged?.Invoke(balance);
            }
            else
            {
                GoldChanged?.Invoke(balance);
            }

            CurrencyChanged?.Invoke(currencyKind, balance);
        }
    }
}
