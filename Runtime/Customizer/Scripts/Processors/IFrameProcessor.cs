using System.Collections;
using System.Collections.Generic;
using RLTY.SessionInfo;
using UnityEngine;
using UnityEngine.Serialization;

namespace RLTY.Customisation
{
    public class IframeProcessor : Processor
    {
        public override void Customize(KeyValueBase keyValue)
        {
            if (string.IsNullOrEmpty(keyValue.value))
            {
                gameObject.SetActive(false);
                return;
            }

            _IframeURL = keyValue.value;
        }
        
        [SerializeField] private string _IframeURL = "https://www.rlty.live";

        public void OpenIframe()
        {
            SessionInfoManagerHandlerData.OpenIframe(_IframeURL);
        }
    }
}
