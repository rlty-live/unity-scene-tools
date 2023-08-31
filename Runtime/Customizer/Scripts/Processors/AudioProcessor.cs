using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RLTY.Customisation
{
    public class AudioProcessor : Processor
    {
        public override Component FindComponent()
        {
            Component target = null;
            AudioSource audioSource = GetComponentInChildren<AudioSource>();

            if (!audioSource)
            {
                if (debug)
                    Debug.LogWarning("No AudioSource found in children" + commonWarning, this);
            }
            else
                target = audioSource;

            return target;
        }
    }
}
