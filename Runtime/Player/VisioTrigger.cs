using System;
using System.Collections.Generic;
using UnityEngine;

namespace Judiva.Metaverse.Interactions
{
    [RequireComponent(typeof(TriggerZone))]
    public class VisioTrigger : MonoBehaviour
    {
        public string visioId="1";
        private TriggerZone _zone;

        [SerializeField]
        private bool _visioStarted = false;

        public event Action<VisioTrigger> OnJoin;
        public event Action<VisioTrigger> OnLeave; 

        void Start()
        {
            _zone = GetComponent<TriggerZone>();
            _zone.onPlayerEnter += (x) => enabled = true;
            _zone.onPlayerExit += (x) => enabled = false;
            enabled = false;
        }

        private void Update()
        {
            //we enter visio only when there are two people inside the trigger
            if (!_visioStarted && _zone.InsideCount>1)
            {
                _visioStarted = true;
                OnJoin?.Invoke(this);
            }

            if (_visioStarted && _zone.InsideCount == 1)
            {
                _visioStarted = false;
                OnLeave?.Invoke(this);
            }
        }

        private void OnDisable()
        {
            if (_visioStarted)
            {
                _visioStarted = false;
                OnLeave?.Invoke(this);
            }
        }

        private void OnValidate()
        {
            if (visioId == "0")
            {
                Debug.LogError("This channel is global chat");
                visioId = "1";
            }

            VisioTrigger[] list = FindObjectsOfType<VisioTrigger>(true);

            int index = 0;
            while (IsDuplicateVisioId(list))
            {
                index++;
                visioId = index.ToString();
                //Debug.LogError("Fixing duplicate visio id, to " + visioId);
            }
            if (_zone == null)
                _zone = GetComponent<TriggerZone>();
        }

        bool IsDuplicateVisioId(VisioTrigger[] list)
        {
            foreach (VisioTrigger v in list)
                if (v.visioId == visioId && v != this)
                    return true;
            return false;
        }
    }
}

