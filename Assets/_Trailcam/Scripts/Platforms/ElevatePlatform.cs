using UnityEngine;

public class ElevatePlatform : MonoBehaviour
{
    [SerializeField] float MaxElevationHeight = 15f;
    [SerializeField] float MinElevationHeight = 0f;
    [SerializeField] float Speed = 1f;

    private bool movingUp = true;

    void Update()
    {
        // 1. Move the platform based on the current direction
        if (movingUp)
        {
            transform.position += Vector3.up * Speed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.down * Speed * Time.deltaTime;
        }

        // 2. Check boundaries and reverse direction
        if (transform.position.y >= MaxElevationHeight)
        {
            // Clamp position to avoid overshooting and switch direction
            transform.position = new Vector3(transform.position.x, MaxElevationHeight, transform.position.z);
            movingUp = false;
        }
        else if (transform.position.y <= MinElevationHeight)
        {
            // Clamp position to avoid undershooting and switch direction
            transform.position = new Vector3(transform.position.x, MinElevationHeight, transform.position.z);
            movingUp = true;
        }
    }

}

//void Update()
//{
//    while (transform.position.y < ElevationHeight)
//    {
//        transform.position += new Vector3(0, 0.000000000001f, 0) * Time.deltaTime;
//    }
//}
// Cant do this, while loop inside Update() causes infinite loop