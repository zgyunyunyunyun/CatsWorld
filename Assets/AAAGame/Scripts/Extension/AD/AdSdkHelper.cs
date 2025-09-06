using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;
public abstract class AdSdkHelper : MonoBehaviour
{
    /// <summary>
    /// ��������ȡ�ص�
    /// </summary>
    public GameFrameworkAction<bool> mInterstitialAdLoadedEvent;
    /// <summary>
    /// ��������ȡ�ص�
    /// </summary>
    public GameFrameworkAction<bool> mRewardedAdLoadedEvent;
    /// <summary>
    /// �������رջص�
    /// </summary>
    public GameFrameworkAction<bool> mInterstitialAdClosedEvent;
    /// <summary>
    /// �������رջص�
    /// </summary>
    public GameFrameworkAction<bool> mRewardedAdClosedEvent;
    /// <summary>
    /// �������򿪻ص�
    /// </summary>
    public GameFrameworkAction<bool> mInterstitialAdOpenEvent;
    /// <summary>
    /// �������򿪻ص�
    /// </summary>
    public GameFrameworkAction<bool> mRewardedAdOpenEvent;

    public GameFrameworkAction<bool> mBannerAdLoadedEvent;
    public GameFrameworkAction<bool> mBannerAdOpenEvent;
    public GameFrameworkAction<bool> mBannerAdCloseEvent;

    public GameFrameworkAction<bool> mOnSdkInitialized;
    public bool SdkIsReady { get; private set; }

    public string SdkKey { get; private set; }
    public string SdkInterAdKey { get; private set; }
    public string SdkRewardAdKey { get; private set; }
    public string SdkBannerAdKey { get; private set; }
    public virtual void InitSdk(string key, string interAdKey, string rewardAdKey, string bannerAdKey, GameFrameworkAction<bool> sdkInitialized = null)
    {
        SdkIsReady = false;
        this.mOnSdkInitialized = sdkInitialized;
        this.SdkKey = key;
        this.SdkInterAdKey = interAdKey;
        this.SdkRewardAdKey = rewardAdKey;
        this.SdkBannerAdKey = bannerAdKey;
    }

    /// <summary>
    /// SDK��ʼ���ɹ��ص�,���������ֶ�����
    /// </summary>
    /// <param name="result">��ʼ���Ƿ�ɹ�</param>
    protected virtual void OnSdkInitialized(bool result)
    {
        SdkIsReady = result;
        mOnSdkInitialized?.Invoke(this);
    }

    public abstract void LoadInterstitialAd();
    public abstract void LoadRewardedAd();
    public abstract bool IsInterstitialReady();
    public abstract bool IsRewardedAdReady();
    public abstract void ShowInterstitialAd();
    public abstract void ShowRewardedAd();
    public abstract void ShowBannerAd();
    public abstract void HideBannerAd();
    public abstract void SetBannerBackgroundColor(Color col);
    public abstract bool UserPrivacyAccepted();
}
