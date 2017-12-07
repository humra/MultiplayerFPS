using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float lookSensitivity = 5f;
    [SerializeField]
    private float thrusterForce = 1000f;
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    [SerializeField]
    private float thrusterFuelAmount = 1f;

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Joint options")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        Cursor.lockState = CursorLockMode.Locked;
        SetJointSettings(jointSpring);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(PauseMenu.isOn)
        {
            if(Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);
            motor.ApplyThruster(Vector3.zero);
            return;
        }

        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Set target position for spring
        //This makes the physics act right when it comes to applying gravity
        //during flyight over objects
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);

        }

        //Calculate movement velocity as a 3D vector
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMovement;
        Vector3 moverVertical = transform.forward * zMovement;

        //Final movemeent vector
        Vector3 velocity = (moveHorizontal + moverVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", zMovement);

        //Apply movement vector
        motor.Move(velocity);

        //Calculate rotation as a 3D vector (turning around)
        //We only rotate around Mouse X because that is the only 
        //rotation we want to affect our player
        float yRotation = Input.GetAxisRaw("Mouse X");
        
        Vector3 rotation = new Vector3(0f, yRotation, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vector
        float xRotation = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRotation * lookSensitivity;

        motor.RotateCamera(cameraRotationX);

        //Calculate thruster force based on player input
        Vector3 thruster = Vector3.zero;
        if(Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount >= 0.01f)
            {
                thruster = Vector3.up * thrusterForce;
                //When jumping we don't want the spring to affect it
                SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //Apply the thruster force
        motor.ApplyThruster(thruster);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
            { positionSpring = _jointSpring,
            maximumForce = jointMaxForce };
    }

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }
}
