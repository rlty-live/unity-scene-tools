using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

#if UNITY_EDITOR
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEditor;
#endif

namespace RLTY.Customisation
{
    [DisallowMultipleComponent]
    [HideMonoScript]
    [HelpURL("https://www.youtube.com/watch?v=-AXetJvTfU0"), AddComponentMenu("RLTY/Customisable/Managers/Customizer")]
    public class CustomisationManager : RLTYMonoBehaviour
    {
        #region Global variables

        [SerializeField, ReadOnly] private string sceneToolsVersion;

        [Title("Organizing")] [Space(5)] [DetailedInfoBox("How to", howTo, InfoMessageType.Info)]
        public List<string> groupLabel = new List<string>();

        public List<string> groups = new List<string>();
        public List<string> sections = new List<string>();

        [Title("Sorting")] public List<Customisable> customisablesInScene = new List<Customisable>();

        //Set in Reset()
        Texture2D customisableClassification;

        [SerializeField] private const string howTo =
            "1) Reorder those lists to change their displaying order on the website \n" +
            "2) You can add and set groups on any Customizable component \n" +
            "see documentation for more information.";

        [PropertyOrder(40)] [SerializeField, HorizontalGroup("selector", Title = "Tools"), LabelWidth(100)]
        private CustomisableType customisables;

        [ReadOnly, SerializeField] private List<Customisable> uncustomizedCustomisables;

        #endregion

        #region EditorOnly logic

#if UNITY_EDITOR

        [Button]
        public void OpenDocumentation() => Application.OpenURL(
            "https://rlty.notion.site/How-to-set-descriptions-for-customizables-15f2ae881601470084454b994f8c1cbf");

        /// <summary>
        /// Select all Customisable corresponding to CustomisablesToSelect in Hierarchy
        /// </summary>
        [Button("Select"), HorizontalGroup("selector")]
        public void SelectAll()
        {
            Customisable[] list = FindObjectsOfType<Customisable>();
            List<GameObject> gos = new List<GameObject>();
            foreach (Customisable c in list)
            {
                if (c.type == customisables)
                    gos.Add(c.gameObject);
            }

            Selection.objects = gos.ToArray();
        }

        public void OnValidate()
        {
            GetPackageVersion();
            RefreshCustomisableList();
        }

        public void Reset()
        {
            customisableClassification = (Texture2D)AssetDatabase.LoadAssetAtPath(
                "Packages/com.live.rlty.scenetools/Docs/Customisables/classification.png", typeof(Texture2D));
        }

        public void RefreshCustomisableList()
        {
            if (!PrefabUtility.IsPartOfPrefabAsset(this))
                //If there are elements in it
                if (customisablesInScene.Any())
                {
                    Customisable[] tempCustomisablesInScene = FindObjectsOfType<Customisable>();

                    //Only add the new ones at the end
                    foreach (Customisable custo in tempCustomisablesInScene)
                    {
                        if (customisablesInScene.Contains(custo)) continue;
                        else customisablesInScene.Add(custo);
                    }

                    JLogBase.Log("Updated customisable List", this);
                }
                //if not create new list
                else
                {
                    customisablesInScene = FindObjectsOfType<Customisable>().ToList();
                    JLogBase.Log("Created new List", this);
                }

            customisablesInScene.RemoveAll(item => item == null);
        }
#endif

        #endregion

        #region Runtime logic

        #region UnityCallbacks

        public void Awake()
        {
            LogPackageVersion();
        }

        public void Update()
        {
            if (Debug.isDebugBuild && uncustomizedCustomisables != null)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    ToggleUnCustomizedActivation();
            }
        }

        #endregion

        public void GetPackageVersion()
        {
#if UNITY_EDITOR
            foreach (PackageInfo packageInfo in PackageInfo.GetAllRegisteredPackages())
            {
                if (packageInfo.name == "live.rlty.scenetools")
                    sceneToolsVersion = packageInfo.version;
            }
#endif
        }

