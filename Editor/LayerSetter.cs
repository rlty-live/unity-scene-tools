using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LayerSetter : EditorWindow
{
    public static string[] savedLayerNames;

    const string menuItemName = "Scene verification/Check layer setup";
    const string warningMessage =
        "All layers have to match those of the Engine, otherwise in can result in undetected collisions and displaying errors. \n \n" +
        "If you click 'Setup Layers', all objects assigned to an altered layer will be assigned 0:Default Layer, \n" +
        "if you whish to correct your layer assigmnent yourself, click 'Cancel' \n\n" +
        "You'll find the correct layer setup in the Console Window, you'll need to match it exactly.";

    static int fallbackLayerIndex = 0;

    [MenuItem("RLTY/" + menuItemName)]
    static void SetupLayerConfiguration()
    {
        LayerSetter window = ScriptableObject.CreateInstance<LayerSetter>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 250);
        window.ShowPopup();
    }

    static string DebugSavedLayerIndexAndNames()
    {
        if (savedLayerNames != null && savedLayerNames.Length > 0)
        {
            string[] debugLayers = savedLayerNames;
            for (int i = 0; i < debugLayers.Length; i++)
            {
                if (debugLayers[i] != string.Empty)
                    debugLayers[i] = LayerMask.NameToLayer(debugLayers[i]) + ": " + debugLayers[i];
            }

            string debugString = string.Join(", ", debugLayers);
            return debugString;
        }
        else
            return "No saved layers";
    }

    public static Transform[] GetAllTransformsFromScene()
    {
        List<Transform> activeSceneTransforms = new List<Transform>();

        foreach (GameObject rootGO in SceneManager.GetActiveScene().GetRootGameObjects())
            foreach (Transform tr in rootGO.GetComponentsInChildren<Transform>())
                activeSceneTransforms.Add(tr);

        return activeSceneTransforms.ToArray();
    }

    static GameObject[] GetGameObjectWithWrongLayers()
    {
        List<GameObject> wrongLayerGOs = new List<GameObject>();

        if (savedLayerNames != null && savedLayerNames.Length > 0)
        {
            foreach (Transform transform in GetAllTransformsFromScene())
                if (!savedLayerNames.ToList().Contains(LayerMask.LayerToName(transform.gameObject.layer)) ||
                    LayerMask.LayerToName(transform.gameObject.layer) == string.Empty)
                    wrongLayerGOs.Add(transform.gameObject);

            return wrongLayerGOs.ToArray();
        }
        else
        {
            Debug.Log("No saved layers");
            return null;
        }
    }

    static void SwitchWrongAndEmptyLayers()
    {
        Undo.RecordObjects(GetGameObjectWithWrongLayers(), "Switched layers to match engine's setup");

        foreach (GameObject go in GetGameObjectWithWrongLayers())
            go.layer = fallbackLayerIndex;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Layer setup", EditorStyles.largeLabel);
        GUILayout.Space(10);
        EditorGUILayout.LabelField(warningMessage, EditorStyles.wordWrappedLabel);

        GUILayout.Space(20);

        if (GUILayout.Button("Setup layers"))
        {
            SwitchWrongAndEmptyLayers();

            Debug.Log("Engine layer setup: " + DebugSavedLayerIndexAndNames());
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            Debug.Log("Engine layer setup: " + DebugSavedLayerIndexAndNames());
            Close();
        }
    }
}
