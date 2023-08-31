using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RLTY.Rendering
{
    [RequireComponent(typeof(Camera))]
    public class IgnoreFog : RLTYMonoBehaviour
    {
        Camera cameraWithoutFog;

        private void Awake()
        {
            cameraWithoutFog = GetComponent<Camera>();
        }

        public override void Start()
        {
            base.Start();

            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        public override void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera == cameraWithoutFog)
                RenderSettings.fog = false;
            else
                RenderSettings.fog = true;
        }

        public override void EventHandlerRegister()
        {

        }

        public override void EventHandlerUnRegister()
        {

        }
    }
}
