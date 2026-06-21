using UnityEngine;
using UnityEngine.InputSystem;

public class CockpitLook : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el CameraPivot")]
    public Transform cameraTransform;

    [Header("Cámara")]
    // Volvemos a los valores altos (ej. 15 o 25) para que coincida con tu interior
    public float mouseSensitivity = 25f; 
    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Inputs")]
    public InputAction lookAction; // <- Volvemos al New Input System

    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // (Opcional) Si quieres cargar la sensibilidad guardada al empezar:
        if (PlayerPrefs.HasKey("Sensibilidad"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("Sensibilidad");
        }
    }

    private void Update()
    {
        if (cameraTransform == null) return;

        // Leemos el input EXACTAMENTE igual que en FirstPersonController
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // 1. Rota el CameraPivot (Vertical)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 2. Rota el Pivot Asiento (Horizontal)
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -110f, 110f); 
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}