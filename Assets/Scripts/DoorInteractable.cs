using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private string promptText = "Presiona [E] para Salir";

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
        Debug.Log("[DoorInteractable] Player interacted with the door.");

        // 1. Guardar el nivel de alcohol en PlayerPrefs
        if (sistemaAlcohol != null)
        {
            float alcoholActual = sistemaAlcohol.nivelDeAlcohol; 
            PlayerPrefs.SetFloat("NivelAlcohol", alcoholActual);
            PlayerPrefs.Save();
            Debug.Log("[DoorInteractable] Nivel de alcohol guardado: " + alcoholActual);
        }

        // 2. Calcular y cargar la Siguiente Escena en el Build Settings
        int indiceEscenaActual = SceneManager.GetActiveScene().buildIndex;
        int siguienteEscena = indiceEscenaActual + 1;

        // Validar que exista una escena después de esta
        if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("[DoorInteractable] Cargando siguiente escena por índice: " + siguienteEscena);
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            Debug.LogError("[DoorInteractable] No hay ninguna escena después de esta en el Build Settings. ¡Añádela primero!");
        }
    }
}