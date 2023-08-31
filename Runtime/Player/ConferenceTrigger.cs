using UnityEngine;
using RLTY.SessionInfo;

namespace Judiva.Metaverse.Interactions
{
    [RequireComponent(typeof(TriggerZone))]
    public class ConferenceTrigger : MonoBehaviour
    {
        public GameObject conferenceScreen;
        public string conferenceId = "1";
        private TriggerZone _zone;

        private void Awake()
        {
            conferenceScreen.SetActive(false);
            _zone = GetComponent<TriggerZone>();
            _zone.onNotEmpty += (x) => enabled = true;
            _zone.onEmpty += (x) => enabled = false;
            _zone.onPlayerEnter += (x) => SessionInfoManagerHandlerData.UserEnterConferenceStage(conferenceId);
            _zone.onPlayerExit += (x) => SessionInfoManagerHandlerData.UserExitConferenceStage(conferenceId);
            enabled = false;
        }
        void OnEnable()
        {
            conferenceScreen.SetActive(true);
            //to do: request stream url
        }

        private void OnDisable()
        {
            conferenceScreen.SetActive(false);
        }
    }
}
