using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLayer : MonoBehaviour
{

    [SerializeField]
    private LayerMask targetLayer;
    int invokeIndex = 0;

    /* removed by François
     * why do we need a component to set an object's layer ?
     * this causes garbage in the error log
    private void Start()
    {
        InvokeRepeating("CheckLayer", 0, 5);
    }

    public void CheckLayer()
    {
        invokeIndex++;

        if (gameObject.layer != targetLayer)
        {
            gameObject.layer = targetLayer;
            Debug.Log("Layer checked and restored to " + targetLayer, this);
        }

        if (invokeIndex == 5)
            CancelInvoke();
    }*/
}
