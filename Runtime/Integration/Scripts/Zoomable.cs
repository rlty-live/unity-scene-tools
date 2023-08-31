using UnityEngine;
using Sirenix.OdinInspector;
using RLTY.UI;

[HideMonoScript]
[RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(RLTYMouseEvent))]
public class Zoomable : SceneTool
{
    [ReadOnly, InfoBox(warningMessage)]
    public Collider col;

    public const string warningMessage = "For now a zoomable must have specifically a BoxCollider component. \n\n" +
    "The camera will move toward and face the object looking at it's bounds center, aligned with the positive Z axis (the blue line)";

#if UNITY_EDITOR
    [Title("Display")]
    [ShowIf("showUtilities"), SerializeField]
    private float cameraGizmoRadius = 0.3f;
    [ShowIf("showUtilities"), SerializeField]
    private float cameraLineLength = 2;
    [ShowIf("showUtilities"), SerializeField]
    private Color gizmoColor = new Color(1, 0, 0, 0.8f);
    static Color allZoomableGizmosColor = new Color(1,0,0,0.8f);

    public void OnDrawGizmos()
    {
        DrawCameraGizmo();
    }

    public void OnDrawGizmosSelected()
    {
        DrawCameraGizmo();
    }

    public void Reset()
    {
        showUtilities = true;
        if (TryGetComponent(out BoxCollider _col)) col = _col;
        else Debug.Log(warningMessage);

    }

    public void OnValidate()
    {
        if (TryGetComponent(out BoxCollider _col)) col = _col;

        if (gizmoColor != null)
            allZoomableGizmosColor = gizmoColor;
    }

    public void DrawCameraGizmo()
    {
        Gizmos.color = allZoomableGizmosColor;

        Vector3 cameraLineStart = col.bounds.center - transform.forward;
        Vector3 cameraLineEnd = col.bounds.center + transform.forward * -cameraLineLength;

        Gizmos.DrawLine(cameraLineStart, cameraLineEnd);
        Gizmos.DrawSphere(cameraLineEnd, cameraGizmoRadius);
    }
#endif
}
