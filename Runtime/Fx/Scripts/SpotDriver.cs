using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpotDriver : MonoBehaviour
{
    public List<Transform> spots;

    public float radius = 1;
    public float xScale = 1;
    public float zScale = 1;

    Vector3 GetPosition(int index)
    {
        float angle = index * Mathf.PI * 2 / spots.Count;
        return transform.TransformPoint(radius*new Vector3(xScale * Mathf.Cos(angle), 0, zScale*Mathf.Sin(angle)));
    }

    void Update()
    {
        for (int i = 0; i < spots.Count; i++)
            spots[i].LookAt(GetPosition(i));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < spots.Count; i++)
        {
            Gizmos.DrawWireSphere(GetPosition(i), 0.1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(GetPosition(i), GetPosition((i+1)%spots.Count));
        }
            
    }
}
