using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí la Main Camera que pusiste en la cabeza")]
    [SerializeField] private Transform cameraTransform;
    private Animator animator;
    private CharacterController controller;

    [Header("Movimiento (Editable)")]
    public float moveSpeed = 5f;
    public float jumpForce = 2f;
    public float gravity = -9.81f;

    [Header("Cámara")]
    public float mouseSensitivity = 15f;
    private float xRotation = 0f;

    [Header("Inputs")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction lookAction;

    private Vector3 velocity;
    private bool isGrounded;

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        lookAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        lookAction.Disable();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        // Bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleJump();
    }

    private void HandleLook()
    {
        if (cameraTransform == null) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 80f); // Evita dar volteretas con la cámara

        // Rota la cámara arriba/abajo
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rota el cuerpo entero izquierda/derecha
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Mantiene al personaje pegado al suelo
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Sincronización suavizada con el Animator
        if (animator != null)
        {
            // Interpola el valor actual hacia el input objetivo (el 10f es la velocidad de transición)
            float smoothHor = Mathf.Lerp(animator.GetFloat("Hor"), moveInput.x, Time.deltaTime * 10f);
            float smoothVert = Mathf.Lerp(animator.GetFloat("Vert"), moveInput.y, Time.deltaTime * 10f);

            animator.SetFloat("Hor", smoothHor);
            animator.SetFloat("Vert", smoothVert);
        }
    }

    private void HandleJump()
    {
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            
            if (animator != null)
            {
                animator.SetBool("IsJump", true);
            }
        }
        else if (isGrounded && animator != null)
        {
            animator.SetBool("IsJump", false);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}