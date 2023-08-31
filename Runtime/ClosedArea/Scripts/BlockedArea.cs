using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BlockedArea : NetworkSceneTool
{
    #region Properties
    [InfoBox("Your blocked area need a name !"
        , InfoMessageType.Error, "CheckIfHasAName")]
    [BoxGroup("BlockedAreaTool", true, true, 0)]
    public string name;

    #region ToggleButtonSection

    [InfoBox("You must select one of the button bellow.", InfoMessageType.Error, "InfoBoxButtonDisplayError")]
    [Button("UseButton"), GUIColor("GetUseButtonColor")]
    [HorizontalGroup("BlockedAreaTool/Buttons", order: 5)]
    private void ToogleUseButton()
    {
        this._toggleButton = !this._toggleButton;
        ManageSpawnButton(!_toggleButton);
        _toggleEvent = true;
    }

    [InfoBox("You must select one of the button bellow.", InfoMessageType.Error, "InfoBoxButtonDisplayError")]
    [Button("UseEvent"), GUIColor("GetEventColor")]
    [HorizontalGroup("BlockedAreaTool/Buttons", order: 6)]
    private void ToogleUseEvent()
    {

        this._toggleEvent = !this._toggleEvent;
        if (!_toggleButton)
            ManageSpawnButton(_toggleButton);
        _toggleButton = true;
    }
    #endregion
    #region SpawnButtonGroupSection
    [SerializeField, HideInInspector]
    private bool _toggleButton = true;
    [SerializeField, HideInInspector]
    private bool _toggleEvent = true;

    private Vector3 _oldButtonPosition;
    private Quaternion _oldButtonQuaternion;
    private Vector3 _oldButtonLocalScale;

    [HideIf("_toggleButton")]
    [InfoBox("Button is missing, if you deleted it by mistake please re toggle the button"
        , InfoMessageType.Error, "CheckButtonPositionExist")]
    [TitleGroup("BlockedAreaTool/ButtonSettings", order: 7)]
    [HorizontalGroup("BlockedAreaTool/ButtonSettings/Split")]
    [BoxGroup("BlockedAreaTool/ButtonSettings/Split/Button"), LabelWidth(100)]
    [SerializeField]
    public Transform buttonPosition;

    [HideIf("_toggleButton")]
    [BoxGroup("BlockedAreaTool/ButtonSettings/Split/Button"), LabelWidth(100)]
    [EnumToggleButtons]
    public ToggleEnum IsAdminOnly;

    [HideIf("_toggleButton")]
    [InfoBox("You are missing an Image for your button when it's Open"
        , InfoMessageType.Error, "CheckButtonOpenSprite")]
    [BoxGroup("BlockedAreaTool/ButtonSettings/Split/Textures"), LabelWidth(100)]
    [OnInspectorGUI("DrawPreviewOpen", append: true)]
    public Sprite buttonOpenImage;

    [HideIf("_toggleButton")]
    [InfoBox("You are missing an Image for your button when it's Closed"
        , InfoMessageType.Error, "CheckButtonCloseSprite")]
    [BoxGroup("BlockedAreaTool/ButtonSettings/Split/Textures"), LabelWidth(100)]
    [OnInspectorGUI("DrawPreviewClose", append: true)]
    public Sprite buttonCloseImage;
    #endregion
    #region EventTriggerSection
    [HideIf("_toggleEvent")]
    [BoxGroup("BlockedAreaTool/EventSettings", true, true, 7)]
    [Title("Event Type")]
    [EnumToggleButtons, HideLabel]
    public EventTypeBitmask EventAction;
    [BoxGroup("BlockedAreaTool/BlockedZone", order: 0)]
    public float SizeOfWallGeneratedWall = 0.1f;
    #endregion
    #region WallAndRoomSpawnSection
    [InfoBox("BoxedRoomAreaSpawner Is missing Please reimport the Prefab"
        , InfoMessageType.Error, "CheckBoxedAreaSpawner")]
    [BoxGroup("BlockedAreaTool/BlockedZone", order: 1)]
    public GameObject BoxedRoomAreaSpawner;

    [InfoBox("You need to spawn at least one wall or room !"
        , InfoMessageType.Error, "CheckWallsExist")]
    [BoxGroup("BlockedAreaTool/BlockedZone", order: 3)]
    [HorizontalGroup("BlockedAreaTool/BlockedZone/Split")]
    [BoxGroup("BlockedAreaTool/BlockedZone/Split/WallList", order: 1)]
    [ListDrawerSettings(CustomAddFunction = nameof(SpawnWall), CustomRemoveElementFunction = nameof(DestroyWall), HideAddButton = true)]
    public List<GameObject> RoomWalls;

    [InfoBox("You need to spawn at least one wall or room !"
        , InfoMessageType.Error, "CheckWallsExist")]
    [InfoBox("Spawning room wont work if there are no BoxedRoomAreaSpawner !"
        , InfoMessageType.Error, "CheckBoxedAreaSpawner")]
    [HorizontalGroup("BlockedAreaTool/BlockedZone/Split")]
    [BoxGroup("BlockedAreaTool/BlockedZone/Split/RoomList", order: 1)]
    [ListDrawerSettings(CustomAddFunction = nameof(SpawnRoom), CustomRemoveElementFunction = nameof(DestroyRoom), HideAddButton = true)]
    public List<GameObject> RoomArea;

    [HideIf("CheckBoxedAreaSpawner")]
    [Button("SpawnRoom")]
    [HorizontalGroup("BlockedAreaTool/BlockedZone/Split")]
    [BoxGroup("BlockedAreaTool/BlockedZone/Split/RoomList", order: 0)]
    private void SpawnRoomButton()
    {
        RoomArea.Add(SpawnRoom());
    }
    [Button("SpawnWall")]
    [HorizontalGroup("BlockedAreaTool/BlockedZone/Split")]
    [BoxGroup("BlockedAreaTool/BlockedZone/Split/WallList", order: 0)]
    private void SpawnWallButton()
    {
        RoomWalls.Add(SpawnWall());
    }
    #endregion
    #endregion
    #region ObjectsInstantiationSection
    public void ManageSpawnButton(bool spawn)
    {
        if (spawn)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.SetParent(this.transform);
            obj.transform.position = _oldButtonPosition != Vector3.zero ? _oldButtonPosition : this.transform.position;
            obj.transform.rotation = _oldButtonQuaternion;
            obj.transform.localScale = _oldButtonLocalScale != Vector3.zero ? _oldButtonLocalScale : new Vector3(1, 1, 0.01f);
            obj.name = "Button";
            DestroyImmediate(obj.GetComponent<BoxCollider>());
            DestroyImmediate(obj.GetComponent<MeshFilter>());
            DestroyImmediate(obj.GetComponent<MeshRenderer>());
            obj.AddComponent<BoxScaleDrawer>();
            obj.AddComponent<ArrowDrawer>();
            buttonPosition = obj.transform;
        }
        else
        {
            _oldButtonPosition = buttonPosition.transform.position;
            _oldButtonQuaternion = buttonPosition.rotation;
            _oldButtonLocalScale = buttonPosition.localScale;
            DestroyImmediate(buttonPosition.gameObject);
        }
    }

    public GameObject SpwanGeneratedWalls(Vector3 position, Vector3 scale)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //obj.transform.SetParent(this.transform);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        obj.name = "GeneratedWall";
        obj.AddComponent<BoxScaleDrawer>();
        obj.AddComponent<ArrowDrawer>();
        DestroyImmediate(obj.GetComponent<BoxCollider>());
        DestroyImmediate(obj.GetComponent<MeshRenderer>());
        DestroyImmediate(obj.GetComponent<MeshFilter>());
        return obj;
    }

    public GameObject SpawnRoom()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.SetParent(this.transform);
        obj.transform.position = BoxedRoomAreaSpawner.transform.position;
        obj.transform.rotation = BoxedRoomAreaSpawner.transform.rotation;
        BoxCollider col = BoxedRoomAreaSpawner.AddComponent<BoxCollider>();
        SpwanGeneratedWalls(new Vector3((col.bounds.center.x + (col.bounds.extents.x) + ((SizeOfWallGeneratedWall / 2))), col.bounds.center.y,
            col.bounds.center.z), new Vector3(SizeOfWallGeneratedWall, col.bounds.extents.y * 2, col.bounds.extents.z * 2)).transform.SetParent(obj.transform);
        SpwanGeneratedWalls(new Vector3((col.bounds.center.x + (-col.bounds.extents.x) - ((SizeOfWallGeneratedWall / 2))), col.bounds.center.y,
            col.bounds.center.z), new Vector3(SizeOfWallGeneratedWall, col.bounds.extents.y * 2, col.bounds.extents.z * 2)).transform.SetParent(obj.transform);
        SpwanGeneratedWalls(new Vector3(col.bounds.center.x, col.bounds.center.y + (col.bounds.extents.y) + ((SizeOfWallGeneratedWall / 2)),
            col.bounds.center.z), new Vector3(col.bounds.extents.x * 2, 0.1f, col.bounds.extents.z * 2)).transform.SetParent(obj.transform);
        SpwanGeneratedWalls(new Vector3(col.bounds.center.x, col.bounds.center.y + (-col.bounds.extents.y) - ((SizeOfWallGeneratedWall / 2)),
            col.bounds.center.z), new Vector3(col.bounds.extents.x * 2, 0.1f, col.bounds.extents.z * 2)).transform.SetParent(obj.transform);
        SpwanGeneratedWalls(new Vector3(col.bounds.center.x, col.bounds.center.y,
                col.bounds.center.z + (col.bounds.extents.z) + ((SizeOfWallGeneratedWall / 2))),
            new Vector3(col.bounds.extents.x * 2, col.bounds.extents.y * 2, SizeOfWallGeneratedWall)).transform.SetParent(obj.transform);
        SpwanGeneratedWalls(new Vector3(col.bounds.center.x, col.bounds.center.y,
                col.bounds.center.z + (-col.bounds.extents.z) - ((SizeOfWallGeneratedWall / 2))),
            new Vector3(col.bounds.extents.x * 2, col.bounds.extents.y * 2, SizeOfWallGeneratedWall)).transform.SetParent(obj.transform);
        DestroyImmediate(obj.GetComponent<MeshRenderer>());
        DestroyImmediate(obj.GetComponent<BoxCollider>());
        DestroyImmediate(obj.GetComponent<MeshFilter>());
        DestroyImmediate(col);
        obj.name = name + "_GeneratedRoom_N°" + RoomArea.Count;
        return obj;
    }

    public GameObject SpawnWall()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.SetParent(this.transform);
        obj.transform.position = this.transform.position;
        obj.name = "Wall";
        obj.transform.localScale = new Vector3(3, 2, SizeOfWallGeneratedWall);
        obj.AddComponent<BoxScaleDrawer>();
        obj.AddComponent<ArrowDrawer>();
        DestroyImmediate(obj.GetComponent<BoxCollider>());
        DestroyImmediate(obj.GetComponent<MeshRenderer>());
        DestroyImmediate(obj.GetComponent<MeshFilter>());
        return obj;
    }

    public void DestroyWall(GameObject obj)
    {
        RoomWalls.Remove(obj);
        DestroyImmediate(obj);
    }

    public void DestroyRoom(GameObject obj)
    {
        RoomArea.Remove(obj);
        DestroyImmediate(obj);
    }
    #endregion
    #region PropertyDrawerSection
    private void DrawPreviewOpen()
    {
        if (buttonOpenImage == null) return;
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(buttonOpenImage.texture);
        GUILayout.EndVertical();
    }

    private void DrawPreviewClose()
    {
        if (buttonCloseImage == null) return;
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label(buttonCloseImage.texture);
        GUILayout.EndVertical();
    }
    #endregion
    #region SetButtonColorSection
    private Color GetUseButtonColor()
    {
        if (!_toggleButton)
        {
            return Color.green;
        }
        else
        {
            return Color.red;
        }
    }

    private Color GetEventColor()
    {
        if (!_toggleEvent)
        {
            return Color.green;
        }
        else
        {
            return Color.red;
        }
    }
    #endregion
    #region ConditionalMethodForDisplay
    public bool InfoBoxButtonDisplayError()
    {
        if (_toggleButton && _toggleEvent)
            return true;
        else
            return false;
    }

    public bool CheckButtonPositionExist()
    {
        return buttonPosition == null ? true : false;
    }

    public bool CheckButtonOpenSprite()
    {
        return buttonPosition == null ? true : false;
    }

    public bool CheckButtonCloseSprite()
    {
        return buttonCloseImage == null ? true : false;
    }

    public bool CheckBoxedAreaSpawner()
    {
        return BoxedRoomAreaSpawner == null ? true : false;
    }

    public bool CheckWallsExist()
    {
        return RoomArea.Count == 0 && RoomWalls.Count == 0 ? true : false;
    }

    public bool CheckIfHasAName()
    {
        return name.Length == 0 ? true : false;
    }
    #endregion
}

[Flags]
public enum EventTypeBitmask
{
    AdminQuizGame = 1 << 1,
    EventBasedOnAreaName = 1 << 2,
    CloseAllRoom = 1 << 3,
    All = AdminQuizGame | EventBasedOnAreaName | CloseAllRoom
}

public enum ToggleEnum
{
    Yes,
    No
}