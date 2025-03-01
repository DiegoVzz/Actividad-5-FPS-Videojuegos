using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform head;
    [SerializeField] private float distanceToFloor;
    [SerializeField] private LayerMask groundLayer;

    private CharacterController cc;

    private InputSystem_Actions input;

    [SerializeField] private float velocityY;

    private const float GRAVITY = -9.8f;

    [SerializeField] private bool isGrounded;

    private void OnEnable()
    {
        input.Player.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Awake()
    {
        input = new();
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        input.Player.Jump.performed += ctx =>
        {
            if(isGrounded)
            {
                velocityY = jumpForce;
            }
                
        };
    }

    private void FixedUpdate()
    {

        Vector2 motion = input.Player.Move.ReadValue<Vector2>();
        Vector3 direction = transform.forward * motion.y + transform.right * motion.x;
        Vector3 movement = direction * speed * Time.deltaTime;

        if (!isGrounded)
        {
            velocityY += GRAVITY * Time.deltaTime;
        }

        cc.Move(movement +  Vector3.up * (velocityY * Time.deltaTime));

        
    }

    private void Update()
    {
        Vector2 lookDelta = input.Player.Look.ReadValue<Vector2>();
        transform.Rotate(new Vector3(0, lookDelta.x * sensitivity, 0));
        head.Rotate(new Vector3(-lookDelta.y * sensitivity, 0, 0));
        head.eulerAngles = new Vector3( Mathf.Clamp(head.eulerAngles.x, 10, 150), head.eulerAngles.y, head.eulerAngles.z);
        
        if(Physics.Raycast(new(transform.position, Vector3.down), out RaycastHit hit, distanceToFloor, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distanceToFloor);
    }
}
