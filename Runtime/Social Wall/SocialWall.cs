using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SocialWall : SceneTool
{
    [PropertyRange(1,20)]
    public float Scale = 1;
    [PropertyRange(2,10)]
    public float Lenght = 2;
    [PropertyRange(1,10)]
    public float Height = 1;
    [PropertyRange(10,50)]
    public int MaxMediasDisplayed = 10;

    public bool TransparentBackground = false;

    public Vector2 GetScreenSize()
    {
        return new Vector2(1080 * Lenght, 1080 * Height);
    }


    //private Vector2 _ScreenSize = new Vector2(2280, 1080);
    //[HideInInspector] public Vector2 ScreenSize => GetScreenSize();
    
    
    
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector2 screenSize = GetScreenSize() / 1080 * Scale;
        
        Color32 color = new Color(0.1f, 0, 0.5f);
        color.a = 255;
        Gizmos.color = color;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.3f * Scale);
        
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = trs;
        
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(screenSize.x, screenSize.y, 0.0001f));
        
        color.a = 125;
        Gizmos.color = color;
        Gizmos.DrawCube(Vector3.zero, new Vector3(screenSize.x, screenSize.y, 0.0001f));
    }
    
    [TitleGroup("Positioning")]
    [PropertyRange(1,100)]
    public float OffsetFromSurfaceInMillimeters = 1;
    
    [Button]
    private void SlapOntoSurfaceBehind()
    {
        SlapOntoSurface.SlapThisOntoSurface(transform, OffsetFromSurfaceInMillimeters);
    }
    
    #endif
}
