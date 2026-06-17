using UnityEngine;
using UnityEngine.SceneManagement; // Obligatorio para cambiar de escena

public class MenuInicio : MonoBehaviour
{
    // Este método se llamará al pulsar el botón Jugar
    public void Jugar()
    {
        // Carga la siguiente escena en el orden de Build (asegúrate de que tu escena de juego sea la número 1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Este método se llamará al pulsar el botón Salir
    public void Salir()
    {
        Debug.Log("Saliendo del juego..."); // Esto solo se ve en el editor
        Application.Quit(); // Cierra el juego (solo funciona en el juego ya exportado/.exe)

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}