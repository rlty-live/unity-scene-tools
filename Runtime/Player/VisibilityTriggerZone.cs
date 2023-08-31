using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Judiva.Metaverse.Interactions
{
    /// <summary>
    /// Use this to show/hide a part of the scene based on a trigger zone
    /// This should only be used on objects that will not have their renderers toggled by code
    /// When hiding, a list of visible renderers is created, and they are all hidden
    /// This same list will be used when showing the scene again
    /// Zones are hierarchical (based on the gameobject hierarchies)
    /// A given zone controls renderers that are not already controlled by a children
    /// </summary>
    public class VisibilityTriggerZone : MonoBehaviour
    {
        static int visibleLayer = 0, invisibleWhenFarLayer = 8;
        public TriggerZone trigger;
        public List<GameObject> rootsToShow;
        public bool startVisible = false;

        public List<Renderer> Monitored { get { return _monitored; } }
        private List<Renderer> _monitored = new List<Renderer>();
        private List<Renderer> _hiddenRenderers=new List<Renderer>();

        [Header("Debug")]
        [SerializeField]private bool _isVisible = false;

        void Start()
        {
            if (transform.GetComponentsInParent<VisibilityTriggerZone>().Length==1)
                Init();

            //remove invisible layer from camera
            Camera mainView = Camera.main;
            mainView.cullingMask = mainView.cullingMask & ~(1 << invisibleWhenFarLayer);

            trigger.onPlayerEnter += (x) => Show(true);
            trigger.onPlayerExit += (x) => Show(false);

            //support for other avatars (not yet implemented)
            /*
            trigger.onEnter += (x, p) =>
            {
                //we check that Player.Me is not null before running this
                //this is mostly because when running as server, we don't care about visibility
                //and if we don't do that test and start the game as server in Unity Editor for testing
                //our character would be hidden because onEnter is triggered before Player.Me is set
                //whereas when we run only as client, Player.Me is set before onEnter is called
                if (Player.Me!=null && p != Player.Me)
                {
                    //show only if in same zone as player
                    p.SwitchLayer(trigger.IsInside(Player.Me) ? visibleLayer : invisibleWhenFarLayer);
                }
            };*/
        }

        public void Init()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                VisibilityTriggerZone v= transform.GetChild(i).GetComponent<VisibilityTriggerZone>();
                if (v != null)
                    v.Init();
            }
            VisibilityTriggerZone[] children = transform.GetComponentsInChildren<VisibilityTriggerZone>();
            foreach (GameObject go in rootsToShow)
            {
                if (go)
                {
                    Renderer[] list = go.GetComponentsInChildren<Renderer>();
                    //we monitor only renderers that are visible at start
                    foreach (Renderer r in list)
                        if (r.enabled)
                        {
                            bool found = false;
                            foreach (VisibilityTriggerZone v in children)
                                if (v.Monitored.Contains(r))
                                {
                                    found = true;
                                    break;
                                }
                            if (!found)
                                _monitored.Add(r);
                        }
                }
            }
            //Debug.Log("Init " + name + " count=" + _monitored.Count);
            Show(startVisible);
        }
        void Show(bool state)
        {
            _isVisible = state;
            foreach (Renderer r in _monitored)
                r.enabled = state;

            /*
            foreach (Player p in trigger.Inside)
                if (p != Player.Me)
                    p.SwitchLayer(state ? visibleLayer : invisibleWhenFarLayer);*/
        }
    }
}
