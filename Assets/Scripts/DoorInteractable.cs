using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private string promptText = "Presiona [E] para Salir";
    [SerializeField] private string nextSceneName = "Exterior";

    public string GetPromptText()
    {
        return promptText;
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Transform cameraTransform, SistemaAlcohol sistemaAlcohol)
    {
        Debug.Log("[DoorInteractable] Player interacted with the door. Loading scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}
