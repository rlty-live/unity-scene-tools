using System;

public static class VoiceManagerHandlerData
{
    public static event Action<int, int> AdjustGlobalVoiceChatUserVolume;
    public static event Action<int, bool> OnSetUserSubscription;
    public static event Action OnStart, OnQuit, OnJoinGlobalAudioChat, OnLeaveGlobalAudioChat;
    public static event Action<string> OnJoinVisio, OnLeaveVisio;
    public static event Action<bool> OnTalk;
    public static event Action<bool> OnMute, OnMicrophoneAvailable, OnMuteLocalMicrophone;

    #region Methods for manager implementing voice
    public static void NotifyOnTalk(bool state) => OnTalk?.Invoke(state);
    public static void NotifyAdjustGlobalVoiceChatUserVolume(int clientId, int volume) => AdjustGlobalVoiceChatUserVolume?.Invoke(clientId,volume);
    public static void NotifySetUserSubscription(int clientId, bool subscribe) => OnSetUserSubscription?.Invoke(clientId, subscribe);
    public static void NotifyOnStart() => OnStart?.Invoke();
    public static void NotifyOnQuit() => OnQuit?.Invoke();
    public static void NotifyOnJoinGlobalAudioChat() => OnJoinGlobalAudioChat?.Invoke();
    public static void NotifyOnLeaveGlobalAudioChat() => OnLeaveGlobalAudioChat?.Invoke();
    public static void NotifyOnJoinVisio(string id) => OnJoinVisio?.Invoke(id);
    public static void NotifyOnLeaveVisio(string id) => OnLeaveVisio?.Invoke(id);
    public static void NotifyMute(bool state) => OnMute?.Invoke(state);
    public static void NotifyMuteLocalMicrophone(bool state) => OnMuteLocalMicrophone?.Invoke(state);
    public static void NotifyMicrophoneAvailable(bool state) => OnMicrophoneAvailable?.Invoke(state);

    #endregion
}
