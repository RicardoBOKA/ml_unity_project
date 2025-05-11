using System;
using UnityEngine;

public class GrabDetector : MonoBehaviour
{
    public ArmController armController;
    public bool isLeftArm;
    private float contactTimer;
    private Rigidbody currentRb;

    // ######## Testes ########
    private Renderer cubeRenderer;
    private Color defaultMaterial;
    private bool isTouching = false;
    // #########

// ######## Testes ########
    // void Start()
    // {
    //     cubeRenderer  = _goal.GetComponent<Renderer>();
    //     Debug.Log("Renderer component found: " + cubeRenderer .name);
    //     if (cubeRenderer  != null)
    //     {
    //         defaultMaterial = cubeRenderer .material.color;
    //     }
    //     else
    //     {
    //         Debug.LogError("Renderer not found on cubeToMove!");
    //     }
    // }

    void Update()
    {        
        // Debug.Log($"isTouchingCube ARHAAAAAA: {isTouchingCube()}");
    }


// ######## Testes ########
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            currentRb = other.attachedRigidbody;
            // ######## Testes ########
            isTouching = true;
            // ######## Testes ########

            // Change la couleur du cube
            cubeRenderer = other.GetComponent<Renderer>();
            Debug.Log($"Touched object: {other.name}");
            if (cubeRenderer != null)
            {
                defaultMaterial = cubeRenderer.material.color;
                cubeRenderer.material.color = Color.green;            
                // Debug.Log($"Renderer found on: {cubeRenderer.name}");
            }
            else
            {
                Debug.LogError("Renderer introuvable sur l'objet détecté.");
            }


            armController.OnArmContact(isLeftArm, true, currentRb);
            // Debug.Log("Objet attrapé !");

            if (isLeftArm)
                armController.BlockPincer(1);
            else
                armController.BlockPincer(2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Goal") && other.attachedRigidbody == currentRb)
        {
            // Debug.Log($"Trigger quitté avec : {other.gameObject.name}");
            contactTimer = 0f;
            currentRb = null;
            // ######## Testes ########
            isTouching = false;
            // ######## Testes ########

            // Restaure la couleur d’origine
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = defaultMaterial;
                // Debug.Log("Couleur du cube restaurée.");
                cubeRenderer = null;
            }  else
            {
                Debug.LogError("Renderer introuvable lors de la restauration de la couleur.");
            }


            armController.OnArmContact(isLeftArm, false, null);
            // Debug.Log("Objet relâché.");

            if (isLeftArm)
                armController.UnblockPincer(1);
            else
                armController.UnblockPincer(2);
        }
    }
    public bool isTouchingCube()
    {
        return isTouching;
    }

    internal void ResetTouchState()
    {
        this.isTouching = false;
    }
}