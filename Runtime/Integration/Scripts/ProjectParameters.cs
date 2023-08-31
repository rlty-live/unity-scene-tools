using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
#endif

public class ProjectParameters : RLTYMonoBehaviourBase
{
    [ReadOnly, SerializeField]
    private string sceneToolsVersion;
    [ReadOnly, SerializeField]
    private string colorSpace;

#if UNITY_EDITOR
    public void OnValidate()
    {
        PackageInfo[] packages = PackageInfo.GetAllRegisteredPackages();

        foreach (PackageInfo packageInfo in PackageInfo.GetAllRegisteredPackages())
        {
            if (packageInfo.name == "live.rlty.scenetools")
                sceneToolsVersion = packageInfo.version;
        }

        colorSpace = PlayerSettings.colorSpace.ToString();

        if (PlayerSettings.colorSpace != ColorSpace.Linear)
        {
            PlayerSettings.colorSpace = ColorSpace.Linear;
            Debug.Log("Project colorSpace has been set to Linear");
        }
    }
#endif
}
