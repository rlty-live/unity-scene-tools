using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DepthOfField = UnityEngine.Rendering.Universal.DepthOfField;

[Serializable]
public struct FadeSettings
{
    [SerializeField, HideInInspector] private AnimationCurve _focusDistance;
    [SerializeField] private AnimationCurve _focalLength;
    [SerializeField] private AnimationCurve _aperture;

    public AnimationCurve FocusDistance => _focusDistance;
    public AnimationCurve FocalLength => _focalLength;
    public AnimationCurve Aperture => _aperture;

    public FadeSettings(float animationTime)
    {
        _focusDistance = new AnimationCurve(new Keyframe(0, 10),
            new Keyframe(animationTime, 1));

        _focalLength = new AnimationCurve(new Keyframe(0, 50), new Keyframe(animationTime, 300));
        _aperture = new AnimationCurve(new Keyframe(0, 5.6f), new Keyframe(animationTime, 1));
    }

    public FadeSettings(AnimationCurve focalLength, AnimationCurve aperture)
    {
        // _focusDistance = focusDistance; //TODO Pas besoin de la set a la main
        _focusDistance = new AnimationCurve(new Keyframe(0, 10),
            new Keyframe(1.5f, 1));
        _focalLength = focalLength;
        _aperture = aperture;
    }
}

public class FadeCamera : MonoBehaviour
{
    private UnityAction _onCallback = null;
    private UnityAction _onReverseCallBack = null;
    
    [SerializeField] private FadeSettings _fadeSettings = new FadeSettings(1.5f);
    [SerializeField] private float _transitionTime;
    [SerializeField] private bool _pingPong = true;

    private DepthOfField _depthOfField = null;

    private float _time;

    private bool _done = true;
    private bool _doReverse = false;

    public void Reset()
    {
        _done = false;
        _time = 0;
        SetMaxTimeCurve();
    }

    [RuntimeInitializeOnLoadMethod]
    public void RedoFade()
    {
        Reset();
    }

    private void OnEnable()
    {
    }

    protected virtual void Update()
    {
        if (!_done)
            Fade(_onCallback, _onReverseCallBack, _doReverse);

        if (_done && _doReverse)
            ReverseFade();
    }

    void SetMaxTimeCurve()
    {
        _fadeSettings.FocusDistance.keys[^1].time =
            _fadeSettings.FocalLength.keys[^1].time =
                _fadeSettings.Aperture.keys[^1].time = _transitionTime;
    }

    void SetDepthOfField()
    {
        if (!_depthOfField)
        {
            Volume volume = FindObjectOfType<Volume>();

            // VolumeManager.instance.GetVolumes(0);
            if (!volume)
            {
                Debug.LogError("No Volume");
                return;
            }

            VolumeProfile volumeProfile = volume.profile;
            if (!volumeProfile)
            {
                Debug.LogError("No VolumeProfile");
                return;
            }

            List<VolumeComponent> components = volumeProfile.components;
            DepthOfField depthOfField = null;
            foreach (VolumeComponent component in components)
            {
                if (component is DepthOfField)
                {
                    depthOfField = component as DepthOfField;
                    break;
                }
            }

            if (!depthOfField)
            {
                depthOfField = volumeProfile.Add<DepthOfField>();
            }

            _depthOfField = depthOfField;
        }
    }

    public void Fade(UnityAction callback, UnityAction callbackReverse, bool reverse = false)
    {
        if (_onCallback == null)
            _onCallback = callback;
        if (_onReverseCallBack == null)
            _onReverseCallBack = callbackReverse;

        _doReverse = reverse;

        if (_done)
            return;

        Debug.Log(
            $"Keys : {_fadeSettings.FocusDistance.keys[0].inTangent} {_fadeSettings.FocusDistance.keys[0].outTangent} {_fadeSettings.FocusDistance.keys[0].inWeight} " +
            $"{_fadeSettings.FocusDistance.keys[0].outWeight} {_fadeSettings.FocusDistance.keys[0].time}");

        // _doReverse = reverse;

        if (!_depthOfField)
            SetDepthOfField();

        if (_depthOfField.mode != DepthOfFieldMode.Bokeh)
            _depthOfField.mode = new DepthOfFieldModeParameter(DepthOfFieldMode.Bokeh);

        if (!_depthOfField.focusDistance.overrideState)
            _depthOfField.focusDistance.overrideState = true;

        if (!_depthOfField.focalLength.overrideState)
            _depthOfField.focalLength.overrideState = true;

        if (!_depthOfField.aperture.overrideState)
            _depthOfField.aperture.overrideState = true;

        _time += Time.deltaTime;

        _depthOfField.focusDistance.value = _fadeSettings.FocusDistance.Evaluate(_time);
        _depthOfField.focalLength.value = _fadeSettings.FocalLength.Evaluate(_time);
        _depthOfField.aperture.value = _fadeSettings.Aperture.Evaluate(_time);

        if (_time >= _transitionTime)
        {
            _done = true;

            _onCallback?.Invoke();
            _onCallback = null;
            // if (reverse)
            //     _doReverse = true;
        }
    }

    void ReverseFade()
    {
        _time -= Time.deltaTime;

        _depthOfField.focusDistance.value = _fadeSettings.FocusDistance.Evaluate(_time);
        _depthOfField.focalLength.value = _fadeSettings.FocalLength.Evaluate(_time);
        _depthOfField.aperture.value = _fadeSettings.Aperture.Evaluate(_time);

        if (_time <= 0)
        {
            _onReverseCallBack?.Invoke();
            _onReverseCallBack = null;
            _doReverse = false;
        }
    }

    public void OnGUI()
    {
        if (_done) return;
        // if (_texture == null) _texture = new Texture2D(1, 1);

        //
        // _texture.SetPixel(0, 0, new Color(0, 0, 0, _alpha));
        // _texture.Apply();

        _time += Time.deltaTime;
        // _alpha = FadeCurve.Evaluate(_time);
        // _focusDistance = FadeCurve.Evaluate(_time);
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);

        // if (_alpha <= 0)
        // {
        //     _done = true;
        //     //Destroy(this);
        // }
    }
}