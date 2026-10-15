using UnityEngine;

public class billboard : MonoBehaviour
{
    [SerializeField] private BillboardType billboardType;
    public enum BillboardType {   LookAtCamera,     CameraForward};

    private void LateUpdate()
    {
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(Camera.main.transform);
                break;
            case BillboardType.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            default:
                break;
        }
    }
}
