using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLTY.Tools
{
    [AddComponentMenu("RLTY/Integration/Replace by Prefab")]
    public class ReplaceByPrefab : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField, Button("Get center position")]
        private Vector3 centerPosition()
        {
            Bounds bound = GetComponent<MeshRenderer>().bounds;
            return bound.center;
        }

        [Button, Tooltip("Will move this transform's to the designated mesh center")]
        private void CenterToBoundsCenter(MeshRenderer rdr)
        {
            transform.position = rdr.bounds.center;
        }

        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private Transform parent;
        [SerializeField]
        private GameObject instance;

        [Button("Instantiate")]
        private void AddPrefab()
        {
            instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);

            //instance.transform.position = transform.position - offset;
            instance.transform.position += centerPosition();
        }

        [Button("Delete copy")]
        public void DestroyCopy()
        {
            DestroyImmediate(instance);
        }

        [Button("Reset Prefab pivot (sortof)")]
        public void ResetPrefabCenter()
        {
            transform.position = Vector3.zero;
            transform.position -= GetComponent<MeshRenderer>().bounds.center;
        }
#endif
    }

}
