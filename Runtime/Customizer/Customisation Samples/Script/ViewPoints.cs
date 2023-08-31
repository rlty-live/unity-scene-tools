using UnityEngine;
using System.Collections.Generic;

namespace RLTY.Customisation
{
    public class ViewPoints : MonoBehaviour
    {
        public Transform transformsParent;
        public List<Transform> transformList;
        public int currentCPIndex = 0;

        public void OnValidate()
        {
            transformList = new List<Transform>();

            if (transformsParent)
            {
                transformList.AddRange(transformsParent.GetComponentsInChildren<Transform>());
                transformList.RemoveAt(0);
            }
        }

        public void ChangePosition(bool next)
        {
            Transform nextTransform;

            if (next)
            {
                if (currentCPIndex == transformList.Count)
                {
                    currentCPIndex = 0;
                    nextTransform = transformList[currentCPIndex];

                }

                else
                {
                    nextTransform = transformList[currentCPIndex++];
                }

                transform.SetPositionAndRotation(nextTransform.position, transform.rotation);
            }

            else
            {
                if (currentCPIndex == 0)
                {
                    currentCPIndex = transformList.Count;
                    nextTransform = transformList[currentCPIndex];
                }

                else
                {
                    nextTransform = transformList[currentCPIndex--];
                }

                transform.SetPositionAndRotation(nextTransform.position, transform.rotation);
            }
        }
    }
}
