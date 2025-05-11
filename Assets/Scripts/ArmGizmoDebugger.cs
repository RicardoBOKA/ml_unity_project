using UnityEngine;

public class ArmGizmoDebugger : MonoBehaviour
{
    [SerializeField] private ArmController armController;
    [SerializeField] private CubeRandomSpawner goal;
    private string environmentName;

    void Start()
    {
        // Récupérer le nom de l'environnement
        environmentName = transform.root.name;
    }

    void Update()
    {
        // if (armController == null || goal == null) return;

        // // Calculer la distance entre bone8 et le cube
        // Vector3 bone8Position = armController.GetBone8Position();
        // Vector3 cubePosition = goal.getPosition();
        // float distanceBone8ToCube = Vector3.Distance(bone8Position, cubePosition);

        // // Afficher la distance dans la console
        // Debug.Log($"[{environmentName}] Distance bone8 - cube : {distanceBone8ToCube:F3}");
    }

    void OnDrawGizmos()
    {
        if (armController == null || goal == null) return;
        Gizmos.color = Color.red;

        // Ligne entre bone8 et le cube
        Gizmos.DrawLine(armController.GetBone8Position(), goal.getPosition());

        Gizmos.color = Color.green;
        Gizmos.DrawLine(armController.GetPincerPositions()[0],  goal.getPosition());
        Gizmos.DrawLine(armController.GetPincerPositions()[1], goal.getPosition());

        Gizmos.color = Color.blue;
        Vector3 centerPincers = (armController.GetPincerPositions()[0] + armController.GetPincerPositions()[1]) / 2f;
        Gizmos.DrawLine(centerPincers, goal.getPosition());

        // Gizmos.color = Color.white;
        // Gizmos.DrawLine(armController.GetBone2Position(), goal.getPosition());

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(armController.GetBone4Position(), goal.getPosition());

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(armController.GetBone7Position(), goal.getPosition());

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(armController.GetBone8Position(), goal.getPosition());


        
        // Vector3[] bonePositions = armController.GetAllBonesAndPincersPositions(); // Assurez-vous que cette méthode existe
        // for (int i = 0; i < bonePositions.Length - 1; i++)
        // {
        //     Gizmos.DrawLine(bonePositions[i], goal.getPosition());
        // }
    }
}