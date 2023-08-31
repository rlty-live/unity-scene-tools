using Judiva.Metaverse.Interactions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class RLTYGameobjectMenu : Editor
{
    const string toolbarfolderName = "GameObject/RLTY/";

    const string assetFolderName = "Packages/live.rlty.scenetools/Runtime";
    static readonly string[] prefabPaths =
    {
        assetFolderName + "Player/Prefabs/TriggerZone.prefab",
        assetFolderName + "Customizer/Prefabs/VideoStream.prefab",
        assetFolderName + "Player/Prefabs/SpawnPoint.prefab",
        assetFolderName + "Player/Prefabs/Teleport.prefab",
        assetFolderName + "Player/Prefabs/Jump.prefab"
    };

    [MenuItem(toolbarfolderName + "TriggerZone")]
    public static void CreateTriggerZoneInstance() =>
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(prefabPaths[0], typeof(GameObject)), EditorSceneManager.GetActiveScene());

    [MenuItem(toolbarfolderName + "VideoStream")]
    public static void CreateVideoStreamPrefab() =>
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(prefabPaths[1], typeof(GameObject)), EditorSceneManager.GetActiveScene());

    [MenuItem(toolbarfolderName + "SpawnPoint")]
    public static void CreateSpawnPoint() =>
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(prefabPaths[2], typeof(GameObject)), EditorSceneManager.GetActiveScene());

    [MenuItem(toolbarfolderName + "Teleporter")]
    public static void CreateTeleporter() =>
    PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(prefabPaths[3], typeof(GameObject)), EditorSceneManager.GetActiveScene());

    [MenuItem(toolbarfolderName + "Jumper")]
    public static void CreateJumper() =>
PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(prefabPaths[4], typeof(GameObject)), EditorSceneManager.GetActiveScene());

    #region CreateFromScratch
    public void CreateTriggerZoneGameObject()
    {
        GameObject go = new GameObject("TriggerZone");

        TriggerZone tZ = go.AddComponent<TriggerZone>();
        SphereCollider sC = go.AddComponent<SphereCollider>();
        BoxCollider bC = go.AddComponent<BoxCollider>();
        MeshCollider mC = go.AddComponent<MeshCollider>();

        tZ.alwaysDisplay = true;
        tZ.solidColor = true;

        sC.isTrigger = true;
        bC.isTrigger = true;
        sC.center = new Vector3(1, 0, 0);
        bC.center += new Vector3(2, 0, 0);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mC.sharedMesh = cylinder.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(cylinder);
        mC.convex = true;
        mC.isTrigger = true;

        go.transform.SetParent(Selection.activeGameObject.transform);
        go.transform.localPosition = Vector3.zero;
    }
    #endregion

}
