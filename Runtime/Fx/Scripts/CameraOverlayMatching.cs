using UnityEngine;
using RLTY.Boot;

namespace Judiva.Metaverse
{
    [ExecuteAlways]
    public class CameraOverlayMatching : MonoBehaviour
    {
        public int transparentMaterialLayer = 31;
        Camera _parent, _this;

        private void Awake()
        {
            _parent = transform.parent.GetComponent<Camera>();
            _this = GetComponent<Camera>();
            BootManagerHandlerData.OnSceneReadyForCustomization += OnSceneReadyForCustomization;
        }

        private void OnDestroy()
        {
            BootManagerHandlerData.OnSceneReadyForCustomization -= OnSceneReadyForCustomization;
        }

        void OnSceneReadyForCustomization()
        {
            //can we find transparent objects ?
            GameObject[] list = FindObjectsOfType<GameObject>();
            bool found = false;
            foreach (GameObject go in list)
                if (go.layer == transparentMaterialLayer)
                {
                    found = true;
                    break;
                }
            if (!found)
            {
                gameObject.SetActive(false);
                _parent.cullingMask = _parent.cullingMask | 1 << transparentMaterialLayer;
            }
            else
            {
                _this.cullingMask = 1 << transparentMaterialLayer;
                _parent.cullingMask = _parent.cullingMask | ~(1 << transparentMaterialLayer);
            }
        }
        void LateUpdate()
        {
            if (_parent != null)
            {
                _this.fieldOfView = _parent.fieldOfView;
                _this.nearClipPlane = _parent.nearClipPlane;
                _this.farClipPlane = _parent.farClipPlane;
            }
        }
    }
}
