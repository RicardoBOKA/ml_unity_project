using UnityEngine;

public class CubeRandomSpawner : MonoBehaviour
{
    public Transform cubeToMove; // ou on glisse le cube dans l’inspecteur
    public float rangeX = 8f;
    public float rangeZ = 8f;

    public float saufZoneX = 4f;
    public float saufZoneZ = 3f;

    public float height = 0.7f;

    // private Renderer rend;
    // private Color defaultMaterial;
    // private int contactCount = 0;
    // private bool isTouched = false;

    void Start()
    {
        Debug.Log($"cubeToMove: {cubeToMove}");
        MoveCubeToRandomPosition();

        // rend = cubeToMove.GetComponent<Renderer>();
        // if (rend != null)
        // {
        //     defaultMaterial = rend.material.color;
        // }
        // else
        // {
        //     Debug.LogError("Renderer not found on cubeToMove!");
        // }
    }

    void Update()
    {
        // Pour tester : relancer une position aléatoire avec la touche espace
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveCubeToRandomPosition();
        }
        
        // UpdateTouchStatus();
    }

    // private void UpdateTouchStatus()
    // {
    //     isTouched = contactCount > 0;
    //     Debug.Log($"isTouched: {isTouched} | contactCount: {contactCount}");
    // }

//    void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Pincers"))
//         {
//             contactCount++;
//             rend.material.color = Color.green;
//             Debug.Log("Collider entered: " + other.name + " | Total: " + contactCount);
//         }
//     }

//     void OnTriggerStay(Collider other)
//     {
//         if (other.CompareTag("Pincers"))
//         {
//             // Debug.Log("Contact persistant avec le cube : " + other.name);
//         }
//     }

//     void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Pincers"))
//         {
//             contactCount = Mathf.Max(0, contactCount - 1);
//             if (contactCount == 0)
//                 rend.material.color = defaultMaterial;

//             // Debug.Log("Fin de contact avec : " + other.name);
//         }
//     }

//     public bool HasBeenTouched()
//     {
//         return isTouched;
//     }

//     public bool GetIsTouching()
//     {
//         return isTouched;
//     }

    public void MoveCubeToRandomPosition()
    {
        Vector3 newPos;
        int s = 0;
        float spawnHeight = height / 2f + 0.5f; // on le lève un peu au-dessus

        do {
            float randomX = Random.Range(-rangeX, rangeX);
            float randomZ = Random.Range(-rangeZ, rangeZ);
            newPos = new Vector3(randomX, spawnHeight, randomZ);
            s++;
        } while (System.Math.Abs(newPos.x) < saufZoneX && System.Math.Abs(newPos.z) < saufZoneZ && s < 100);

        cubeToMove.localPosition = newPos;
    }

    public float getLocalPosX()
    {
        return cubeToMove.position.x;
    }
    public float getLocalPosZ()
    {
        return cubeToMove.position.z;
    }

    public Vector3 getPosition()
    {
        return cubeToMove.position;
    }

}
