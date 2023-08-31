using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

public class ZoomManager : MonoBehaviour
{
    private static ZoomManager instance = default;
    public static ZoomManager Instance => instance;

    [SerializeField] private List<ZoomController> _controllers = new List<ZoomController>();

    [SerializeField, ReadOnly] private UnityEngine.Camera _mainCamera = null;
    [SerializeField, ReadOnly] private CinemachineBrain _mainCinemachineBrain = null;
    [SerializeField, ReadOnly] private CinemachineVirtualCamera _mainVirtualCamera = null;

    private CinemachineVirtualCamera _current = null;
    private ParentConstraint _parentConstraint = null;

    private float _time = 0;
    private float _duration = 1.5f;

    private bool transitionStart = false;

    public bool IsZoomed = false;

    #region Unity Methods

    private void Awake()
    {
        if (instance && this != instance)
        {
            Debug.Log("Destroy");
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        foreach (ZoomController controller in FindObjectsOfType<ZoomController>())
        {
            if (!_controllers.Contains(controller))
                _controllers.Add(controller);
        }

        ZoomHandlerData.OnZoomStart += () => { UIManagerHandlerData.EnablePlayerInput(false); };
        ZoomHandlerData.OnZoomEnd += () => { IsZoomed = true; };

        ZoomHandlerData.OnUnzoomEnd += () =>
        {
            IsZoomed = false;
            UIManagerHandlerData.EnablePlayerInput(true);
        };

        _mainCinemachineBrain = FindObjectOfType<CinemachineBrain>();
    }

    private void Update()
    {
        if (transitionStart)
        {
            _time += Time.deltaTime;
            if (_time > _duration)
            {
                transitionStart = false;
                _time = 0;
                if (!IsZoomed) ZoomHandlerData.ZoomEnd();
                else ZoomHandlerData.UnzoomEnd();
            }
        }
    }

    #endregion
    
    public void ZoomOnThis(CinemachineVirtualCamera virtualCamera, float duration)
    {
        if (_current) return;
        _current = virtualCamera;
        ZoomHandlerData.ZoomStart();

        transitionStart = true;
        _duration = duration;
        _mainCinemachineBrain.m_DefaultBlend.m_Time = duration;
        
        virtualCamera.gameObject.SetActive(true);
    }

    public void UnZoom(CinemachineVirtualCamera virtualCamera, float duration)
    {
        if (_current != virtualCamera) return;


        ZoomHandlerData.UnzoomStart();

        transitionStart = true;
        _duration = duration;
        _mainCinemachineBrain.m_DefaultBlend.m_Time = duration;

        virtualCamera.gameObject.SetActive(false);
        _current = null;
    }
}