using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlendCamera : MonoBehaviour
{
    public UnityAction OnBlendFinish;

    [SerializeField, HideInInspector] private AnimationCurve _opacityCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1.5f, 1));

    private Camera _mainCamera = null;
    private Camera _targetCamera = null;

    private RenderTexture _outTexture = null;

    private Canvas _canvas = null;
    private RawImage _rawImage = null;

    [SerializeField] private float _blendDuration = 1.5f;
    private float _timer = 0;

    private bool _done = true;
    private bool IsValid => _mainCamera && _targetCamera;

    private void Start()
    {
        _blendDuration = _opacityCurve.keys[^1].time;
    }

    void Update()
    {
        if (!_done)
        {
            Blend();
        }
    }

    public void StartBlend(Camera main, Camera target)
    {
        _mainCamera = main;
        _targetCamera = target;

        _canvas = (Canvas) new GameObject().AddComponent(typeof(Canvas));
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        _rawImage = (RawImage) new GameObject().AddComponent(typeof(RawImage));
        _rawImage.transform.SetParent(_canvas.transform);
        _rawImage.rectTransform.anchorMin = Vector2.zero;
        _rawImage.rectTransform.anchorMax = Vector2.one;
        //
        _rawImage.rectTransform.offsetMin = Vector2.zero;
        _rawImage.rectTransform.offsetMax = Vector2.zero;

        if (!_outTexture)
        {
            _outTexture = new RenderTexture(Screen.width, Screen.height, 16);
            Debug.Assert(_outTexture.Create(), "Failed to create camera blending render texture");
        }

        _targetCamera.targetTexture = _outTexture;
        _rawImage.texture = _outTexture;

        _rawImage.CrossFadeAlpha(0, 0, true);

        _done = false;
        _timer = 0;
        // Blend();
    }

    public void Reset()
    {
        _done = true;

        if (_mainCamera)
            _mainCamera.targetTexture = null;

        if (_targetCamera)
            _targetCamera.targetTexture = null;

        _mainCamera = null;
        _targetCamera = null;

        _outTexture = null;

        if (_rawImage)
            Destroy(_rawImage.gameObject);
        if (_canvas)
            Destroy(_canvas.gameObject);

        OnBlendFinish = null;
    }

    void Blend()
    {
        _timer += Time.deltaTime;
        float alpha = _opacityCurve.Evaluate(_timer);
        _rawImage.CrossFadeAlpha(alpha, 0, true);

        if (_timer >= _blendDuration)
        {
            OnBlendFinish?.Invoke();
            OnBlendFinish = null;
            _done = true;
        }
    }
}