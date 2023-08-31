using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections;
using Judiva.Metaverse.Interactions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RLTY.Customisation
{
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [HideMonoScript]
#endif
    public class VideoStreamProcessorV2 : Processor
    {
        #region Global Variables
        [Title("Global Parameters")]
        public string videoURL;
        [SerializeField]
        public bool isLiveStream;

        [Title("Rendering")]
        [Tooltip("Apply either to shared material or instanced material")]
        public bool applyToMeshes;
        [ShowIf("applyToMeshes"), InfoBox("All those meshes will have their material swapped with one loaded with the video, all material customisations will be lost.")]
        public Renderer[] targetRenderers;

        public bool applyToUI;
        [ShowIf("applyToUI")]
        public MaskableGraphic[] targetsUIs;

        //[Title("Audio")]
        //public bool spatializedAudio;
        ////public AudioOutput audioOutput;
        //public Transform[] speakers;

        [Title("Playback")]
        public bool useTriggerZone;
        [ShowIf("useTriggerZone")]
        public TriggerZone triggerZone;
        #endregion

        public override void Customize(KeyValueBase kvo)
        {
            videoURL = kvo.value;
            if(String.IsNullOrEmpty(videoURL)) JLogError("videoURL is null or empty for " + this);
            JLog("Got " + kvo.value + " from sceneDescription");
        }

        

        public void FindTriggerZone()
        {
            if (!triggerZone && GetComponentInChildren<TriggerZone>())
                triggerZone = GetComponentInChildren<TriggerZone>();
            else
                JLogError("No Trigger zone associated, and none can be found in children, the associated video will never start");
        }
    }
}
