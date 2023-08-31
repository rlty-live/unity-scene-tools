using UnityEngine;
using Judiva.Metaverse.Interactions;
using RLTY.SessionInfo;

namespace RLTY.Customisation
{
    public class TypeFormProcessor : Processor
    {
        public override void Customize(KeyValueBase keyValue)
        {
            if (string.IsNullOrEmpty(keyValue.value))
            {
                gameObject.SetActive(false);
                return;
            }
            _typeformId = keyValue.value;
        }

        [SerializeField] private bool _checkUserOrientationAlignedWithForward = false;
        [SerializeField] private string _typeformId = "1";

        public void OpenTypeForm()
        {
            SessionInfoManagerHandlerData.OpenTypeForm(_typeformId);
        }
    }
}
