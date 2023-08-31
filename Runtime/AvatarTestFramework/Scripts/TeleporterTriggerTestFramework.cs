using UnityEngine;
using System;
using TMPro;

namespace Judiva.Metaverse.Interactions
{
    public class TeleporterTriggerTestFramework : MonoBehaviour
    {
        public float fadeOutTime = 0.01f;
        public float fadeInTime = 0.3f;
        
        [Header("Connected teleport")]
        public TeleporterTriggerTestFramework connected;
        [Tooltip("if true, the player will align with the connected teleport Z axis")]
        public bool useConnectedOrientation=false;
        [Tooltip("additional Y orientation offset when using connected orientation")]
        [Range(-180,180)]
        public float orientationOffset = 0;

        private DateTime _arrivedTime;
        public event Action OnTeleport;

        public TextMeshPro Text;
        public AudioClip TeleportSFX;

        private void Start()
        {
            //enabled = false;
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

        public void SetupTeleport(TeleporterTriggerTestFramework teleporterTriggerTestFrameworkConnected, Transform localTransform)
        {
            connected = teleporterTriggerTestFrameworkConnected;
            enabled = true;
            transform.position = localTransform.position;
            transform.rotation = localTransform.rotation;
            Text.text = connected.gameObject.name;
            string[] splitCharacter = connected.gameObject.name.Split(char.Parse("$"));
            Debug.Log("name is "+splitCharacter[0]+ splitCharacter[1]);
            Text.text = splitCharacter[0];
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject + " entered teleport " + gameObject.name);
            
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

        
    }

}
