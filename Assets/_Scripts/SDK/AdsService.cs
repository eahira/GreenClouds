using System;
using UnityEngine;

#if UNITY_WEBGL
using Playgama.Modules.Advertisement;
#endif

public class AdsService : MonoBehaviour
{
    public static AdsService Instance { get; private set; }

    [SerializeField] private int minDelayBetweenInterstitial = 60;

#if UNITY_WEBGL
    private AdvertisementModule _ads;
#endif

    private Action _pendingReward;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_WEBGL
        _ads = GetComponent<AdvertisementModule>();
        if (_ads == null)
            _ads = gameObject.AddComponent<AdvertisementModule>();
#endif
    }

    private void OnEnable()
    {
#if UNITY_WEBGL
        if (_ads != null)
            _ads.rewardedStateChanged += OnRewardedStateChanged;
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        if (_ads != null)
            _ads.rewardedStateChanged -= OnRewardedStateChanged;
#endif
    }

    private void Start()
    {
#if UNITY_WEBGL
        _ads.SetMinimumDelayBetweenInterstitial(minDelayBetweenInterstitial);
#endif
    }

    public void ShowInterstitial(string placement = null)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (_ads == null || !_ads.isInterstitialSupported) return;
        _ads.ShowInterstitial(placement);
#endif
    }

    public void ShowRewarded(Action onReward, string placement = null)
{
#if UNITY_WEBGL && !UNITY_EDITOR
    if (_ads == null || !_ads.isRewardedSupported) return;

    _pendingReward = onReward;
    _ads.ShowRewarded(placement);
#else
    onReward?.Invoke();
#endif
}


#if UNITY_WEBGL
    private void OnRewardedStateChanged(RewardedState state)
    {
        if (state == RewardedState.Rewarded)
        {
            _pendingReward?.Invoke();
            _pendingReward = null;
        }

        if (state == RewardedState.Closed || state == RewardedState.Failed)
        {
            _pendingReward = null;
        }
    }
#endif
}
