using UnityEngine.Events;


public static class ZoomHandlerData
{
    public static UnityAction OnZoomStart;
    public static UnityAction OnZoomEnd;
    public static UnityAction OnUnzoomStart;
    public static UnityAction OnUnzoomEnd;

    public static void ZoomStart() => OnZoomStart?.Invoke();
    public static void ZoomEnd() => OnZoomEnd?.Invoke();

    public static void UnzoomStart() => OnUnzoomStart?.Invoke();
    public static void UnzoomEnd() => OnUnzoomEnd?.Invoke();
}