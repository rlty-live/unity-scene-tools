using Cinemachine;
using UnityEngine;

public static class ZoomUtility
{
    /**
    Set up the camera to position and focus on the target object
    @param cam - the camera object to set up
    @param target - the target object to focus on
    @return Vector3 - the final position of the camera
    */
    // public static Vector3 CalculateCameraPosition(CinemachineVirtualCamera virtualCam, Transform target, float screenPercent = 1)
    // {
    //     float objectHeight = 0;
    //     float objectWidth = 0;
    //     float objectDepth = 0;
    //
    //     if (target.TryGetComponent(out Renderer renderer))
    //     {
    //         Bounds bounds = renderer.bounds;
    //         Vector3 objectSize = bounds.size;
    //         objectWidth = objectSize.x;
    //         objectHeight = objectSize.y;
    //
    //         if (objectWidth > objectHeight && (objectWidth - objectHeight) >= 3)
    //             objectHeight = CalculateLargestDiagonal(bounds);
    //
    //         objectHeight /= screenPercent;
    //         
    //         objectDepth = objectSize.z;
    //     }
    //     else if (target.TryGetComponent(out RectTransform rectTransform))
    //     {
    //         Rect rect = rectTransform.rect;
    //         Vector3 lossyScale = rectTransform.lossyScale;
    //         objectWidth = rect.width * lossyScale.x;
    //         objectHeight = rect.height * lossyScale.y;
    //         
    //         if (objectWidth > objectHeight && (objectWidth - objectHeight) >= 3)
    //             objectHeight = CalculateLargestDiagonal(rectTransform);
    //
    //         objectHeight /= screenPercent;
    //     }
    //
    //     Transform virtualCamTransform = virtualCam.transform;
    //     
    //     // Set the camera's distance to the object to the calculated distance plus half of object depth
    //     float theoreticalDistance = objectHeight / (2 * Mathf.Tan(virtualCam.m_Lens.FieldOfView * Mathf.Deg2Rad / 2));
    //     virtualCamTransform.position =
    //         target.position - virtualCamTransform.forward * (theoreticalDistance + (objectDepth != 0 ? objectDepth / 2 : 0));
    //     virtualCam.LookAt = target;
    //     
    //     return virtualCamTransform.position;
    // }

    public static Vector3 CalculateCameraPosition(CinemachineVirtualCamera virtualCam, Transform target, float fov, float screenPercent = 1)
    {
        //Debug.Log("Zoom : " + target.gameObject.name + " FOV = "+fov+" ScreenPercent = "+screenPercent);
        
        if(target.TryGetComponent(out MeshRenderer meshRenderer))
            FitCameraToObject3D(virtualCam, target.gameObject, fov, screenPercent);
        else
            FitCameraToObjectUI(virtualCam, target.gameObject, fov, screenPercent);

        return Vector3.zero;
    }
    
    public static void FitCameraToObject3D(CinemachineVirtualCamera camera, GameObject target, float fov, float zoomFactor) {
        BoxCollider collider = target.GetComponent<BoxCollider>();
        if (collider == null) {
            Debug.LogError("Target object must have a BoxCollider component");
            return;
        }

        // Calculate the center and size of the object's bounding box
        Vector3 center = collider.bounds.center;
        Vector3 size = collider.bounds.size;

        // Calculate the distance between the camera and the object
        float distance = CalculateOptimalDistance(camera, size, fov, zoomFactor);
        float depth = size.z / 2;
        distance += depth;

        // Position the camera to face the object's center
        camera.transform.position = center - target.transform.forward * distance;

        // Look at the object's center
        camera.transform.LookAt(center);
        camera.transform.rotation = target.transform.rotation;    }
    
    public static void FitCameraToObjectUI(CinemachineVirtualCamera camera, GameObject target, float fov, float zoomFactor) {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        if (rectTransform == null) {
            Debug.LogError("Target object must have a RectTransform component");
            return;
        }

        // Calculate the center and size of the object's bounding box
        Vector3 center = rectTransform.TransformPoint(rectTransform.rect.center);
        Vector3 size = rectTransform.rect.size * rectTransform.lossyScale;

        // Calculate the distance between the camera and the object
        float distance = CalculateOptimalDistance(camera, size, fov, zoomFactor);

        // Position the camera to face the object's center
        camera.transform.position = center - target.transform.forward * distance;
        camera.transform.rotation = target.transform.rotation;

        // Look at the object's center
        camera.transform.LookAt(center);
    }
    
    private static float CalculateOptimalDistance(CinemachineVirtualCamera camera, Vector3 size, float fov, float zoomFactor)
    {
        //Debug.Log("Zoom : size = " + size + " FOV = "+fov+" ScreenPercent = "+zoomFactor);
        
        // Calculate the distance between the camera and the object
        float halfFOV = fov * 0.5f * Mathf.Deg2Rad;
        
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;
        float aspect = screenWidth / screenHeight;
        float height = size.y;
        float width = size.x / aspect;
        //Debug.Log("halfFOV = "+ halfFOV + " aspect = "+aspect+" screen height = "+screenHeight+" screen width = "+screenWidth);
        

        // get the larger dimension (height or width)
        float maxDimension = Mathf.Max(height, width);
        maxDimension /= zoomFactor;
        
        //Debug.Log("Zoom : maxdimension = "+maxDimension);
        // use the larger dimension to calculate distance
        float distance = maxDimension / (2 * Mathf.Tan(halfFOV));
        return distance;
    }
    
    static float CalculateLargestDiagonal(Bounds bounds) {
        Vector3 size = bounds.size;
        return Mathf.Sqrt(Mathf.Pow(size.x, 2) + Mathf.Pow(size.y, 2) + Mathf.Pow(size.z, 2));
    }

    static float CalculateLargestDiagonal(RectTransform rectTransform)
    {
        Vector2 size = rectTransform.rect.size * rectTransform.lossyScale;
        return Mathf.Sqrt(Mathf.Pow(size.x, 2) + Mathf.Pow(size.y, 2));
    }
}