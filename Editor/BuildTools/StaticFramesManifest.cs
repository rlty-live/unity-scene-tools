using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

namespace RLTY.Customisation
{
    //[Serializable]
    public static class StaticFramesManifest
    {
        public static string GetStaticFramesManifest()
        {
            StaticFrame[] allStaticFrames = GameObject.FindObjectsOfType<StaticFrame>();

            List<StaticFrameConfig> staticFrameConfigs = new List<StaticFrameConfig>();

            foreach (var frame in allStaticFrames)
            {
                StaticFrameConfig staticFrameConfig = new StaticFrameConfig();
                staticFrameConfig.FrameID = frame.ID;
                staticFrameConfig.IsAdmin = frame.Type != StaticFrame.StaticFrameTypeEnum.StaticFramePublic;
                
                staticFrameConfigs.Add(staticFrameConfig);
            }
            Debug.Log("staticFrameConfigs.Count = " + staticFrameConfigs.Count);

            string st = JsonConvert.SerializeObject(staticFrameConfigs, Formatting.Indented);
            if(allStaticFrames.Length == 0) st = "no Frames found";
            if(staticFrameConfigs.Count == 0) st = "staticFrameConfigs.Count = 0";

            return st;
        }

        [MenuItem("RLTY/DebugStaticFramesManifest")]
        public static void DebugStaticFramesManifest()
        {
            Debug.Log(GetStaticFramesManifest());
        }
    }

    [Serializable]
    public class StaticFrameConfig
    {
        public string FrameID;
        public bool IsAdmin;
    }
}
