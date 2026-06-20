using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ControladorIntroduccion : MonoBehaviour
{
    [Header("Configuración de UI")]
    [SerializeField] private Image componenteImagenUI;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Contenido de la Historia")]
    [SerializeField] private Sprite[] vinetas;

    [Header("Ajustes de Transición")]
    [SerializeField] private float duracionFundido = 0.5f;

    private int indiceActual = 0;
    private bool estaTransicionando = false;

    void Start()
    {
        if (vinetas.Length > 0 && componenteImagenUI != null && canvasGroup != null)
        {
            componenteImagenUI.sprite = vinetas[indiceActual];
            
            // CAMBIO: Empezamos en invisible y lanzamos el fundido inicial
            canvasGroup.alpha = 0f; 
            StartCoroutine(FundidoInicial());
        }
        else
        {
            Debug.LogError("Por favor, asigna la Imagen UI, el Canvas Group y al menos una viñeta en el Inspector.");
        }
    }

    void Update()
    {
        if (estaTransicionando) return;

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame ||
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(SecuenciaTransicion());
        }
    }

    // Corrutina para que la primera viñeta aparezca suavemente al cargar la escena
    private IEnumerator FundidoInicial()
    {
        estaTransicionando = true;
        yield return StartCoroutine(ModificarOpacidad(0f, 1f));
        estaTransicionando = false;
    }

    private IEnumerator SecuenciaTransicion()
    {
        estaTransicionando = true;

        // 1. Desvanecer hacia el fondo negro (Fade Out)
        yield return StartCoroutine(ModificarOpacidad(1f, 0f));

        indiceActual++;

        if (indiceActual < vinetas.Length)
        {
            componenteImagenUI.sprite = vinetas[indiceActual];

            // 2. Aparecer desde el fondo negro (Fade In)
            yield return StartCoroutine(ModificarOpacidad(0f, 1f));
            
            estaTransicionando = false;
        }
        else
        {
            CargarSiguienteEscena();
        }
    }

    private IEnumerator ModificarOpacidad(float opacidadInicial, float opacidadFinal)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionFundido)
        {
            tiempoTranscurrido += Time.deltaTime;
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
}