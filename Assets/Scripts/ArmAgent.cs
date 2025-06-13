using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Unity.VisualScripting;
using Unity.Barracuda;

public class ArmAgent : Agent
{
    [SerializeField] private ArmController _armController;
    [SerializeField] private CubeRandomSpawner _goal;
    [SerializeField] private RedPlaneOnArmContact redPlaneOnArmContact;
    [SerializeField] private GrabDetector _grabDetectorLeft;
    [SerializeField] private GrabDetector _grabDetectorRight;
    // [SerializeField] private Transform _goal;
    [SerializeField] private float _mooveSpeed = 1.5f;
    [SerializeField] private float _rotationSpeed = 180f;
    [SerializeField] private float _previousDistanceToCube = float.MinValue;
    private Renderer _renderer; //Utilisé par exemple pour l'interaction entre le bras et son environnement (ex : changer la couleur du bras quand il interagir avec son environnement)
    [HideInInspector] public int CurrentEpisode = 0;
    // private int _maxEpisodes = 1000; // Nombre d'épisodes maximum
    [HideInInspector] public float CumulativeReward = 0f; // Récompense cumulée pour l'épisode en cours
    private Color _defaultMaterial;
    private float timeTouchingPlane = 0f;
    private float maxTouchTime = 5f; // ou une autre valeur selon tes essais



    public override void Initialize()
    {
        // Initialisation de l'agent
        // armController = GetComponent<ArmController>();
        // Debug.Log("Initialize()");
        _renderer = redPlaneOnArmContact.GetComponent<Renderer>();
        CurrentEpisode = 0;
        CumulativeReward = 0f;
        _defaultMaterial = _renderer.material.color;
    }

    public override void OnEpisodeBegin()
    {
        // Reset de la scène et du bras au début de chaque épisode
        CurrentEpisode++;
        CumulativeReward = 0f;
        _defaultMaterial = _renderer.material.color;

        if (_grabDetectorLeft != null)
        {
            _grabDetectorLeft.ResetTouchState();
        }
        if (_grabDetectorRight != null)
        {
            _grabDetectorRight.ResetTouchState();
        }

        SpawnObjects();

        // Reinitialize the distance tracking so that the first reward
        // of the episode is computed from the current cube position
        // and not from the previous episode's state.
        Vector3 pincerCenter = (_armController.GetPincerPositions()[0] + _armController.GetPincerPositions()[1]) / 2f;
        _previousDistanceToCube = Vector3.Distance(pincerCenter, _goal.getPosition());
    }

