using UnityEngine.Events;
using UnityEngine;

namespace RLTY.Customisation
{
    public static class CustomisationManagerHandlerData
    {
        public static event UnityAction OnGetCustomisables;
        public static void GetLoadedCustolisbales() => OnGetCustomisables?.Invoke();


        public static event UnityAction OnCustomizeScene;
        public static void CustomizationStarted()
        {
            OnCustomizeScene?.Invoke();
        }


        public static event UnityAction OnResetScene;
        public static void ResetScene() => OnResetScene?.Invoke();


        public static event UnityAction OnSceneCustomized;
        public static void CustomisationFinished()
        {
            OnSceneCustomized?.Invoke();
        }
    }
}
