using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Base class containing useful helper methods for all mono behaviours
/// </summary>
public class JMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// Includes component type in logs to make it easier to spot the log source
    /// </summary>
    /// <param name="message"></param>
    protected void JLog(string message)
    {
        JLogBase.Log(message, this);
    }

    protected void JLogWarning(string message)
    {
        JLogBase.LogWarning(message, this);
    }

    protected void JLogError(string message)
    {
        JLogBase.LogError(message, this);
    }
}

public static class JLogBase
{
    public static bool logInRelease = false;
    private static JLogFilter _logFilter;
    private static bool _logFilterLoaded = false;

    public static void Log(string message, Component c)
    {
#if UNITY_EDITOR
        string header = GetLogHeader(c);
        if (!string.IsNullOrEmpty(header))
            Debug.Log(header + message, c);
#elif !UNITY_SERVER
    if (logInRelease || Debug.isDebugBuild)
        Debug.Log(GetLogHeader(c) + message);
#else
        Debug.Log(GetLogHeader(c) + message);
#endif
    }

    public static void Log(string message, Type t)
    {
#if UNITY_EDITOR
        string header = GetLogHeader(t);
        if (!string.IsNullOrEmpty(header))
            Debug.Log(header + message);
#elif !UNITY_SERVER
    if (logInRelease || Debug.isDebugBuild)
        Debug.Log(GetLogHeader(t) + message);
#else
        Debug.Log(GetLogHeader(t) + message);
#endif
    }

    public static void LogWarning(string message, Component c)
    {
#if UNITY_EDITOR
        string header = GetLogHeader(c);
        if (!string.IsNullOrEmpty(header))
            Debug.LogWarning(header + message);
#elif !UNITY_SERVER
    if (logInRelease || Debug.isDebugBuild)
        Debug.LogWarning(GetLogHeader(c) + message);
#else
        Debug.LogWarning(GetLogHeader(c) + message);
#endif
    }
    public static void LogWarning(string message, Type t)
    {
#if UNITY_EDITOR
        string header = GetLogHeader(t);
        if (!string.IsNullOrEmpty(header))
            Debug.LogWarning(header + message);
#elif !UNITY_SERVER
    if (logInRelease || Debug.isDebugBuild)
        Debug.LogWarning(GetLogHeader(t) + message);
#else
        Debug.LogWarning(GetLogHeader(t) + message);
#endif
    }

    public static void LogError(string message, Component c)
    {
        Debug.LogError("[" + c.GetType().ToString() + ":" + c.gameObject.name + "] " + message);
    }
    public static void LogError(string message, Type t)
    {
        Debug.LogError("[" + t.ToString() + "] " + message);
    }

    private static string GetLogHeader(Component c)
    {
#if UNITY_EDITOR
        if (!_logFilterLoaded)
        {
            _logFilterLoaded = true;
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(JLogFilter)));
            if (guids.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                _logFilter = AssetDatabase.LoadAssetAtPath<JLogFilter>(assetPath);
            }
        }
        string header = c.GetType().ToString() + ":" + c.gameObject.name;
        if (_logFilter)
        {
            if (_logFilter.Filter(header, out string color))
                return "<Color=#" + color + ">[" + header + "]</Color> ";
            else
                return null;
        }
        return "<Color=#FFFFFF>[" + header + "]</Color> ";
#elif UNITY_WEBGL
        return "\x1B[32m[" + c.GetType().ToString() + ":" + c.gameObject.name + "]\x1B[30m ";
#else
        return "[" + c.GetType().ToString() + ":" + c.gameObject.name + "] ";
#endif
    }

    private static string GetLogHeader(Type t)
    {
#if UNITY_EDITOR
        if (!_logFilterLoaded)
        {
            _logFilterLoaded = true;
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(JLogFilter)));
            if (guids.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                _logFilter = AssetDatabase.LoadAssetAtPath<JLogFilter>(assetPath);
            }
        }
        string header = t.ToString();
        if (_logFilter)
        {
            if (_logFilter.Filter(header, out string color))
                return "<Color=#" + color + ">[" + header + "]</Color> ";
            else
                return null;
        }
        return "<Color=#FFFFFF>[" + header + "]</Color> ";
#elif UNITY_WEBGL
        return "\x1B[32m[" + t.ToString() + "]\x1B[30m ";
#else
        return "[" + t.ToString() + "] ";
#endif
    }
}

public abstract class JMonoSingleton<T> : JMonoBehaviour where T : JMonoBehaviour
{
    protected static T _onlyInstance = null;
    protected static bool _instanceChecked = false;
    protected static bool _keepInstanceAliveWhenDeactivated = false;

    public static bool Exists
    {
        get
        {
            if (_instanceChecked)
                return _onlyInstance != null && _onlyInstance.enabled && _onlyInstance.gameObject.activeInHierarchy;
            if (_onlyInstance == null)
                _onlyInstance = FindObjectOfType<T>();
            if (_onlyInstance != null)
                _instanceChecked = true;
            return _onlyInstance != null;
        }
    }

    public static T Instance
    {
        get
        {
            //you sure we need this ?
            if (_onlyInstance != null && (!_onlyInstance.enabled || !_onlyInstance.gameObject.activeInHierarchy) && !_keepInstanceAliveWhenDeactivated)
                _onlyInstance = null;
            if (_onlyInstance == null)
            {
                _instanceChecked = true;
                _onlyInstance = FindObjectOfType<T>();
                if (_onlyInstance==null && !_logged)
                {
                    _logged = true;
                    Debug.LogError("Could not locate a " + typeof(T).ToString() + " object. You have to have exactly one in the scene.");
                }
            }
            return _onlyInstance;
        }
    }

    private static bool _logged = false;

    public bool keepAliveWhenDeactivated = false;

    protected virtual void Awake()
    {
        _keepInstanceAliveWhenDeactivated = keepAliveWhenDeactivated;
        //init service
        if (_onlyInstance == null)
        {
            //Debug.Log("Init singleton " + typeof(T));
            _onlyInstance = gameObject.GetComponent<T>();
        }
        else if (_onlyInstance!=this)
        {
            //Debug.LogError("Duplicate singleton: " + gameObject.name + " type=" + GetType());
            DestroyImmediate(gameObject);
        }
            
    }
    protected virtual void OnDisable()
    {
        _onlyInstance = null;
        _instanceChecked = false;
    }
}
