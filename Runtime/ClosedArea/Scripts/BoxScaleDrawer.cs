using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScaleDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.matrix = trs;
        
        Color32 color = Color.green;
        color.a = 255;
        Color32 colorSeeThrough = color;
        colorSeeThrough.a = 64;
        
        Gizmos.color = color;

        Vector3 toto = new Vector3(1,1,1);
        Gizmos.DrawWireCube(Vector3.zero, toto);
        Gizmos.color = colorSeeThrough;
        Gizmos.DrawCube(Vector3.zero, toto);
    }
#endif
}
