using System.Collections;
using UnityEngine;

public class BottleInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public string promptText = "Presiona [E] para Beber";
    public float alcoholAmount = 10f;
    private float drinkDuration = 1.5f;

    private bool isBeingDrunk = false;
    private Collider bottleCollider;

    private void Awake()
    {
        bottleCollider = GetComponent<Collider>();
    }

    public string GetPromptText()
    {
        return promptText;
    }

    public bool CanInteract()
    {
        return !isBeingDrunk;
    }

    public void Interact(Transform cameraTransform, SistemaAlcohol sistemaAlcohol)
    {
        if (isBeingDrunk) return;
        StartCoroutine(DrinkCoroutine(cameraTransform, sistemaAlcohol));
    }

    private IEnumerator DrinkCoroutine(Transform cameraTransform, SistemaAlcohol sistemaAlcohol)
    {
        isBeingDrunk = true;
        
        // Disable collider immediately so player cannot interact again or hit it
        if (bottleCollider != null)
        {
            bottleCollider.enabled = false;
        }

        // Parent to camera to keep it in front of the player
        transform.SetParent(cameraTransform, true);

        // Store initial local states relative to the camera
        Vector3 startLocalPos = transform.localPosition;
        Quaternion startLocalRot = transform.localRotation;

        // Define target drinking position and rotation in front of the camera
        // slightly to the right, slightly down, and tilted directly towards the player, further away from the face
        Vector3 targetLocalPos = new Vector3(0.12f, -0.22f, 0.55f);
        Quaternion targetLocalRot = Quaternion.Euler(-45f, 0f, 0f); // Tilted directly towards player's face

        float elapsed = 0f;
        float raiseDuration = drinkDuration * 0.4f; // Raise it over 40% of time
        float drinkDurationPart = drinkDuration * 0.6f; // Hold/Tilt it over 60% of time

        // Phase 1: Raise to mouth
        while (elapsed < raiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / raiseDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, t);
            transform.localRotation = Quaternion.Slerp(startLocalRot, targetLocalRot, t);
            yield return null;
        }

        // Ensure we are exactly at target position/rotation
        transform.localPosition = targetLocalPos;
        transform.localRotation = targetLocalRot;

        // Phase 2: Drink/Tilt further back (pouring action)
        elapsed = 0f;
        Quaternion tiltRot = Quaternion.Euler(-80f, 0f, 0f); // Tip it directly towards the player's mouth even further
        while (elapsed < drinkDurationPart)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / drinkDurationPart;
            t = Mathf.SmoothStep(0f, 1f, t);

            // Tip the bottle further as time goes on, and bring it slightly closer to simulate pouring
            transform.localPosition = Vector3.Lerp(targetLocalPos, targetLocalPos + new Vector3(-0.02f, 0.02f, -0.05f), t);
            transform.localRotation = Quaternion.Slerp(targetLocalRot, tiltRot, t);
            yield return null;
        }

        // Phase 3: Increase alcohol level & destroy
        if (sistemaAlcohol != null)
        {
            sistemaAlcohol.nivelDeAlcohol = Mathf.Clamp(sistemaAlcohol.nivelDeAlcohol + alcoholAmount, 0f, 100f);
            Debug.Log($"[Interaction] Player drank bottle. Alcohol level increased by {alcoholAmount} to {sistemaAlcohol.nivelDeAlcohol}.");
        }

        // Destroy the bottle object
        Destroy(gameObject);
    }
}
