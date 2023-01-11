using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RigidBodyMovement : NetworkBehaviour
{
    #region
    public static Transform instance;

    private void Awake()
    {
        instance = this.transform;
        knee.transform.position = new Vector3(knee.transform.position.x, stepHeight, knee.transform.position.z);
    }
    #endregion

    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;
    private float xRotation;

    [Header("Functional Options")]
    [SerializeField] bool lockCursor = true;

    [Header("Controls")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform feetTransform;
    Transform playerCamera;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private CapsuleCollider playerCollider;

    [Header("Jumping Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float jumpForce;
    [SerializeField] GameObject knee;
    [SerializeField] GameObject feet;

    [Header("Stairs Walking Parameters")]
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.1f;

    [Header("Crouching Parameters")]
    [SerializeField] private float originalHeight = 2f;
    [SerializeField] private float reducedHeight = 0.5f;

    [Header("Camera Parameters")]
    Transform _mainCamera;

    void Start()
    {
        GetReferences();
    }

    void Update()
    {
        if (!isOwned) return;
        if (isServerOnly) return;

        lockCursor = Input.GetKeyDown(KeyCode.E) ? !lockCursor : lockCursor; //TEST: just to toggle mouse so i can easily test

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        StepClimb();
        Crouch();
    }

    void LateUpdate()
    {
        if (!isOwned) return;
        if (isServerOnly) return;

        var cameraPosition = transform.position + Vector3.up;
        _mainCamera.position = cameraPosition;
        MovePlayerCamera();
    }

    void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * speed;
        playerBody.velocity = new Vector3(moveVector.x, playerBody.velocity.y, moveVector.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Physics.CheckSphere(feetTransform.position, 0.3f, floorMask))
            {
                playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                playerBody.AddForce(Physics.gravity * (playerBody.mass * playerBody.mass));
            }
        }
    }

    private void Crouch()
    {
        if (Input.GetKeyDown(crouchKey))
            GoDown();
        else if (Input.GetKeyUp(crouchKey))
            GoUp();
    }

    private void GoDown()
    {
        playerCollider.height = reducedHeight;
    }

    private void GoUp()
    {
        playerCollider.height = originalHeight;
    }

    private void MovePlayerCamera()
    {
        xRotation -= playerMouseInput.y * sensitivity;

        transform.Rotate(0.0f, playerMouseInput.x * sensitivity, 0.0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
    }

    void StepClimb()
    {
        RaycastHit hitFeet;
        if (Physics.Raycast(feet.transform.position, transform.TransformDirection(Vector3.forward), out hitFeet, 0.1f))
        {
            RaycastHit hitKnee;
            if (!Physics.Raycast(knee.transform.position, transform.TransformDirection(Vector3.forward), out hitKnee, 0.2f))
            {
                playerBody.position -= new Vector3(0.0f, -stepSmooth, 0.0f);
            }
        }

        RaycastHit hitFeet45;
        if (Physics.Raycast(feet.transform.position, transform.TransformDirection(1.5f, 0,1), out hitFeet45, 0.1f))
        {
            RaycastHit hitKnee45;
            if (!Physics.Raycast(knee.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitKnee45, 0.2f))
            {
                playerBody.position -= new Vector3(0.0f, -stepSmooth, 0.0f);
            }
        }

        RaycastHit hitFeetMinus45;
        if (Physics.Raycast(feet.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitFeetMinus45, 0.1f))
        {
            RaycastHit hitKneeMinus45;
            if (!Physics.Raycast(knee.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitKneeMinus45, 0.2f))
            {
                playerBody.position -= new Vector3(0.0f, -stepSmooth, 0.0f);
            }
        }
    }

    private void GetReferences()
    {
        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        _mainCamera = Camera.main.transform;
        playerCamera = Camera.main.transform;
    }
}