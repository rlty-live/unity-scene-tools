using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SlapOntoSurface
{
    public static void SlapThisOntoSurface(Transform transform,float offsetFromSurfaceInMillimeters)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward * -1), out hit, Mathf.Infinity))
        {
            SetPositionAndRotation(transform, hit.point, Quaternion.LookRotation(hit.normal), offsetFromSurfaceInMillimeters);
        }
        else
        {
            Debug.Log("Raycast did not Hit for SlapOntoSurface");
        }
    }

    private static void SetPositionAndRotation(Transform transform, Vector3 pos, Quaternion rot, float offsetFromSurfaceInMillimeters)
    {
        transform.position = pos;
        transform.rotation = rot;

        transform.Translate(new Vector3(0, 0, offsetFromSurfaceInMillimeters / 1000));
    }
}
