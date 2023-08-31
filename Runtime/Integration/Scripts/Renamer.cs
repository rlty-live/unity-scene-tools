using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
[HideMonoScript, CanEditMultipleObjects]
#endif

public class Renamer : RLTYMonoBehaviourBase
{
#if UNITY_EDITOR
    private GameObject[] toSave;

    [SerializeField, ReadOnly, HideIf("manualSelection")]
    private List<Transform> toRename;
    [SerializeField]
    [InfoBox("By default Renamer will get all children of this GameObject, to select others check 'Manual selection' and fill the list.")]
    private bool manualSelection;
    [SerializeField]
    [ShowIf("manualSelection"), LabelText("To Rename")]
    private List<Transform> toRenameManualSelection;

    [Title("Naming")]
    [SerializeField]
    private string baseName = string.Empty;

    [SerializeField, ShowIf("showUtilities")]
    [HorizontalGroup("Parents", LabelWidth = 120), Space(5)]
    private bool useParentNames;
    [SerializeField, ShowIf("useParentNames"), ShowIf("showUtilities")]
    [HorizontalGroup("Parents", LabelWidth = 60), Space(5)]
    private int nAncestors;
    [SerializeField, ShowIf("useParentNames"), ShowIf("showUtilities")]
    [HorizontalGroup("Parents", LabelWidth = 40), Space(5)]
    private char separator;
    [SerializeField, ReadOnly, ShowIf("useParentNames"), ShowIf("showUtilities")]
    [Space(5)]
    private string[] parentNames;

    [Title("Indexing")]
    [SerializeField]
    private int startIndex;
    [SerializeField, ShowIf("showUtilities")]
    private bool linkedIndex = false;
    [SerializeField, ShowIf("showUtilities"), ShowIf("linkedIndex")]
    private Renamer renamerToFollow;

    private void OnValidate() => GetSetup();

    private void GetSetup()
    {
        if (manualSelection)
        {
            toRename = toRenameManualSelection.ToList();
        }

        else
        {
            toRename = gameObject.GetComponentsInChildren<Transform>().ToList();
            toRename.Remove(toRename[0]);
        }

        toSave = new GameObject[toRename.Count];
        for (int i = 0; i < toSave.Length; i++)
            toSave[i] = toRename[i].gameObject;

        if (linkedIndex && renamerToFollow)
            startIndex = renamerToFollow.startIndex + renamerToFollow.toRename.Count;

        if (useParentNames && nAncestors > 0)
        {
            parentNames = new string[nAncestors];
            Transform currentParent = transform;

            for (int i = 0; i < nAncestors; i++)
            {
                if (i == 0)
                {
                    parentNames[i] = transform.parent.name;
                    currentParent = transform.parent;

                    if (currentParent.root)
                        break;
                }
                else
                {
                    parentNames[i] = currentParent.parent.name;
                    currentParent = currentParent.parent;
                }
            }
        }
    }

    [Button, Tooltip("Will add _001,_022, etc at the end")]
    [Title("Actions")]
    private void BatchRename()
    {
        Undo.RecordObjects(toSave, "Batch Rename");

        string parentsNames = string.Empty;

        if (useParentNames)
            for (int i = 0; i < nAncestors; i++)
            {
                if (i < nAncestors - 1)
                    parentsNames += parentNames[i] + separator;
                else
                    parentsNames += parentNames[i];
            }

        for (int i = 0; i < toRename.Count; i++)
            toRename[i].gameObject.name = baseName + "_" + (i + startIndex).ToString("000") + "_" + parentsNames;
    }

    [Button]
    private void AddPrefix(string prefix)
    {
        Undo.RecordObjects(toSave, "Batch Rename");
        int i = 0;

        foreach (Transform _object in toRename)
        {
            if (i != 0)
                _object.gameObject.name = prefix + _object.gameObject.name;

            i++;
        }
    }

    [Button]
    private void RemoveNCharactersAtStart(int n)
    {
        Undo.RecordObjects(toSave, "Batch Rename");
        int i = 0;

        foreach (Transform _object in toRename)
        {
            if (i != 0)
                _object.gameObject.name = _object.gameObject.name.Remove(0, n);
            i++;
        }
    }
#endif
}
