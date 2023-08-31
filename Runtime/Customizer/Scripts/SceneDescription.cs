using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace RLTY.Customisation
{
    /// <summary>
    /// All information needed to build the scene on the client side
    /// </summary>
    [System.Serializable]
    public class SceneDescription
    {
        /// <summary>
        /// URL of the assetbundle to load on the server side
        /// </summary>
        public string assetbundleServer="";
        /// <summary>
        /// URL of the assetbundle to load on the client side
        /// </summary>
        public string assetbundleClient="";
        /// <summary>
        /// Explicit URL of the assetbundle, as the web overlay puts "client" in assetbundleClient, god knows why
        /// </summary>
        public string assetbundleClientUrl = "";
        public AssetBundleClientUrl assetBundleClientUrls;
        public string eventSlug;
        public string eventSession;
        public string clientVersion;
        public string dnsName="localhost";
        public string playerSessionId="noPlayerSessionId";
        public string eventId;
        public string playerId;
        public string socialWallAddress;
        public string staticFramesAddress;
        public string backendApiHost;

        public AvatarData initialAvatar;

        public string ResolvedDnsName {  get { return string.IsNullOrEmpty(dnsName) ? "localhost" : dnsName; } }

        /// <summary>
        /// A unique identifier pointing to the server we are connected to
        /// </summary>
        public string EventUniqueID { get { return eventSlug + "_" + eventSession; } }


        public List<CustomisableTypeEntry> entries = new List<CustomisableTypeEntry>();

        public AgoraToken agoraToken;

        public string SOCIAL_API_HOST;

        [JsonIgnore]
        public int totalSize;

        public void InitTypes()
        {
            foreach (CustomisableTypeEntry c in entries)
            {
                CustomisableType type = c.Type;
                foreach (var pair in c.keyPairs)
                    pair.Type = type;
            }
        }

        [JsonIgnore]
        public List<KeyValueBase> Images
        {
            get
            {
                List<KeyValueBase> list = new List<KeyValueBase>();
                CustomisableTypeEntry tmp = GetEntryByType(CustomisableType.Texture);
                if (tmp != null)
                    list.AddRange(tmp.keyPairs);
                tmp = GetEntryByType(CustomisableType.Sprite);
                if (tmp != null)
                    list.AddRange(tmp.keyPairs);
                return list;
            }
        }

        [JsonIgnore]
        public List<KeyValueBase> Sounds
        {
            get
            {
                List<KeyValueBase> list = new List<KeyValueBase>();
                CustomisableTypeEntry tmp = GetEntryByType(CustomisableType.Audio);
                if (tmp != null)
                    list.AddRange(tmp.keyPairs);
                return list;
            }
        }

        protected CustomisableTypeEntry GetEntryByType(CustomisableType type)
        {
            string typeString = type.ToString();
            foreach (CustomisableTypeEntry entry in entries)
                if (entry.type == typeString)
                    return entry;
            return null;
        }

        /// <summary>
        /// Method used for creating an empty scene description with all the keys
        /// </summary>
        /// <param name="type"></param>
        /// <param name="k"></param>
        public void Populate(CustomisableType type, string k)
        {
            CustomisableTypeEntry s = GetEntryByType(type);
            if (s == null)
            {
                s = new CustomisableTypeEntry();
                s.type = type.ToString();
                entries.Add(s);
            }

            //check if duplicate key
            foreach (KeyValueBase kv in s.keyPairs)
                if (kv.key == k)
                    return;
            s.keyPairs.Add(new KeyValueBase() { key = k });
        }
        public void SaveAsJSON()
        {
            string dir = Application.dataPath + "/../../StreamingAssets";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path = dir + "/sceneDescription.json";
            File.WriteAllText(dir, JsonConvert.SerializeObject(this, Formatting.Indented));
            Debug.Log("SceneDescription saved to " + dir);
        }

        [System.Serializable]
        public class AgoraToken
        {
            public string token;
            public uint id;
            public string channelName;
        }

        [System.Serializable]
        public class AvatarData
        {
            public string id;
            public string type;
            public string skin;
            public bool customReadyPlayerMe = false;
        }
    }
    
    
    [Serializable]
    public class AssetBundleClientUrl
    {
        public string Windows;
        public string WebGL;
        public string Android;
        public string IOS;
    }

    #region Processor declaration

    public enum CustomisableType
    {
        Texture,
        Sprite,
        Video,
        Audio,
        Model,
        Color,
        Text,
        Font,
        ExternalPage,
        DonationBox,
        Web3Transaction,
        TypeForm,
        Iframe,
        Invalid
    }

    public class ProcessorDefinition
    {
        public Type type;
        public string formatInfo;

        public ProcessorDefinition (Type t, string f)
        {
            type = t;
            formatInfo = f;
        }
    }

    public static class CustomisableUtility
    {
        public static CustomisableType GetType(string type)
        {
            if (!Enum.TryParse<CustomisableType>(type, out CustomisableType result))
                result = CustomisableType.Invalid;
            return result;
        }

        public static Dictionary<CustomisableType, ProcessorDefinition> Processors
        {
            get
            {
                if (_allProcessors == null)
                {
                    _allProcessors = new Dictionary<CustomisableType, ProcessorDefinition>();
                    _allProcessors[CustomisableType.Texture] = new ProcessorDefinition(typeof(MaterialProcessor), "URL to a png or jpg image");
                    _allProcessors[CustomisableType.Color] = new ProcessorDefinition(typeof(MaterialProcessor), "HTML color (example: #FF0590)");
                    _allProcessors[CustomisableType.Sprite] = new ProcessorDefinition(typeof(SpriteProcessor), "URL to a png or jpg image");
                    _allProcessors[CustomisableType.Video] = new ProcessorDefinition(typeof(VideoStreamProcessorV2), "URL to a mp4 video or a live stream");
                    _allProcessors[CustomisableType.Text] = new ProcessorDefinition(typeof(TextProcessor), "string");
                    _allProcessors[CustomisableType.ExternalPage] = new ProcessorDefinition(typeof(ExternalPageProcessor), "URL to webpage");
                    _allProcessors[CustomisableType.Audio] = new ProcessorDefinition(typeof(AudioProcessor), "URL to mp3 file");
                    _allProcessors[CustomisableType.DonationBox] = new ProcessorDefinition(typeof(DonationBoxProcessor), "walletId string, empty string will hide object in scene");
                    _allProcessors[CustomisableType.Web3Transaction] = new ProcessorDefinition(typeof(Web3TransactionProcessor), "string containing smartContractAddress and activeChainId, separated by a comma, empty string will hide object in scene");
                    _allProcessors[CustomisableType.TypeForm] = new ProcessorDefinition(typeof(TypeFormProcessor), "typeFormId string, empty string will hide object in scene");
                    _allProcessors[CustomisableType.Iframe] =  new ProcessorDefinition(typeof(IframeProcessor), "URL to iframe");
                }
                return _allProcessors;
            }
        }
        private static Dictionary<CustomisableType, ProcessorDefinition> _allProcessors;

        public static bool TryParseColor(KeyValueBase keyValue, out Color color)
        {
            color = Color.white;
            string value = keyValue.value;
            if (ColorUtility.TryParseHtmlString(value, out Color c))
            {
                color = c;
                return true;
            }
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            string[] parsedString = value.Split(",");

            if (parsedString.Length != 4)
            {
                Debug.Log("Wrong string input, requires Vector4 [0,255]");
                return true;
            }

            try
            {
                for (int i = 0; i < parsedString.Length; i++)
                {
                    parsedString[i] = parsedString[i].Replace(" ", "");
                }

                parsedString[0] = parsedString[0].Replace("[", "");
                parsedString[parsedString.Length - 1] = parsedString[parsedString.Length - 1].Replace("]", "");

                Vector4 parsedColor = new Vector4();

                parsedColor.x = int.Parse(parsedString[0]);
                parsedColor.y = int.Parse(parsedString[1]);
                parsedColor.z = int.Parse(parsedString[2]);
                parsedColor.w = int.Parse(parsedString[3]);

                color = new Color32((byte)parsedColor.x, (byte)parsedColor.y, (byte)parsedColor.z, (byte)parsedColor.w);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }

        }
    }

    #endregion

    #region Data

    [System.Serializable]
    public class CustomisableTypeEntry
    {
        [JsonIgnore]
        public CustomisableType Type
        {
            get
            {
                return CustomisableUtility.GetType(type);
            }
        }
        public string type;
        public List<KeyValueBase> keyPairs = new List<KeyValueBase>();
    }

    [Serializable]
    public class KeyValueBase
    {
        [JsonIgnore]
        public object data;
        [JsonIgnore]
        public CustomisableType Type { get; set; }
        public string key;
        [TextArea]
        public string value;
    }
    #endregion
}
