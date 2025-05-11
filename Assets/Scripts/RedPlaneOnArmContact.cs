using UnityEngine;

public class RedPlaneOnArmContact : MonoBehaviour
{

    public Renderer rend;
    private Color defaultMaterial;
    private int contactCount = 0;  // Compte combien de colliders "Pincers" touchent le plan
    private bool isTouching = false;

    void Start() {
        rend = GetComponent<Renderer>();
        defaultMaterial = rend.material.color;
        // Debug.Log("Renderer component found: " + rend.name);
        if (rend == null) {
            Debug.LogError("Renderer component not found on this GameObject.");
            return;
        }
    }


    void Update()
    {
        UpdateIsTouching();
        // Debug.Log($"isTouching: {isTouching} | contactCount: {contactCount}");
    }

    private void UpdateIsTouching()
    {
        // Met à jour l'état de contact en fonction de contactCount
        isTouching = contactCount > 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pincers")) {
            contactCount++;
            rend.material.color = Color.red;
            // Debug.Log("Collider entered: " + other.name + " | Total: " + contactCount);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pincers")) {
            // Si le collider est toujours en contact, on ne fait rien ici
            // Debug.Log("Collider is still touching: " + other.name + " | Total: " + contactCount);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pincers")) {
            contactCount = Mathf.Max(0, contactCount - 1); // Évite d'aller dans les négatifs
            // Debug.Log("Collider exited: " + other.name + " | Total: " + contactCount);
            
            if (contactCount == 0)
                rend.material.color = defaultMaterial;
        }
    }

    
    public bool GetIsTouching()
    {
        return isTouching;
    }
}