using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using RLTY.Customisation;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "AssetBundleSceneSetup", menuName = "RLTY/BuildSetup/Assetbundles", order = 1)]
public class AssetbundleBuildSetup : ScriptableObject
{
    public bool useCustomStreamingAssetLocalPath = false;
    public string customStreamingAssetsLocalPath = "../../StreamingAssets";

    public string StreamingAssetsLocalPath
    {
        get
        {
            string tmp= useCustomStreamingAssetLocalPath ? customStreamingAssetsLocalPath : "../../StreamingAssets";
            if (!tmp.Contains(":"))
                tmp = Application.dataPath + "/" + tmp;
            return tmp;
        }
    }

    public string PublishS3Path
    {
        get
        {
            return "../Publish";
        }
    }

    [System.Serializable]
    public class Environment
    {
        public string id;
        public List<SceneAsset> scenes=new List<SceneAsset>();
        public bool rebuild = true;
    }

    public List<Environment> environmentList;

    private void OnValidate()
    {
        foreach (Environment e in environmentList)
            e.scenes.Sort((x, y) => AssetDatabase.GetAssetPath(x).CompareTo(AssetDatabase.GetAssetPath(y)));
    }

    public void PrepareForBuild()
    {
        //build a list of all scenes in project
        string sceneType = typeof(SceneAsset).ToString();
        int k = sceneType.LastIndexOf(".");
        if (k>0)
            sceneType =sceneType.Substring(k+1);
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", sceneType));
        
        //clear their assetbundle assignment
        foreach (string g in guids)
            AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(g)).SetAssetBundleNameAndVariant("", "");

        //prepare assignment for scenes that we actually want to build
        foreach (Environment e in environmentList)
        {
            if (e.rebuild)
            {
                foreach (SceneAsset s in e.scenes)
                {
                    string assetPath = AssetDatabase.GetAssetPath(s);
                    AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(e.id, "");
                }
            }
        }
    }
}
