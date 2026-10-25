using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    InputControls controls;

    [Header("Player Movement")]
    [SerializeField] float speed = 2f;
    [SerializeField] float sprintSpeed = 4f;
    //[SerializeField] float characterRotationSmoothing = 0.03f;
    float playerHeight;

    [SerializeField] CharacterController charController;
    Rigidbody rb;

    bool isSprinting = false;

    public Vector2 MoveDirection;
    bool isGrounded;

    public float gravity = -9.8f;

    Vector3 velocity;
    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask GroundMask;

    public bool CanMove;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] float lowerLookLimit = 80.0f;
    [SerializeField] float rotationDamper = 0.1f;

    float rotationX = 0f;
    Camera playerCamera;

    private void Awake()
    {
        CanMove = true;
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        
        playerHeight = transform.position.y;
        rb = GetComponent<Rigidbody>();

        
    }

    private void OnEnable()
    {
        
            controls = gameObject.GetComponent<Player>().controls;

            controls.Player.Sprint.performed += sprinting => OnSprinting();
        
       
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Sprint.performed -= sprinting => OnSprinting();
        }
    }

    private void Update()
    {
        if (CanMove)
        {
            
            isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, GroundMask);
            
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2;
            }
            MoveDirection = controls.Player.Move.ReadValue<Vector2>();

            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;

            // Keep movement horizontal
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 move = cameraForward * MoveDirection.y
                         + cameraRight * MoveDirection.x;

            charController.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            charController.Move(velocity * Time.deltaTime);

            MouseLook();
        }
    }



    void MouseLook()
    {
        if (Mouse.current == null)
        {
            Debug.LogWarning("There is currently no mouse defined.");
            return;
        }

        float mouseY = Mouse.current.delta.y.ReadValue();
        float mouseX = Mouse.current.delta.x.ReadValue();

        rotationX -= mouseY * lookSpeedY * 0.1f; //For Smoothness
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeedX * rotationDamper, 0);
    }

    void OnSprinting()
    {
        if (!isSprinting)
        {
            speed += sprintSpeed;
            isSprinting = !isSprinting;
        }
        else if (isSprinting)
        {
            speed -= sprintSpeed;
            isSprinting = !isSprinting;
        }
    }
}
