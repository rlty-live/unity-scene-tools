using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class RLTYMonoBehaviour : RLTYMonoBehaviourBase
{
    public abstract void EventHandlerRegister();
    public abstract void EventHandlerUnRegister();

    public virtual void Start() => EventHandlerRegister();
    public virtual void OnDestroy()
    {
        EventHandlerUnRegister();
    }
}

public abstract class RLTYMonoBehaviourBase : JMonoBehaviour
{
    [Space(10)]
    [SerializeField, PropertyOrder(30), LabelWidth(90)]
    protected bool showUtilities;

    [Button, PropertyOrder(31)]
    [HorizontalGroup("debug", MinWidth = 100, MaxWidth = 100)]
    public virtual void CheckSetup() { }

    [SerializeField, ShowIf("showUtilities", true), ReadOnly]
    [PropertyOrder(32), HorizontalGroup("debug", LabelWidth = 90)]
    protected bool correctSetup;

    //To be deprecateds
    [Tooltip("To be deprecated, won't always deactivate logs")]
    [SerializeField, ShowIf("showUtilities", true), PropertyOrder(33)]
    [HorizontalGroup("debug", VisibleIf = "showUtilities", LabelWidth = 20)]
    protected bool debug = true;

    protected bool slowTrigger;

    #region ToolBox
    [ExecuteAlways]
    public virtual IEnumerator TemporaryBoolSwitch(int duration)
    {
        slowTrigger = true;
        yield return new WaitForSeconds(duration);

        slowTrigger = false;
        yield return null;
    }

    [ExecuteAlways]
    public static void DestroyEditorSafe(Component component)
    {
#if UNITY_EDITOR
            if (Application.isPlaying)
                Destroy(component);
            else
                DestroyImmediate(component, true);
#else
                Destroy(component);
#endif
    }
    [ExecuteAlways]
    public static void DestroyEditorSafe(GameObject gameObject)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            Destroy(gameObject);
        else
            DestroyImmediate(gameObject);
#else
                Destroy(gameObject);
#endif
    }

    /// <summary>
    /// RLTY's overload of Unity's Debug function, only activated in Debug build, inEditor display depends on the inherited "debug" boolean.
    /// </summary>
    /// <param name="message"> Message to be displayed in the console </param>
    /// <param name="context"> Identical to context Debug's context paramater</param>
    /// <param name="type"> Unity's Debug.Type</param>
    public virtual void RLTYLog(string message, Object context, LogType type)
    {
#if !UNITY_EDITOR
        if (Debug.isDebugBuild) debug = true;
        else debug = false;
#endif

        string formattedMessage = "<b>[" + context.GetType().ToString() + "]</b>" + message;

        switch (type)
        {
            case LogType.Log:
                if (debug) Debug.Log(formattedMessage, context);

                break;
            case LogType.Error:
                if (debug) Debug.LogError(formattedMessage, context);

                break;
            case LogType.Warning:
                if (debug) Debug.LogWarning(formattedMessage, context);

                break;
            case LogType.Assert:
                Debug.LogError("Assertions are not yet supported, please use the regular unity one");

                break;
            case LogType.Exception:
                Debug.LogError("Exceptions are not yet supported, please use the regular unity one");

                break;

        }
    }

    /// <summary>
    /// RLTY's overload of Unity's Debug function, only activated in Debug build, inEditor display depends on the inherited "debug" boolean.
    /// </summary>
    /// <param name="message"> Message to be displayed in the console </param>
    /// <param name="context"> Identical to context Debug's context paramater</param>
    /// <param name="type"> Unity's Debug.Type</param>
    /// <param name="color"> Color Display in editor </param>
    public virtual void RLTYLog(string message, Object context, LogType type, Color color)
    {
#if !UNITY_EDITOR
        if (Debug.isDebugBuild) debug = true;
        else debug = false;
#endif
        string hexValue = ColorUtility.ToHtmlStringRGBA(color);
        string formattedMessage =
            "<color = " + hexValue + ">" + "<b>" + "[" + context.GetType().ToString() + "]</b> </color>" + message;

        switch (type)
        {
            case LogType.Log:
                if (debug) Debug.Log(formattedMessage, context);

                break;
            case LogType.Error:
                if (debug) Debug.LogError(formattedMessage, context);

                break;
            case LogType.Warning:
                if (debug) Debug.LogWarning(formattedMessage, context);

                break;
            case LogType.Assert:
                Debug.LogError("Assertions are not yet supported, please use the regular unity one");

                break;
            case LogType.Exception:
                Debug.LogError("Exceptions are not yet supported, please use the regular unity one");

                break;
        }
    }
    #endregion
}