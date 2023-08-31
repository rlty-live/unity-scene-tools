using UnityEngine;
using Judiva.Metaverse.Interactions;
using RLTY.SessionInfo;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace RLTY.Customisation
{
    [RequireComponent(typeof(TriggerZone))]
    public class Web3TransactionProcessor : Processor
    {
        [SerializeField]
        public NFTData data;

        public static Action<string, Action<Texture>> downloadImageAction;
        public Renderer image;
        public Renderer backFaceImage;

        public bool validWeb3Data;

        public static NFTData DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<NFTData>(json);
        }
        string formatedKeyValue;

        public override void Customize(KeyValueBase keyValue)
        {
            try
            {
                formatedKeyValue = keyValue.value.Replace(@"\", "");

                data = new NFTData();
                data = DeserializeJson(formatedKeyValue);
                
                if (!string.IsNullOrEmpty(data.image))
                {
                    if (image.material)
                        downloadImageAction?.Invoke(data.image, (x) => image.material.mainTexture = x);

                    if (backFaceImage.material)
                        downloadImageAction?.Invoke(data.image, (x) => backFaceImage.material.mainTexture = x);
                }
            }

            catch (System.Exception e)
            {
                gameObject.GetComponent<Customisable>()._keyValue = null;
                JLogBase.LogError("Invalid Web3 data on key=" + keyValue.key + " value=" + keyValue.value, this);
            }
        }

        public void Web3Transaction()
        {
            SessionInfoManagerHandlerData.Web3Transaction(JsonConvert.SerializeObject(data));
            JLogBase.Log("Web3: " + JsonConvert.SerializeObject(data), this);
        }

        //USE THIS TO ACTIVATE TRANSACTION ON COLLISION
        //private TriggerZone _zone;
        //private bool _actionProcessed = false;
        //[SerializeField] private bool _checkUserOrientationAlignedWithForward = false;

        //protected override void Awake()
        //{
        //    base.Awake();
        //    _zone = GetComponent<TriggerZone>();
        //    _zone.onPlayerEnter += (x) => enabled = true;
        //    _zone.onPlayerExit += (x) => enabled = false;
        //    enabled = false;
        //}

        //private void OnEnable()
        //{
        //    _actionProcessed = false;
        //}

        //private void Update()
        //{
        //    //check if we trigger a donation
        //    if (!_actionProcessed && (!_checkUserOrientationAlignedWithForward || Vector3.Dot(AllPlayers.Me.Transform.forward, transform.forward) > 0.3f))
        //    {
        //        _actionProcessed = true;
        //        SessionInfoManagerHandlerData.Web3Transaction(JsonConvert.SerializeObject(_data));
        //    }
        //}
    }
}

[System.Serializable]
public class NFTData
{
    public string type { get; set; }
    public string address { get; set; }
    public string tokenid { get; set; }
    public string image { get; set; }
    public string chain { get; set; }

    public NFTData()
    {
        type = string.Empty;
        address = string.Empty;
        tokenid = string.Empty;
        image = string.Empty;
        chain = string.Empty;
    }
}
