using System;
using System.Collections.Generic;
using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Economy;
using EcoGarden.Items;
using EcoGarden.Rewards;
using UnityEngine;

namespace EcoGarden.Missions
{
    public sealed class MissionController : MonoBehaviour
    {
        [SerializeField] private MissionDefinition[] missionDefinitions;
        [SerializeField] private BoardController boardController;
        [SerializeField] private EconomyController economyController;

        private readonly List<MissionRuntimeState> missions = new List<MissionRuntimeState>();
        private readonly Dictionary<string, MissionRuntimeState> missionsById = new Dictionary<string, MissionRuntimeState>();

        public IReadOnlyList<MissionRuntimeState> Missions { get { return missions; } }
        public event Action MissionsChanged;

        private void Awake()
        {
            LoadDefinitions();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void Start()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void SetMissionDefinitions(MissionDefinition[] definitions)
        {
            missionDefinitions = definitions;
            LoadDefinitions();
        }

        public bool TryGetMission(string missionId, out MissionRuntimeState state)
        {
            state = null;
            return !string.IsNullOrWhiteSpace(missionId) && missionsById.TryGetValue(missionId, out state);
        }

        public void RestoreMissionStates(IEnumerable<MissionSaveState> saveStates)
        {
            if (saveStates == null)
            {
                return;
            }

            foreach (MissionSaveState saveState in saveStates)
            {
                if (saveState != null && TryGetMission(saveState.MissionId, out MissionRuntimeState state))
                {
                    state.SetProgress(saveState.Progress);
                    state.SetRewardClaimed(saveState.RewardClaimed);
                }
            }

            MissionsChanged?.Invoke();
        }

        public MissionSaveState[] CaptureMissionStates()
        {
            List<MissionSaveState> saveStates = new List<MissionSaveState>();
            for (int i = 0; i < missions.Count; i++)
            {
                MissionRuntimeState state = missions[i];
                if (state == null || string.IsNullOrWhiteSpace(state.MissionId))
                {
                    continue;
                }

                saveStates.Add(new MissionSaveState(state.MissionId, state.Progress, state.RewardClaimed));
            }

            return saveStates.ToArray();
        }

        public bool SetMissionProgress(string missionId, int progress)
        {
            if (!TryGetMission(missionId, out MissionRuntimeState state))
            {
                return false;
            }

            state.SetProgress(progress);
            MissionsChanged?.Invoke();
            return true;
        }

        public MissionClaimResult TryClaimMissionReward(string missionId)
        {
            if (!TryGetMission(missionId, out MissionRuntimeState state))
            {
                return new MissionClaimResult(MissionClaimStatus.NotFound, default);
            }

            if (state.RewardClaimed)
            {
                return new MissionClaimResult(MissionClaimStatus.AlreadyClaimed, default);
            }

            if (!state.IsComplete)
            {
                return new MissionClaimResult(MissionClaimStatus.NotComplete, default);
            }

            ResolveRewardTargets();
            RewardGrantResult rewardResult = RewardService.Grant(
                state.Definition.Reward,
                economyController,
                boardController != null ? boardController.AbilityInventory : null,
                boardController != null ? boardController.PlantUnlockService : null);

            if (!rewardResult.HasAnyGrant)
            {
                return new MissionClaimResult(MissionClaimStatus.RewardUnavailable, rewardResult);
            }

            state.SetRewardClaimed(true);
            MissionsChanged?.Invoke();
            return new MissionClaimResult(MissionClaimStatus.Claimed, rewardResult);
        }

        public void RecordMerge(BoardItem item)
        {
            RecordItemProgress(MissionType.Merge, item);
        }

        public void RecordProduce(BoardItem item)
        {
            RecordItemProgress(MissionType.Produce, item);
        }

        public void RecordSell(BoardItem item)
        {
            RecordItemProgress(MissionType.Sell, item);
        }

        public void RecordDeliver(BoardItem item)
        {
            RecordItemProgress(MissionType.Deliver, item);
        }

        public void RecordAbilityUse(AbilityKind abilityKind)
        {
            bool changed = false;
            for (int i = 0; i < missions.Count; i++)
            {
                MissionRuntimeState state = missions[i];
                MissionDefinition definition = state != null ? state.Definition : null;
                if (state == null ||
                    definition == null ||
                    state.IsComplete ||
                    definition.MissionType != MissionType.UseAbility ||
                    definition.TargetAbility != abilityKind)
                {
                    continue;
                }

                state.AddProgress(1);
                changed = true;
            }

            if (changed)
            {
                MissionsChanged?.Invoke();
            }
        }

        private void LoadDefinitions()
        {
            missions.Clear();
            missionsById.Clear();

            if (missionDefinitions == null)
            {
                return;
            }

            List<MissionDefinition> sortedDefinitions = new List<MissionDefinition>(missionDefinitions);
            sortedDefinitions.Sort(CompareMissionDefinitions);
            for (int i = 0; i < sortedDefinitions.Count; i++)
            {
                MissionDefinition definition = sortedDefinitions[i];
                if (definition == null || !definition.IsValid || missionsById.ContainsKey(definition.MissionId))
                {
                    continue;
                }

                MissionRuntimeState state = new MissionRuntimeState(definition);
                missions.Add(state);
                missionsById.Add(definition.MissionId, state);
            }
        }

        private static int CompareMissionDefinitions(MissionDefinition a, MissionDefinition b)
        {
            int aOrder = a != null ? a.SortOrder : int.MaxValue;
            int bOrder = b != null ? b.SortOrder : int.MaxValue;
            return aOrder.CompareTo(bOrder);
        }

        private void RecordItemProgress(MissionType missionType, BoardItem item)
        {
            if (item == null)
            {
                return;
            }

            bool changed = false;
            for (int i = 0; i < missions.Count; i++)
            {
                MissionRuntimeState state = missions[i];
                MissionDefinition definition = state != null ? state.Definition : null;
                if (state == null ||
                    definition == null ||
                    state.IsComplete ||
                    definition.MissionType != missionType ||
                    !MatchesItem(definition, item))
                {
                    continue;
                }

                state.AddProgress(1);
                changed = true;
            }

            if (changed)
            {
                MissionsChanged?.Invoke();
            }
        }

        private static bool MatchesItem(MissionDefinition definition, BoardItem item)
        {
            bool familyMatches = string.IsNullOrWhiteSpace(definition.TargetFamilyId) ||
                                 definition.TargetFamilyId == item.FamilyId;
            bool levelMatches = definition.TargetItemLevel <= 0 ||
                                definition.TargetItemLevel == item.Level;
            return familyMatches && levelMatches;
        }

        private void Subscribe()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (boardController == null)
            {
                return;
            }

            boardController.ItemMerged -= RecordMerge;
            boardController.ItemProduced -= RecordProduce;
            boardController.ItemSold -= RecordSell;
            boardController.ItemDelivered -= RecordDeliver;
            boardController.AbilityUsed -= RecordAbilityUse;

            boardController.ItemMerged += RecordMerge;
            boardController.ItemProduced += RecordProduce;
            boardController.ItemSold += RecordSell;
            boardController.ItemDelivered += RecordDeliver;
            boardController.AbilityUsed += RecordAbilityUse;
        }

        private void Unsubscribe()
        {
            if (boardController == null)
            {
                return;
            }

            boardController.ItemMerged -= RecordMerge;
            boardController.ItemProduced -= RecordProduce;
            boardController.ItemSold -= RecordSell;
            boardController.ItemDelivered -= RecordDeliver;
            boardController.AbilityUsed -= RecordAbilityUse;
        }

        private void ResolveRewardTargets()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (economyController == null)
            {
                economyController = FindAnyObjectByType<EconomyController>();
            }
        }
    }
}
