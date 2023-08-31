using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Judiva.Metaverse.Interactions
{
    public class Teleporter : SceneTool
    {
        public float fadeOutTime = 0.01f;
        public float fadeInTime = 0.3f;
        
        [Header("Connected teleport")]
        public Teleporter connected;
        [Tooltip("if true, the player will align with the connected teleport Z axis")]
        public bool useConnectedOrientation=false;
        [Tooltip("additional Y orientation offset when using connected orientation")]
        [Range(-180,180)]
        public float orientationOffset = 0;

        private DateTime _arrivedTime;
        public event Action OnTeleport;

        private void Start()
        {
            enabled = false;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            //move to transparent fx layer, set isTrigger
            if (gameObject.layer != 2) gameObject.layer = 2;
            Collider c = GetComponent<Collider>();
            if (c)
                c.isTrigger = true;
            if (connected != null)
                connected.connected = this;
        }

        void OnTriggerEnter(Collider other)
        {
            if (DateTime.Now.Subtract(_arrivedTime).TotalSeconds < 0.5f || connected==null)
                return;
            IPlayer avatar = other.GetComponentInParent<IPlayer>();
            //only teleport me
            if (avatar == null || avatar!= AllPlayers.Me) return;

            UIManagerHandlerData.CrossFade(fadeOutTime, fadeInTime, () =>
            {
                Vector3 pos = connected.transform.TransformPoint(transform.InverseTransformPoint(AllPlayers.Me.Transform.position));
                connected.NotifyTeleport();
                if (useConnectedOrientation)
                {
                    Quaternion r=Quaternion.Euler(new Vector3(0, orientationOffset, 0)) * connected.transform.rotation;
                    AllPlayers.Me.Teleport(pos, r);
                }
                else
                    AllPlayers.Me.Teleport(pos, AllPlayers.Me.Transform.rotation);
                OnTeleport?.Invoke();
            });
        }

        public void NotifyTeleport()
        {
            _arrivedTime = DateTime.Now;
        }
        
        
        
        #if UNITY_EDITOR
        private float _GizmoRadius = 0.8f;
        private void OnDrawGizmos()
        {
            Color defaultColor = Color.cyan;
            if(connected == null) defaultColor = Color.red;
            
            Gizmos.color = defaultColor;
            Matrix4x4 DefaultMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(1, 0.1f, 1));
            Gizmos.DrawSphere(Vector3.zero, _GizmoRadius);
            
            Gizmos.color = new Color (1,0.2f,0.8f);
            
            Gizmos.DrawWireSphere(new Vector3(0,0.1f,0),_GizmoRadius * 1.0f);
            Gizmos.color = defaultColor;
            Gizmos.DrawWireSphere(new Vector3(0,0.1f,0),_GizmoRadius * 1.2f);
            Gizmos.matrix = DefaultMatrix;
            
            Gizmos.DrawLine(transform.position+ transform.forward * _GizmoRadius * 1.2f, transform.position + transform.forward * 3);
            
            if(connected) Handles.Label(transform.position + new Vector3(0,2,0), "To "+ connected.name);
            else Handles.Label(transform.position + new Vector3(0, 2, 0), "Not connected");
        }

        private void OnDrawGizmosSelected()
        {
            if (connected)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, connected.transform.position);

                if (useConnectedOrientation)
                {
                    Quaternion r = Quaternion.Euler(new Vector3(0, orientationOffset, 0)) * connected.transform.rotation;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(connected.transform.position, connected.transform.position + r * Vector3.forward);
                }
            }
        }
        
        #endif
    }

}
