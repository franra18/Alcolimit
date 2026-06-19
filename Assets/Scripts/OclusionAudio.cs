using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OclusionAudio : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;
    [Tooltip("Selecciona la capa (Layer) en la que están tus suelos, techos y paredes.")]
    public LayerMask capasObstaculos; 
    
    [Header("Ajustes de Oclusión")]
    [Range(0f, 1f)] public float multiplicadorVolumenOcluido = 0.2f;
    [Tooltip("Frecuencia del filtro. Cuanto más baja, más 'ahogado' sonará el audio.")]
    public float frecuenciaOcluida = 1000f; 
    public float velocidadTransicion = 5f;

    private AudioSource audioSource;
    private AudioLowPassFilter lowPass;
    
    private float volumenOriginal;
    private float targetVolumen;
    private float targetFrecuencia;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        volumenOriginal = audioSource.volume;

        // Añade automáticamente el componente de filtro si no existe
        lowPass = GetComponent<AudioLowPassFilter>();
        if (lowPass == null)
        {
            lowPass = gameObject.AddComponent<AudioLowPassFilter>();
        }
        
        // Inicializa el filtro abierto (sin alterar el sonido)
        lowPass.cutoffFrequency = 22000f;

        // Si no asignas al jugador en el inspector, lo busca automáticamente por su etiqueta
        if (jugador == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (jugador == null || !audioSource.isPlaying) return;

        // Calcula la dirección y la distancia exacta hacia el jugador
        Vector3 posicionOjosNPC = transform.position + Vector3.up * 1.5f; 
        Vector3 posicionOjosJugador = jugador.position + Vector3.up * 1.5f;
        Vector3 direccion = posicionOjosJugador - posicionOjosNPC;
        float distancia = direccion.magnitude;

        // Lanza un rayo para verificar si hay un suelo/techo interrumpiendo la línea de visión recta
        if (Physics.Raycast(posicionOjosNPC, direccion.normalized, out RaycastHit hit, distancia, capasObstaculos))
        {
            // Hay un suelo o pared en medio -> Sonido tapado
            targetVolumen = volumenOriginal * multiplicadorVolumenOcluido;
            targetFrecuencia = frecuenciaOcluida;
        }
        else
        {
            // Línea de visión directa -> Sonido nítido y normal
            targetVolumen = volumenOriginal;
            targetFrecuencia = 22000f; // Frecuencia máxima (desactiva el efecto del filtro)
        }

        // Aplica los cambios de forma suave para evitar saltos bruscos de volumen
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolumen, Time.deltaTime * velocidadTransicion);
        lowPass.cutoffFrequency = Mathf.Lerp(lowPass.cutoffFrequency, targetFrecuencia, Time.deltaTime * velocidadTransicion);
    }
}