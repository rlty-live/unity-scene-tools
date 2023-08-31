using UnityEngine;

public class ArrowDrawer : MonoBehaviour
{
    public float angle = 20f;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = trs;
        
        Color32 color = Color.green;
        color.a = 255;
        Color32 colorSeeThrough = color;
        colorSeeThrough.a = 64;
        
        Gizmos.color = color;
        DrawArrow.ForGizmo(Vector3.zero, Vector3.forward*2, Color.green);
    }
#endif
}