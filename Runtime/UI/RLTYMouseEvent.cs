using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace RLTY.UI
{
    [RequireComponent(typeof(Collider))]
    public class RLTYMouseEvent : JMonoBehaviour
    {
        /// <summary>
        /// Maximum distance from the viewpoint at which events are triggered
        /// </summary>
        public float mouseDetectDistance = 1000;
        /// <summary>
        /// UnityEvents for Inspector
        /// </summary>
        public UnityEvent OnPointerEnter, OnPointerExit, OnPointerDown, OnPointerUp, OnClick;

        public void NotifyOnPointerEnter() { OnPointerEnter?.Invoke(); }
        public void NotifyOnPointerExit() { OnPointerExit?.Invoke(); }
        public void NotifyOnPointerDown() { OnPointerDown?.Invoke(); }
        public void NotifyOnPointerUp() { OnPointerUp?.Invoke(); }
        public void NotifyOnClick() { OnClick?.Invoke(); }
    }
}
