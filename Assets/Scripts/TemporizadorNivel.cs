using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TemporizadorNivel : MonoBehaviour
{
    [Header("Configuración del Temporizador")]
    [Tooltip("Tiempo en segundos. 5 minutos = 300 segundos.")]
    [SerializeField] private float tiempoRestante = 300f; 
    [SerializeField] private TMP_Text textoTemporizador;

    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto que contiene el script SistemaAlcohol")]
    [SerializeField] private SistemaAlcohol sistemaAlcohol;

    private bool tiempoAgotado = false;

    private void Update()
    {
        if (tiempoAgotado) return;

        tiempoRestante -= Time.deltaTime;

        ActualizarUI();

        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            tiempoAgotado = true;
            FinalizarTiempo();
        }
    }

    private void ActualizarUI()
    {
        if (textoTemporizador != null)
        {
            // Convertir los segundos a formato Minutos:Segundos
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    private void FinalizarTiempo()
    {
        Debug.Log("[Temporizador] El tiempo se ha agotado.");

        // 1. Guardar el nivel de alcohol
        if (sistemaAlcohol != null)
        {
            float alcoholActual = sistemaAlcohol.nivelDeAlcohol; 
            PlayerPrefs.SetFloat("NivelAlcohol", alcoholActual);
            PlayerPrefs.Save();
            Debug.Log("[Temporizador] Nivel de alcohol guardado: " + alcoholActual);
        }

        // 2. Cargar la Siguiente Escena
        int indiceEscenaActual = SceneManager.GetActiveScene().buildIndex;
        int siguienteEscena = indiceEscenaActual + 1;

        if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("[Temporizador] Cargando siguiente escena por índice: " + siguienteEscena);
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            Debug.LogError("[Temporizador] No hay ninguna escena después de esta en el Build Settings.");
        }
    }
}