using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SistemaAlcohol : MonoBehaviour
{
    [Header("Variables de Jugabilidad")]
    [Range(0f, 100f)] 
    public float nivelDeAlcohol = 10f; // Puedes inicializarlo en 10 aquí

    [Header("UI Referencias")]
    [Tooltip("Arrastra aquí el Slider de la interfaz que servirá como barra de progreso")]
    [SerializeField] private Slider barraAlcohol;

    [Header("Referencias")]
    [Tooltip("Arrastra aquí la Main Camera (la que es hija del CameraPivot)")]
    public Transform mainCamera;
    [Tooltip("Arrastra aquí el objeto Global Volume de la jerarquía")]
    public Volume volumenGlobal;

    [Header("Ajustes de Mareo")]
    public float velocidadMareo = 1.2f;
    public float anguloMaximoMareo = 12f;

    // Variables internas para el post-procesado
    private ColorAdjustments ajustesColor;
    private LensDistortion distorsionLente;

    private void Start()
    {
        // Vinculamos los efectos del Volume a las variables del script
        if (volumenGlobal != null && volumenGlobal.profile != null)
        {
            volumenGlobal.profile.TryGet(out ajustesColor);
            volumenGlobal.profile.TryGet(out distorsionLente);
        }

        // Configuración inicial de la barra de progreso
        if (barraAlcohol != null)
        {
            barraAlcohol.minValue = 0f;
            barraAlcohol.maxValue = 100f; 
            barraAlcohol.value = nivelDeAlcohol;
        }
    }

    private void Update()
    {
        // Forzar límite mínimo de 10
        if (nivelDeAlcohol < 10f)
        {
            nivelDeAlcohol = 10f;
        }

        // Normalizamos el nivel de 0 a 1 para hacer cálculos más fáciles
        float porcentajeAlcohol = nivelDeAlcohol / 100f; 

        AplicarMareoCamara(porcentajeAlcohol);
        AplicarEfectosVisuales(porcentajeAlcohol);
        ActualizarUI(); 
    }

    private void AplicarMareoCamara(float porcentaje)
    {
        if (mainCamera == null) return;

        // Usamos Perlin Noise para generar un movimiento oscilante suave
        float ruidoX = (Mathf.PerlinNoise(Time.time * velocidadMareo, 0f) * 2f) - 1f;
        float ruidoY = (Mathf.PerlinNoise(0f, Time.time * velocidadMareo) * 2f) - 1f;
        float ruidoZ = (Mathf.PerlinNoise(Time.time * velocidadMareo, Time.time * velocidadMareo) * 2f) - 1f;

        // La intensidad del ruido se multiplica por el porcentaje de alcohol
        float rotX = ruidoX * anguloMaximoMareo * porcentaje;
        float rotY = ruidoY * anguloMaximoMareo * porcentaje;
        float rotZ = ruidoZ * (anguloMaximoMareo * 0.5f) * porcentaje; // Menos giro en Z para que sea jugable

        // Aplicamos la rotación local
        mainCamera.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    private void AplicarEfectosVisuales(float porcentaje)
    {
        // 1. Modificar Color (Desaturación y Tono)
        if (ajustesColor != null)
        {
            // Baja la saturación hasta -80 a nivel máximo
            ajustesColor.saturation.value = Mathf.Lerp(0f, -80f, porcentaje); 
            // Aplica un ligero tinte amarillento/enfermizo
            ajustesColor.colorFilter.value = Color.Lerp(Color.white, new Color(0.85f, 0.9f, 0.6f), porcentaje); 
        }

        // 2. Modificar Lente (Efecto "Ojo de pez" o difuminado en los bordes)
        if (distorsionLente != null)
        {
            // Aumenta la intensidad de la distorsión
            distorsionLente.intensity.value = Mathf.Lerp(0f, -0.6f, porcentaje);
        }
    }

    private void ActualizarUI()
    {
        // Si hay una barra asignada, actualizamos su valor visual
        if (barraAlcohol != null)
        {
            barraAlcohol.value = nivelDeAlcohol;
        }
    }
}