        public void LogPackageVersion()
        {
            if (sceneToolsVersion != null)
                JLog("RLTY SceneTools package version: " + sceneToolsVersion);
        }

        /// <summary>
        /// Gathers all customisables, check compatibility with SceneDescription stored in database, 
        /// and starts customization
        /// </summary>
        /// <param name="sceneDescription">The configuration file that lists all customisation keys and values for this building and this event</param>
        public void CustomizeScene(SceneDescription sceneDescription)
        {
            Customisable[] fullList = FindObjectsOfType<Customisable>();
            JLog("Starting Customization, customisable count=" + fullList.Length);
            CustomisationManagerHandlerData.CustomizationStarted();
            foreach (CustomisableTypeEntry entry in sceneDescription.entries)
            {
                CustomisableType type = entry.Type;
                foreach (KeyValueBase k in entry.keyPairs)
                {
                    if (type == CustomisableType.Invalid)
                        JLogBase.LogError(
                            "Invalid key type in scene description: key=" + k.key + " value=" + k.value + " type=" +
                            entry.type, this);
                    else
                        SearchAndCustomize(type, fullList, k);
                }
            }

#if UNITY_EDITOR
            JLogBase.Log("\n <b>All gameobject that does not receive a value for customisation will be deactivated," +
                         "that can happen for two reasons: </b> \n" +
                         "1) It's SceneDescription key is not up to date and need to be regenerated <i>(Toolbar/RLTY/CreateSceneDescription)</i>\n" +
                         "2) No value as being assigned to its key, look for the corresponding <i>SessionConfig</i> ScriptableObject in the asset folder\n\n",
                this);
#else
            JLog("All gameobject that does not receive a value for customisation will be deactivated, it means that no value has been set in the Event Configuration tab of the event");
#endif

            CustomisationManagerHandlerData.CustomisationFinished();
            JLogBase.Log("Finished Customization", this);
        }

        void SearchAndCustomize(CustomisableType type, Customisable[] fullList, KeyValueBase k)
        {
            string foundKeys = null;
            bool found = false;

            foreach (Customisable customisable in fullList)
                if (customisable.type == type && customisable.key.Contains(k.key))
                {
                    customisable.Customize(k);
                    found = true;
                    if (debug)
                    {
                        if (foundKeys != null)
                            foundKeys += ",";
                        foundKeys += customisable.key;
                    }
                }

            if (!found)
                JLogWarning("No customisable found for key=" + k.key + " type=" + type);
        }

        private void DeactivateUnCustomized()
        {
            uncustomizedCustomisables = new List<Customisable>();

            foreach (Customisable customisable in customisablesInScene)
            {
                if (customisable.deactivable)
                {
                    if (customisable._keyValue == null || customisable._keyValue.value.IsNullOrWhitespace())
                    {
                        customisable.gameObject.SetActive(false);
                        uncustomizedCustomisables.Add(customisable);
                    }
                }
            }

            JLogBase.Log("Deactivated " + uncustomizedCustomisables.Count +
                         " unmodified customisables, press A in development build to toggle activation of those.",
                this);
        }

        private void ToggleUnCustomizedActivation()
        {
            foreach (Customisable custo in uncustomizedCustomisables)
            {
                if (custo.gameObject.activeInHierarchy)
                    custo.gameObject.SetActive(false);
                else
                    custo.gameObject.SetActive(true);
            }
        }

        #endregion

        #region Event Registrations

        public override void EventHandlerRegister()
        {
            AssetDownloaderManagerHandlerData.OnAllDownloadFinished += CustomizeScene;
            CustomisationManagerHandlerData.OnSceneCustomized += DeactivateUnCustomized;
        }

        public override void EventHandlerUnRegister()
        {
            AssetDownloaderManagerHandlerData.OnAllDownloadFinished -= CustomizeScene;
            CustomisationManagerHandlerData.OnSceneCustomized -= DeactivateUnCustomized;
        }

        #endregion
    }
}