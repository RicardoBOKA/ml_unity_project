using System;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public Transform armRoot;

    //Les commentaires à verifier
    public Transform bone1; // rotation Y libre
    public Transform bone2; // position Z limitée
    public Transform bone3; // rotation X [-120, 120]
    public Transform bone4; // rotation X [-150, 150]
    public Transform bone6; // rotation X [-60, 60]
    public Transform bone7; // rotation Y [-60, 60]
    public Transform bone8;
    public Transform pincer1; // position X [0.005, 0.008]
    public Transform pincer2; // position X [-0.008, -0.005]

    // Angles de départ de chaque articulation + pinces
    private Quaternion bone1InitialRotation;
    private Quaternion bone7InitialRotation;
    private float bone3Angle = 120f;
    private float bone4Angle = 150f;
    private float bone6Angle = 0f;
    private float bone7Angle = 0f;
    private float pincer1PosX = 0.008f;
    private float pincer2PosX = -0.008f;

    // Vitesse de mouvement
    public float rotationSpeed = 45f; // degrés par seconde
    public float moveSpeed = 0.01f;  // unités par seconde


    [Header("Grab System")]
    public bool alternativeMethod;
    public Rigidbody gripperRigidbody;
    public Rigidbody gripperRigidbodyLeft;
    public Rigidbody gripperRigidbodyRight;
    private Rigidbody boxRigidbody;
    private FixedJoint grabJoint;

    private bool isHoldingBox;
    private bool leftArmContact;
    private bool rightArmContact;
    private float releaseTimer = 0f;
    private float releaseDelay = 0.1f;

    //Parties à decommenter si on veut que les pinces s'arretent lors du contact du cube (évite que les pinces s'enfoncent dans le cube)
    private bool pincer1Blocked = false;
    private bool pincer2Blocked = false;
    
    // public CubeRandomSpawner cubeRandomSpawner;

    void Start()
    {
        bone1InitialRotation = bone1.localRotation;
        bone7InitialRotation = bone7.localRotation;
    }


    public void resetArm(){;
        Vector3 bone2Pos = bone2.localPosition;
        bone2Pos.z = 0f;
        bone2.localPosition = bone2Pos;

        bone3Angle = 120f;
        bone4Angle = 150f;
        bone6Angle = 0f;
        bone7Angle = 0f;
        pincer1PosX = 0.008f;
        pincer2PosX = -0.008f;

        bone1.localRotation = bone1InitialRotation;
        bone3.localRotation = Quaternion.Euler(bone3Angle, 0f, 0f);
        bone4.localRotation = Quaternion.Euler(bone4Angle, 0f, 0f);
        bone6.localRotation = Quaternion.Euler(bone6Angle, 0f, 0f);
        bone7.localRotation = bone7InitialRotation * Quaternion.Euler(0f, bone7Angle, 0f);

        Vector3 posP1 = pincer1.localPosition;
        Vector3 posP2 = pincer2.localPosition;
        posP1.x = pincer1PosX;
        posP2.x = pincer2PosX;
        pincer1.localPosition = posP1;
        pincer2.localPosition = posP2;

        // cubeRandomSpawner.MoveCubeToRandomPosition();
    }


    public void moveBone1(bool forward) {
        if (forward) {
            bone1.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        } else {
            bone1.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
        }
    }
    public void MoveBone2Z(bool forward)
    {
        Vector3 pos = bone2.localPosition;
        float delta = moveSpeed * Time.deltaTime;
        pos.z = Mathf.Clamp(pos.z + (forward ? delta : -delta), -0.008f, 0.008f);
        bone2.localPosition = pos;
    }
    public void RotateBone3(bool forward)
    {
        float delta = rotationSpeed * Time.deltaTime;
        bone3Angle = Mathf.Clamp(bone3Angle + (forward ? delta : -delta), 120f, 250f);
        bone3.localRotation = Quaternion.Euler(bone3Angle, 0f, 0f);
    }

    public void RotateBone4(bool forward)
    {
        float delta = rotationSpeed * Time.deltaTime;
        bone4Angle = Mathf.Clamp(bone4Angle + (forward ? delta : -delta), -150f, 150f);
        bone4.localRotation = Quaternion.Euler(bone4Angle, 0f, 0f);
    }

    public void RotateBone6(bool forward)
    {
        float delta = rotationSpeed * Time.deltaTime;
        bone6Angle = Mathf.Clamp(bone6Angle + (forward ? delta : -delta), -60f, 60f);
        bone6.localRotation = Quaternion.Euler(bone6Angle, 0f, 0f);
    }

    public void RotateBone7(bool forward)
    {
        float delta = rotationSpeed * Time.deltaTime;
        bone7Angle = Mathf.Clamp(bone7Angle + (forward ? delta : -delta), -180f, 180f);
        bone7.localRotation = bone7InitialRotation * Quaternion.Euler(0f, bone7Angle, 0f);
    }

    public void ControlPincers(bool close)
    {
        float delta = moveSpeed * Time.deltaTime;
        if (close)
        {
            if (!pincer1Blocked)
                pincer1PosX = Mathf.Clamp(pincer1.localPosition.x - delta, 0.005f, 0.008f);
            if (!pincer2Blocked)
                pincer2PosX = Mathf.Clamp(pincer2.localPosition.x + delta, -0.008f, -0.005f);
        }
        else
        {
            pincer1PosX = Mathf.Clamp(pincer1.localPosition.x + delta, 0.005f, 0.008f);
            pincer2PosX = Mathf.Clamp(pincer2.localPosition.x - delta, -0.008f, -0.005f);
        }

        Vector3 posP1 = pincer1.localPosition;
        Vector3 posP2 = pincer2.localPosition;
        posP1.x = pincer1PosX;
        posP2.x = pincer2PosX;
        pincer1.localPosition = posP1;
        pincer2.localPosition = posP2;
    }



    void Update()
    {
        // Contrôle de Bone.001 - Rotation sur Y
        if (Input.GetKey("a")){moveBone1(true);}
        if (Input.GetKey("e")){moveBone1(false);}

        // Contrôle de Bone.002 - Position sur Z ### On la néglige pour l'entrainement 
        // --- Bone2 (translation Z)
        if (Input.GetKey("z")) MoveBone2Z(false);
        if (Input.GetKey("s")) MoveBone2Z(true);

        // Bone.003 - Rotation X [-120, 250]
        if (Input.GetKey("r")) RotateBone3(true);
        if (Input.GetKey("t")) RotateBone3(false);

        // Bone.005 - On ommet cette partie pour l'instant
        // todo: ajouter la rotation de bone5

        // Bone.004 - Rotation X [-150, 150]
         // --- Bone4 (rotation X)
        if (Input.GetKey("f")) RotateBone4(false);
        if (Input.GetKey("g")) RotateBone4(true);

        // Bone.006 - Rotation X [-60, 60]
        if (Input.GetKey("c")) RotateBone6(false);
        if (Input.GetKey("v")) RotateBone6(true);

        // Bone.007 - Rotation Y [-60, 60]
        if (Input.GetKey("b")) RotateBone7(false);
        if (Input.GetKey("n")) RotateBone7(true); 

        // Pince - Position X [0.005, 0.008]
        //fermer la pince (les deux s’approchent)
        if (Input.GetKey("u")) ControlPincers(true);
        if (Input.GetKey("i")) ControlPincers(false); 

        if (Input.GetKey("space")){
            resetArm();
        }

        //Pour verifier des choses
        // Debug.Log();

    }

    /// <summary>
    /// Met à jour le comportement physique du contrôleur de bras à un intervalle de temps fixe.
    /// Cette méthode est appelée à chaque frame à taux fixe et est utilisée pour gérer la logique
    /// de maintien ou de libération d'une boîte en fonction de l'état des points de contact du bras.
    /// </summary>
    /// <remarks>
    /// - Si le bras tient une boîte (`isHoldingBox` est vrai) et que le Rigidbody de la boîte n'est pas nul :
    ///   - Si l'un des bras (gauche ou droit) perd le contact avec la boîte (`leftArmContact` ou `rightArmContact` est faux) :
    ///     - Un minuteur de libération (`releaseTimer`) est incrémenté par le temps delta fixe.
    ///     - Si le minuteur dépasse le délai spécifié (`releaseDelay`), la boîte est libérée en appelant `ReleaseBox()`.
    ///   - Sinon, si les deux bras maintiennent le contact :
    ///     - La position de la boîte est mise à jour pour correspondre à la position de la pince.
    ///     - La rotation de la boîte est mise à jour pour correspondre à la rotation de `bone8`.
    ///     - Le minuteur de libération est réinitialisé à 0.
    /// </remarks>
    /// <seealso cref="ReleaseBox"/>
    void FixedUpdate() {
        if (isHoldingBox && boxRigidbody != null)
        {
            if (!leftArmContact || !rightArmContact)
            {
                releaseTimer += Time.fixedDeltaTime;
                if (releaseTimer >= releaseDelay)
                {
                    ReleaseBox();
                }
            }
            else
            {
                boxRigidbody.transform.position = gripperRigidbody.transform.position;
                boxRigidbody.transform.rotation = bone8.rotation;
                releaseTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Gère l'état de contact des bras robotiques et
    /// en fonction des conditions de contact et de la méthode sélectionnée.
    /// </summary>
    /// <param name="isLeftArm">Indique si le contact est avec le bras gauche (true) ou le bras droit (false).</param>
    /// <param name="isContacting">Indique si le bras est actuellement en contact avec un objet (true) ou non (false).</param>
    /// <param name="boxRb">Le Rigidbody de la boîte avec laquelle le bras interagit.</param>
    /// <remarks>
    /// Cette méthode met à jour l'état de contact pour le bras gauche ou droit en fonction des paramètres fournis.
    /// Si les deux bras sont en contact et que le robot ne tient pas déjà la boîte, elle initie le processus de saisie
    /// en utilisant la pince par défaut. Si une méthode alternative est activée, elle permet de saisir avec un seul bras
    /// (gauche ou droit) en fonction du bras en contact.
    /// </remarks>>
    public void OnArmContact(bool isLeftArm, bool isContacting, Rigidbody boxRb) {
        if (isLeftArm)
            leftArmContact = isContacting;
        else
            rightArmContact = isContacting;

        if (leftArmContact && rightArmContact && !isHoldingBox)
        {
            StartGrab(boxRb, gripperRigidbody);
        }

        if (alternativeMethod)
        {
            if (leftArmContact && !rightArmContact)
            {
                StartGrab(boxRb, gripperRigidbodyLeft);
            }

            if (!leftArmContact && rightArmContact)
            {
                StartGrab(boxRb, gripperRigidbodyRight);
            }
        }
    }

    /// <summary>
    /// Initie le processus de saisie d'une boîte en l'attachant à une pince à l'aide d'un FixedJoint.
    /// Cette méthode configure les propriétés du Rigidbody de la boîte et crée un FixedJoint si celui-ci n'existe pas déjà.
    /// </summary>
    /// <param name="boxRb">Le Rigidbody de la boîte à saisir.</param>
    /// <param name="gripperRb">Le Rigidbody de la pince qui tiendra la boîte.</param>
    /// <remarks>
    /// - Configure le Rigidbody de la boîte pour utiliser l'interpolation et la détection de collision continue
    ///   afin d'assurer des interactions physiques plus fluides.
    /// - Définit le Rigidbody de la boîte comme cinématique et désactive la gravité pour éviter tout mouvement non désiré.
    /// - Réinitialise les vitesses linéaire et angulaire de la boîte à zéro.
    /// - Crée un FixedJoint sur la boîte si elle n'en possède pas déjà un, et le connecte au Rigidbody de la pince.
    /// - Configure le FixedJoint avec une force et un couple de rupture infinis, et désactive le prétraitement pour un contrôle précis.
    /// </remarks>
    void StartGrab(Rigidbody boxRb, Rigidbody gripperRb)
    {
        isHoldingBox = true;
        boxRigidbody = boxRb;

        boxRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        boxRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        grabJoint = boxRigidbody.gameObject.GetComponent<FixedJoint>();

        if (!grabJoint)
        {
            grabJoint = boxRigidbody.gameObject.AddComponent<FixedJoint>();
        }

        grabJoint.connectedBody = gripperRb;
        grabJoint.breakForce = Mathf.Infinity;
        grabJoint.breakTorque = Mathf.Infinity;
        grabJoint.enablePreprocessing = false;

        boxRigidbody.linearVelocity = Vector3.zero;
        boxRigidbody.angularVelocity = Vector3.zero;
        boxRigidbody.isKinematic = true;
        boxRigidbody.useGravity = false;
    }

    /// <summary>
        /// Libère la boîte actuellement tenue en désactivant son état cinématique, en activant la gravité,
        /// et en détruisant le joint utilisé pour la maintenir. Réinitialise les variables associées pour indiquer
    /// que la boîte n'est plus tenue.
    /// </summary>
    /// <remarks>
        /// Cette méthode est généralement appelée lorsque l'utilisateur ou le système décide de lâcher la boîte.
        /// Elle garantit que la boîte se comporte naturellement sous la simulation physique après avoir été relâchée.
    /// </remarks>
    void ReleaseBox()
    {
        boxRigidbody.isKinematic = false;
        boxRigidbody.useGravity = true;
        Destroy(grabJoint);
        isHoldingBox = false;
        boxRigidbody = null;
        grabJoint = null;
        releaseTimer = 0f;
    }

    public void BlockPincer(int index)
    {
        if (index == 1) pincer1Blocked = true;
        else if (index == 2) pincer2Blocked = true;
    }

    public void UnblockPincer(int index)
    {
        if (index == 1) pincer1Blocked = false;
        else if (index == 2) pincer2Blocked = false;
    }
    // Getters pour les angles et positions

    public float GetBone1RotationY()
    {
        return bone1.localRotation.eulerAngles.y;
    }
    public float GetBone7RotationY()
    {
        return bone7.localRotation.eulerAngles.y;
    }
    public float GetBone3Angle()
    {
        return bone3Angle;
    }
    public float GetBone4Angle()
    {
        return bone4Angle;
    }
    public float GetBone6Angle()
    {
        return bone6Angle;
    }
    public float GetBone7Angle()
    {
        return bone7Angle;
    }
    //Rapprochement des pinces
    public Vector3 GetPincerHold()
    {
        return new Vector3(pincer1PosX, 0, pincer2PosX);
    }
    public bool getIsHoldingBox() {
        return isHoldingBox;
    }
    //Position des pinces
    // public Vector3[] GetPincerPositions()
    // {
    //     return new Vector3[]
    //     {
    //         pincer1.localPosition, // Toutes les coordonnées de pincer1 (x, y, z)
    //         pincer2.localPosition  // Toutes les coordonnées de pincer2 (x, y, z)
    //     };
    // }

    // public Vector3[] GetPincerPositions()
    // {
    //     return new Vector3[] {
    //         armRoot.InverseTransformPoint(pincer1.position),
    //         armRoot.InverseTransformPoint(pincer2.position)
    //     };
    // }

    public Vector3[] GetPincerPositions()
    {
        return new Vector3[] {
            new Vector3(pincer1.position.x - 0.6f, pincer1.position.y, pincer1.position.z),
            new Vector3(pincer2.position.x - 0.6f, pincer2.position.y, pincer2.position.z)
        };
    }

    public Vector3 GetPincer2Positions()
    {
        return armRoot.InverseTransformPoint(pincer2.position);
    }

    public Vector3 GetBone2Position()
    {
        return bone2.position;
    }


    public Vector3 GetBone4Position()
    {
        return bone4.position;
    }

    public Vector3 GetBone7Position()
    {
        return bone7.position;
    }

    public Vector3 GetBone8Position()
    {
        return bone8.position;
    }

    public Vector3 GetPincer1Position()
    {
        return pincer1.position;
    }

    public Vector3 GetPincer2Position()
    {
        return pincer2.position;
    }


    public Vector3[] GetAllBonesAndPincersPositions()
    {
        return new Vector3[]
        {
            // armRoot.InverseTransformPoint(bone1.position),
            // armRoot.InverseTransformPoint(bone2.position),
            // armRoot.InverseTransformPoint(bone3.position),
            // armRoot.InverseTransformPoint(bone4.position),
            // armRoot.InverseTransformPoint(bone6.position),
            // armRoot.InverseTransformPoint(bone7.position),
            // armRoot.InverseTransformPoint(bone8.position),
            // armRoot.InverseTransformPoint(pincer1.position),
            // armRoot.InverseTransformPoint(pincer2.position)
        };
    }

    bool getLeftArmContact()
    {
        return leftArmContact;
    }
    bool getRightArmContact()
    {
        return rightArmContact;
    }
}