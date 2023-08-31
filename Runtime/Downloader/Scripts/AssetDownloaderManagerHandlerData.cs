using UnityEngine.Events;
using RLTY.SessionInfo;

namespace RLTY.Customisation
{
    public static class AssetDownloaderManagerHandlerData
    {
        public static UnityAction<KeyValueBase> OnDownloadRequested;
        public static void RequestDownload(KeyValueBase urlKeyValue) => OnDownloadRequested?.Invoke(urlKeyValue);

        public static UnityAction<KeyValueBase> OnDownloadSuccess;
        public static void DownloadSuccess(KeyValueBase keyValueObject) => OnDownloadSuccess?.Invoke(keyValueObject);

        public static UnityAction<KeyValueBase> OnDownloadFailed;
        public static void DownloadFailed(KeyValueBase urlKeyValue) => OnDownloadFailed?.Invoke(urlKeyValue);

        public static UnityAction<SceneDescription> OnAllDownloadFinished;
        public static void AllDownloadFinished(SceneDescription downloadedAssets) => OnAllDownloadFinished?.Invoke(downloadedAssets);
    }
}
