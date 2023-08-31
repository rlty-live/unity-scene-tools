using UnityEngine;
using Judiva.Metaverse.Interactions;
using RLTY.SessionInfo;

namespace RLTY.Customisation
{
    public class DonationBoxProcessor : Processor
    {
        public override void Customize(KeyValueBase keyValue)
        {
            if (string.IsNullOrEmpty(keyValue.value))
            {
                gameObject.SetActive(false);
                return;
            }
            _walletId = keyValue.value;
        }
        [SerializeField] private bool _checkUserOrientationAlignedWithForward = false;
        [SerializeField] private string _walletId = string.Empty;

        public void UserDonation()
        {
            SessionInfoManagerHandlerData.UserDonation(_walletId);
        }
    }
}
