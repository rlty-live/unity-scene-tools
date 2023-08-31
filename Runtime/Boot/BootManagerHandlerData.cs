using UnityEngine.Events;
using UnityEngine;

namespace RLTY.Boot
{
    public static class BootManagerHandlerData
    {
        public static event UnityAction OnSceneReadyForCustomization;
        public static event UnityAction OnSceneReadyForNetworkedCustomization;

        /// <summary>
        /// Called by manager
        /// </summary>
        public static void NotifySceneReadyForCustomization() => OnSceneReadyForCustomization?.Invoke();
        public static void NotifySceneReadyForNetworkedCustomization() => OnSceneReadyForNetworkedCustomization?.Invoke();
    }
}
