using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StaticFrame : SceneTool
{
    [TitleGroup("Settings")]
    public string ID;
    public enum StaticFrameTypeEnum
    {
        StaticFramePublic,
        StaticFrameReservedToAdmins,
        StaticFrameBranding
    }
    public StaticFrameTypeEnum Type = StaticFrameTypeEnum.StaticFramePublic;
    
    [TitleGroup("Size")]
    [ReadOnly]
    public Vector2Int ScreenSize = new Vector2Int(1920, 1080);
    [PropertyRange(1,10)]
    public float Scale = 1;
    [PropertyRange(1,10)]
    public float Width = 1;
    [PropertyRange(1,10)]
    public float Height = 1;

    [HideInInspector][ReadOnly]
    public int BaselineSize = 1080;

#if UNITY_EDITOR
    private void Reset()
    {
        ID = SceneManager.GetActiveScene().name + "_Frame_" + DateTime.Now.Second + DateTime.Now.Millisecond;
        OnValidate();
    }

    private void OnValidate()
    {
        ID = ID.Replace(" ", "_");
        SetSize();
    }

    [TitleGroup("Size", indent:false)]
    [Button]
    private void FlipXY()
    {
        float height = Width;
        float width = Height;
        Height = height;
        Width = width;
        SetSize();
    }

    [Button]
    private void SetTo16_9()
    {
        Width = 16f/9f * Height;
        SetSize();
    }
    
    [Button]
    private void SetToSquare()
    {
        Width = Height;
        SetSize();
    }
    
    [Button]
    private void SetToAxPortrait()
    {
        Width = 841f/1189f * Height;
        SetSize();
    }

    [Button]
    private void SetToAxLandscape()
    {
        Width = 1189f/841f * Height;
        SetSize();
    }
    
    private void SetSize()
    {
        if (Width < 1)
        {
            float factor = 1 / Width *2;
            Width = Width * factor;
            Height = Height * factor;
        }
        
        if (Height < 1)
        {
            float factor = 1 / Height;
            Width = Width * factor;
            Height = Height * factor;
        }
        
        
        int widthInPixels = BaselineSize;
        int heightInPixels = BaselineSize;
        
        if (Height >= Width)
        {
            heightInPixels = (int)(BaselineSize * Height /Width);
        }
        else
        {
            widthInPixels = (int)(BaselineSize * Width / Height);
        }

        
        
        ScreenSize = new Vector2Int(widthInPixels, heightInPixels);
    }
    
    private Color32 _FrameBaseColor = Color.blue;
    private Color32 _AdminColor = new Color(0.1f, 0.5f, 1);

    private Color32 GetGizmoColor()
    {
        Color32 color = Color.white;
        switch (Type)
        {
            case StaticFrameTypeEnum.StaticFramePublic:
                color = _FrameBaseColor;
                break;
            case StaticFrameTypeEnum.StaticFrameReservedToAdmins:
                color = _AdminColor;
                break;
            default:
                break;
                //throw new ArgumentOutOfRangeException();
        }

        return color;
    }

    void OnDrawGizmos()
    {
        Vector2 screenSize = new Vector2(Width, Height) * Scale;

        Color32 gizmoColor = GetGizmoColor();
        gizmoColor.a = 255;
        Gizmos.color = gizmoColor;
        
        if(String.IsNullOrEmpty(ID)) Gizmos.color = Color.red;
        
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.3f * Scale);
        
        Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.matrix = trs;
        
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(screenSize.x, screenSize.y, 0.0001f));

        string idDisplay = ID;
        if (String.IsNullOrEmpty(idDisplay))
        {
            Handles.color = Color.red;
            idDisplay = "No ID";
        }
        
        Handles.Label(transform.position, "Frame : "+ idDisplay);
        
        gizmoColor = _FrameBaseColor;
        gizmoColor.a = 125;
        Gizmos.color = gizmoColor;
        
        if(String.IsNullOrEmpty(ID)) Gizmos.color = new Color(1,0,0,0.5f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(screenSize.x, screenSize.y, 0.0001f));
    }


    private void OnDrawGizmosSelected()
    {
        List<Transform> framesWithSameID = GeteveryFrameWithSameID();

        foreach (var otherFrameTransform in framesWithSameID)
        {
            Debug.DrawLine(otherFrameTransform.position, gameObject.transform.position, GetGizmoColor());
        }
    }
    
    
    
    
    private List<Transform> GeteveryFrameWithSameID()
    {
        StaticFrame[] AllStaticFrames = FindObjectsOfType<StaticFrame>();
        List<Transform> transforms= new List<Transform>();

        foreach (var staticFrame in AllStaticFrames)
        {
            if(staticFrame.ID == ID && !String.IsNullOrEmpty(ID)) transforms.Add(staticFrame.transform);
        }

        return transforms;
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
