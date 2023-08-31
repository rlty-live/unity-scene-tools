using UnityEngine;

namespace RLTY.Tools
{
    public class AnimatorBoolSetter : MonoBehaviour
    {
        public Animator animator;
        public bool debug;

        public void Start()
        {
            //Initialize
            if (!animator)
            {
                if (TryGetComponent<Animator>(out Animator animator))
                    animator = GetComponent<Animator>();

                else
                {
                    if (!GetComponentInChildren<Animator>())
                        if (debug) Debug.Log("An animator is required on this gamobject, or its children", this);

                        else
                            animator = GetComponentInChildren<Animator>();
                }
            }
        }

        public void SetAnimatorBoolParameterToTrue(string boolName)
        {
            animator.SetBool(boolName, true);
            if (debug) Debug.Log(animator + " parameter " + boolName + " set to true.", this);
        }

        public void SetAnimatorBoolParameterToFalse(string boolName)
        {
            animator.SetBool(boolName, false);
            if (debug) Debug.Log(animator + " parameter " + boolName + " set to false.", this);
        }
    }
}