    private void SpawnObjects()
    {
        //###########################################
        //Spawn du _goal (cube, plages possibles de spawn, etc.) 
        _goal.MoveCubeToRandomPosition();
        //###########################################
        //Reinitialisation des angles du bras
        _armController.resetArm();
        //###########################################
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float maxDist = 23f;
        // --- Position du cube normalisée (-8 à 8)
        float goalPosX = _goal.GetLocalPosX() / 8f; // Normalisation de la position du cube (9 car dans CubeRandomSpawner la range d'apparition est de 9f)
        float goalPosZ = _goal.GetLocalPosZ() / 8f; // Normalisation de la position du cube
        sensor.AddObservation(goalPosX);            // Position du cube sur l'axe X
        sensor.AddObservation(goalPosZ);            // Position du cube sur l'axe Z

        //Informations sur les angles des articulations du bras

        // ###### Je l'ai mis en commentaire car je ne sais pas si c'est utile de le garder pour l'apprentissage (c'est le bout du bras qui glisse)
        // // --- Position de bone2 sur Z (normalisée entre -0.008 et 0.008)
        // float bone2Z = _armController.GetBone2Position().z / 0.008f;
        // sensor.AddObservation(bone2Z);

        //Dans armController bone 1 et 7 sont un peu plus complexes, ce sont des objets de classe Quaternion, il faut donc les convertir en angles (en degrés) pour les normaliser (fait dans armController)
        float bone1AngleY = _armController.GetBone1RotationY() / 360f * 2 - 1; //Y parce que c'est l'axe de rotation posible
        float bone7AngleY = _armController.GetBone7RotationY() / 360f * 2 - 1;
        sensor.AddObservation(bone1AngleY);
        sensor.AddObservation(bone7AngleY);
        // --- Angles normalisés : Mathf.InverseLerp(a, b, value) => (value - a) / (b - a)
        float bone3Norm = Mathf.InverseLerp(120f, 250f, _armController.GetBone3Angle());
        float bone4Norm = Mathf.InverseLerp(-150f, 150f, _armController.GetBone4Angle());
        float bone6Norm = Mathf.InverseLerp(-60f, 60f, _armController.GetBone6Angle());
        // float bone7Norm = Mathf.InverseLerp(-180f, 180f, _armController.GetBone7Angle()); pas sur
        sensor.AddObservation(bone3Norm);
        sensor.AddObservation(bone4Norm);
        sensor.AddObservation(bone6Norm);
        // sensor.AddObservation(bone7Norm); pas sur

        // --- A quel point les pinces sont rapprochées (normalisée) je suis pas sur de la pertinence 
        // Vector3 pincers = _armController.GetPincerHold();
        // float pincer1Norm = Mathf.InverseLerp(0.005f, 0.008f, pincers.x);
        // float pincer2Norm = Mathf.InverseLerp(-0.008f, -0.005f, pincers.z);
        // sensor.AddObservation(pincer1Norm);
        // sensor.AddObservation(pincer2Norm);
        // #### Distance entre la pince, les bones : 8, 7 4 et le cube
        // Vector3 goalPos = _goal.getPosition();
        float distancePincer1ToCube = Vector3.Distance(_armController.GetPincerPositions()[0], _goal.getPosition()) / maxDist; // Normalisé
        float distancePincer2ToCube = Vector3.Distance(_armController.GetPincerPositions()[1], _goal.getPosition()) / maxDist; // Normalisé
        // Debug.Log("#############################################################");
        // Debug.Log($"(Env {_goal.getEnvironmentName()})");
        // Debug.Log("-----------------------------------------");
        
        Vector3 pincerCenterLocal = (_armController.GetPincerLocalPositions_Env(_armController.GetEnvironmentTransform())[0] + _armController.GetPincerLocalPositions_Env(_armController.GetEnvironmentTransform())[1]) / 2f;

        sensor.AddObservation(pincerCenterLocal.x / 13.5f);
        sensor.AddObservation(pincerCenterLocal.y / 11f);
        sensor.AddObservation(pincerCenterLocal.z / 12f);

        //Direction (vers ou se situe le cube) => (1,0,0) vers lavant sur l'axe x
        Vector3 pincerCenter = (_armController.GetPincerPositions()[0] + _armController.GetPincerPositions()[1]) / 2f;
        Vector3 directionToCube = (_goal.getPosition() - pincerCenter).normalized;
        sensor.AddObservation(directionToCube.x);
        sensor.AddObservation(directionToCube.z);

        // Debug.Log("Localposition " + _armController.GetPincerLocalPositions_Env(_armController.GetEnvironmentTransform())[0]);
        // Debug.Log("Localposition " + _armController.GetPincerLocalPositions_Env(_armController.GetEnvironmentTransform())[1]);
        // Debug.Log("#############################################################");

        float distanceBone8ToCube = Vector3.Distance(_armController.GetBone8Position(), _goal.getPosition()) / maxDist;
        float distanceBone7ToCube = Vector3.Distance(_armController.GetBone7Position(), _goal.getPosition()) / maxDist;
        float distanceBone4ToCube = Vector3.Distance(_armController.GetBone4Position(), _goal.getPosition()) / (maxDist - 5f);
        // Debug.Log($"(Env {_goal.getEnvironmentName()}) : Distances bones 8, 7 et 4 + pincers : [{distanceBone8ToCube} | {distanceBone7ToCube} | {distanceBone4ToCube} || {distancePincer1ToCube} | {distancePincer2ToCube}]");


        sensor.AddObservation(distanceBone4ToCube);
        sensor.AddObservation(distanceBone7ToCube);
        sensor.AddObservation(distanceBone8ToCube);

        sensor.AddObservation(distancePincer1ToCube);
        sensor.AddObservation(distancePincer2ToCube);

        // ### Est-ce que la pince touche le sol ou un mur....
        bool isTouching = redPlaneOnArmContact.GetIsTouching();
        sensor.AddObservation(isTouching ? 1f : 0f);

        // --- Est-ce que le cube est attrapé ?
        // sensor.AddObservation(_armController.getIsHoldingBox() ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Convertir les actions de l'agent en mouvements
        MoveAgent(actions.DiscreteActions);

        // ### PENALITÉS
        // --- Pénalité pour chaque pas de temps (pour encourager l'agent à agir rapidement), il pourrait juste rien faire et attendre la fin de l'épisode
        AddReward(-2f / MaxStep);
        // --- Pénalité si contact avec le sol ou mur
        // PlaneTuched();
        PlaneTouchedProgressive();

        // ### RÉCOMPENSES
        // --- Récompense intermédiaire : se rapprocher du cube
        CenterPincerReward();
        // --- Récompense si le cube est bien tenu et levé
        // TimeHoldingReward();
        // --- Récompense si le cube est attrapé
        // GoalReached();
        // Si on touche le cube 
        GoalTouched();

        CumulativeReward = GetCumulativeReward();
    }

