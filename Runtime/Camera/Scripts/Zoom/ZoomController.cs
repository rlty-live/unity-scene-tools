using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("RLTY/Zoom/Zoom Controller")]
public class ZoomController : MonoBehaviour
{
    [InfoBox("If using this on a mesh, it has to have a box Collider")]
    [SerializeField] public CinemachineVirtualCamera _zoomVirtualCamera = null;
    [SerializeField] private float _range = 20;
    [SerializeField] private float _durationTransition = 0.75f;
    [SerializeField] private float _screenPercentObjectSetup = 1;
    [SerializeField] private bool _useAutoSetup = true;

    private Vector3 _savePosition;
    private bool _canZoom = true;
    private bool _isSetup = false;

    #region MyRegion

    private void OnEnable()
    {
        if (!_zoomVirtualCamera)
        {
            CinemachineVirtualCamera virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            if (!virtualCamera)
            {
                // GameObject obj = Instantiate(new GameObject(), transform, true);
                GameObject obj = new GameObject();
                obj.transform.SetParent(transform);
                virtualCamera = obj.AddComponent<CinemachineVirtualCamera>();
                virtualCamera.m_Lens.FieldOfView = 60;
                obj.name = $"{gameObject.name}_VirtualCamera";
            }

            _zoomVirtualCamera = virtualCamera;
        }
        else
        {
            _zoomVirtualCamera.gameObject.SetActive(true);
        }

        if (!Application.isPlaying && !_isSetup && _useAutoSetup)
        {
            _zoomVirtualCamera.transform.position = transform.position;
            // ZoomManager.Setup(transform, 60, _zoomVirtualCamera, 0.8f);
            ZoomUtility.CalculateCameraPosition(_zoomVirtualCamera, transform, _zoomVirtualCamera.m_Lens.FieldOfView, _screenPercentObjectSetup);

        }
    }

    private void Start()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_zoomVirtualCamera)
        {
            _zoomVirtualCamera.gameObject.SetActive(false);
        }

        if (_useAutoSetup)
        {
            _zoomVirtualCamera.transform.position = _savePosition;
            _zoomVirtualCamera.LookAt = transform;
        }
    }

    #endregion

    #region RunTime

    [Button, DisableInEditorMode]
    void ZoomOnThis()
    {
        if (!Application.isPlaying) return;
        if (!_canZoom) return;
        ZoomManager.Instance.ZoomOnThis(_zoomVirtualCamera, _durationTransition);
    }

    [Button, DisableInEditorMode]
    void UnZoom()
    {
        if (!Application.isPlaying) return;
        if (!_canZoom) return;
        ZoomManager.Instance.UnZoom(_zoomVirtualCamera, _durationTransition);
    }

    public void Zoom()
    {
        if (Vector3.Distance(AllPlayers.Me.Transform.position, transform.position) > _range) return;

        if (ZoomManager.Instance.IsZoomed)
            UnZoom();
        else
        {
            ZoomOnThis();
        }
    }

    [Button, DisableInPlayMode, ShowIf("_useAutoSetup")]
    void SetupCamera()
    {
        if (!_useAutoSetup) return;

        _savePosition = ZoomUtility.CalculateCameraPosition(_zoomVirtualCamera, transform, _screenPercentObjectSetup);
    }

    #endregion
}