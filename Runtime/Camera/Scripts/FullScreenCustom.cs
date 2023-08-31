using System;
using RLTY.Rendering;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode, AddComponentMenu("RLTY/Camera/FullScreen"), HideMonoScript]
public class FullScreenCustom : MonoBehaviour
{
    public UnityAction<bool> OnFullScreen;
    public UnityAction<Camera> OnCameraCreate;
    public UnityAction OnFadeFinish;
    public UnityAction OnCameraDestroy;
    
    [SerializeField] private FadeCamera _fullScreenFade = null;
    // [SerializeField] private FadeCamera _playerScreenFade = null;
    [SerializeField] private BlendCamera _blendCamera = null;

    [SerializeField, HideIf("_hideRect")] private RectTransform _targetObjectRectTransform = null;

    [SerializeField, HideIf("_hideRenderer")]
    private Renderer _targetObjectRenderer = null;

    #region Settings Camera

    [SerializeField, ShowIf("_startSetup")]
    private LayerMask _layerMask;

    [SerializeField, Range(-100, 100), ShowIf("_startSetup")]
    private float _distance = 0;

    [SerializeField, Range(0, 180), ShowIf("_startSetup")]
    private float _fieldOfView = 60;

    [SerializeField, ShowIf("_startSetup")]
    private float _clipRange = 5;

    #endregion

    [SerializeField, ReadOnly] Vector3 _spawnCamera = Vector3.zero;
    [SerializeField, HideInInspector] private bool _spawnSetup = false;

    private Camera _fullScreenCamera = null;
    private Camera _mainCamera = null;

    private float _distanceVerif = 0;
    private Vector3 _targetBoundCenter = Vector3.zero;

    private bool _startSetup = false;
    private bool isFullScreen = false;
    private bool _isFading = false;

    private bool _hideRect => !_targetObjectRectTransform && _targetObjectRenderer;
    private bool _hideRenderer => _targetObjectRenderer && _targetObjectRectTransform;

    #region Unity Methods

    private void OnEnable()
    {
        if (!_fullScreenFade)
        {
            _fullScreenFade = gameObject.AddComponent<FadeCamera>();
            _fullScreenCamera.name = "From Player To FullScreen Fade";
        }

        // if (!_playerScreenFade)
        // {
        //     _playerScreenFade = gameObject.AddComponent<FadeCamera>();
        //     _playerScreenFade.name = "From FullScreen to Player Fade";
        // }
    }

    private void Awake()
    {
        OnFullScreen += (b) =>
        {
            if (_isFading) return;
            _isFading = true;
            if (!b)
                SetUpOrientation();
            else
            {
                RemoveFullScreen();
            }
        };

        // OnCameraCreate += (mc, fc) => { StartBlend(mc, fc); };
    }

    private void Start()
    {
        if (_fullScreenCamera)
            DestroyFullScreenCamera();

        OnCameraCreate += (cam) => { StartBlend(_mainCamera, cam); };

        if (!_blendCamera)
            _blendCamera = GetComponent<BlendCamera>();
        if (!_blendCamera)
            _blendCamera = gameObject.AddComponent<BlendCamera>();

        // _blendCamera.OnBlendFinish += _blendCamera.Reset;

        OnFadeFinish += () => {
            SwitchCamera();
            _isFading = false;
        };
    }

// #if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying) return;

        if (_fullScreenCamera)
        {
            _fullScreenCamera.transform.LookAt(_targetBoundCenter);
            UpdateClip();
            UpdateFieldOfView();
        }
    }

    private void LateUpdate()
    {
        if (Application.isPlaying) return;

        if (_distanceVerif != _distance && _fullScreenCamera && !Application.isPlaying)
        {
            _distanceVerif = _distance;
            // Vector3 movement = _targetBoundCenter + (transform.forward * _distance);
            Vector3 movement = _targetBoundCenter + (-_fullScreenCamera.transform.forward * _distance);
            _fullScreenCamera.transform.position = movement;
        }
// #endif
    }

    private void OnDrawGizmos()
    {
        if (_fullScreenCamera)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_targetBoundCenter, 2f);
            Gizmos.color = Color.white;

            Gizmos.color = Color.blue;
            Vector3 pos = _targetBoundCenter + (-_fullScreenCamera.transform.forward * _distance);
            Gizmos.DrawSphere(pos, 2);
            Gizmos.DrawLine(_targetBoundCenter, _fullScreenCamera.transform.position);
            Gizmos.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        if (_fullScreenCamera)
            DestroyFullScreenCamera();

        OnFullScreen = null;
        OnCameraCreate = null;
        // OnCameraCreate = null;
    }

    private void OnApplicationQuit()
    {
        if (_fullScreenCamera)
            DestroyFullScreenCamera();

        OnFullScreen = null;
    }

    private void OnMouseDown()
    {
        OnFullScreen?.Invoke(isFullScreen);
    }

    private void OnMouseOver()
    {
        //TODO Animate outline
    }

    #endregion

