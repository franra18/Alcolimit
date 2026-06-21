using UnityEngine;

public class DetectorMeta : MonoBehaviour
{
    [Header("Referencias Audio")]
    public AudioSource musicaFondo;
    public AudioSource sonidoMotor; // Nueva referencia para el sonido del motor

    private GeneradorRutas gestorRutas;
    private bool victoriaRegistrada = false; // Bloqueo para que solo cuente una vez

    void Start()
    {
        gestorRutas = FindObjectOfType<GeneradorRutas>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si ya ha detectado una pieza, ignoramos las demás
        if (victoriaRegistrada) return;

        // other.transform.root viaja hasta el objeto más alto de la jerarquía
        GameObject objetoPrincipal = other.transform.root.gameObject;

        // Comprobamos si ese objeto principal tiene la etiqueta Player
        if (objetoPrincipal.CompareTag("Player"))
        {
            victoriaRegistrada = true; // Activamos el bloqueo

            // Parar la música de fondo
            if (musicaFondo != null && musicaFondo.isPlaying)
            {
                musicaFondo.Stop();
            }

            // Parar el sonido del motor
            if (sonidoMotor != null && sonidoMotor.isPlaying)
            {
                sonidoMotor.Stop();
            }

            Debug.Log($"🏆 ¡Victoria! El cilindro detectó la pieza '{other.gameObject.name}' del camión.");
            
            // Sumar 1 al contador de victorias y guardar
            int victoriasActuales = PlayerPrefs.GetInt("Victorias", 0);
            victoriasActuales++;
            PlayerPrefs.SetInt("Victorias", victoriasActuales);
            PlayerPrefs.Save();

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