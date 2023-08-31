using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace RLTY.UX
{
    [HideMonoScript]
    [AddComponentMenu("RLTY/Integration/Smooth billboard")]
    public class CameraFacer : RLTYMonoBehaviourBase
    {
        #region Variables

        [ShowIf("showUtilities")]
        [SerializeField, ReadOnly]
        public float currentDistance;
        [Tooltip("Object will start facing mainCamera when closer than this distance")]
        [SerializeField]
        private float activationDistance = 2;
        [SerializeField]
        [HorizontalGroup("Activation")]
        private bool display;

        [ShowIf("showUtilities")]
        [SerializeField, ReadOnly]
        public float timer;
        [Tooltip("in seconds")]
        [ShowIf("showUtilities")]
        [SerializeField]
        private float updateFrequency = 1f;
        float lookAtTime = 1f;

        RectTransform rectTransform;
        Transform pointer;
        Camera mainCamera;

        #endregion

        #region UnityCallbacks
        public void Start()
        {
            rectTransform = transform.GetComponent<RectTransform>();
            pointer = new GameObject(this.name + " pointer").transform;

            pointer.SetParent(rectTransform.parent);
            pointer.localPosition = Vector3.zero;
            pointer.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            if (!Camera.main)
                JLogBase.Log("No mainCamera present in the scene, facing won't work until there's one", this);
            else
                mainCamera = Camera.main;
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (mainCamera)
            {
                currentDistance = Vector3.Distance(mainCamera.transform.position, rectTransform.position);

                if (currentDistance < activationDistance && timer > updateFrequency)
                    StartCoroutine(LerpToPointAtMainCamera());
            }
            else
            {
                if (Camera.main)
                    mainCamera = Camera.main;
            }
        }

        public void FaceCamera() => StartCoroutine(LerpToPointAtMainCamera());

        public IEnumerator LerpToPointAtMainCamera()
        {
            timer = 0;
            float elapsedTime = 0;
            //JLogBase.Log("Started facing main camera at " + pointer.transform.position, this);

            Vector3 startPosition = mainCamera.transform.position;
            pointer.LookAt(new Vector3(startPosition.x, rectTransform.position.y, startPosition.z));
            pointer.Rotate(new Vector3(0, 180, 0));

            while (elapsedTime < lookAtTime)
            {
                rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, pointer.rotation, elapsedTime / lookAtTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return null;
        }
        #endregion

        #region EditorOnly Logic
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (display)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, activationDistance);
            }
        }
#endif
        #endregion
    }
}
