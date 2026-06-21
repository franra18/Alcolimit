using UnityEngine;

public class DetectorMeta : MonoBehaviour
{
    private GeneradorRutas gestorRutas;

    void Start()
    {
        gestorRutas = FindObjectOfType<GeneradorRutas>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // other.transform.root viaja hasta el objeto más alto de la jerarquía (tu Electric Truck)
        GameObject objetoPrincipal = other.transform.root.gameObject;

        // Comprobamos si ese objeto principal tiene la etiqueta Player
        if (objetoPrincipal.CompareTag("Player"))
        {
            Debug.Log($"🏆 ¡Victoria! El cilindro detectó la pieza '{other.gameObject.name}' del camión.");

            if (gestorRutas != null)
            {
                gestorRutas.CompletarNivel();
                // Liberar el cursor para poder hacer clic
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Apagamos la meta para que no vuelva a chocar con las ruedas traseras
                gameObject.SetActive(false); 
            }
        }
    }
}