using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawArrow
{
    public static void ForGizmo (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay (pos, direction);
        DrawArrowEnd(true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }
 
    public static void ForGizmo (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20)
    {
        Gizmos.DrawRay (pos, direction);
        DrawArrowEnd(true, pos, direction, color, arrowHeadLength, arrowHeadAngle);
    }
 
    public static void ForDebug (Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay (pos, direction);
        DrawArrowEnd(false, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }
 
    public static void ForDebug (Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay (pos, direction, color);
        DrawArrowEnd(false, pos, direction, color, arrowHeadLength, arrowHeadAngle);
    }
   
    private static void DrawArrowEnd (bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Vector3 forward = Quaternion.AngleAxis(arrowHeadAngle, Vector3.forward) * direction;
        Vector3 backward = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * new Vector3(0, 0, arrowHeadLength);
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * new Vector3(0, 0, arrowHeadLength);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * new Vector3(0, 0, arrowHeadLength);
        if (gizmos) {
            Gizmos.color = color;
            Gizmos.DrawRay (pos + direction, -((pos + direction + right )* arrowHeadLength));
            Gizmos.DrawRay (pos + direction, -((pos + direction + left )* arrowHeadLength));
            Gizmos.DrawRay (pos + direction, -((pos + direction + backward )* arrowHeadLength));
            Gizmos.DrawRay (pos + direction, -((pos + direction + forward )* arrowHeadLength));
        } else {
            Debug.DrawLine(pos + direction, -((pos + direction + right) * arrowHeadLength), color);
            Debug.DrawLine(pos + direction, -((pos + direction + left) * arrowHeadLength), color);
            Debug.DrawLine(pos + direction, -((pos + direction + backward) * arrowHeadLength), color);
            Debug.DrawLine(pos + direction, -((pos + direction + forward) * arrowHeadLength), color);
        }
    }
}