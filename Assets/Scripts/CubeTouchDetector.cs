using UnityEngine;

public class CubeTouchDetector : MonoBehaviour
{
    private int contactCount = 0;
    private bool isTouching = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pincers"))
        {
            contactCount++;
            isTouching = true;
            Debug.Log("Cube touched by: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pincers"))
        {
            contactCount = Mathf.Max(0, contactCount - 1);
            if (contactCount == 0)
                isTouching = false;
        }
    }

    public bool GetIsTouching()
    {
        return isTouching;
    }
}
