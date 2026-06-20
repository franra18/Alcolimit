using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ControladorEscenaIntermedia : MonoBehaviour
{
    [Header("Configuración de UI")]
    [SerializeField] private Image componenteImagenUI;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Menú de Fin de Juego")]
    [Tooltip("Arrastra aquí el GameObject del menú de derrota (debe empezar desactivado)")]
    [SerializeField] private GameObject menuDerrota;

    [Header("Contenido Fijo (Primera Viñeta)")]
    [SerializeField] private Sprite vinetaInicialFija;

    [Header("Opciones Siguiente Viñeta")]
    [SerializeField] private Sprite vinetaSobrio;
    [SerializeField] private Sprite vinetaEbrio;

    [Header("Rango para Estado Sobrio")]
    [Tooltip("El nivel de alcohol mínimo para considerar al jugador sobrio")]
    [SerializeField] private float alcoholMinSobrio = 0f;
    [Tooltip("El nivel de alcohol máximo para considerar al jugador sobrio")]
    [SerializeField] private float alcoholMaxSobrio = 4f; 

    [Header("Ajustes de Transición")]
    [SerializeField] private float duracionFundido = 0.5f;

    private Sprite[] vinetasElegidas = new Sprite[2];
    private int indiceActual = 0;
    private bool estaTransicionando = false;
    private bool esEbrio = false; // Nos guardará el resultado de la condición

    void Start()
    {
        // 1. Asegurar que el tiempo corre normalmente al iniciar la escena
        Time.timeScale = 1f;

        // 2. Asegurar que el menú de derrota empiece oculto por código por si acaso
        if (menuDerrota != null)
        {
            menuDerrota.SetActive(false);
        }

        // 3. Recuperar el nivel de alcohol guardado
        float nivelAlcohol = PlayerPrefs.GetFloat("NivelAlcohol", 0f);

        // 4. Configurar la primera viñeta
        vinetasElegidas[0] = vinetaInicialFija;

        // 5. Evaluar el rango
        if (nivelAlcohol >= alcoholMinSobrio && nivelAlcohol <= alcoholMaxSobrio)
        {
            vinetasElegidas[1] = vinetaSobrio;
            esEbrio = false;
        }
        else
        {
            vinetasElegidas[1] = vinetaEbrio;
            esEbrio = true; // El jugador ha perdido
        }

        // 6. Inicializar la UI
        if (vinetasElegidas[0] != null && componenteImagenUI != null && canvasGroup != null)
        {
            componenteImagenUI.sprite = vinetasElegidas[indiceActual];
            canvasGroup.alpha = 0f; 
            StartCoroutine(FundidoInicial());
        }
        else
        {
            Debug.LogError("Faltan referencias por asignar en el Inspector.");
        }
    }

    void Update()
    {
        if (estaTransicionando) return;

        // Si el menú de derrota ya está activo, bloqueamos el cambio de viñetas con el teclado/ratón
        if (menuDerrota != null && menuDerrota.activeSelf) return;

        if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            StartCoroutine(SecuenciaTransicion());
        }
    }

    private IEnumerator FundidoInicial()
    {
        estaTransicionando = true;
        yield return StartCoroutine(ModificarOpacidad(0f, 1f));
        estaTransicionando = false;
    }

    private IEnumerator SecuenciaTransicion()
    {
        estaTransicionando = true;

        // Fade Out
        yield return StartCoroutine(ModificarOpacidad(1f, 0f));

        indiceActual++;

        if (indiceActual < vinetasElegidas.Length)
        {
            componenteImagenUI.sprite = vinetasElegidas[indiceActual];

            // Fade In
            yield return StartCoroutine(ModificarOpacidad(0f, 1f));
            estaTransicionando = false;
        }
        else
        {
            // Hemos terminado de mostrar las viñetas. Decidimos el destino:
            if (esEbrio)
            {
                ActivarMenuDerrota();
            }
            else
            {
                CargarSiguienteEscena();
            }
        }
    }

    private void ActivarMenuDerrota()
    {
        if (menuDerrota != null)
        {
            menuDerrota.SetActive(true); // Activa el panel del menú en el Canvas
            Time.timeScale = 0f;         // Pausa el tiempo del juego por completo
            Debug.Log("[ControladorEscenaIntermedia] Jugador detenido. Menú de derrota activado y juego pausado.");
        }
        else
        {
            Debug.LogError("No has asignado el 'MenuDerrota' en el Inspector.");
        }
    }

    private IEnumerator ModificarOpacidad(float opacidadInicial, float opacidadFinal)
    {
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionFundido)
        {
            // NOTA: Usamos Time.unscaledDeltaTime para que la transición funcione 
            // incluso si el tiempo se congelara por alguna razón externa.
            tiempoTranscurrido += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(opacidadInicial, opacidadFinal, tiempoTranscurrido / duracionFundido);
            yield return null;
        }
        canvasGroup.alpha = opacidadFinal;
    }

    private void CargarSiguienteEscena()
    {
        int indiceEscenaActual = SceneManager.GetActiveScene().buildIndex;
        int siguienteEscena = indiceEscenaActual + 1;

        if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            Debug.LogError("No hay ninguna escena añadida después de esta en el Build Settings.");
        }
    }

    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f; // Asegúrate de restablecer el tiempo antes de cambiar de escena
        SceneManager.LoadScene(0); // Cambia el 0 por el índice o nombre de tu menú de inicio
    }
}