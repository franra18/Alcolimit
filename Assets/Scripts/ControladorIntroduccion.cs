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
    [Tooltip("Asigna las 5 viñetas iniciales en orden: 0 para 5-6, 1 para 6-7, 2 para 7-8, 3 para 8-9, 4 para 9-10")]
    [SerializeField] private Sprite[] vinetasUmbrales; 
    [Tooltip("Pon en el elemento 0 cualquier viñeta (se sustituirá sola) y en el elemento 1 la segunda viñeta")]
    [SerializeField] private Sprite[] vinetas;

    [Header("Ajustes de Transición")]
    [SerializeField] private float duracionFundido = 0.5f;

    private int indiceActual = 0;
    private bool estaTransicionando = false;

    void Start()
    {
        if (vinetas.Length > 0 && componenteImagenUI != null && canvasGroup != null)
        {
            float umbralMin = PlayerPrefs.GetFloat("UmbralMin", 5f);
            
            // Convertimos los posibles valores (5, 6, 7, 8, 9) en los índices del array (0, 1, 2, 3, 4)
            int indiceUmbral = (int)umbralMin - 5;
            indiceUmbral = Mathf.Clamp(indiceUmbral, 0, 4);

            if (vinetasUmbrales != null && vinetasUmbrales.Length == 5)
            {
                // Sobrescribimos la primera viñeta con la imagen correcta según el umbral
                vinetas[0] = vinetasUmbrales[indiceUmbral];
            }

            componenteImagenUI.sprite = vinetas[indiceActual];
            
            canvasGroup.alpha = 0f; 
            StartCoroutine(FundidoInicial());
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

        if (indiceActual < vinetas.Length)
        {
            componenteImagenUI.sprite = vinetas[indiceActual];
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
    }
}