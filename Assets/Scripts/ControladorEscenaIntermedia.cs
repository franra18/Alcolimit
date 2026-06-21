using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class ControladorEscenaIntermedia : MonoBehaviour
{
    [Header("Configuración de UI")]
    [SerializeField] private Image componenteImagenUI;
    [SerializeField] private CanvasGroup canvasGroup;

    [Tooltip("Arrastra aquí el componente TextMeshPro - Text de 'NivelAlcohol'")]
    [SerializeField] private TextMeshProUGUI textoNivelAlcohol;

    [Header("Menú de Fin de Juego")]
    [Tooltip("Arrastra aquí el GameObject del menú de derrota (debe empezar desactivado)")]
    [SerializeField] private GameObject menuDerrota;

    [Header("Contenido Fijo")]
    [SerializeField] private Sprite vinetaInicialFija;
    [SerializeField] private Sprite vinetaIntermediaFija; // <-- NUEVA: Segunda viñeta fija

    [Header("Opciones Siguiente Viñeta")]
    [SerializeField] private Sprite vinetaSobrio;
    [SerializeField] private Sprite vinetaEbrio;

    [Header("Ajustes de Transición")]
    [SerializeField] private float duracionFundido = 0.5f;

    private Sprite[] vinetasElegidas = new Sprite[3]; // <-- Cambiado a tamaño 3
    private int indiceActual = 0;
    private bool estaTransicionando = false;
    private bool esEbrio = false; 
    private float nivelAlcoholConvertido;

    void Start()
    {
        Time.timeScale = 1f;

        if (menuDerrota != null)
        {
            menuDerrota.SetActive(false);
        }

        if (textoNivelAlcohol != null)
        {
            textoNivelAlcohol.gameObject.SetActive(false);
        }

        float nivelAlcoholOriginal = PlayerPrefs.GetFloat("NivelAlcohol", 0f);
        nivelAlcoholConvertido = nivelAlcoholOriginal / 10f; 

        float umbralMin = PlayerPrefs.GetFloat("UmbralMin", 5f);
        float umbralMax = PlayerPrefs.GetFloat("UmbralMax", 6f);

        // Asignamos las dos viñetas fijas consecutivas
        vinetasElegidas[0] = vinetaInicialFija;
        vinetasElegidas[1] = vinetaIntermediaFija; // <-- NUEVA

        // La tercera viñeta (índice 2) es la variable
        if (nivelAlcoholConvertido >= umbralMin && nivelAlcoholConvertido <= umbralMax)
        {
            vinetasElegidas[2] = vinetaSobrio;
            esEbrio = false;
        }
        else
        {
            vinetasElegidas[2] = vinetaEbrio;
            esEbrio = true; 
        }

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

        yield return StartCoroutine(ModificarOpacidad(1f, 0f));

        indiceActual++;

        if (indiceActual < vinetasElegidas.Length)
        {
            componenteImagenUI.sprite = vinetasElegidas[indiceActual];

            if (textoNivelAlcohol != null)
            {
                // Quiero que el texto solo sea el numero del nivel de alcohol
                textoNivelAlcohol.text = nivelAlcoholConvertido.ToString("0.0");
                textoNivelAlcohol.gameObject.SetActive(indiceActual == 1);
            }

            yield return StartCoroutine(ModificarOpacidad(0f, 1f));
            estaTransicionando = false;
        }
        else
        {
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
            menuDerrota.SetActive(true); 
            Time.timeScale = 0f;         

            // --- SOLUCIÓN AL RATÓN ---
            Cursor.visible = true;                       // Hace que el cursor sea visible
            Cursor.lockState = CursorLockMode.None;       // Desbloquea el ratón para que se mueva libremente
            // -------------------------

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
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0); 
    }
}