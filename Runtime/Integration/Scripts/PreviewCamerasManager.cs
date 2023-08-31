using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace RLTY.Tools
{
    public class PreviewCamerasManager
    {
        [ReadOnly, SerializeField]
        List<Camera> previewCameras;

        [SerializeField]
        Vector2 rowAndColumns = new Vector2(2, 2);

        //[MenuItem("RLTY/Integration/Add preview Camera")]

        [Button("Add camera")]
        private void AddCamera()
        {
            if (previewCameras == null || previewCameras.Count == 0)
                previewCameras = new List<Camera>();

            Camera camera = new Camera();

            //Change display dimensions and positions according to the number of camera to be layed out
        }

        private void RemoveEmptyEntries()
        {
            foreach(Camera cam in previewCameras)
                if (!cam)
                    previewCameras.Remove(cam);
        }
    }
}
