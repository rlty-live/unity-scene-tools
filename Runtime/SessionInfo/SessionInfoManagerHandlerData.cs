using System;

using RLTY.Customisation;

namespace RLTY.SessionInfo
{
    public class SessionInfoManagerHandlerData
    {
        /// <summary>
        /// Do not listen to this event otherwise you will override SessionInfoManager
        /// </summary>
        public static event Func<SceneDescription> OnGetSceneDescription;

        /// <summary>
        /// Get the scene description
        /// </summary>
        /// <returns></returns>
        public static SceneDescription GetSceneDescription() => OnGetSceneDescription?.Invoke();

        /// <summary>
        /// Do not listen to this event otherwise you will override SessionInfoManager
        /// </summary>
        public static event Func<string> OnGetAccessToken;

        /// <summary>
        /// Get the scene description
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken() => OnGetAccessToken?.Invoke();

        /// <summary>
        /// Do not listen to this event otherwise you will override SessionInfoManager
        /// </summary>
        public static event Func<string> OnGetUserName;
        public static string GetUserName() => OnGetUserName?.Invoke();

        public static event Action OnServerReady;
        public static void ServerReady() => OnServerReady?.Invoke();


        /// <summary>
        /// Notifies that the client is ready, and provides the admin status
        /// At this point Sync_IsAdmin may not be valid (it may take a few milliseconds to sync),
        /// so please use this bool and not AllPlayers.Me.Sync_IsAdmin (unless you are sure that a few milliseconds have passed)
        /// </summary>
        public static event Action<bool> OnClientIsAdminReady;
        public static void NotifyClientReady(bool isAdmin)
        {
            JLogBase.Log("Client ready", typeof(SessionInfoManagerHandlerData));
            OnClientIsAdminReady?.Invoke(isAdmin);
        }

        public static event Action OnClientJoined;
        public static void NotifyClientJoined() => OnClientJoined?.Invoke();


        public static event Action<float> OnAssetLoadProgress;
        public static void AssetLoadProgress(float p) => OnAssetLoadProgress?.Invoke(p);

        public static event Action OnStartAnalyzeVoice;
        public static void StartAnalyzeVoice() => OnStartAnalyzeVoice?.Invoke();

        public static event Action<string> OnUserEnterConferenceStage;
        public static void UserEnterConferenceStage(string stageId) => OnUserEnterConferenceStage?.Invoke(stageId);

        public static event Action<string> OnUserExitConferenceStage;
        public static void UserExitConferenceStage(string stageId) => OnUserExitConferenceStage?.Invoke(stageId);

        public static event Action<string> OnUserDonation;
        public static void UserDonation(string walletId) => OnUserDonation?.Invoke(walletId);

        public static event Action<string> OnWeb3Transaction;
        public static void Web3Transaction(string data) => OnWeb3Transaction?.Invoke(data);

        public static event Action<string> OnOpenTypeForm;
        public static void OpenTypeForm(string typeformId) => OnOpenTypeForm?.Invoke(typeformId);
        
        public static event Action<string> OnOpenIframe;
        public static void OpenIframe(string iframeURL) => OnOpenIframe?.Invoke(iframeURL);



        public static event Action<string, Action<bool>> OnPlayerValidate;

        public static void ValidatePlayer(string playerSessionId, Action<bool> callback) => OnPlayerValidate?.Invoke(playerSessionId, callback);

        public static event Action<string, Action<bool>> OnCheckIsAdmin;

        public static void CheckPlayerIsAdmin(string playerSessionId, Action<bool> callback) => OnCheckIsAdmin?.Invoke(playerSessionId, callback);


        public static event Action<string, Action<bool>> OnPlayerDisconnect;

        public static void DisconnectPlayer(string playerSessionId, Action<bool> callback) => OnPlayerDisconnect?.Invoke(playerSessionId, callback);

        public static event Action<Action<bool>> OnServerClose;

        public static void ServerClose(Action<bool> callback) => OnServerClose?.Invoke(callback);
        
        public static event Action<string> OnChangeCurrentUrl;
        public static void ChangeCurrentUrl(string url) => OnChangeCurrentUrl?.Invoke(url);
        
        public static event Action<string> OnSeeUserProfileInteraction;
        public static void SeeUserProfileInteraction(string playerSessionId) => OnSeeUserProfileInteraction?.Invoke(playerSessionId);
        public static event Action<string> OnBanUserInteraction;
        public static void BanUserInteraction(string playerSessionId) => OnBanUserInteraction?.Invoke(playerSessionId);
        public static event Action<string> OnChatWithUserInteraction;
        public static void ChatWithUserInteraction(string playerSessionId) => OnChatWithUserInteraction?.Invoke(playerSessionId);
        public static event Action<string> OnReportUserInteraction;
        public static void ReportUserInteraction(string playerSessionId) => OnReportUserInteraction?.Invoke(playerSessionId);
        
        public static event Action<string> OnKickUserInteraction;
        public static void KickUserInteraction(string playerSessionId) => OnKickUserInteraction?.Invoke(playerSessionId);
        
        public static event Action<string> OnResetUserInteraction;
        public static void ResetUserInteraction(string playerSessionId) => OnResetUserInteraction?.Invoke(playerSessionId);
        
        public static event Action<string> OnGrowUserInteraction;
        public static void GrowUserInteraction(string playerSessionId) => OnGrowUserInteraction?.Invoke(playerSessionId);
        
        public static event Action<string> OnUnGrowUserInteraction;
        public static void UnGrowUserInteraction(string playerSessionId) => OnUnGrowUserInteraction?.Invoke(playerSessionId);
        
        public static event Action<string> OnLocalMuteUserInteraction;
        public static void LocalMuteUserInteraction(string playerSessionId) => OnLocalMuteUserInteraction?.Invoke(playerSessionId);
        
        public static event Action<string> OnServerMuteUserInteraction;
        public static void ServerMuteUserInteraction(string playerSessionId) => OnServerMuteUserInteraction?.Invoke(playerSessionId);
        
        public static event Action<string> OnBoostUserVoiceInteraction;
        public static void BoostUserVoiceInteraction(string playerSessionId) => OnBoostUserVoiceInteraction?.Invoke(playerSessionId);

    }

    /// <summary>
    /// All information related to the game session (this is useful only on the client side)
    /// </summary>
    [System.Serializable]
    public class GameLiftGameSession
    {
        public string PlayerSessionId = "sessionId";
        public string GameSessionId = "gameSessionId";
        public string DnsName = "localhost";
        public string Voice = "none"; //agora or chime
        public int Port = 7777;
    }
}
