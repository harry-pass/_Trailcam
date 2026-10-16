using UnityEngine;

public class Billboard : MonoBehaviour
{
    public enum BillboardType { LookAtCamera, CameraForward }

    [SerializeField] private BillboardType billboardType = BillboardType.CameraForward;
    [SerializeField] private bool lockYAxis = false; // stay upright, only rotate around Y

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null) return;
        }

        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                // Direction pointing AWAY from the camera, so the quad's -Z face looks at it
                Vector3 dir = transform.position - mainCam.transform.position;
                if (lockYAxis) dir.y = 0;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(dir);
                break;

            case BillboardType.CameraForward:
                Vector3 fwd = mainCam.transform.forward;
                if (lockYAxis) fwd.y = 0;
                if (fwd.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(fwd);
                break;
        }
    }
}
