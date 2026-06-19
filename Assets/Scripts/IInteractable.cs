using UnityEngine;

public interface IInteractable
{
    string GetPromptText();
    bool CanInteract();
    void Interact(Transform cameraTransform, SistemaAlcohol sistemaAlcohol);
}
