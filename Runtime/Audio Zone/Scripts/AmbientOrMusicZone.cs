using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AmbientOrMusicZone : SceneTool
{
    public AudioClip AudioClip;
    [PropertyRange(0,1)]
    public float Volume = 1;
    //[PropertyRange(0,50)]
    //public int Priority = 1;
    //public bool IsGlobal;
    
    public enum ColliderTypeEnum
    {
        Global,
        Sphere,
        Box,
        Mesh
    }
    
    public ColliderTypeEnum ColliderType = ColliderTypeEnum.Box;
    
    [ShowIf(nameof(ColliderType), ColliderTypeEnum.Sphere)]
    [PropertyRange(1,200)]
    public float SphereColliderDiameter = 5;

    [ShowIf(nameof(ColliderType), ColliderTypeEnum.Box)]
    public Vector3 BoxColliderSize = new Vector3(3, 2, 5);

    [ShowIf(nameof(ColliderType), ColliderTypeEnum.Mesh)][Required("You need to specify a mesh to define the collider bounds. If not, this audio zone will be ignored")]
    public Mesh ColliderMesh;

    

    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = trs;
        
        Color32 color = Color.green;
        color.a = 255;
        Color32 colorSeeThrough = color;
        colorSeeThrough.a = 64;
        
        Gizmos.color = color;
        
        if (ColliderType == ColliderTypeEnum.Sphere)
        {
            Gizmos.DrawWireSphere(Vector3.zero, SphereColliderDiameter/2);
            Gizmos.color = colorSeeThrough;
            Gizmos.DrawSphere(Vector3.zero, SphereColliderDiameter/2);
        }

        if (ColliderType == ColliderTypeEnum.Box)
        {
            Gizmos.DrawWireCube(Vector3.zero, BoxColliderSize);
            Gizmos.color = colorSeeThrough;
            Gizmos.DrawCube(Vector3.zero, BoxColliderSize);
        }

        if (ColliderType == ColliderTypeEnum.Mesh && ColliderMesh)
        {
            Gizmos.DrawWireMesh(ColliderMesh,Vector3.zero);
            Gizmos.color = colorSeeThrough;
            Gizmos.DrawMesh(ColliderMesh,Vector3.zero);
        }
    }
#endif
}
