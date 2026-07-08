using UnityEngine;

public class CameraPositionSafety : MonoBehaviour
{
    public Vector3 normalLocalPosition;
    public float maximumDistanceFromNormal = 2f;
    public bool resetEveryFrameIfInvalid = true;

    private void Awake()
    {
        normalLocalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (resetEveryFrameIfInvalid)
            CheckCameraPosition();
    }

    public void CheckCameraPosition()
    {
        Vector3 p = transform.localPosition;

        if (float.IsNaN(p.x) || float.IsNaN(p.y) || float.IsNaN(p.z) ||
            float.IsInfinity(p.x) || float.IsInfinity(p.y) || float.IsInfinity(p.z) ||
            Vector3.Distance(p, normalLocalPosition) > maximumDistanceFromNormal)
        {
            ResetCameraPosition();
        }
    }

    public void ResetCameraPosition()
    {
        transform.localPosition = normalLocalPosition;
    }
}