// #if UNITY_EDITOR
    [Button("Setup Camera"), HideIf("_startSetup")]
    void SetupCamera()
    {
        if (Application.isPlaying) return;
        CreateCamera();
        _spawnSetup = false;
        _startSetup = true;
    }

    [Button("Save Setup Camera"), ShowIf("_startSetup")]
    void SaveSetup()
    {
        if (Application.isPlaying) return;

        if (!_fullScreenCamera) return;

        _spawnCamera = _fullScreenCamera.transform.localPosition;
        Debug.Log("Position " + _fullScreenCamera.transform.position);
        Debug.Log("Local " + _fullScreenCamera.transform.localPosition);
        _distance = _spawnCamera.z;
        _distanceVerif = _distance;
        _spawnSetup = true;
        DestroyImmediate(_fullScreenCamera.gameObject);
        _startSetup = false;
    }
// #endif
    /// <summary>
    /// Setup camera in Runtime and start the transition to the FullScreen Camera
    /// </summary>
    void SetUpOrientation()
    {
        // if (_isFading) return;
        _mainCamera = Camera.main;

        _fullScreenFade.Reset();
        _blendCamera.OnBlendFinish += _blendCamera.Reset;

        _fullScreenFade.Fade(() => { CreateCamera(); }, () => OnFadeFinish?.Invoke(), true);
        
        // if (Judiva.Metaverse.Player.Me && Judiva.Metaverse.Player.Me.PlayerController)
        //     Judiva.Metaverse.Player.Me.PlayerController.enabled = false;

        if (AllPlayers.List.Count > 0)
        {
            // AllPlayers.Me.EnableInput(false);
            UIManagerHandlerData.EnablePlayerInput(false);
        }
    }

    /// <summary>
    /// Transition to the Player camera and Destroy the FullScreen Camera
    /// </summary>
    void RemoveFullScreen()
    {
        // _playerScreenFade.Reset();
        _fullScreenFade.Reset();
        _blendCamera.OnBlendFinish += () =>
        {
            DestroyFullScreenCamera();
            _blendCamera.Reset();
        };

        // _playerScreenFade.Fade(() =>
        // {
        //     StartBlend(_fullScreenCamera, _mainCamera);
        // }, () => OnFadeFinish?.Invoke(), true);
        
        _fullScreenFade.Fade(() =>
        {
            StartBlend(_fullScreenCamera, _mainCamera);
        }, () => OnFadeFinish?.Invoke(), true);


        // if (Judiva.Metaverse.Player.Me && Judiva.Metaverse.Player.Me.PlayerController)
        //     Judiva.Metaverse.Player.Me.PlayerController.enabled = true;

        if (AllPlayers.List.Count > 0)
        {
            // AllPlayers.Me
            UIManagerHandlerData.EnablePlayerInput(true);
        }
    }
    
    void SwitchCamera()
    {
        if (isFullScreen)
        {
            DestroyFullScreenCamera();
            // Camera.SetupCurrent(_mainCamera);
            isFullScreen = false;
            RenderSettings.fog = true;
            // _blendCamera.Reset();
        }
        else
        {
            // Camera.SetupCurrent(_fullScreenCamera);
            isFullScreen = true;
        }

        if (_mainCamera) _mainCamera.targetTexture = null;
        if (_fullScreenCamera) _fullScreenCamera.targetTexture = null;
    }
    /// <summary>
    /// Start the blend of 2 cameras
    /// </summary>
    /// <param name="mainCamera">The camera that will take the Render Texture of the other camera</param>
    /// <param name="_targetCamera">The camera we will use the Render Texture</param>
    void StartBlend(Camera mainCamera, Camera _targetCamera)
    {
        if (!_fullScreenCamera) return;
        if (!_blendCamera)
        {
            _blendCamera = gameObject.GetComponent<BlendCamera>();
            if (!_blendCamera)
                _blendCamera = gameObject.AddComponent<BlendCamera>();
        }


        // blendCamera.Reset();

        _blendCamera.StartBlend(mainCamera, _targetCamera);
    }

    /// <summary>
    /// Update the near clip and the far clip of the FullScreen Camera
    /// </summary>
    void UpdateClip()
    {
        if (!_fullScreenCamera) return;

        float distance = Vector3.Distance(_targetBoundCenter, _fullScreenCamera.transform.position);

        if (distance - _clipRange <= 0)
            _fullScreenCamera.nearClipPlane = 1;
        else
            _fullScreenCamera.nearClipPlane =
                MathF.Abs(distance - _clipRange);

        _fullScreenCamera.farClipPlane =
            MathF.Abs(distance + _clipRange);
    }

    /// <summary>
    /// Update the Field Of View of the FullScreen Camera
    /// </summary>
    void UpdateFieldOfView()
    {
        if (!_fullScreenCamera) return;

        _fullScreenCamera.fieldOfView = _fieldOfView;
    }

    /// <summary>
    /// Create the FullScreen Camera, set the target and set the camera settings
    /// </summary>
    void CreateCamera()
    {
        bool value = SetTarget();

        if (!value)
        {
            Debug.LogError($"{name} => Can't create camera because can't find target Renderer or target rectTransform");
            return;
        }

        SetTargetCenter();

// #if UNITY_EDITOR
        if (_fullScreenCamera && !Application.isPlaying)
            DestroyImmediate(_fullScreenCamera.gameObject);
// #endif

        if (_fullScreenCamera && Application.isPlaying)
            DestroyFullScreenCamera();

        _fullScreenCamera = (Camera) new GameObject().AddComponent(typeof(Camera));

        if (Camera.main)
            _fullScreenCamera.depth = Camera.main.depth + 1;

        _fullScreenCamera.cullingMask = _layerMask;
        _fullScreenCamera.fieldOfView = _fieldOfView;
        _fullScreenCamera.name = $"{name}_FullScreenCamera";

        Transform fullScreenTransform = _fullScreenCamera.transform;

        if (_targetObjectRectTransform)
            fullScreenTransform.SetParent(_targetObjectRectTransform.transform);
        else
            fullScreenTransform.SetParent(_targetObjectRenderer.transform);

        if (_spawnSetup)
        {
            Debug.Log("Spawn Setup True");
            fullScreenTransform.localPosition = new Vector3(_spawnCamera.x, _spawnCamera.y, _spawnCamera.z);
        }
        else
        {
            Debug.Log("Spawn Setup False");

            fullScreenTransform.position = _targetBoundCenter;
        }
        
       

        fullScreenTransform.LookAt(_targetBoundCenter);
        _fullScreenCamera.GetUniversalAdditionalCameraData().renderPostProcessing = true;

        _fullScreenCamera.gameObject.AddComponent<IgnoreFog>();

        UpdateClip();
        
        if(Application.isPlaying)
            OnCameraCreate?.Invoke(_fullScreenCamera);
    }

    void DestroyFullScreenCamera()
    {
        if (_fullScreenCamera)
        {
            Destroy(_fullScreenCamera.gameObject);
            OnCameraDestroy?.Invoke();
        }
    }

    bool SetTarget()
    {
        if (!_targetObjectRectTransform)
            _targetObjectRectTransform = GetComponent<RectTransform>();

        if (!_targetObjectRectTransform)
            _targetObjectRenderer = GetComponent<Renderer>();

        if (!_targetObjectRenderer && !_targetObjectRectTransform)
            return false;
        return true;
    }

    void SetTargetCenter()
    {
        if (_targetObjectRectTransform)
        {
            //TODO Button 
            Vector3 center = _targetObjectRectTransform.rect.center;
            _targetBoundCenter = _targetObjectRectTransform.position + center;
        }
        else
        {
            _targetBoundCenter = _targetObjectRenderer.bounds.center;
            Collider colliderComponent = GetComponent<Collider>();
            if (!colliderComponent)
                gameObject.AddComponent<BoxCollider>();
        }
    }
}