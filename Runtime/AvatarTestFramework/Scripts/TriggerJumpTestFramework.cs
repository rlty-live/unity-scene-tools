using System.Collections.Generic;
using UnityEngine;

namespace Judiva.Metaverse.Interactions
{

    [RequireComponent(typeof(TriggerZone))]
    public class TriggerJumpTestFramework : MonoBehaviour
    {
        private float verticalVelocity = 5;
        private float orientationOffset = 0;
        private float additionalSpeed = 0;

        public void SetTriggerJump(float verticalVel, float orientationOfst, float addSpeed,
            Transform locationTransform)
        {
            verticalVelocity = verticalVel;
            orientationOffset = Mathf.Clamp(orientationOfst, -180, 180);
            additionalSpeed = addSpeed;
            
            transform.SetParent(locationTransform);

            transform.position = locationTransform.position;
            transform.rotation = locationTransform.rotation;
        }

        Quaternion Orientation { get { return Quaternion.Euler(new Vector3(0, orientationOffset, 0)) * transform.rotation; } }

        private void Start()
        {
            GetComponent<TriggerZone>().onPlayerEnter += (x) =>
            {
                AllPlayers.Me.SetAdditionalSpeed(additionalSpeed * (Orientation * Vector3.forward));
                AllPlayers.Me.SetVerticalVelocity(verticalVelocity);  
            };
        }
    }
}