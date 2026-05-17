using System;
using UnityEngine;
using UnityEngine.UI;

namespace EcoGarden.Economy
{
    public sealed class EconomyController : MonoBehaviour
    {
        [SerializeField] private int startingGold;
        [SerializeField] private Text goldText;

        public int Gold { get; private set; }

        public event Action<int> GoldChanged;

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
    }
}
