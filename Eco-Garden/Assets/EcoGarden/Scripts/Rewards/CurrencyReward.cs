using System;
using EcoGarden.Economy;
using UnityEngine;

namespace EcoGarden.Rewards
{
    [Serializable]
    public sealed class CurrencyReward
    {
        [SerializeField] private CurrencyKind currencyKind;
        [SerializeField] private int amount;

        public CurrencyKind CurrencyKind { get { return currencyKind; } }
        public int Amount { get { return amount; } }

        public CurrencyReward()
        {
        }

        public CurrencyReward(CurrencyKind currencyKind, int amount)
        {
            this.currencyKind = currencyKind;
            this.amount = amount;
        }
    }
}
