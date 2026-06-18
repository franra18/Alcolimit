using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Distance from which the player can interact with the bottle")]
    public float interactionDistance = 3f;
    [Tooltip("Layer mask for the interactable objects (optional)")]
    public LayerMask interactableLayer = ~0;

    [Header("Input Action")]
    [Tooltip("The interaction action (default is E)")]
    [SerializeField] private InputAction interactAction = new InputAction("Interact", binding: "<Keyboard>/e", type: InputActionType.Button);

    [Header("Crosshair Settings")]
    [Tooltip("Show a center crosshair dot for aiming")]
    [SerializeField] private bool showCrosshair = true;
    [Tooltip("Color of the central point")]
    [SerializeField] private Color crosshairColor = Color.white;
    [Tooltip("Color of the outline border")]
    [SerializeField] private Color crosshairBorderColor = Color.black;
    [Tooltip("Size of the inner white dot in pixels")]
    [SerializeField] private float dotSize = 4f;
    [Tooltip("Size of the outer black border in pixels")]
    [SerializeField] private float borderSize = 8f;

    private Camera playerCamera;
    private SistemaAlcohol sistemaAlcohol;
    private TMPro.TextMeshProUGUI promptText;

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    private void Start()
    {
        // Try to find the camera from the player
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        sistemaAlcohol = GetComponent<SistemaAlcohol>();
        if (sistemaAlcohol == null)
        {
            Debug.LogError("[PlayerInteraction] SistemaAlcohol component not found on player!");
        }

        SetupUI();
    }

    private void SetupUI()
    {
        // Try to find existing canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("InteractionCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // Setup center crosshair dot if requested
        if (showCrosshair)
        {
            var existingCrosshair = GameObject.Find("CenterCrosshair");
            if (existingCrosshair == null)
            {
                // Create outer border (black background)
                GameObject crosshairBg = new GameObject("CenterCrosshair");
                crosshairBg.transform.SetParent(canvas.transform, false);
                
                RectTransform bgRect = crosshairBg.AddComponent<RectTransform>();
                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                bgRect.pivot = new Vector2(0.5f, 0.5f);
                bgRect.anchoredPosition = Vector2.zero;
                bgRect.sizeDelta = new Vector2(borderSize, borderSize);
                
                var bgImage = crosshairBg.AddComponent<UnityEngine.UI.Image>();
                bgImage.color = crosshairBorderColor;
                bgImage.raycastTarget = false; // Prevent blocking interactions
                
                // Create inner point (white dot)
                GameObject crosshairDot = new GameObject("CenterDot");
                crosshairDot.transform.SetParent(crosshairBg.transform, false);
                
                RectTransform dotRect = crosshairDot.AddComponent<RectTransform>();
                dotRect.anchorMin = new Vector2(0.5f, 0.5f);
                dotRect.anchorMax = new Vector2(0.5f, 0.5f);
                dotRect.pivot = new Vector2(0.5f, 0.5f);
                dotRect.anchoredPosition = Vector2.zero;
                dotRect.sizeDelta = new Vector2(dotSize, dotSize);
                
                var dotImage = crosshairDot.AddComponent<UnityEngine.UI.Image>();
                dotImage.color = crosshairColor;
                dotImage.raycastTarget = false; // Prevent blocking interactions
            }
        }

        // Check if there is an existing text element, otherwise create it
        var existingText = GameObject.Find("InteractionText");
        if (existingText != null)
        {
            promptText = existingText.GetComponent<TMPro.TextMeshProUGUI>();
        }

        if (promptText == null)
        {
            var textObj = new GameObject("InteractionText");
            textObj.transform.SetParent(canvas.transform, false);
            
            var rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0f, -100f);
            rectTransform.sizeDelta = new Vector2(600f, 100f);
            
            promptText = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            promptText.fontSize = 32f;
            promptText.alignment = TMPro.TextAlignmentOptions.Center;
            promptText.color = Color.white;
            
            promptText.outlineWidth = 0.2f;
            promptText.outlineColor = Color.black;
        }
        
        promptText.text = ""; // Start empty
    }

    private void Update()
    {
        if (playerCamera == null) return;

        // Raycast from the center of the camera forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        BottleInteractable targetBottle = null;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            // Check if we hit a bottle or if a bottle is in its parents
            targetBottle = hit.collider.GetComponentInParent<BottleInteractable>();
            if (targetBottle != null && !targetBottle.CanInteract())
            {
                targetBottle = null;
            }
        }

        if (targetBottle != null)
        {
            // Display interaction prompt
            if (promptText != null)
            {
                promptText.text = targetBottle.promptText;
            }

            // Check if player presses interaction key
            if (interactAction.triggered)
            {
                targetBottle.Interact(playerCamera.transform, sistemaAlcohol);
                if (promptText != null)
                {
                    promptText.text = ""; // Clear immediately after interacting
                }
            }
        }
        else
        {
            // Clear prompt text when not looking at an interactable bottle
            if (promptText != null && promptText.text != "")
            {
                promptText.text = "";
            }
        }
    }
}
