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

    [Header("Ajustes de Transición")]
    [SerializeField] private float duracionFundido = 0.5f;

    private Sprite[] vinetasElegidas = new Sprite[2];
    private int indiceActual = 0;
    private bool estaTransicionando = false;
    private bool esEbrio = false; 

    void Start()
    {
        Time.timeScale = 1f;

        if (menuDerrota != null)
        {
            menuDerrota.SetActive(false);
        }

        float nivelAlcoholOriginal = PlayerPrefs.GetFloat("NivelAlcohol", 0f);
        // Convertimos el nivel (ej: 75 pasa a ser 7.5) para compararlo con los umbrales
        float nivelAlcoholConvertido = nivelAlcoholOriginal / 10f; 

        float umbralMin = PlayerPrefs.GetFloat("UmbralMin", 5f);
        float umbralMax = PlayerPrefs.GetFloat("UmbralMax", 6f);

        vinetasElegidas[0] = vinetaInicialFija;

        if (nivelAlcoholConvertido >= umbralMin && nivelAlcoholConvertido <= umbralMax)
        {
            vinetasElegidas[1] = vinetaSobrio;
            esEbrio = false;
        }
        else
        {
            vinetasElegidas[1] = vinetaEbrio;
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