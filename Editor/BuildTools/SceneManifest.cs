using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.SceneManagement;

namespace RLTY.Customisation
{
    [System.Serializable]
    public class SceneManifest
    {
        public List<CustomisableTypeDesc> entries = new List<CustomisableTypeDesc>();

        public SceneManifest()
        {

        }

        public SceneManifest(List<SceneAsset> scenes)
        {
            foreach (SceneAsset sceneAsset in scenes)
            {
                string path = AssetDatabase.GetAssetPath(sceneAsset);
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Customisable[] customisables = GameObject.FindObjectsOfType<Customisable>();
                foreach (Customisable customisable in customisables)
                {
                    if (customisable.gameObject.activeInHierarchy)
                    {
                        Populate(customisable.type, customisable.key, customisable.commentary);
                    }
                }
            }
        }

        public void Populate(CustomisableType type, string key, string description)
        {
            CustomisableTypeDesc ct = null;
            string typeString = type.ToString();
            foreach (CustomisableTypeDesc entry in entries)
                if (entry.type == typeString)
                    ct = entry;
            if (ct == null)
            {
                ct = new CustomisableTypeDesc(typeString);
                entries.Add(ct);
            }
            foreach (KeyInfo ki in ct.list)
                if (ki.key == key)
                    return;
            ct.list.Add(new KeyInfo() { key = key, type = typeString, description = description });
        }

        public string ToJson()
        {
            string data = JsonConvert.SerializeObject(this, Formatting.Indented);
            data = data.Remove(data.Length - 1, 1);
            data += ",\"static_frames\":" + StaticFramesManifest.GetStaticFramesManifest() + "}";
            return data;
        }

        [MenuItem("RLTY/DebugSceneManifest")]
        public static void DebugSceneManifest()
        {
            Customisable[] fullList = GameObject.FindObjectsOfType<Customisable>();
            SceneManifest manifest = new SceneManifest();
            foreach (Customisable c in fullList)
                manifest.Populate(c.type, c.key, c.commentary);
            Debug.Log("SceneManifest=" + manifest.ToJson());
        }
    }


    [System.Serializable]
    public class CustomisableTypeDesc
    {
        public string type;
        public string format;
        public List<KeyInfo> list = new List<KeyInfo>();

        public CustomisableTypeDesc(string t)
        {
            type = t;
            format = CustomisableUtility.Processors[CustomisableUtility.GetType(type)].formatInfo;
        }
    }
    [Serializable]
    public class KeyInfo
    {
        public string key;
        public string description;
        public string technicalInfo;
        public string type;
    }
}