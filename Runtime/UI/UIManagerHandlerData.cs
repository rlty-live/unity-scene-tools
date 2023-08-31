using System;

public static class UIManagerHandlerData
{

    public static event Action<bool> OnPlayerInputEnabled;

    /// <summary>
    /// Call this to enable or disable player input
    /// </summary>
    /// <param name="state"></param>
    public static void EnablePlayerInput(bool state) { OnPlayerInputEnabled?.Invoke(state); }


    /// <summary>
    /// Listen to these events if you want to be informed about fades
    /// </summary>
    public static event Action OnWillFadeOut, OnFinishedFadeOut, OnWillFadeIn, OnFinishedFadeIn;

    #region Events and Methods for code implementing the fade

    public static event Action<float, Action> OnFadeOut;

    public static event Action<float, Action> OnFadeIn;

    public static event Action<float, float, Action, Action> OnCrossFade;

    public static void NotifyWillFadeOut() => OnWillFadeOut?.Invoke();
    public static void NotifyFinishedFadeOut() => OnFinishedFadeOut?.Invoke();
    public static void NotifyWillFadeIn() => OnWillFadeIn?.Invoke();
    public static void NotifyFinishedFadeIn() => OnFinishedFadeIn?.Invoke();


    #endregion

    #region Methods to call to trigger fades
    public static void FadeOut(float time, Action onFinished = null) => OnFadeOut?.Invoke(time, onFinished);

    public static void FadeIn(float time, Action onFinished = null) => OnFadeIn?.Invoke(time, onFinished);

    public static void CrossFade(float outTime, float inTime, Action onFadedOut = null, Action onFinished = null)
    { 
        if (OnCrossFade!=null)
            OnCrossFade.Invoke(outTime, inTime, onFadedOut, onFinished);
        else
        {
            onFadedOut?.Invoke();
            onFinished?.Invoke();
        }    
    }

    #endregion
}