    private bool IsCubeTouched()
    {
        return (_grabDetectorLeft != null && _grabDetectorLeft.isTouchingCube()) ||
            (_grabDetectorRight != null && _grabDetectorRight.isTouchingCube());
    }
    void GoalTouched() {
        // Debug.Log("TEST DEHORS");
        // Debug.Log($"grabdetector null ? : {_grabDetector == null}");
        // Debug.Log("Droit TOUCHE ? : " + _grabDetectorLeft.isTouchingCube());
        // Debug.Log("Gauche TOUCHE ? : " + _grabDetectorRight.isTouchingCube());
        if (IsCubeTouched())
        {
            Debug.Log("TEST DEDANS");
            AddReward(15f);
            EndEpisode();
        }
    }

    public void CenterPincerReward()
    {
        // Tolerence entre distance actuelle et distance précédente + la distance max
        float tolerance = 0.2f;
        float maxDist = 23f;

        // Calculer la distance entre le centre des pinces et le centre du cube
        Vector3 pincerCenter = (_armController.GetPincerPositions()[0] + _armController.GetPincerPositions()[1]) / 2f;
        float distanceToCube = Vector3.Distance(pincerCenter, _goal.getPosition());

        // Vérifier si la distance a diminué par rapport à la précédente
        if (Mathf.Abs(distanceToCube - _previousDistanceToCube) > tolerance) {
            if (distanceToCube < _previousDistanceToCube)
            {   
                // Si on se rapproche, on ajoute une récompense
                // Debug.Log("RAPPROCHED : Distance to cube : " + distanceToCube);
                float normalizedDistance = 1f - Mathf.Clamp01(distanceToCube / maxDist);
                AddReward(normalizedDistance * 0.030f);
            }
            else if (distanceToCube > _previousDistanceToCube) {
                // Si on s'éloigne, on ajoute une pénalité
                // Debug.Log("ELLOIGNED : Distance to cube : " + distanceToCube);
                float normalizedDistance = 1f - Mathf.Clamp01(distanceToCube / maxDist);
                AddReward(-normalizedDistance * 0.05f);
            } 
            _previousDistanceToCube = distanceToCube;
        }
    }
    public void TimeHoldingReward()
    {
        // Récompense pour le temps de maintien du cube
        if (_armController.getIsHoldingBox() && _goal.getPosition().y > -4f)
        {
            AddReward(0.05f); // maintenir l’objet en l'air
        }
    }
    public void PlaneTuched()
    {
        // Pénalité si le bras touche le sol ou un mur
        if (redPlaneOnArmContact.GetIsTouching())
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }
    public void PlaneTouchedProgressive()
    {
        if (redPlaneOnArmContact.GetIsTouching())
        {
            timeTouchingPlane += Time.deltaTime;

            // Petite pénalité continue
            float penalty = -0.08f * timeTouchingPlane;
            AddReward(penalty * Time.deltaTime);

            if (timeTouchingPlane >= maxTouchTime)
            {
                AddReward(-1f); // ou autre pénalité finale
                EndEpisode();
            }
        }
        else
        {
            // AddReward(0.0005f * Time.deltaTime);
            // Si plus en contact, on remet à zéro
            timeTouchingPlane = 0f;
        }
    }


    public void GoalReached()
    {
        // Récompense si le cube est attrapé
        if (_armController.getIsHoldingBox())
        {
            AddReward(1f);
            Debug.Log("Goal Reached !");
            EndEpisode();
        }
    }

    private void MoveAgent(ActionSegment<int> actions)
    {
        // Bone1 - rotation Y
        if (actions[0] == 1) _armController.moveBone1(false);
        else if (actions[0] == 2) _armController.moveBone1(true);

        // ### On va négliger le mouvement de bone2 pour l'instant, on va se concentrer sur les rotations des autres bones
        // // Bone2 - déplacement Z
        // if (actions[1] == 1) _armController.MoveBone2Z(false);
        // else if (actions[1] == 2) _armController.MoveBone2Z(true);

        // Bone3 - rotation X
        if (actions[1] == 1) _armController.RotateBone3(true);
        else if (actions[1] == 2) _armController.RotateBone3(false);

        // Bone4 - rotation X
        if (actions[2] == 1) _armController.RotateBone4(true);
        else if (actions[2] == 2) _armController.RotateBone4(false);

        // Bone6 - rotation X
        if (actions[3] == 1) _armController.RotateBone6(true);
        else if (actions[3] == 2) _armController.RotateBone6(false);

        // Bone7 - rotation Y
        if (actions[4] == 1) _armController.RotateBone7(false);
        else if (actions[4] == 2) _armController.RotateBone7(true);

        // Pinces
        if (actions[5] == 1) _armController.ControlPincers(true);
        else if (actions[5] == 2) _armController.ControlPincers(false);
    }    
}
