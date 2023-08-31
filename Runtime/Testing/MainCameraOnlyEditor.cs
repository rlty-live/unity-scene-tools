using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLTY.Testing
{
    [RequireComponent(typeof(Camera)), AddComponentMenu("RLTY/Testing/EditorOnlyMainCamera")]
    public class MainCameraOnlyEditor : RLTYMonoBehaviourBase
    {
        [InfoBox("Use this script to makes tests on a MainCamera, it will be destroyed at runtime")]
        public void Start()
        {
            if(isActiveAndEnabled)
                DestroyEditorSafe(this);
        }
    }
}
