using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        transform.RotateAround(target.position, Vector3.up, 10 * Time.deltaTime); // Rotate around the target
    }
}