using EcoGarden.Abilities;
using EcoGarden.Board;
using EcoGarden.Level;
using EcoGarden.Save;
using EcoGarden.Shop;
using UnityEngine;

namespace EcoGarden.Audio
{
    public sealed class EcoGardenAudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private BoardController boardController;
        [SerializeField] private LevelStateController levelStateController;
        [SerializeField] private float sfxVolume = 0.78f;
        [SerializeField] private float musicVolume = 0.32f;
        [SerializeField] private float ambienceVolume = 0.18f;
        [SerializeField] private float timerWarningSeconds = 20f;

        [Header("Gameplay SFX")]
        [SerializeField] private AudioClip itemPickupClip;
        [SerializeField] private AudioClip itemDropValidClip;
        [SerializeField] private AudioClip itemDropInvalidClip;
        [SerializeField] private AudioClip mergeClip;
        [SerializeField] private AudioClip producerSpawnClip;
        [SerializeField] private AudioClip sellItemClip;
        [SerializeField] private AudioClip deliverySubmitClip;
        [SerializeField] private AudioClip orderCompleteClip;
        [SerializeField] private AudioClip levelCompleteClip;
        [SerializeField] private AudioClip levelFailedClip;
        [SerializeField] private AudioClip timerWarningClip;

        [Header("Ability SFX")]
        [SerializeField] private AudioClip shovelClip;
        [SerializeField] private AudioClip magicWandClip;
        [SerializeField] private AudioClip sortingMagnetClip;
        [SerializeField] private AudioClip abilityUnavailableClip;

        [Header("Economy and UI SFX")]
        [SerializeField] private AudioClip goldGainClip;
        [SerializeField] private AudioClip gemGainClip;
        [SerializeField] private AudioClip rewardClaimClip;
        [SerializeField] private AudioClip missionClaimClip;
        [SerializeField] private AudioClip shopPurchaseSuccessClip;
        [SerializeField] private AudioClip shopPurchaseFailedClip;
        [SerializeField] private AudioClip iapPendingClip;
        [SerializeField] private AudioClip buttonTapClip;
        [SerializeField] private AudioClip panelOpenClip;
        [SerializeField] private AudioClip panelCloseClip;
        [SerializeField] private AudioClip pauseOpenClip;
        [SerializeField] private AudioClip decorationApplyClip;

        [Header("Music and Ambience")]
        [SerializeField] private AudioClip pondAmbienceClip;
        [SerializeField] private AudioClip levelMusicClip;
        [SerializeField] private AudioClip menuMusicClip;

        private BoardController subscribedBoardController;
        private LevelStateController subscribedLevelStateController;
        private bool soundEnabled = true;
        private bool musicEnabled = true;
        private int lastTimerWarningSecond = -1;
        private bool wasTimerWarningActive;

