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

    // NUEVA VARIABLE PARA CONTROLAR EL MOVIMIENTO
    public bool puedeMoverse = true;

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
        HandleLook(); // La cámara siempre debe funcionar

        if (puedeMoverse)
        {
            HandleMovement();
            HandleJump();
        }
        else
        {
            // Si está congelado, detiene las animaciones suavemente y aplica solo la gravedad
            DetenerAnimaciones();
            AplicarGravedad();
        }
    }

    private void HandleLook()
    {
        if (cameraTransform == null) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 80f);

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
            velocity.y = -2f;
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (animator != null)
        {
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

        AplicarGravedad();
    }

    private void AplicarGravedad()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void DetenerAnimaciones()
    {
        if (animator != null)
        {
            float smoothHor = Mathf.Lerp(animator.GetFloat("Hor"), 0f, Time.deltaTime * 10f);
            float smoothVert = Mathf.Lerp(animator.GetFloat("Vert"), 0f, Time.deltaTime * 10f);

            animator.SetFloat("Hor", smoothHor);
            animator.SetFloat("Vert", smoothVert);
        }
    }

    // NUEVA FUNCIÓN PARA RECIBIR EL MENSAJE
    public void PermitirMovimiento(bool estado)
    {
        puedeMoverse = estado;
    }
}