        public static EcoGardenAudioController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            ResolveReferences();
            EnsureSources();
            ApplySavedSettings();
        }

        private void OnEnable()
        {
            ResolveReferences();
            Subscribe();
            ApplySavedSettings();
            PlayDefaultLoops();
        }

        private void OnDisable()
        {
            Unsubscribe();
            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }
        }

        private void Update()
        {
            UpdateTimerWarning();
        }

        public void PlayItemPickup()
        {
            PlaySfx(itemPickupClip);
        }

        public void PlayValidDrop()
        {
            PlaySfx(itemDropValidClip);
        }

        public void PlayInvalidDrop()
        {
            PlaySfx(itemDropInvalidClip);
        }

        public void PlayAbilityUnavailable()
        {
            PlaySfx(abilityUnavailableClip);
        }

        public void PlayRewardClaim()
        {
            PlaySfx(rewardClaimClip);
        }

        public void PlayMissionClaim(bool succeeded)
        {
            PlaySfx(succeeded ? missionClaimClip : itemDropInvalidClip);
        }

        public void PlayShopPurchase(ShopPurchaseStatus status)
        {
            switch (status)
            {
                case ShopPurchaseStatus.Success:
                    PlaySfx(shopPurchaseSuccessClip);
                    break;
                case ShopPurchaseStatus.Pending:
                    PlaySfx(iapPendingClip);
                    break;
                case ShopPurchaseStatus.IapCancelled:
                case ShopPurchaseStatus.IapFailed:
                case ShopPurchaseStatus.InsufficientCurrency:
                case ShopPurchaseStatus.InvalidProduct:
                case ShopPurchaseStatus.ProductNotFound:
                case ShopPurchaseStatus.UnsupportedPurchaseKind:
                    PlaySfx(shopPurchaseFailedClip);
                    break;
            }
        }

        public void PlayButtonTap()
        {
            PlaySfx(buttonTapClip, 0.72f);
        }

        public void PlayPanelOpen()
        {
            PlaySfx(panelOpenClip);
        }

        public void PlayPanelClose()
        {
            PlaySfx(panelCloseClip);
        }

        public void PlayPauseOpen()
        {
            PlaySfx(pauseOpenClip);
        }

        public void PlayDecorationApply()
        {
            PlaySfx(decorationApplyClip);
        }

        public void SetClipByAssetId(string assetId, AudioClip clip)
        {
            if (string.IsNullOrWhiteSpace(assetId) || clip == null)
            {
                return;
            }

            switch (assetId)
            {
                case "sfx_item_pickup_01": itemPickupClip = clip; break;
                case "sfx_item_drop_valid_01": itemDropValidClip = clip; break;
                case "sfx_item_drop_invalid_01": itemDropInvalidClip = clip; break;
                case "sfx_merge_01": mergeClip = clip; break;
                case "sfx_producer_spawn_01": producerSpawnClip = clip; break;
                case "sfx_sell_item_01": sellItemClip = clip; break;
                case "sfx_delivery_submit_01": deliverySubmitClip = clip; break;
                case "sfx_order_complete_01": orderCompleteClip = clip; break;
                case "sfx_level_complete_01": levelCompleteClip = clip; break;
                case "sfx_level_failed_01": levelFailedClip = clip; break;
                case "sfx_timer_warning_01": timerWarningClip = clip; break;
                case "sfx_ability_shovel_01": shovelClip = clip; break;
                case "sfx_ability_magic_wand_01": magicWandClip = clip; break;
                case "sfx_ability_sorting_magnet_01": sortingMagnetClip = clip; break;
                case "sfx_ability_unavailable_01": abilityUnavailableClip = clip; break;
                case "sfx_gold_gain_01": goldGainClip = clip; break;
                case "sfx_gem_gain_01": gemGainClip = clip; break;
                case "sfx_reward_claim_01": rewardClaimClip = clip; break;
                case "sfx_mission_claim_01": missionClaimClip = clip; break;
                case "sfx_shop_purchase_success_01": shopPurchaseSuccessClip = clip; break;
                case "sfx_shop_purchase_failed_01": shopPurchaseFailedClip = clip; break;
                case "sfx_iap_pending_01": iapPendingClip = clip; break;
                case "sfx_button_tap_01": buttonTapClip = clip; break;
                case "sfx_panel_open_01": panelOpenClip = clip; break;
                case "sfx_panel_close_01": panelCloseClip = clip; break;
                case "sfx_pause_open_01": pauseOpenClip = clip; break;
                case "sfx_decoration_apply_01": decorationApplyClip = clip; break;
                case "amb_pond_day_loop_01": pondAmbienceClip = clip; break;
                case "music_level_pastel_zen_01": levelMusicClip = clip; break;
                case "music_menu_garden_01": menuMusicClip = clip; break;
            }
        }

        private void Subscribe()
        {
            if (!ReferenceEquals(subscribedBoardController, boardController))
            {
                UnsubscribeBoard();
                if (boardController != null)
                {
                    boardController.ItemMerged += OnItemMerged;
                    boardController.ItemProduced += OnItemProduced;
                    boardController.ItemSold += OnItemSold;
                    boardController.ItemDelivered += OnItemDelivered;
                    boardController.OrderCompleted += OnOrderCompleted;
                    boardController.AbilityUsed += OnAbilityUsed;
                    subscribedBoardController = boardController;
                }
            }

            if (!ReferenceEquals(subscribedLevelStateController, levelStateController))
            {
                UnsubscribeLevelState();
                if (levelStateController != null)
                {
                    levelStateController.LevelCompleted += OnLevelCompleted;
                    levelStateController.LevelFailed += OnLevelFailed;
                    subscribedLevelStateController = levelStateController;
                }
            }
        }

        private void Unsubscribe()
        {
            UnsubscribeBoard();
            UnsubscribeLevelState();
        }

        private void UnsubscribeBoard()
        {
            if (subscribedBoardController == null)
            {
                return;
            }

            subscribedBoardController.ItemMerged -= OnItemMerged;
            subscribedBoardController.ItemProduced -= OnItemProduced;
            subscribedBoardController.ItemSold -= OnItemSold;
            subscribedBoardController.ItemDelivered -= OnItemDelivered;
            subscribedBoardController.OrderCompleted -= OnOrderCompleted;
            subscribedBoardController.AbilityUsed -= OnAbilityUsed;
            subscribedBoardController = null;
        }

        private void UnsubscribeLevelState()
        {
            if (subscribedLevelStateController == null)
            {
                return;
            }

            subscribedLevelStateController.LevelCompleted -= OnLevelCompleted;
            subscribedLevelStateController.LevelFailed -= OnLevelFailed;
            subscribedLevelStateController = null;
        }

        private void OnItemMerged(Items.BoardItem item)
        {
            PlaySfx(mergeClip);
        }

        private void OnItemProduced(Items.BoardItem item)
        {
            PlaySfx(producerSpawnClip);
        }

        private void OnItemSold(Items.BoardItem item)
        {
            PlaySfx(sellItemClip);
        }

        private void OnItemDelivered(Items.BoardItem item)
        {
            PlaySfx(deliverySubmitClip);
        }

        private void OnOrderCompleted()
        {
            PlaySfx(orderCompleteClip);
        }

        private void OnLevelCompleted()
        {
            PlaySfx(levelCompleteClip);
        }

        private void OnLevelFailed()
        {
            PlaySfx(levelFailedClip);
        }

        private void OnAbilityUsed(AbilityKind abilityKind)
        {
            switch (abilityKind)
            {
                case AbilityKind.Shovel:
                    PlaySfx(shovelClip);
                    break;
                case AbilityKind.MagicWand:
                    PlaySfx(magicWandClip);
                    break;
                case AbilityKind.SortingMagnet:
                    PlaySfx(sortingMagnetClip);
                    break;
            }
        }

        private void UpdateTimerWarning()
        {
            if (levelStateController == null || !levelStateController.IsPlaying)
            {
                wasTimerWarningActive = false;
                lastTimerWarningSecond = -1;
                return;
            }

            float remaining = levelStateController.RemainingSeconds;
            bool warningActive = remaining > 0f && remaining <= timerWarningSeconds;
            if (!warningActive)
            {
                wasTimerWarningActive = false;
                lastTimerWarningSecond = -1;
                return;
            }

            int second = Mathf.CeilToInt(remaining);
            if (!wasTimerWarningActive || second != lastTimerWarningSecond)
            {
                PlaySfx(timerWarningClip, 0.55f);
                wasTimerWarningActive = true;
                lastTimerWarningSecond = second;
            }
        }

        private void PlayDefaultLoops()
        {
            if (musicSource != null && levelMusicClip != null && musicSource.clip != levelMusicClip)
            {
                musicSource.clip = levelMusicClip;
                musicSource.loop = true;
            }

            if (ambienceSource != null && pondAmbienceClip != null && ambienceSource.clip != pondAmbienceClip)
            {
                ambienceSource.clip = pondAmbienceClip;
                ambienceSource.loop = true;
            }

            RefreshLoopPlayback();
        }

        private void RefreshLoopPlayback()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
                if (musicEnabled && musicSource.clip != null)
                {
                    if (!musicSource.isPlaying)
                    {
                        musicSource.Play();
                    }
                }
                else
                {
                    musicSource.Stop();
                }
            }

            if (ambienceSource != null)
            {
                ambienceSource.volume = ambienceVolume;
                if (musicEnabled && ambienceSource.clip != null)
                {
                    if (!ambienceSource.isPlaying)
                    {
                        ambienceSource.Play();
                    }
                }
                else
                {
                    ambienceSource.Stop();
                }
            }
        }

        private void PlaySfx(AudioClip clip, float volumeScale = 1f)
        {
            if (!soundEnabled || sfxSource == null || clip == null)
            {
                return;
            }

            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
        }

        private void ApplySavedSettings()
        {
            SaveData saveData = SaveService.Load();
            soundEnabled = saveData == null || saveData.soundEnabled;
            musicEnabled = saveData == null || saveData.musicEnabled;
            RefreshLoopPlayback();
        }

        private void ResolveReferences()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (levelStateController == null)
            {
                levelStateController = FindAnyObjectByType<LevelStateController>();
            }
        }

        private void EnsureSources()
        {
            sfxSource = EnsureSource(sfxSource, "SfxSource", false, sfxVolume);
            musicSource = EnsureSource(musicSource, "MusicSource", true, musicVolume);
            ambienceSource = EnsureSource(ambienceSource, "AmbienceSource", true, ambienceVolume);
        }

        private AudioSource EnsureSource(AudioSource source, string childName, bool loop, float volume)
        {
            if (source == null)
            {
                Transform child = transform.Find(childName);
                if (child == null)
                {
                    GameObject sourceObject = new GameObject(childName);
                    sourceObject.transform.SetParent(transform, false);
                    child = sourceObject.transform;
                }

                source = child.GetComponent<AudioSource>();
                if (source == null)
                {
                    source = child.gameObject.AddComponent<AudioSource>();
                }
            }

            source.playOnAwake = false;
            source.loop = loop;
            source.volume = volume;
            return source;
        }
    }
}
